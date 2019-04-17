using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KMod
{
	internal static class DLLLoader
	{
		private const string managed_path = "Managed";

		public static bool LoadUserModLoaderDLL()
		{
			try
			{
				string path = Path.Combine(Path.Combine(Application.dataPath, "Managed"), "ModLoader.dll");
				if (File.Exists(path))
				{
					Assembly assembly = Assembly.LoadFile(path);
					if (assembly != null)
					{
						Type type = assembly.GetType("ModLoader.ModLoader");
						if (type != null)
						{
							MethodInfo method = type.GetMethod("Start");
							if (method != null)
							{
								method.Invoke(null, null);
								Debug.Log("Successfully started ModLoader.dll");
								return true;
							}
							return false;
						}
						return false;
					}
					return false;
				}
				return false;
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
			return false;
		}

		public static bool LoadDLLs(string path)
		{
			try
			{
				if (Testing.dll_loading != Testing.DLLLoading.Fail)
				{
					if (Testing.dll_loading != Testing.DLLLoading.UseModLoaderDLLExclusively)
					{
						Debug.LogFormat("Using built-in mod system...looking for DLL mods in {0}", Manager.GetDirectory());
						DirectoryInfo directoryInfo = new DirectoryInfo(path);
						if (directoryInfo.Exists)
						{
							List<Assembly> list = new List<Assembly>();
							FileInfo[] files = directoryInfo.GetFiles();
							foreach (FileInfo fileInfo in files)
							{
								if (fileInfo.Name.ToLower().EndsWith(".dll"))
								{
									Debug.Log($"Loading MOD dll: {fileInfo.Name}");
									Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
									if (assembly != null)
									{
										list.Add(assembly);
									}
								}
							}
							if (list.Count != 0)
							{
								HarmonyInstance harmonyInstance = HarmonyInstance.Create($"OxygenNotIncluded_v{0}.{1}");
								if (harmonyInstance != null)
								{
									foreach (Assembly item in list)
									{
										harmonyInstance.PatchAll(item);
									}
								}
								Type[] types = new Type[0];
								Type[] types2 = new Type[1]
								{
									typeof(string)
								};
								object[] parameters = new object[1]
								{
									path
								};
								foreach (Assembly item2 in list)
								{
									Type[] types3 = item2.GetTypes();
									foreach (Type type in types3)
									{
										if (type != null)
										{
											type.GetMethod("OnLoad", types)?.Invoke(null, null);
											type.GetMethod("OnLoad", types2)?.Invoke(null, parameters);
										}
									}
								}
								return true;
							}
							return false;
						}
						return false;
					}
					return false;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}
	}
}
