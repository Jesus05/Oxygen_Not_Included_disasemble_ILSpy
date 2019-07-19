using KSerialization;
using Newtonsoft.Json;
using ProcGenGame;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

[SerializationConfig(KSerialization.MemberSerialization.OptIn)]
public class SaveGame : KMonoBehaviour, ISaveLoadable
{
	public struct Header
	{
		public uint buildVersion;

		public int headerSize;

		public uint headerVersion;

		public int compression;

		public bool IsCompressed => 0 != compression;
	}

	public struct GameInfo
	{
		public int numberOfCycles;

		public int numberOfDuplicants;

		public string baseName;

		public bool isAutoSave;

		public string originalSaveName;

		public int saveMajorVersion;

		public int saveMinorVersion;

		public GameInfo(int numberOfCycles, int numberOfDuplicants, string baseName, bool isAutoSave, string originalSaveName, bool sandboxEnabled = false)
		{
			this.numberOfCycles = numberOfCycles;
			this.numberOfDuplicants = numberOfDuplicants;
			this.baseName = baseName;
			this.isAutoSave = isAutoSave;
			this.originalSaveName = originalSaveName;
			saveMajorVersion = 7;
			saveMinorVersion = 11;
		}

		public GameInfo(int numberOfCycles, int numberOfDuplicants, string baseName, bool sandboxEnabled = false)
		{
			this.numberOfCycles = numberOfCycles;
			this.numberOfDuplicants = numberOfDuplicants;
			this.baseName = baseName;
			isAutoSave = false;
			originalSaveName = "";
			saveMajorVersion = 7;
			saveMinorVersion = 11;
		}

		public bool IsVersionOlderThan(int major, int minor)
		{
			return saveMajorVersion < major || (saveMajorVersion == major && saveMinorVersion < minor);
		}

		public bool IsVersionExactly(int major, int minor)
		{
			return saveMajorVersion == major && saveMinorVersion == minor;
		}
	}

	[Serialize]
	private int speed;

	[Serialize]
	public List<Tag> expandedResourceTags = new List<Tag>();

	[Serialize]
	public int minGermCountForDisinfect = 10000;

	[Serialize]
	public bool enableAutoDisinfect = true;

	[Serialize]
	public bool sandboxEnabled = false;

	[Serialize]
	public int autoSaveCycleInterval = 1;

	[Serialize]
	public Vector2I timelapseResolution = new Vector2I(640, 360);

	private string baseName;

	public static SaveGame Instance;

	public EntombedItemManager entombedItemManager;

	public WorldGenSpawner worldGenSpawner;

	[MyCmpReq]
	public MaterialSelectorSerializer materialSelectorSerializer;

	public WorldGen worldGen;

	public string BaseName => baseName;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		ColonyRationMonitor.Instance instance = new ColonyRationMonitor.Instance(this);
		instance.StartSM();
		VignetteManager.Instance instance2 = new VignetteManager.Instance(this);
		instance2.StartSM();
		entombedItemManager = base.gameObject.AddComponent<EntombedItemManager>();
		worldGen = SaveLoader.Instance.worldGen;
		worldGenSpawner = base.gameObject.AddComponent<WorldGenSpawner>();
	}

	[OnSerializing]
	private void OnSerialize()
	{
		speed = SpeedControlScreen.Instance.GetSpeed();
	}

	[OnDeserializing]
	private void OnDeserialize()
	{
		GameInfo gameInfo = SaveLoader.Instance.GameInfo;
		baseName = gameInfo.baseName;
	}

	public int GetSpeed()
	{
		return speed;
	}

	public byte[] GetSaveHeader(bool isAutoSave, bool isCompressed, out Header header)
	{
		string text = null;
		text = ((!isAutoSave) ? JsonConvert.SerializeObject(new GameInfo(GameClock.Instance.GetCycle(), Components.LiveMinionIdentities.Count, baseName, false)) : JsonConvert.SerializeObject(new GameInfo(GameClock.Instance.GetCycle(), Components.LiveMinionIdentities.Count, baseName, true, SaveLoader.GetActiveSaveFilePath(), false)));
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		header = default(Header);
		header.buildVersion = 353289u;
		header.headerSize = bytes.Length;
		header.headerVersion = 1u;
		header.compression = (isCompressed ? 1 : 0);
		return bytes;
	}

	public static Header GetHeader(BinaryReader br)
	{
		Header result = default(Header);
		result.buildVersion = br.ReadUInt32();
		result.headerSize = br.ReadInt32();
		result.headerVersion = br.ReadUInt32();
		if (1 <= result.headerVersion)
		{
			result.compression = br.ReadInt32();
		}
		return result;
	}

	public static GameInfo GetHeader(IReader br, out Header header)
	{
		header = default(Header);
		header.buildVersion = br.ReadUInt32();
		header.headerSize = br.ReadInt32();
		header.headerVersion = br.ReadUInt32();
		if (1 <= header.headerVersion)
		{
			header.compression = br.ReadInt32();
		}
		byte[] data = br.ReadBytes(header.headerSize);
		return GetGameInfo(data);
	}

	public static GameInfo GetGameInfo(byte[] data)
	{
		return JsonConvert.DeserializeObject<GameInfo>(Encoding.UTF8.GetString(data));
	}

	public void SetBaseName(string newBaseName)
	{
		if (string.IsNullOrEmpty(newBaseName))
		{
			UnityEngine.Debug.LogWarning("Cannot give the base an empty name");
		}
		else
		{
			baseName = newBaseName;
		}
	}

	protected override void OnSpawn()
	{
		ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
		Game.Instance.Trigger(-1917495436, null);
	}
}
