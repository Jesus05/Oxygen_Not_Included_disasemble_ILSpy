using Harmony;
using Ionic.Zip;
using Klei;
using Newtonsoft.Json;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

internal class ModDB
{
	private struct ModAssemblyInfo
	{
		public Assembly assembly;

		public int infoIdx;

		public string modDir;
	}

	private struct MethodInfoQueryData
	{
		public string methodName;

		public Type[] parameterTypes;

		public MethodInfoQueryData(string method_name, Type[] parameter_types)
		{
			methodName = method_name;
			parameterTypes = parameter_types;
		}
	}

	[JsonProperty]
	private List<ModInfo> mods = new List<ModInfo>();

	private const int MAJOR_VERSION = 0;

	private const int MINOR_VERSION = 1;

	public List<ModInfo> Mods
	{
		get;
		set;
	}

	public ModDB(string filename, string mods_root)
	{
		Load(filename);
		Start(mods_root);
	}

	public ICollection<ModInfo> GetMods()
	{
		return mods;
	}

	private void Load(string moddb_filename)
	{
		try
		{
			if (File.Exists(moddb_filename))
			{
				string value = File.ReadAllText(moddb_filename);
				ModInfo[] collection = JsonConvert.DeserializeObject<ModInfo[]>(value);
				mods.Clear();
				mods.AddRange(collection);
			}
		}
		catch
		{
			mods = new List<ModInfo>();
			ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas);
			confirmDialogScreen.PopupConfirmDialog(string.Format(UI.FRONTEND.MODS.DB_CORRUPT, moddb_filename), null, null, null, null, null, null, null, null);
			UnityEngine.Object.DontDestroyOnLoad(confirmDialogScreen.gameObject);
		}
	}

	public bool Save(string moddb_filename)
	{
		moddb_filename = Path.GetFullPath(moddb_filename);
		string directoryName = Path.GetDirectoryName(moddb_filename);
		if (!FileUtil.CreateDirectory(directoryName))
		{
			return false;
		}
		using (FileStream fileStream = FileUtil.Create(moddb_filename))
		{
			if (fileStream == null)
			{
				return false;
			}
			using (StreamWriter streamWriter = new StreamWriter(fileStream))
			{
				string value = JsonConvert.SerializeObject(mods, Formatting.Indented);
				streamWriter.Write(value);
			}
		}
		return true;
	}

	public void Start(string mods_root)
	{
		List<ModInfo> list = new List<ModInfo>();
		for (int i = 0; i < mods.Count; i++)
		{
			ModInfo modInfo = mods[i];
			bool flag = true;
			if (modInfo.markedForDelete)
			{
				string modDir = GetModDir(mods_root, modInfo);
				FileUtil.DeleteDirectory(modDir);
				if (!File.Exists(modDir))
				{
					flag = false;
				}
			}
			if (modInfo.markedForUpdate)
			{
				string modDir2 = GetModDir(mods_root, modInfo);
				if (InstallCodeMod(modInfo, modDir2))
				{
					modInfo.markedForUpdate = false;
					mods[i] = modInfo;
				}
			}
			if (flag)
			{
				list.Add(modInfo);
			}
		}
		mods = list;
	}

	public void LoadMods(ModManager mgr, string mods_root)
	{
		if (!StartModLoader())
		{
			Console.Out.WriteLine("Failed to use ModLoader. Using built in mod system instead.");
			List<ModAssemblyInfo> list = new List<ModAssemblyInfo>();
			HarmonyInstance harmonyInstance = HarmonyInstance.Create($"OxygenNotIncluded_v{0}.{1}");
			for (int i = 0; i < mods.Count; i++)
			{
				ModInfo modInfo = mods[i];
				if (modInfo.enabled)
				{
					string modDir = GetModDir(mods_root, modInfo);
					if (Directory.Exists(modDir))
					{
						string[] files = Directory.GetFiles(modDir, "*.dll");
						string[] array = files;
						foreach (string path in array)
						{
							try
							{
								string fullPath = Path.GetFullPath(path);
								Output.Log(string.Format("Loading MOD: {0}, {1}, {2}", modInfo.assetID, (modInfo.description != null) ? modInfo.description : "no desc", fullPath));
								Assembly assembly = Assembly.LoadFrom(fullPath);
								if (assembly != null)
								{
									harmonyInstance?.PatchAll(assembly);
									list.Add(new ModAssemblyInfo
									{
										assembly = assembly,
										infoIdx = i,
										modDir = modDir
									});
									goto IL_01fc;
								}
							}
							catch (Exception ex)
							{
								modInfo.enabled = false;
								mods[i] = modInfo;
								ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas);
								confirmDialogScreen.PopupConfirmDialog(string.Format(UI.FRONTEND.MODS.FAILED_TO_LOAD, modInfo.assetID, modInfo.description, ex.ToString()), null, null, null, null, null, null, null, null);
								UnityEngine.Object.DontDestroyOnLoad(confirmDialogScreen.gameObject);
								goto IL_01fc;
							}
						}
					}
					else
					{
						Output.Log("Disabling mod because its directory does not exist: " + modDir);
						modInfo.enabled = false;
						mods[i] = modInfo;
					}
				}
				IL_01fc:;
			}
			MethodInfoQueryData[] array2 = new MethodInfoQueryData[2]
			{
				new MethodInfoQueryData("OnLoad", new Type[0]),
				new MethodInfoQueryData("OnLoad", new Type[1]
				{
					typeof(string)
				})
			};
			foreach (ModAssemblyInfo item in list)
			{
				ModAssemblyInfo current = item;
				Type[] types = current.assembly.GetTypes();
				foreach (Type type in types)
				{
					if (type != null)
					{
						try
						{
							MethodInfoQueryData[] array3 = array2;
							for (int l = 0; l < array3.Length; l++)
							{
								MethodInfoQueryData methodInfoQueryData = array3[l];
								MethodInfo method = type.GetMethod(methodInfoQueryData.methodName, methodInfoQueryData.parameterTypes);
								if (method != null)
								{
									method.Invoke(null, new object[1]
									{
										current.modDir
									});
									break;
								}
							}
						}
						catch (Exception ex2)
						{
							ModInfo value = mods[current.infoIdx];
							value.enabled = false;
							mods[current.infoIdx] = value;
							ConfirmDialogScreen confirmDialogScreen2 = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas);
							confirmDialogScreen2.PopupConfirmDialog(string.Format(UI.FRONTEND.MODS.FAILED_TO_LOAD, value.assetID, value.description, ex2.ToString()), null, null, null, null, null, null, null, null);
							UnityEngine.Object.DontDestroyOnLoad(confirmDialogScreen2.gameObject);
						}
					}
				}
			}
		}
	}

	private bool StartModLoader()
	{
		bool result = false;
		string path = Path.Combine(Application.dataPath, "Managed/ModLoader.dll");
		if (File.Exists(path))
		{
			try
			{
				Assembly assembly = Assembly.LoadFile(path);
				if (assembly == null)
				{
					return result;
				}
				Type type = assembly.GetType("ModLoader.ModLoader");
				if (type == null)
				{
					return result;
				}
				MethodInfo method = type.GetMethod("Start");
				if (method == null)
				{
					return result;
				}
				method.Invoke(null, null);
				Console.Out.WriteLine("Started ModLoader.dll");
				result = true;
				return result;
			}
			catch (Exception ex)
			{
				Console.Out.WriteLine(ex.ToString());
				return result;
			}
		}
		return result;
	}

	public bool IsInstalled(ModInfo info)
	{
		return mods.Contains(info);
	}

	public void Install(string mods_root, ModInfo mod_info)
	{
		bool flag = false;
		string modDir = GetModDir(mods_root, mod_info);
		int num = mods.IndexOf(mod_info);
		if (num >= 0)
		{
			flag = true;
			ModInfo value = mods[num];
			value.markedForDelete = false;
			bool markedForUpdate = true;
			if (Directory.Exists(modDir))
			{
				System.DateTime lastWriteTime = Directory.GetLastWriteTime(modDir);
				System.DateTime t = UnixTimeStampToDateTime((double)mod_info.lastModifiedTime);
				if (lastWriteTime > t)
				{
					markedForUpdate = false;
				}
			}
			value.markedForUpdate = markedForUpdate;
			if (value.markedForUpdate)
			{
				Output.Log($"Mod marked for update: {mod_info.assetID}");
			}
			mods[num] = value;
		}
		if (!flag)
		{
			ModInfo.ModType type = mod_info.type;
			if (type == ModInfo.ModType.Mod && InstallCodeMod(mod_info, modDir))
			{
				mods.Add(mod_info);
			}
		}
	}

	public bool Uninstall(string mods_root, ModInfo mod_info)
	{
		int num = mods.IndexOf(mod_info);
		if (num < 0)
		{
			return false;
		}
		ModInfo value = mods[num];
		value.enabled = false;
		value.markedForDelete = true;
		mods[num] = value;
		return true;
	}

	public bool IsEnabled(ModInfo mod_info)
	{
		int num = mods.IndexOf(mod_info);
		if (num < 0)
		{
			return false;
		}
		ModInfo modInfo = mods[num];
		return modInfo.enabled;
	}

	public bool Enable(ModInfo mod_info)
	{
		int num = mods.IndexOf(mod_info);
		if (num < 0)
		{
			return false;
		}
		ModInfo value = mods[num];
		value.enabled = true;
		mods[num] = value;
		return true;
	}

	public bool Disable(ModInfo mod_info)
	{
		int num = mods.IndexOf(mod_info);
		if (num < 0)
		{
			return false;
		}
		ModInfo value = mods[num];
		value.enabled = false;
		mods[num] = value;
		return true;
	}

	public string GetModDir(string root, ModInfo info)
	{
		return Path.Combine(root, info.assetID);
	}

	public void Reorder(int a_idx, int b_idx)
	{
		if (a_idx >= 0 && b_idx >= 0)
		{
			ModInfo value = mods[a_idx];
			mods[a_idx] = mods[b_idx];
			mods[b_idx] = value;
		}
	}

	private bool InstallCodeMod(ModInfo info, string dest_path)
	{
		Output.Log($"Installing Mod: {info.assetID}");
		bool result = false;
		ModInfo.Source source = info.source;
		if (source == ModInfo.Source.Steam)
		{
			if (Directory.Exists(dest_path))
			{
				Directory.Delete(dest_path, true);
			}
			if (FileUtil.CreateDirectory(dest_path))
			{
				string assetPath = info.assetPath;
				if (File.Exists(assetPath))
				{
					Output.Log($"Extracting {assetPath} to {dest_path}");
					using (ZipFile zipFile = ZipFile.Read(assetPath))
					{
						zipFile.ExtractAll(dest_path, ExtractExistingFileAction.OverwriteSilently);
						return true;
					}
				}
			}
		}
		return result;
	}

	public static System.DateTime UnixTimeStampToDateTime(double unixTimeStamp)
	{
		System.DateTime result = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		result = result.AddSeconds(unixTimeStamp).ToLocalTime();
		return result;
	}
}
