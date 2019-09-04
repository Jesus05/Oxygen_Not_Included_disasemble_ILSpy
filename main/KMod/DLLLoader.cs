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
								ListPool<MethodInfo, Manager>.PooledList pooledList = ListPool<MethodInfo, Manager>.Allocate();
								ListPool<MethodInfo, Manager>.PooledList pooledList2 = ListPool<MethodInfo, Manager>.Allocate();
								ListPool<MethodInfo, Manager>.PooledList pooledList3 = ListPool<MethodInfo, Manager>.Allocate();
								ListPool<MethodInfo, Manager>.PooledList pooledList4 = ListPool<MethodInfo, Manager>.Allocate();
								Type[] types = new Type[0];
								Type[] types2 = new Type[1]
								{
									typeof(string)
								};
								Type[] types3 = new Type[1]
								{
									typeof(HarmonyInstance)
								};
								MethodInfo methodInfo = null;
								foreach (Assembly item in list)
								{
									Type[] types4 = item.GetTypes();
									foreach (Type type in types4)
									{
										if (type != null)
										{
											methodInfo = type.GetMethod("OnLoad", types);
											if (methodInfo != null)
											{
												pooledList3.Add(methodInfo);
											}
											methodInfo = type.GetMethod("OnLoad", types2);
											if (methodInfo != null)
											{
												pooledList4.Add(methodInfo);
											}
											methodInfo = type.GetMethod("PrePatch", types3);
											if (methodInfo != null)
											{
												pooledList.Add(methodInfo);
											}
											methodInfo = type.GetMethod("PostPatch", types3);
											if (methodInfo != null)
											{
												pooledList2.Add(methodInfo);
											}
										}
									}
								}
								HarmonyInstance harmonyInstance = HarmonyInstance.Create($"OxygenNotIncluded_v{0}.{1}");
								if (harmonyInstance != null)
								{
									object[] parameters = new object[1]
									{
										harmonyInstance
									};
									foreach (MethodInfo item2 in pooledList)
									{
										item2.Invoke(null, parameters);
									}
									foreach (Assembly item3 in list)
									{
										harmonyInstance.PatchAll(item3);
									}
									foreach (MethodInfo item4 in pooledList2)
									{
										item4.Invoke(null, parameters);
									}
								}
								pooledList.Recycle();
								pooledList2.Recycle();
								foreach (MethodInfo item5 in pooledList3)
								{
									item5.Invoke(null, null);
								}
								object[] parameters2 = new object[1]
								{
									path
								};
								foreach (MethodInfo item6 in pooledList4)
								{
									item6.Invoke(null, parameters2);
								}
								pooledList3.Recycle();
								pooledList4.Recycle();
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
