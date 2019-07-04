using Ionic.Zlib;
using Klei;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using KSerialization;
using Newtonsoft.Json;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SaveLoader : KMonoBehaviour
{
	public class FlowUtilityNetworkInstance
	{
		public int id = -1;

		public SimHashes containedElement = SimHashes.Vacuum;

		public float containedMass = 0f;

		public float containedTemperature = 0f;
	}

	[SerializationConfig(KSerialization.MemberSerialization.OptOut)]
	public class FlowUtilityNetworkSaver : ISaveLoadable
	{
		public List<FlowUtilityNetworkInstance> gas;

		public List<FlowUtilityNetworkInstance> liquid;

		public FlowUtilityNetworkSaver()
		{
			gas = new List<FlowUtilityNetworkInstance>();
			liquid = new List<FlowUtilityNetworkInstance>();
		}
	}

	public struct SaveFileEntry
	{
		public string path;

		public System.DateTime timeStamp;
	}

	private struct MinionAttrFloatData
	{
		public string Name;

		public float Value;
	}

	private struct MinionMetricsData
	{
		public string Name;

		public List<MinionAttrFloatData> Modifiers;

		public float TotalExperienceGained;

		public List<string> Skills;
	}

	private struct SavedPrefabMetricsData
	{
		public string PrefabName;

		public int Count;
	}

	private struct WorldInventoryMetricsData
	{
		public string Name;

		public float Amount;
	}

	private struct DailyReportMetricsData
	{
		public string Name;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Net;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Positive;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Negative;
	}

	private struct PerformanceMeasurement
	{
		public string name;

		public float value;
	}

	[MyCmpGet]
	private GridSettings gridSettings;

	private bool saveFileCorrupt = false;

	private bool compressSaveData = true;

	public bool saveAsText = false;

	public bool zipStreams = false;

	public const string MAINMENU_LEVELNAME = "launchscene";

	public const string FRONTEND_LEVELNAME = "frontend";

	public const string BACKEND_LEVELNAME = "backend";

	public const string SAVE_EXTENSION = ".sav";

	public const int MAX_AUTOSAVE_FILES = 10;

	[NonSerialized]
	public SaveManager saveManager;

	private const string CorruptFileSuffix = "_";

	private bool mustRestartOnFail = false;

	public WorldGen worldGen;

	public const string METRIC_SAVED_PREFAB_KEY = "SavedPrefabs";

	public const string METRIC_IS_AUTO_SAVE_KEY = "IsAutoSave";

	public const string METRIC_WAS_DEBUG_EVER_USED = "WasDebugEverUsed";

	public const string METRIC_RESOURCES_ACCESSIBLE_KEY = "ResourcesAccessible";

	public const string METRIC_DAILY_REPORT_KEY = "DailyReport";

	public const string METRIC_MINION_METRICS_KEY = "MinionMetrics";

	public const string METRIC_CUSTOM_GAME_SETTINGS = "CustomGameSettings";

	public const string METRIC_PERFORMANCE_MEASUREMENTS = "PerformanceMeasurements";

	public const string METRIC_FRAME_TIME = "AverageFrameTime";

	private static bool force_infinity = false;

	[CompilerGenerated]
	private static Sim.GAME_MessageHandler _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Sim.GAME_MessageHandler _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Sim.GAME_MessageHandler _003C_003Ef__mg_0024cache2;

	public bool loadedFromSave
	{
		get;
		private set;
	}

	public static SaveLoader Instance
	{
		get;
		private set;
	}

	public System.Action OnWorldGenComplete
	{
		get;
		set;
	}

	public SaveGame.Header LoadedHeader
	{
		get;
		private set;
	}

	public SaveGame.GameInfo GameInfo
	{
		get;
		private set;
	}

	public GameSpawnData cachedGSD
	{
		get;
		private set;
	}

	public WorldDetailSave worldDetailSave
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		saveManager = GetComponent<SaveManager>();
	}

	private void MoveCorruptFile(string filename)
	{
		try
		{
		}
		catch
		{
			File.Replace(filename, filename + "_", filename + "_.bak", true);
		}
	}

	protected override void OnSpawn()
	{
		string activeSaveFilePath = GetActiveSaveFilePath();
		if (WorldGen.CanLoad(activeSaveFilePath))
		{
			Sim.SIM_Initialize(Sim.DLL_MessageHandler);
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			SimMessages.CreateDiseaseTable();
			loadedFromSave = true;
			loadedFromSave = Load(activeSaveFilePath);
			saveFileCorrupt = !loadedFromSave;
			if (!loadedFromSave)
			{
				SetActiveSaveFilePath(null);
				if (mustRestartOnFail)
				{
					MoveCorruptFile(activeSaveFilePath);
					Sim.Shutdown();
					App.LoadScene("frontend");
					return;
				}
			}
		}
		if (!loadedFromSave)
		{
			Sim.Shutdown();
			if (!string.IsNullOrEmpty(activeSaveFilePath))
			{
				DebugUtil.LogArgs("Couldn't load [" + activeSaveFilePath + "]");
			}
			if (saveFileCorrupt)
			{
				MoveCorruptFile(activeSaveFilePath);
			}
			bool flag = WorldGen.CanLoad(WorldGen.SIM_SAVE_FILENAME);
			if (!flag || !LoadFromWorldGen())
			{
				DebugUtil.LogWarningArgs("Couldn't start new game with current world gen, moving file");
				if (flag)
				{
					KMonoBehaviour.isLoadingScene = true;
					MoveCorruptFile(WorldGen.SIM_SAVE_FILENAME);
				}
				App.LoadScene("frontend");
			}
		}
	}

	public static byte[] CompressContents(byte[] uncompressed)
	{
		return CompressContents(uncompressed, uncompressed.Length);
	}

	public static byte[] CompressContents(byte[] uncompressed, int length)
	{
		using (MemoryStream memoryStream = new MemoryStream(length))
		{
			using (ZlibStream zlibStream = new ZlibStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestSpeed))
			{
				zlibStream.Write(uncompressed, 0, length);
				zlibStream.Flush();
			}
			memoryStream.Flush();
			return memoryStream.ToArray();
		}
	}

	private byte[] FloatToBytes(float[] floats)
	{
		byte[] array = new byte[floats.Length * 4];
		Buffer.BlockCopy(floats, 0, array, 0, array.Length);
		return array;
	}

	public static byte[] DecompressContents(byte[] compressed)
	{
		return ZlibStream.UncompressBuffer(compressed);
	}

	private float[] BytesToFloat(byte[] bytes)
	{
		float[] array = new float[bytes.Length / 4];
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		return array;
	}

	private SaveFileRoot PrepSaveFile()
	{
		SaveFileRoot saveFileRoot = new SaveFileRoot();
		saveFileRoot.WidthInCells = Grid.WidthInCells;
		saveFileRoot.HeightInCells = Grid.HeightInCells;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter writer = new BinaryWriter(memoryStream))
			{
				Sim.Save(writer);
			}
			if (zipStreams)
			{
				saveFileRoot.streamed["SimBZ"] = CompressContents(memoryStream.ToArray());
			}
			else
			{
				saveFileRoot.streamed["Sim"] = memoryStream.ToArray();
			}
		}
		if (zipStreams)
		{
			saveFileRoot.streamed["GridVisibleBZ"] = CompressContents(Grid.Visible);
			saveFileRoot.streamed["GridSpawnableBZ"] = CompressContents(Grid.Spawnable);
			saveFileRoot.streamed["GridDamageBZ"] = CompressContents(FloatToBytes(Grid.Damage));
		}
		else
		{
			saveFileRoot.streamed["GridVisible"] = Grid.Visible;
			saveFileRoot.streamed["GridSpawnable"] = Grid.Spawnable;
			saveFileRoot.streamed["GridDamage"] = FloatToBytes(Grid.Damage);
		}
		Global.Instance.modManager.SendMetricsEvent();
		saveFileRoot.active_mods = new List<Label>();
		foreach (Mod mod in Global.Instance.modManager.mods)
		{
			if (mod.enabled)
			{
				saveFileRoot.active_mods.Add(mod.label);
			}
		}
		string text = saveFileRoot.worldID = ((Game.worldID == null) ? CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World).id : Game.worldID);
		using (MemoryStream memoryStream2 = new MemoryStream())
		{
			using (BinaryWriter writer2 = new BinaryWriter(memoryStream2))
			{
				Camera.main.transform.parent.GetComponent<CameraController>().Save(writer2);
			}
			saveFileRoot.streamed["Camera"] = memoryStream2.ToArray();
		}
		return saveFileRoot;
	}

	private void Save(BinaryWriter writer)
	{
		writer.WriteKleiString("world");
		SaveFileRoot obj = PrepSaveFile();
		Serializer.Serialize(obj, writer);
		Game.SaveSettings(writer);
		saveManager.Save(writer);
		Game.Instance.Save(writer);
	}

	private bool Load(IReader reader)
	{
		string a = reader.ReadKleiString();
		Debug.Assert(a == "world");
		Deserializer deserializer = new Deserializer(reader);
		SaveFileRoot saveFileRoot = new SaveFileRoot();
		deserializer.Deserialize(saveFileRoot);
		SaveGame.GameInfo gameInfo = GameInfo;
		if (gameInfo.saveMajorVersion != 7)
		{
			SaveGame.GameInfo gameInfo2 = GameInfo;
			if (gameInfo2.saveMinorVersion >= 8)
			{
				goto IL_00ff;
			}
		}
		if (saveFileRoot.requiredMods != null)
		{
			saveFileRoot.active_mods = new List<Label>();
			foreach (ModInfo requiredMod in saveFileRoot.requiredMods)
			{
				ModInfo current = requiredMod;
				saveFileRoot.active_mods.Add(new Label
				{
					id = current.assetID,
					version = current.lastModifiedTime,
					distribution_platform = Label.DistributionPlatform.Steam,
					title = current.description
				});
			}
			saveFileRoot.requiredMods.Clear();
		}
		goto IL_00ff;
		IL_00ff:
		KMod.Manager modManager = Global.Instance.modManager;
		modManager.Load(Content.LayerableFiles);
		if (!modManager.MatchFootprint(saveFileRoot.active_mods, Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation | Content.Animation))
		{
			DebugUtil.LogWarningArgs("Mod footprint of save file doesn't match current mod configuration");
		}
		Global.Instance.modManager.SendMetricsEvent();
		string text = saveFileRoot.worldID;
		if (text == null)
		{
			try
			{
				text = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World).id;
			}
			catch
			{
				text = "worlds/SandstoneDefault";
			}
		}
		Game.worldID = text;
		worldGen = new WorldGen(text, null);
		Game.LoadSettings(deserializer);
		GridSettings.Reset(saveFileRoot.WidthInCells, saveFileRoot.HeightInCells);
		Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
		Sim.SIM_Initialize(Sim.DLL_MessageHandler);
		SimMessages.CreateSimElementsTable(ElementLoader.elements);
		SimMessages.CreateDiseaseTable();
		byte[] bytes = saveFileRoot.streamed["Sim"];
		FastReader reader2 = new FastReader(bytes);
		if (Sim.Load(reader2) == 0)
		{
			SceneInitializer.Instance.PostLoadPrefabs();
			mustRestartOnFail = true;
			if (saveManager.Load(reader))
			{
				Grid.Visible = saveFileRoot.streamed["GridVisible"];
				if (saveFileRoot.streamed.ContainsKey("GridSpawnable"))
				{
					Grid.Spawnable = saveFileRoot.streamed["GridSpawnable"];
				}
				Grid.Damage = BytesToFloat(saveFileRoot.streamed["GridDamage"]);
				Game.Instance.Load(deserializer);
				FastReader reader3 = new FastReader(saveFileRoot.streamed["Camera"]);
				CameraSaveData.Load(reader3);
				return true;
			}
			Sim.Shutdown();
			DebugUtil.LogWarningArgs("\n--- Error loading save ---\n");
			SetActiveSaveFilePath(null);
			return false;
		}
		DebugUtil.LogWarningArgs("\n--- Error loading save ---\nSimDLL found bad data\n");
		Sim.Shutdown();
		return false;
	}

	public static string GetSavePrefix()
	{
		string path = Util.RootFolder();
		return Path.Combine(path, "save_files/");
	}

	public static string GetSavePrefixAndCreateFolder()
	{
		string savePrefix = GetSavePrefix();
		if (!System.IO.Directory.Exists(savePrefix))
		{
			System.IO.Directory.CreateDirectory(savePrefix);
		}
		return savePrefix;
	}

	public static string GetAutoSavePrefix()
	{
		string text = Path.Combine(GetSavePrefixAndCreateFolder(), "auto_save/");
		if (!System.IO.Directory.Exists(text))
		{
			System.IO.Directory.CreateDirectory(text);
		}
		return text;
	}

	public static void SetActiveSaveFilePath(string path)
	{
		KPlayerPrefs.SetString("SaveFilenameKey/", path);
	}

	public static string GetActiveSaveFilePath()
	{
		return KPlayerPrefs.GetString("SaveFilenameKey/");
	}

	public static string GetAutosaveFilePath()
	{
		return GetAutoSavePrefix() + "AutoSave Cycle 1.sav";
	}

	public static string GetActiveSaveFolder()
	{
		string activeSaveFilePath = GetActiveSaveFilePath();
		if (string.IsNullOrEmpty(activeSaveFilePath))
		{
			return null;
		}
		return Path.GetDirectoryName(activeSaveFilePath);
	}

	public static List<string> GetSaveFiles(string save_dir)
	{
		List<string> list = new List<string>();
		try
		{
			if (!System.IO.Directory.Exists(save_dir))
			{
				System.IO.Directory.CreateDirectory(save_dir);
			}
			string[] files = System.IO.Directory.GetFiles(save_dir, "*.sav", SearchOption.AllDirectories);
			List<SaveFileEntry> list2 = new List<SaveFileEntry>();
			string[] array = files;
			foreach (string text in array)
			{
				try
				{
					System.DateTime lastWriteTime = File.GetLastWriteTime(text);
					SaveFileEntry saveFileEntry = default(SaveFileEntry);
					saveFileEntry.path = text;
					saveFileEntry.timeStamp = lastWriteTime;
					SaveFileEntry item = saveFileEntry;
					list2.Add(item);
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Problem reading file: " + text + "\n" + ex.ToString());
				}
			}
			list2.Sort((SaveFileEntry x, SaveFileEntry y) => y.timeStamp.CompareTo(x.timeStamp));
			foreach (SaveFileEntry item2 in list2)
			{
				SaveFileEntry current = item2;
				list.Add(current.path);
			}
		}
		catch (Exception ex2)
		{
			string text2 = null;
			if (ex2 is UnauthorizedAccessException)
			{
				text2 = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, save_dir);
			}
			else if (ex2 is IOException)
			{
				text2 = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, save_dir);
			}
			if (text2 == null)
			{
				throw ex2;
			}
			GameObject parent = (!((UnityEngine.Object)FrontEndManager.Instance == (UnityEngine.Object)null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas;
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(text2, null, null, null, null, null, null, null, null, true);
		}
		return list;
	}

	public static List<string> GetAllFiles()
	{
		return GetSaveFiles(GetSavePrefixAndCreateFolder());
	}

	public static string GetLatestSaveFile()
	{
		List<string> allFiles = GetAllFiles();
		if (allFiles.Count != 0)
		{
			return allFiles[0];
		}
		return null;
	}

	public void InitialSave()
	{
		string text = GetActiveSaveFilePath();
		if (string.IsNullOrEmpty(text))
		{
			text = GetAutosaveFilePath();
		}
		else if (!text.Contains(".sav"))
		{
			text += ".sav";
		}
		Save(text, false, true);
	}

	public string Save(string filename, bool isAutoSave = false, bool updateSavePointer = true)
	{
		KSerialization.Manager.Clear();
		ReportSaveMetrics(isAutoSave);
		RetireColonyUtility.SaveColonySummaryData();
		if (isAutoSave && !GenericGameSettings.instance.keepAllAutosaves)
		{
			List<string> saveFiles = GetSaveFiles(Path.GetDirectoryName(filename));
			for (int num = saveFiles.Count - 1; num >= 9; num--)
			{
				string text = saveFiles[num];
				try
				{
					Debug.Log("Deleting old autosave: " + text);
					File.Delete(text);
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Problem deleting autosave: " + text + "\n" + ex.ToString());
				}
				if (GenericGameSettings.instance.takeSaveScreenshots)
				{
					string text2 = Path.ChangeExtension(text, ".png");
					try
					{
						if (File.Exists(text2))
						{
							File.Delete(text2);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogWarning("Problem deleting autosave screenshot: " + text2 + "\n" + ex2.ToString());
					}
				}
			}
		}
		byte[] buffer = null;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter writer = new BinaryWriter(memoryStream))
			{
				Save(writer);
				buffer = ((!compressSaveData) ? memoryStream.ToArray() : CompressContents(memoryStream.GetBuffer(), (int)memoryStream.Length));
			}
		}
		try
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(filename, FileMode.Create)))
			{
				SaveGame.Header header;
				byte[] saveHeader = SaveGame.Instance.GetSaveHeader(isAutoSave, compressSaveData, out header);
				binaryWriter.Write(header.buildVersion);
				binaryWriter.Write(header.headerSize);
				binaryWriter.Write(header.headerVersion);
				binaryWriter.Write(header.compression);
				binaryWriter.Write(saveHeader);
				KSerialization.Manager.SerializeDirectory(binaryWriter);
				binaryWriter.Write(buffer);
				Stats.Print();
			}
		}
		catch (Exception ex3)
		{
			if (!(ex3 is UnauthorizedAccessException))
			{
				if (!(ex3 is IOException))
				{
					throw ex3;
				}
				DebugUtil.LogArgs("IOException (probably out of disk space) for " + filename);
				ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
				confirmDialogScreen.PopupConfirmDialog(string.Format(UI.CRASHSCREEN.SAVEFAILED, "IOException. You may not have enough free space!"), null, null, null, null, null, null, null, null, true);
				return GetActiveSaveFilePath();
			}
			DebugUtil.LogArgs("UnauthorizedAccessException for " + filename);
			ConfirmDialogScreen confirmDialogScreen2 = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			confirmDialogScreen2.PopupConfirmDialog(string.Format(UI.CRASHSCREEN.SAVEFAILED, "Unauthorized Access Exception"), null, null, null, null, null, null, null, null, true);
			return GetActiveSaveFilePath();
		}
		if (updateSavePointer)
		{
			SetActiveSaveFilePath(filename);
		}
		if (GenericGameSettings.instance.takeSaveScreenshots)
		{
			string filename2 = Path.ChangeExtension(filename, ".png");
			ScreenCapture.CaptureScreenshot(filename2, 1);
		}
		DebugUtil.LogArgs("Saved to", "[" + filename + "]");
		GC.Collect();
		return filename;
	}

	public static SaveGame.GameInfo LoadHeader(string filename, out SaveGame.Header header)
	{
		using (BinaryReader binaryReader = new BinaryReader(File.Open(filename, FileMode.Open)))
		{
			header = SaveGame.GetHeader(binaryReader);
			byte[] data = binaryReader.ReadBytes(header.headerSize);
			return SaveGame.GetGameInfo(data);
		}
	}

	public bool Load(string filename)
	{
		SetActiveSaveFilePath(filename);
		try
		{
			KSerialization.Manager.Clear();
			byte[] array = File.ReadAllBytes(filename);
			IReader reader = new FastReader(array);
			GameInfo = SaveGame.GetHeader(reader, out SaveGame.Header header);
			LoadedHeader = header;
			DebugUtil.LogArgs(string.Format("Loading save file: {4}\n headerVersion:{0}, buildVersion:{1}, headerSize:{2}, IsCompressed:{3}", header.headerVersion, header.buildVersion, header.headerSize, header.IsCompressed, filename));
			object[] obj = new object[1];
			object[] obj2 = new object[7];
			SaveGame.GameInfo gameInfo = GameInfo;
			obj2[0] = gameInfo.numberOfCycles;
			SaveGame.GameInfo gameInfo2 = GameInfo;
			obj2[1] = gameInfo2.numberOfDuplicants;
			SaveGame.GameInfo gameInfo3 = GameInfo;
			obj2[2] = gameInfo3.baseName;
			SaveGame.GameInfo gameInfo4 = GameInfo;
			obj2[3] = gameInfo4.isAutoSave;
			SaveGame.GameInfo gameInfo5 = GameInfo;
			obj2[4] = gameInfo5.originalSaveName;
			SaveGame.GameInfo gameInfo6 = GameInfo;
			obj2[5] = gameInfo6.saveMajorVersion;
			SaveGame.GameInfo gameInfo7 = GameInfo;
			obj2[6] = gameInfo7.saveMinorVersion;
			obj[0] = string.Format("GameInfo: numberOfCycles:{0}, numberOfDuplicants:{1}, baseName:{2}, isAutoSave:{3}, originalSaveName:{4}, saveVersion:{5}.{6}", obj2);
			DebugUtil.LogArgs(obj);
			SaveGame.GameInfo gameInfo8 = GameInfo;
			if (gameInfo8.saveMajorVersion == 7)
			{
				SaveGame.GameInfo gameInfo9 = GameInfo;
				if (gameInfo9.saveMinorVersion < 4)
				{
					Helper.SetTypeInfoMask((SerializationTypeInfo)191);
				}
			}
			KSerialization.Manager.DeserializeDirectory(reader);
			if (header.IsCompressed)
			{
				int num = array.Length - reader.Position;
				byte[] array2 = new byte[num];
				Array.Copy(array, reader.Position, array2, 0, num);
				byte[] bytes = DecompressContents(array2);
				IReader reader2 = new FastReader(bytes);
				Load(reader2);
			}
			else
			{
				Load(reader);
			}
			SaveGame.GameInfo gameInfo10 = GameInfo;
			if (gameInfo10.isAutoSave)
			{
				SaveGame.GameInfo gameInfo11 = GameInfo;
				if (!string.IsNullOrEmpty(gameInfo11.originalSaveName))
				{
					SaveGame.GameInfo gameInfo12 = GameInfo;
					SetActiveSaveFilePath(gameInfo12.originalSaveName);
				}
			}
		}
		catch (Exception ex)
		{
			DebugUtil.LogWarningArgs("\n--- Error loading save ---\n" + ex.Message + "\n" + ex.StackTrace);
			Sim.Shutdown();
			SetActiveSaveFilePath(null);
			return false;
		}
		Stats.Print();
		DebugUtil.LogArgs("Loaded", "[" + filename + "]");
		DebugUtil.LogArgs("World Seeds", "[" + worldDetailSave.globalWorldSeed + "/" + worldDetailSave.globalWorldLayoutSeed + "/" + worldDetailSave.globalTerrainSeed + "/" + worldDetailSave.globalNoiseSeed + "]");
		GC.Collect();
		return true;
	}

	public bool LoadFromWorldGen()
	{
		DebugUtil.LogArgs("Attempting to start a new game with current world gen");
		WorldGen.LoadSettings();
		string worldName;
		try
		{
			worldName = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World).id;
		}
		catch
		{
			worldName = "worlds/SandstoneDefault";
		}
		worldGen = new WorldGen(worldName, null);
		SimSaveFileStructure simSaveFileStructure = worldGen.LoadWorldGenSim();
		if (simSaveFileStructure != null)
		{
			worldDetailSave = simSaveFileStructure.worldDetail;
			if (worldDetailSave == null)
			{
				Debug.LogError("Detail is null");
			}
			GridSettings.Reset(simSaveFileStructure.WidthInCells, simSaveFileStructure.HeightInCells);
			Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
			Sim.SIM_Initialize(Sim.DLL_MessageHandler);
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			SimMessages.CreateDiseaseTable();
			try
			{
				FastReader reader = new FastReader(simSaveFileStructure.Sim);
				if (Sim.Load(reader) != 0)
				{
					DebugUtil.LogWarningArgs("\n--- Error loading save ---\nSimDLL found bad data\n");
					Sim.Shutdown();
					return false;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("--- Error loading Sim FROM NEW WORLDGEN ---" + ex.Message + "\n" + ex.StackTrace);
				Sim.Shutdown();
				return false;
			}
			Debug.Log("Attempt success");
			SceneInitializer.Instance.PostLoadPrefabs();
			SceneInitializer.Instance.NewSaveGamePrefab();
			worldGen.ReplayGenerate(Reset);
			OnWorldGenComplete.Signal();
			ThreadedHttps<KleiMetrics>.Instance.StartNewGame();
			return true;
		}
		Debug.LogError("Attempt failed");
		return false;
	}

	public void SetWorldDetail(WorldDetailSave worldDetail)
	{
		worldDetailSave = worldDetail;
	}

	private void Reset(GameSpawnData gsd)
	{
		cachedGSD = gsd;
	}

	private void ReportSaveMetrics(bool is_auto_save)
	{
		if (ThreadedHttps<KleiMetrics>.Instance != null && ThreadedHttps<KleiMetrics>.Instance.enabled && !((UnityEngine.Object)saveManager == (UnityEngine.Object)null))
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary[GameClock.NewCycleKey] = GameClock.Instance.GetCycle() + 1;
			dictionary["WasDebugEverUsed"] = Game.Instance.debugWasUsed;
			dictionary["IsAutoSave"] = is_auto_save;
			dictionary["SavedPrefabs"] = GetSavedPrefabMetrics();
			dictionary["ResourcesAccessible"] = GetWorldInventoryMetrics();
			dictionary["MinionMetrics"] = GetMinionMetrics();
			if (is_auto_save)
			{
				dictionary["DailyReport"] = GetDailyReportMetrics();
				dictionary["PerformanceMeasurements"] = GetPerformanceMeasurements();
				dictionary["AverageFrameTime"] = GetFrameTime();
			}
			dictionary["CustomGameSettings"] = CustomGameSettings.Instance.GetSettingsForMetrics();
			ThreadedHttps<KleiMetrics>.Instance.SendEvent(dictionary);
		}
	}

	private List<MinionMetricsData> GetMinionMetrics()
	{
		List<MinionMetricsData> list = new List<MinionMetricsData>();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
			{
				Modifiers component = item.gameObject.GetComponent<Modifiers>();
				Amounts amounts = component.amounts;
				List<MinionAttrFloatData> list2 = new List<MinionAttrFloatData>(amounts.Count);
				foreach (AmountInstance item2 in amounts)
				{
					float value = item2.value;
					if (!float.IsNaN(value) && !float.IsInfinity(value))
					{
						list2.Add(new MinionAttrFloatData
						{
							Name = item2.modifier.Id,
							Value = item2.value
						});
					}
				}
				MinionResume component2 = item.gameObject.GetComponent<MinionResume>();
				float totalExperienceGained = component2.TotalExperienceGained;
				List<string> list3 = new List<string>();
				foreach (KeyValuePair<string, bool> item3 in component2.MasteryBySkillID)
				{
					if (item3.Value)
					{
						list3.Add(item3.Key);
					}
				}
				list.Add(new MinionMetricsData
				{
					Name = item.name,
					Modifiers = list2,
					TotalExperienceGained = totalExperienceGained,
					Skills = list3
				});
			}
		}
		return list;
	}

	private List<SavedPrefabMetricsData> GetSavedPrefabMetrics()
	{
		Dictionary<Tag, List<SaveLoadRoot>> lists = saveManager.GetLists();
		List<SavedPrefabMetricsData> list = new List<SavedPrefabMetricsData>(lists.Count);
		foreach (KeyValuePair<Tag, List<SaveLoadRoot>> item in lists)
		{
			Tag key = item.Key;
			List<SaveLoadRoot> value = item.Value;
			if (value.Count > 0)
			{
				list.Add(new SavedPrefabMetricsData
				{
					PrefabName = key.ToString(),
					Count = value.Count
				});
			}
		}
		return list;
	}

	private List<WorldInventoryMetricsData> GetWorldInventoryMetrics()
	{
		Dictionary<Tag, float> accessibleAmounts = WorldInventory.Instance.GetAccessibleAmounts();
		List<WorldInventoryMetricsData> list = new List<WorldInventoryMetricsData>(accessibleAmounts.Count);
		foreach (KeyValuePair<Tag, float> item in accessibleAmounts)
		{
			float value = item.Value;
			if (!float.IsInfinity(value) && !float.IsNaN(value))
			{
				list.Add(new WorldInventoryMetricsData
				{
					Name = item.Key.ToString(),
					Amount = value
				});
			}
		}
		return list;
	}

	private List<DailyReportMetricsData> GetDailyReportMetrics()
	{
		List<DailyReportMetricsData> list = new List<DailyReportMetricsData>();
		int cycle = GameClock.Instance.GetCycle();
		ReportManager.DailyReport dailyReport = ReportManager.Instance.FindReport(cycle);
		if (dailyReport != null)
		{
			foreach (ReportManager.ReportEntry reportEntry in dailyReport.reportEntries)
			{
				DailyReportMetricsData item = default(DailyReportMetricsData);
				item.Name = reportEntry.reportType.ToString();
				if (!float.IsInfinity(reportEntry.Net) && !float.IsNaN(reportEntry.Net))
				{
					item.Net = reportEntry.Net;
				}
				if (force_infinity)
				{
					item.Net = null;
				}
				if (!float.IsInfinity(reportEntry.Positive) && !float.IsNaN(reportEntry.Positive))
				{
					item.Positive = reportEntry.Positive;
				}
				if (!float.IsInfinity(reportEntry.Negative) && !float.IsNaN(reportEntry.Negative))
				{
					item.Negative = reportEntry.Negative;
				}
				list.Add(item);
			}
			list.Add(new DailyReportMetricsData
			{
				Name = "MinionCount",
				Net = new float?((float)Components.LiveMinionIdentities.Count),
				Positive = new float?(0f),
				Negative = new float?(0f)
			});
		}
		return list;
	}

	private List<PerformanceMeasurement> GetPerformanceMeasurements()
	{
		List<PerformanceMeasurement> list = new List<PerformanceMeasurement>();
		if ((UnityEngine.Object)Global.Instance != (UnityEngine.Object)null)
		{
			PerformanceMonitor component = Global.Instance.GetComponent<PerformanceMonitor>();
			list.Add(new PerformanceMeasurement
			{
				name = "FramesAbove30",
				value = (float)(double)component.NumFramesAbove30
			});
			list.Add(new PerformanceMeasurement
			{
				name = "FramesBelow30",
				value = (float)(double)component.NumFramesBelow30
			});
			component.Reset();
		}
		return list;
	}

	private float GetFrameTime()
	{
		PerformanceMonitor component = Global.Instance.GetComponent<PerformanceMonitor>();
		DebugUtil.LogArgs("Average frame time:", 1f / component.FPS);
		return 1f / component.FPS;
	}
}
