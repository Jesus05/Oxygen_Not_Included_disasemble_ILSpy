using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MemorySnapshot
{
	public struct HierarchyNode
	{
		public Type parent0;

		public Type parent1;

		public Type parent2;

		public Type parent3;

		public Type parent4;

		public HierarchyNode(Type parent_0, Type parent_1, Type parent_2, Type parent_3, Type parent_4)
		{
			parent0 = parent_0;
			parent1 = parent_1;
			parent2 = parent_2;
			parent3 = parent_3;
			parent4 = parent_4;
		}

		public bool Equals(HierarchyNode a, HierarchyNode b)
		{
			return a.parent0 == b.parent0 && a.parent1 == b.parent1 && a.parent2 == b.parent2 && a.parent3 == b.parent3 && a.parent4 == b.parent4;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (parent0 != null)
			{
				num += parent0.GetHashCode();
			}
			if (parent1 != null)
			{
				num += parent1.GetHashCode();
			}
			if (parent2 != null)
			{
				num += parent2.GetHashCode();
			}
			if (parent3 != null)
			{
				num += parent3.GetHashCode();
			}
			if (parent4 != null)
			{
				num += parent4.GetHashCode();
			}
			return num;
		}

		public override string ToString()
		{
			if (parent4 != null)
			{
				return parent4.ToString() + "--" + parent3.ToString() + "--" + parent2.ToString() + "--" + parent1.ToString() + "--" + parent0.ToString();
			}
			if (parent3 != null)
			{
				return parent3.ToString() + "--" + parent2.ToString() + "--" + parent1.ToString() + "--" + parent0.ToString();
			}
			if (parent2 != null)
			{
				return parent2.ToString() + "--" + parent1.ToString() + "--" + parent0.ToString();
			}
			if (parent1 != null)
			{
				return parent1.ToString() + "--" + parent0.ToString();
			}
			return parent0.ToString();
		}
	}

	public class FieldCount
	{
		public string name;

		public int count;
	}

	public class TypeData
	{
		public Dictionary<HierarchyNode, int> hierarchies = new Dictionary<HierarchyNode, int>();

		public Type type;

		public List<FieldInfo> fields;

		public int instanceCount;

		public int refCount;

		public int numArrayEntries;

		public TypeData(Type type)
		{
			this.type = type;
			fields = new List<FieldInfo>();
			instanceCount = 0;
			refCount = 0;
			numArrayEntries = 0;
			FieldInfo[] array = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			foreach (FieldInfo fieldInfo in array)
			{
				if (!fieldInfo.IsStatic && !ShouldExclude(fieldInfo.FieldType))
				{
					fields.Add(fieldInfo);
				}
			}
		}
	}

	public struct DetailInfo
	{
		public int count;

		public int numArrayEntries;
	}

	private struct Lineage
	{
		public object obj;

		public Type parent0;

		public Type parent1;

		public Type parent2;

		public Type parent3;

		public Type parent4;

		public Lineage(object obj, Type parent4, Type parent3, Type parent2, Type parent1, Type parent0)
		{
			this.obj = obj;
			this.parent0 = parent0;
			this.parent1 = parent1;
			this.parent2 = parent2;
			this.parent3 = parent3;
			this.parent4 = parent4;
		}
	}

	private struct ReferenceArgs
	{
		public Type reference_type;

		public string field_name;

		public Lineage lineage;

		public ReferenceArgs(Type reference_type, string field_name, Lineage lineage)
		{
			this.reference_type = reference_type;
			this.lineage = lineage;
			this.field_name = field_name;
		}
	}

	private struct FieldArgs
	{
		public FieldInfo field;

		public Lineage lineage;

		public FieldArgs(FieldInfo field, Lineage lineage)
		{
			this.field = field;
			this.lineage = lineage;
		}
	}

	public Dictionary<int, TypeData> types = new Dictionary<int, TypeData>();

	public Dictionary<int, FieldCount> fieldCounts = new Dictionary<int, FieldCount>();

	public HashSet<object> walked = new HashSet<object>();

	public List<FieldInfo> statics = new List<FieldInfo>();

	public Dictionary<string, DetailInfo> detailTypeCount = new Dictionary<string, DetailInfo>();

	private static readonly Type detailType = typeof(byte[]);

	private static readonly string detailTypeStr = detailType.ToString();

	private List<FieldArgs> fieldsToProcess = new List<FieldArgs>();

	private List<ReferenceArgs> refsToProcess = new List<ReferenceArgs>();

	public MemorySnapshot()
	{
		Lineage lineage = new Lineage(null, null, null, null, null, null);
		foreach (Type currentDomainType in App.GetCurrentDomainTypes())
		{
			FieldInfo[] fields = currentDomainType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.IsStatic)
				{
					statics.Add(fieldInfo);
					lineage.parent0 = fieldInfo.DeclaringType;
					fieldsToProcess.Add(new FieldArgs(fieldInfo, lineage));
				}
			}
		}
		CountAll();
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
		for (int j = 0; j < array.Length; j++)
		{
			UnityEngine.Object @object = (UnityEngine.Object)(lineage.obj = array[j]);
			lineage.parent0 = @object.GetType();
			refsToProcess.Add(new ReferenceArgs(@object.GetType(), "Object." + @object.name, lineage));
		}
		CountAll();
	}

	public static TypeData GetTypeData(Type type, Dictionary<int, TypeData> types)
	{
		int hashCode = type.GetHashCode();
		TypeData value = null;
		if (!types.TryGetValue(hashCode, out value))
		{
			value = (types[hashCode] = new TypeData(type));
		}
		return value;
	}

	public static void IncrementFieldCount(Dictionary<int, FieldCount> field_counts, string name)
	{
		int hashCode = name.GetHashCode();
		FieldCount value = null;
		if (!field_counts.TryGetValue(hashCode, out value))
		{
			value = new FieldCount();
			value.name = name;
			field_counts[hashCode] = value;
		}
		value.count++;
	}

	private void CountReference(ReferenceArgs refArgs)
	{
		if (!ShouldExclude(refArgs.reference_type))
		{
			if (refArgs.reference_type == detailType)
			{
				string text = "\"";
				text = ((!(refArgs.lineage.obj as UnityEngine.Object != (UnityEngine.Object)null)) ? ("\"" + detailTypeStr) : ("\"" + ((UnityEngine.Object)refArgs.lineage.obj).name));
				if (refArgs.lineage.parent0 != null)
				{
					text += "\",\"";
					text += refArgs.lineage.parent0.ToString();
				}
				if (refArgs.lineage.parent1 != null)
				{
					text = text + "\",\"" + refArgs.lineage.parent1.ToString();
				}
				if (refArgs.lineage.parent2 != null)
				{
					text = text + "\",\"" + refArgs.lineage.parent2.ToString();
				}
				if (refArgs.lineage.parent3 != null)
				{
					text = text + "\",\"" + refArgs.lineage.parent3.ToString();
				}
				if (refArgs.lineage.parent4 != null)
				{
					text = text + "\",\"" + refArgs.lineage.parent4.ToString();
				}
				text += "\"\n";
				detailTypeCount.TryGetValue(text, out DetailInfo value);
				value.count++;
				if (typeof(Array).IsAssignableFrom(refArgs.reference_type) && refArgs.lineage.obj != null)
				{
					Array array = refArgs.lineage.obj as Array;
					value.numArrayEntries += (array?.Length ?? 0);
				}
				detailTypeCount[text] = value;
			}
			if (refArgs.reference_type.IsClass)
			{
				TypeData typeData = GetTypeData(refArgs.reference_type, types);
				typeData.refCount++;
				IncrementFieldCount(fieldCounts, refArgs.field_name);
			}
			if (refArgs.lineage.obj != null)
			{
				try
				{
					if (refArgs.lineage.obj.GetType().IsClass && !walked.Add(refArgs.lineage.obj))
					{
						return;
					}
				}
				catch
				{
					return;
				}
				TypeData typeData2 = GetTypeData(refArgs.lineage.obj.GetType(), types);
				if (typeData2.type.IsClass)
				{
					typeData2.instanceCount++;
					if (typeof(Array).IsAssignableFrom(typeData2.type))
					{
						Array array2 = refArgs.lineage.obj as Array;
						typeData2.numArrayEntries += (array2?.Length ?? 0);
					}
					HierarchyNode key = new HierarchyNode(refArgs.lineage.parent0, refArgs.lineage.parent1, refArgs.lineage.parent2, refArgs.lineage.parent3, refArgs.lineage.parent4);
					int value2 = 0;
					typeData2.hierarchies.TryGetValue(key, out value2);
					typeData2.hierarchies[key] = value2 + 1;
				}
				foreach (FieldInfo field in typeData2.fields)
				{
					fieldsToProcess.Add(new FieldArgs(field, new Lineage(refArgs.lineage.obj, refArgs.lineage.parent3, refArgs.lineage.parent2, refArgs.lineage.parent1, refArgs.lineage.parent0, field.DeclaringType)));
				}
				ICollection collection = refArgs.lineage.obj as ICollection;
				if (collection != null)
				{
					Type type = typeof(object);
					if (collection.GetType().GetElementType() != null)
					{
						type = collection.GetType().GetElementType();
					}
					else if (collection.GetType().GetGenericArguments().Length > 0)
					{
						type = collection.GetType().GetGenericArguments()[0];
					}
					if (!ShouldExclude(type))
					{
						IEnumerator enumerator2 = collection.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								object current2 = enumerator2.Current;
								refsToProcess.Add(new ReferenceArgs(type, refArgs.field_name + ".Item", new Lineage(current2, refArgs.lineage.parent3, refArgs.lineage.parent2, refArgs.lineage.parent1, refArgs.lineage.parent0, collection.GetType())));
							}
						}
						finally
						{
							IDisposable disposable;
							if ((disposable = (enumerator2 as IDisposable)) != null)
							{
								disposable.Dispose();
							}
						}
					}
				}
			}
		}
	}

	private void CountField(FieldArgs fieldArgs)
	{
		if (!ShouldExclude(fieldArgs.field.FieldType))
		{
			object obj = null;
			try
			{
				if (!fieldArgs.field.FieldType.Name.Contains("*"))
				{
					obj = fieldArgs.field.GetValue(fieldArgs.lineage.obj);
				}
			}
			catch
			{
				obj = null;
			}
			string field_name = fieldArgs.field.DeclaringType.ToString() + "." + fieldArgs.field.Name;
			refsToProcess.Add(new ReferenceArgs(fieldArgs.field.FieldType, field_name, new Lineage(obj, fieldArgs.lineage.parent3, fieldArgs.lineage.parent2, fieldArgs.lineage.parent1, fieldArgs.lineage.parent0, fieldArgs.field.DeclaringType)));
		}
	}

	private static bool ShouldExclude(Type type)
	{
		return type.IsPrimitive || type.IsEnum || type == typeof(MemorySnapshot);
	}

	private void CountAll()
	{
		while (refsToProcess.Count > 0 || fieldsToProcess.Count > 0)
		{
			while (fieldsToProcess.Count > 0)
			{
				FieldArgs fieldArgs = fieldsToProcess[fieldsToProcess.Count - 1];
				fieldsToProcess.RemoveAt(fieldsToProcess.Count - 1);
				CountField(fieldArgs);
			}
			while (refsToProcess.Count > 0)
			{
				ReferenceArgs refArgs = refsToProcess[refsToProcess.Count - 1];
				refsToProcess.RemoveAt(refsToProcess.Count - 1);
				CountReference(refArgs);
			}
		}
	}

	public void WriteTypeDetails(MemorySnapshot compare)
	{
		List<KeyValuePair<string, DetailInfo>> list = null;
		if (compare != null)
		{
			list = compare.detailTypeCount.ToList();
		}
		List<KeyValuePair<string, DetailInfo>> list2 = detailTypeCount.ToList();
		list2.Sort(delegate(KeyValuePair<string, DetailInfo> x, KeyValuePair<string, DetailInfo> y)
		{
			DetailInfo value5 = y.Value;
			int count = value5.count;
			DetailInfo value6 = x.Value;
			return count - value6.count;
		});
		using (StreamWriter streamWriter = new StreamWriter(GarbageProfiler.GetFileName("type_details_" + detailTypeStr)))
		{
			streamWriter.WriteLine("Delta,Count,NumArrayEntries,Type");
			foreach (KeyValuePair<string, DetailInfo> item in list2)
			{
				DetailInfo value = item.Value;
				int num = value.count;
				if (list != null)
				{
					foreach (KeyValuePair<string, DetailInfo> item2 in list)
					{
						if (item2.Key == item.Key)
						{
							int num2 = num;
							DetailInfo value2 = item2.Value;
							num = num2 - value2.count;
							break;
						}
					}
				}
				StreamWriter streamWriter2 = streamWriter;
				object[] obj = new object[7]
				{
					num,
					",",
					null,
					null,
					null,
					null,
					null
				};
				DetailInfo value3 = item.Value;
				obj[2] = value3.count;
				obj[3] = ",";
				DetailInfo value4 = item.Value;
				obj[4] = value4.numArrayEntries;
				obj[5] = ",";
				obj[6] = item.Key;
				streamWriter2.Write(string.Concat(obj));
			}
		}
	}
}
