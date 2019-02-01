using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class GarbageProfiler
{
	private class InstanceCountComparer : IComparer<MemorySnapshot.TypeData>
	{
		public int Compare(MemorySnapshot.TypeData a, MemorySnapshot.TypeData b)
		{
			return b.instanceCount - a.instanceCount;
		}
	}

	private class RefCountComparer : IComparer<MemorySnapshot.TypeData>
	{
		public int Compare(MemorySnapshot.TypeData a, MemorySnapshot.TypeData b)
		{
			return b.refCount - a.refCount;
		}
	}

	private class FieldCountComparer : IComparer<MemorySnapshot.FieldCount>
	{
		public int Compare(MemorySnapshot.FieldCount a, MemorySnapshot.FieldCount b)
		{
			return b.count - a.count;
		}
	}

	private static MemorySnapshot previousSnapshot;

	private static string ROOT_MEMORY_DUMP_PATH = "./memory/";

	private static string filename_suffix = null;

	private static Type DEBUG_STATIC_TYPE = null;

	private static void UnloadUnusedAssets()
	{
		Resources.UnloadUnusedAssets();
	}

	private static void ClearFileName()
	{
		filename_suffix = null;
	}

	public static string GetFileName(string name)
	{
		string fullPath = Path.GetFullPath(ROOT_MEMORY_DUMP_PATH);
		if (filename_suffix == null)
		{
			if (!Directory.Exists(fullPath))
			{
				Directory.CreateDirectory(fullPath);
			}
			System.DateTime now = System.DateTime.Now;
			filename_suffix = "_" + now.Year.ToString() + "-" + now.Month.ToString() + "-" + now.Day.ToString() + "_" + now.Hour.ToString() + "-" + now.Minute.ToString() + "-" + now.Second.ToString() + ".csv";
		}
		return Path.Combine(fullPath, name + filename_suffix);
	}

	private static void Dump()
	{
		Debug.Log("Writing snapshot...", null);
		MemorySnapshot memorySnapshot = new MemorySnapshot();
		ClearFileName();
		MemorySnapshot.TypeData[] array = new MemorySnapshot.TypeData[memorySnapshot.types.Count];
		memorySnapshot.types.Values.CopyTo(array, 0);
		Array.Sort(array, 0, array.Length, new InstanceCountComparer());
		using (StreamWriter streamWriter = new StreamWriter(GetFileName("memory_instances")))
		{
			streamWriter.WriteLine("Delta,Instances,NumArrayEntries,Type Name");
			MemorySnapshot.TypeData[] array2 = array;
			foreach (MemorySnapshot.TypeData typeData in array2)
			{
				if (typeData.instanceCount != 0)
				{
					int num = typeData.instanceCount;
					if (previousSnapshot != null)
					{
						MemorySnapshot.TypeData typeData2 = MemorySnapshot.GetTypeData(typeData.type, previousSnapshot.types);
						num = typeData.instanceCount - typeData2.instanceCount;
					}
					streamWriter.WriteLine(num + "," + typeData.instanceCount + "," + typeData.numArrayEntries + ",\"" + typeData.type.ToString() + "\"");
				}
			}
		}
		using (StreamWriter streamWriter2 = new StreamWriter(GetFileName("memory_hierarchies")))
		{
			streamWriter2.WriteLine("Delta,Count,Type Hierarchy");
			MemorySnapshot.TypeData[] array3 = array;
			foreach (MemorySnapshot.TypeData typeData3 in array3)
			{
				if (typeData3.instanceCount != 0)
				{
					foreach (KeyValuePair<MemorySnapshot.HierarchyNode, int> hierarchy in typeData3.hierarchies)
					{
						int num2 = hierarchy.Value;
						if (previousSnapshot != null)
						{
							MemorySnapshot.TypeData typeData4 = MemorySnapshot.GetTypeData(typeData3.type, previousSnapshot.types);
							int value = 0;
							if (typeData4.hierarchies.TryGetValue(hierarchy.Key, out value))
							{
								num2 = hierarchy.Value - value;
							}
						}
						streamWriter2.WriteLine(num2 + "," + hierarchy.Value + ", \"" + typeData3.type.ToString() + ": " + hierarchy.Key.ToString() + "\"");
					}
				}
			}
		}
		previousSnapshot = memorySnapshot;
		Debug.Log("Done writing snapshot!", null);
	}

	public static void DebugDumpGarbageStats()
	{
		Debug.Log("Writing reference stats...", null);
		MemorySnapshot memorySnapshot = new MemorySnapshot();
		ClearFileName();
		MemorySnapshot.TypeData[] array = new MemorySnapshot.TypeData[memorySnapshot.types.Count];
		memorySnapshot.types.Values.CopyTo(array, 0);
		Array.Sort(array, 0, array.Length, new InstanceCountComparer());
		using (StreamWriter streamWriter = new StreamWriter(GetFileName("garbage_instances")))
		{
			MemorySnapshot.TypeData[] array2 = array;
			foreach (MemorySnapshot.TypeData typeData in array2)
			{
				if (typeData.instanceCount != 0)
				{
					int num = typeData.instanceCount;
					if (previousSnapshot != null)
					{
						MemorySnapshot.TypeData typeData2 = MemorySnapshot.GetTypeData(typeData.type, previousSnapshot.types);
						num = typeData.instanceCount - typeData2.instanceCount;
					}
					streamWriter.WriteLine(num + ", " + typeData.instanceCount + ", \"" + typeData.type.ToString() + "\"");
				}
			}
		}
		Array.Sort(array, 0, array.Length, new RefCountComparer());
		using (StreamWriter streamWriter2 = new StreamWriter(GetFileName("garbage_refs")))
		{
			MemorySnapshot.TypeData[] array3 = array;
			foreach (MemorySnapshot.TypeData typeData3 in array3)
			{
				if (typeData3.refCount != 0)
				{
					int num2 = typeData3.refCount;
					if (previousSnapshot != null)
					{
						MemorySnapshot.TypeData typeData4 = MemorySnapshot.GetTypeData(typeData3.type, previousSnapshot.types);
						num2 = typeData3.refCount - typeData4.refCount;
					}
					streamWriter2.WriteLine(num2 + ", " + typeData3.refCount + ", \"" + typeData3.type.ToString() + "\"");
				}
			}
		}
		MemorySnapshot.FieldCount[] array4 = new MemorySnapshot.FieldCount[memorySnapshot.fieldCounts.Count];
		memorySnapshot.fieldCounts.Values.CopyTo(array4, 0);
		Array.Sort(array4, 0, array4.Length, new FieldCountComparer());
		using (StreamWriter streamWriter3 = new StreamWriter(GetFileName("garbage_fields")))
		{
			MemorySnapshot.FieldCount[] array5 = array4;
			foreach (MemorySnapshot.FieldCount fieldCount in array5)
			{
				int num3 = fieldCount.count;
				if (previousSnapshot != null)
				{
					foreach (KeyValuePair<int, MemorySnapshot.FieldCount> fieldCount2 in previousSnapshot.fieldCounts)
					{
						if (fieldCount2.Value.name == fieldCount.name)
						{
							num3 = fieldCount.count - fieldCount2.Value.count;
							break;
						}
					}
				}
				streamWriter3.WriteLine(num3 + ", " + fieldCount.count + ", \"" + fieldCount.name + "\"");
			}
		}
		memorySnapshot.WriteTypeDetails(previousSnapshot);
		previousSnapshot = memorySnapshot;
		Debug.Log("Done writing reference stats!", null);
	}

	public static void DebugDumpRootItems()
	{
		Debug.Log("Writing root items...", null);
		Type[] array = new Type[11]
		{
			typeof(string),
			typeof(HashedString),
			typeof(KAnimHashedString),
			typeof(Tag),
			typeof(bool),
			typeof(CellOffset),
			typeof(Color),
			typeof(Color32),
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector2I)
		};
		Type[] array2 = new Type[3]
		{
			typeof(List<>),
			typeof(HashSet<>),
			typeof(Dictionary<, >)
		};
		string fileName = GetFileName("statics");
		ClearFileName();
		using (StreamWriter streamWriter = new StreamWriter(fileName))
		{
			streamWriter.WriteLine("FieldName,Type,ListLength");
			Assembly[] array3 = new Assembly[2]
			{
				Assembly.GetAssembly(typeof(Game)),
				Assembly.GetAssembly(typeof(App))
			};
			Assembly[] array4 = array3;
			foreach (Assembly assembly in array4)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (type == DEBUG_STATIC_TYPE)
					{
						Debugger.Break();
					}
					if (!type.IsAbstract && !type.IsGenericType)
					{
						string text = type.ToString();
						if (!text.StartsWith("STRINGS."))
						{
							FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
							foreach (FieldInfo fieldInfo in fields)
							{
								if (fieldInfo.IsStatic && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral && !fieldInfo.Name.Contains("$cache"))
								{
									Type fieldType = fieldInfo.FieldType;
									if (!fieldType.IsPointer && !Helper.IsPOD(fieldType) && Array.IndexOf(array, fieldType) < 0)
									{
										if (typeof(Array).IsAssignableFrom(fieldType))
										{
											Type elementType = fieldType.GetElementType();
											if (elementType.IsPointer || Helper.IsPOD(elementType) || Array.IndexOf(array, elementType) >= 0)
											{
												continue;
											}
										}
										if (fieldType.IsGenericType)
										{
											Type genericTypeDefinition = fieldType.GetGenericTypeDefinition();
											Type[] genericArguments = fieldType.GetGenericArguments();
											bool flag = false;
											Type[] array5 = array2;
											foreach (Type type2 in array5)
											{
												if (genericTypeDefinition == type2)
												{
													bool flag2 = true;
													Type[] array6 = genericArguments;
													foreach (Type type3 in array6)
													{
														if (!Helper.IsPOD(type3) && Array.IndexOf(array, type3) < 0)
														{
															flag2 = false;
															break;
														}
													}
													if (flag2)
													{
														flag = true;
														break;
													}
												}
											}
											if (flag)
											{
												continue;
											}
										}
										object value = fieldInfo.GetValue(null);
										if (value != null)
										{
											string value2;
											if (typeof(ICollection).IsAssignableFrom(fieldType))
											{
												ICollection collection = value as ICollection;
												int count = collection.Count;
												value2 = $"\"{type}.{fieldInfo.Name}\",\"{fieldType}\",{count}";
											}
											else
											{
												value2 = $"\"{type}.{fieldInfo.Name}\",\"{fieldType}\"";
											}
											streamWriter.WriteLine(value2);
										}
									}
								}
							}
						}
					}
				}
			}
		}
		Debug.Log("Done writing reference stats!", null);
	}
}
