using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class LegacyModMain
{
	private struct Entry
	{
		public int count;

		public Type type;
	}

	private struct ElementInfo
	{
		public SimHashes id;

		public float decor;

		public float overheatMod;
	}

	public static void Load()
	{
		List<Type> list = new List<Type>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			Type[] types = assembly.GetTypes();
			if (types != null)
			{
				list.AddRange(types);
			}
		}
		EntityTemplates.CreateTemplates();
		EntityTemplates.CreateBaseOreTemplates();
		LoadOre(list);
		LoadBuildings(list);
		ConfigElements();
		LoadEntities(list);
		LoadEquipment();
		EntityTemplates.DestroyBaseOreTemplates();
	}

	private static void Test()
	{
		Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(Component));
		for (int i = 0; i < array.Length; i++)
		{
			Component component = (Component)array[i];
			Type type = component.GetType();
			int value = 0;
			dictionary.TryGetValue(type, out value);
			dictionary[type] = value + 1;
		}
		List<Entry> list = new List<Entry>();
		foreach (KeyValuePair<Type, int> item in dictionary)
		{
			if (item.Key.GetMethod("Update", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy) != null)
			{
				list.Add(new Entry
				{
					count = item.Value,
					type = item.Key
				});
			}
		}
		list.Sort((Entry x, Entry y) => y.count.CompareTo(x.count));
		string text = "";
		foreach (Entry item2 in list)
		{
			Entry current2 = item2;
			string text2 = text;
			text = text2 + current2.type.Name + ": " + current2.count + "\n";
		}
		Debug.Log(text);
	}

	private static void ListUnusedTypes()
	{
		HashSet<Type> hashSet = new HashSet<Type>();
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject));
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = (GameObject)array[i];
			Component[] components = gameObject.GetComponents<Component>();
			foreach (Component component in components)
			{
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
				{
					for (Type type = component.GetType(); type != typeof(Component); type = type.BaseType)
					{
						hashSet.Add(type);
					}
				}
			}
		}
		HashSet<Type> hashSet2 = new HashSet<Type>();
		foreach (Type currentDomainType in App.GetCurrentDomainTypes())
		{
			if (typeof(MonoBehaviour).IsAssignableFrom(currentDomainType) && !hashSet.Contains(currentDomainType))
			{
				hashSet2.Add(currentDomainType);
			}
		}
		List<Type> list = new List<Type>(hashSet2);
		list.Sort((Type x, Type y) => x.FullName.CompareTo(y.FullName));
		string text = "Unused types:";
		foreach (Type item in list)
		{
			text = text + "\n" + item.FullName;
		}
		Debug.Log(text);
	}

	private static void DebugSelected()
	{
	}

	private static void DebugSelected(GameObject go)
	{
		Constructable component = go.GetComponent<Constructable>();
		int num = 0;
		num++;
		Debug.Log(component);
	}

	private static void LoadOre(List<Type> types)
	{
		GeneratedOre.LoadGeneratedOre(types);
	}

	private static void LoadBuildings(List<Type> types)
	{
		LocString.CreateLocStringKeys(typeof(BUILDINGS.PREFABS), "STRINGS.BUILDINGS.");
		LocString.CreateLocStringKeys(typeof(BUILDINGS.DAMAGESOURCES), "STRINGS.BUILDINGS.DAMAGESOURCES");
		LocString.CreateLocStringKeys(typeof(BUILDINGS.REPAIRABLE), "STRINGS.BUILDINGS.REPAIRABLE");
		LocString.CreateLocStringKeys(typeof(BUILDINGS.DISINFECTABLE), "STRINGS.BUILDINGS.DISINFECTABLE");
		GeneratedBuildings.LoadGeneratedBuildings(types);
	}

	private static void LoadEntities(List<Type> types)
	{
		EntityConfigManager.Instance.LoadGeneratedEntities(types);
		BuildingConfigManager.Instance.ConfigurePost();
	}

	private static void LoadEquipment()
	{
		LocString.CreateLocStringKeys(typeof(EQUIPMENT.PREFABS), "STRINGS.EQUIPMENT.");
		GeneratedEquipment.LoadGeneratedEquipment();
	}

	private static void ConfigElements()
	{
		ElementInfo[] array = new ElementInfo[19]
		{
			new ElementInfo
			{
				id = SimHashes.Katairite,
				overheatMod = 200f
			},
			new ElementInfo
			{
				id = SimHashes.Cuprite,
				decor = 0.1f
			},
			new ElementInfo
			{
				id = SimHashes.Copper,
				decor = 0.2f,
				overheatMod = 50f
			},
			new ElementInfo
			{
				id = SimHashes.Gold,
				decor = 0.5f,
				overheatMod = 50f
			},
			new ElementInfo
			{
				id = SimHashes.Lead,
				overheatMod = -20f
			},
			new ElementInfo
			{
				id = SimHashes.Granite,
				decor = 0.2f,
				overheatMod = 15f
			},
			new ElementInfo
			{
				id = SimHashes.SandStone,
				decor = 0.1f
			},
			new ElementInfo
			{
				id = SimHashes.ToxicSand,
				overheatMod = -10f
			},
			new ElementInfo
			{
				id = SimHashes.Dirt,
				overheatMod = -10f
			},
			new ElementInfo
			{
				id = SimHashes.IgneousRock,
				overheatMod = 15f
			},
			new ElementInfo
			{
				id = SimHashes.Obsidian,
				overheatMod = 15f
			},
			new ElementInfo
			{
				id = SimHashes.Ceramic,
				overheatMod = 200f,
				decor = 0.2f
			},
			new ElementInfo
			{
				id = SimHashes.Iron,
				overheatMod = 50f
			},
			new ElementInfo
			{
				id = SimHashes.Tungsten,
				overheatMod = 50f
			},
			new ElementInfo
			{
				id = SimHashes.Steel,
				overheatMod = 200f
			},
			new ElementInfo
			{
				id = SimHashes.GoldAmalgam,
				overheatMod = 50f,
				decor = 0.1f
			},
			new ElementInfo
			{
				id = SimHashes.Diamond,
				overheatMod = 200f,
				decor = 1f
			},
			new ElementInfo
			{
				id = SimHashes.Niobium,
				decor = 0.5f,
				overheatMod = 500f
			},
			new ElementInfo
			{
				id = SimHashes.TempConductorSolid,
				overheatMod = 900f
			}
		};
		ElementInfo[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			ElementInfo elementInfo = array2[i];
			Element element = ElementLoader.FindElementByHash(elementInfo.id);
			if (elementInfo.decor != 0f)
			{
				AttributeModifier item = new AttributeModifier("Decor", elementInfo.decor, element.name, true, false, true);
				element.attributeModifiers.Add(item);
			}
			if (elementInfo.overheatMod != 0f)
			{
				AttributeModifier item2 = new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id, elementInfo.overheatMod, element.name, false, false, true);
				element.attributeModifiers.Add(item2);
			}
		}
	}
}
