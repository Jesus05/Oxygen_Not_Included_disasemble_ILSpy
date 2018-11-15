using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProcGenGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ElementLoader
{
	public class ElementEntry : Resource
	{
		public string elementId;

		public float specificHeatCapacity;

		public float thermalConductivity;

		public float solidSurfaceAreaMultiplier;

		public float liquidSurfaceAreaMultiplier;

		public float gasSurfaceAreaMultiplier;

		public float defaultMass;

		public float defaultTemperature;

		public float defaultPressure;

		public float molarMass;

		public float lightAbsorptionFactor;

		public string lowTempTransitionTarget;

		public float lowTemp;

		public string highTempTransitionTarget;

		public float highTemp = 10000f;

		public string lowTempTransitionOreId;

		public float lowTempTransitionOreMassConversion;

		public string highTempTransitionOreId;

		public float highTempTransitionOreMassConversion;

		public string sublimateId;

		public string sublimateFx;

		public string materialCategory;

		public string[] tags;

		public bool isDisabled;

		public float strength;

		public float maxMass;

		public byte hardness;

		public float toxicity;

		public float liquidCompression;

		public float speed;

		public float minHorizontalFlow;

		public float minVerticalFlow;

		public string convertId;

		public float flow;

		public int buildMenuSort;

		[JsonConverter(typeof(StringEnumConverter))]
		public Element.State state;

		public string localizationID;
	}

	public static List<Element> elements;

	public static Dictionary<int, Element> elementTable;

	private static readonly Color noColour = new Color(0f, 0f, 0f, 0f);

	private static SimHashes getElementIndexHash;

	private static Predicate<Element> getElementIndexCallback = (Element e) => getElementIndexHash == e.id;

	public static void Load(ref Hashtable substanceList, string elementsFileContent, SubstanceTable substanceTable)
	{
		SetupElementsTable();
		ElementEntry[] array = JsonConvert.DeserializeObject<ElementEntry[]>(elementsFileContent);
		ElementEntry[] array2 = array;
		foreach (ElementEntry elementEntry in array2)
		{
			int hash = Hash.SDBMLower(elementEntry.elementId);
			Element element = FindElementByHash((SimHashes)hash);
			element.name = Strings.Get(elementEntry.localizationID);
			element.nameUpperCase = element.name.ToUpper();
			element.tag = TagManager.Create(element.id.ToString(), element.name);
			Copy(elementEntry, element);
		}
		LoadUserElementData();
		foreach (Element element2 in elements)
		{
			if (!SetOrCreateSubstanceForElement(element2.id, ref substanceList, substanceTable))
			{
				Debug.LogWarning("Missing substance for element: " + element2.id.ToString(), null);
			}
		}
		FinaliseElementsTable(ref substanceList, substanceTable);
		WorldGen.SetupDefaultElements();
	}

	private static void LoadUserElementData()
	{
		string text = Path.Combine(Application.dataPath, "elements.json");
		if (!((UnityEngine.Object)Global.Instance == (UnityEngine.Object)null) && Global.Instance.layeredFileSystem != null && Global.Instance.layeredFileSystem.Exists(text))
		{
			string value = Global.Instance.layeredFileSystem.ReadText(text);
			ElementEntry[] array = JsonConvert.DeserializeObject<ElementEntry[]>(value);
			ElementEntry elementEntry = new ElementEntry();
			ElementEntry[] array2 = array;
			foreach (ElementEntry elementEntry2 in array2)
			{
				int num = Hash.SDBMLower(elementEntry2.elementId);
				Element element = FindElementByHash((SimHashes)num);
				if (element == null)
				{
					element = new Element();
					element.id = (SimHashes)num;
					element.name = Strings.Get(elementEntry2.localizationID);
					element.nameUpperCase = element.name.ToUpper();
					element.tag = TagManager.Create(element.id.ToString(), element.name);
					elements.Add(element);
					elementTable[(int)element.id] = element;
				}
				if (elementEntry2.specificHeatCapacity != elementEntry.specificHeatCapacity)
				{
					element.specificHeatCapacity = elementEntry2.specificHeatCapacity;
				}
				if (elementEntry2.thermalConductivity != elementEntry.thermalConductivity)
				{
					element.thermalConductivity = elementEntry2.thermalConductivity;
				}
				if (elementEntry2.molarMass != elementEntry.molarMass)
				{
					element.molarMass = elementEntry2.molarMass;
				}
				if (elementEntry2.strength != elementEntry.strength)
				{
					element.strength = elementEntry2.strength;
				}
				if (elementEntry2.flow != elementEntry.flow)
				{
					element.flow = elementEntry2.flow;
				}
				if (elementEntry2.maxMass != elementEntry.maxMass)
				{
					element.maxMass = elementEntry2.maxMass;
				}
				if (elementEntry2.liquidCompression != elementEntry.liquidCompression)
				{
					element.maxCompression = elementEntry2.liquidCompression;
				}
				if (elementEntry2.speed != elementEntry.speed)
				{
					element.viscosity = elementEntry2.speed;
				}
				if (elementEntry2.minHorizontalFlow != elementEntry.minHorizontalFlow)
				{
					element.minHorizontalFlow = elementEntry2.minHorizontalFlow;
				}
				if (elementEntry2.minVerticalFlow != elementEntry.minVerticalFlow)
				{
					element.minVerticalFlow = elementEntry2.minVerticalFlow;
				}
				if (elementEntry2.maxMass != elementEntry.maxMass)
				{
					element.maxMass = elementEntry2.maxMass;
				}
				if (elementEntry2.solidSurfaceAreaMultiplier != elementEntry.solidSurfaceAreaMultiplier)
				{
					element.solidSurfaceAreaMultiplier = elementEntry2.solidSurfaceAreaMultiplier;
				}
				if (elementEntry2.liquidSurfaceAreaMultiplier != elementEntry.liquidSurfaceAreaMultiplier)
				{
					element.liquidSurfaceAreaMultiplier = elementEntry2.liquidSurfaceAreaMultiplier;
				}
				if (elementEntry2.gasSurfaceAreaMultiplier != elementEntry.gasSurfaceAreaMultiplier)
				{
					element.gasSurfaceAreaMultiplier = elementEntry2.gasSurfaceAreaMultiplier;
				}
				if (elementEntry2.state != elementEntry.state)
				{
					element.state = elementEntry2.state;
				}
				if (elementEntry2.hardness != elementEntry.hardness)
				{
					element.hardness = elementEntry2.hardness;
				}
				if (elementEntry2.lowTemp != elementEntry.lowTemp)
				{
					element.lowTemp = elementEntry2.lowTemp;
				}
				if (elementEntry2.lowTempTransitionTarget != elementEntry.lowTempTransitionTarget)
				{
					element.lowTempTransitionTarget = (SimHashes)Hash.SDBMLower(elementEntry2.lowTempTransitionTarget);
				}
				if (elementEntry2.highTemp != elementEntry.highTemp)
				{
					element.highTemp = elementEntry2.highTemp;
				}
				if (elementEntry2.highTempTransitionTarget != elementEntry.highTempTransitionTarget)
				{
					element.highTempTransitionTarget = (SimHashes)Hash.SDBMLower(elementEntry2.highTempTransitionTarget);
				}
				if (elementEntry2.highTempTransitionOreId != elementEntry.highTempTransitionOreId)
				{
					element.highTempTransitionOreID = (SimHashes)Hash.SDBMLower(elementEntry2.highTempTransitionOreId);
				}
				if (elementEntry2.highTempTransitionOreMassConversion != elementEntry.highTempTransitionOreMassConversion)
				{
					element.highTempTransitionOreMassConversion = elementEntry2.highTempTransitionOreMassConversion;
				}
				if (elementEntry2.lowTempTransitionOreId != elementEntry.lowTempTransitionOreId)
				{
					element.lowTempTransitionOreID = (SimHashes)Hash.SDBMLower(elementEntry2.lowTempTransitionOreId);
				}
				if (elementEntry2.lowTempTransitionOreMassConversion != elementEntry.lowTempTransitionOreMassConversion)
				{
					element.lowTempTransitionOreMassConversion = elementEntry2.lowTempTransitionOreMassConversion;
				}
				if (elementEntry2.sublimateId != elementEntry.sublimateId)
				{
					element.sublimateId = (SimHashes)Hash.SDBMLower(elementEntry2.sublimateId);
				}
				if (elementEntry2.convertId != elementEntry.convertId)
				{
					element.convertId = (SimHashes)Hash.SDBMLower(elementEntry2.convertId);
				}
				if (elementEntry2.sublimateFx != elementEntry.sublimateFx)
				{
					element.sublimateFX = (SpawnFXHashes)Hash.SDBMLower(elementEntry2.sublimateFx);
				}
				if (elementEntry2.lightAbsorptionFactor != elementEntry.lightAbsorptionFactor)
				{
					element.lightAbsorptionFactor = elementEntry2.lightAbsorptionFactor;
				}
				Sim.PhysicsData defaultValues = element.defaultValues;
				if (elementEntry2.defaultTemperature != elementEntry.defaultTemperature)
				{
					defaultValues.temperature = elementEntry2.defaultTemperature;
				}
				if (elementEntry2.defaultMass != elementEntry.defaultMass)
				{
					defaultValues.mass = elementEntry2.defaultMass;
				}
				if (elementEntry2.defaultPressure != elementEntry.defaultPressure)
				{
					defaultValues.pressure = elementEntry2.defaultPressure;
				}
				element.defaultValues = defaultValues;
				if (elementEntry2.toxicity != elementEntry.toxicity)
				{
					element.toxicity = elementEntry2.toxicity;
				}
				Tag phaseTag = TagManager.Create(elementEntry2.state.ToString());
				if (elementEntry2.materialCategory != elementEntry.materialCategory)
				{
					element.materialCategory = CreateMaterialCategoryTag(element.id, phaseTag, elementEntry2.materialCategory);
				}
				if (elementEntry2.tags != elementEntry.tags)
				{
					element.oreTags = CreateOreTags(element.materialCategory, phaseTag, elementEntry2.tags);
				}
				if (elementEntry2.buildMenuSort != elementEntry.buildMenuSort)
				{
					element.buildMenuSort = elementEntry2.buildMenuSort;
				}
				switch (elementEntry2.state)
				{
				case Element.State.Solid:
					GameTags.SolidElements.Add(element.tag);
					break;
				case Element.State.Liquid:
					GameTags.LiquidElements.Add(element.tag);
					break;
				case Element.State.Gas:
					GameTags.GasElements.Add(element.tag);
					break;
				}
			}
		}
	}

	private static void Copy(ElementEntry entry, Element elem)
	{
		int num = Hash.SDBMLower(entry.elementId);
		elem.tag = TagManager.Create(entry.elementId.ToString());
		elem.specificHeatCapacity = entry.specificHeatCapacity;
		elem.thermalConductivity = entry.thermalConductivity;
		elem.molarMass = entry.molarMass;
		elem.strength = entry.strength;
		elem.flow = entry.flow;
		elem.maxMass = entry.maxMass;
		elem.maxCompression = entry.liquidCompression;
		elem.viscosity = entry.speed;
		elem.minHorizontalFlow = entry.minHorizontalFlow;
		elem.minVerticalFlow = entry.minVerticalFlow;
		elem.maxMass = entry.maxMass;
		elem.solidSurfaceAreaMultiplier = entry.solidSurfaceAreaMultiplier;
		elem.liquidSurfaceAreaMultiplier = entry.liquidSurfaceAreaMultiplier;
		elem.gasSurfaceAreaMultiplier = entry.gasSurfaceAreaMultiplier;
		elem.state = entry.state;
		elem.hardness = entry.hardness;
		elem.lowTemp = entry.lowTemp;
		elem.lowTempTransitionTarget = (SimHashes)Hash.SDBMLower(entry.lowTempTransitionTarget);
		elem.highTemp = entry.highTemp;
		elem.highTempTransitionTarget = (SimHashes)Hash.SDBMLower(entry.highTempTransitionTarget);
		elem.highTempTransitionOreID = (SimHashes)Hash.SDBMLower(entry.highTempTransitionOreId);
		elem.highTempTransitionOreMassConversion = entry.highTempTransitionOreMassConversion;
		elem.lowTempTransitionOreID = (SimHashes)Hash.SDBMLower(entry.lowTempTransitionOreId);
		elem.lowTempTransitionOreMassConversion = entry.lowTempTransitionOreMassConversion;
		elem.sublimateId = (SimHashes)Hash.SDBMLower(entry.sublimateId);
		elem.convertId = (SimHashes)Hash.SDBMLower(entry.convertId);
		elem.sublimateFX = (SpawnFXHashes)Hash.SDBMLower(entry.sublimateFx);
		elem.lightAbsorptionFactor = entry.lightAbsorptionFactor;
		elem.toxicity = entry.toxicity;
		Tag phaseTag = TagManager.Create(entry.state.ToString());
		elem.materialCategory = CreateMaterialCategoryTag(elem.id, phaseTag, entry.materialCategory);
		elem.oreTags = CreateOreTags(elem.materialCategory, phaseTag, entry.tags);
		elem.buildMenuSort = entry.buildMenuSort;
		Sim.PhysicsData defaultValues = default(Sim.PhysicsData);
		defaultValues.temperature = entry.defaultTemperature;
		defaultValues.mass = entry.defaultMass;
		defaultValues.pressure = entry.defaultPressure;
		switch (entry.state)
		{
		case Element.State.Solid:
			GameTags.SolidElements.Add(elem.tag);
			break;
		case Element.State.Liquid:
			GameTags.LiquidElements.Add(elem.tag);
			break;
		case Element.State.Gas:
			GameTags.GasElements.Add(elem.tag);
			defaultValues.mass = 1f;
			elem.maxMass = 1.8f;
			break;
		}
		elem.defaultValues = defaultValues;
	}

	private static bool SetOrCreateSubstanceForElement(SimHashes key, ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		bool result = false;
		Element element = FindElementByHash(key);
		if (!substanceList.ContainsKey(key))
		{
			result = true;
			Substance substance = null;
			if ((UnityEngine.Object)substanceTable != (UnityEngine.Object)null)
			{
				substance = substanceTable.GetSubstance(key);
			}
			if (substance == null)
			{
				substance = new Substance();
				substanceTable.GetList().Add(substance);
			}
			CleanupSubstance(substance, element);
			substance.elementID = key;
			substance.renderedByWorld = element.IsSolid;
			substance.idx = substanceList.Count;
			if ((Color)substance.debugColour == noColour)
			{
				int length = Enum.GetValues(typeof(SimHashes)).Length;
				int idx = substance.idx;
				substance.debugColour = Color.HSVToRGB((float)idx / (float)length, 1f, 1f);
			}
			if (substance.name == null || substance.name == string.Empty)
			{
				substance.name = key.ToString();
			}
			substance.audioConfig = ElementsAudio.Instance.GetConfigForElement(key);
			substanceList.Add(key, substance);
		}
		element.substance = (substanceList[key] as Substance);
		return result;
	}

	private static void CleanupSubstance(Substance substance, Element element)
	{
	}

	public static Element GetElement(string name)
	{
		SimHashes hash = (SimHashes)Enum.Parse(typeof(SimHashes), name);
		return FindElementByHash(hash);
	}

	public static Element FindElementByName(string name)
	{
		Element value = null;
		object obj = Enum.Parse(typeof(SimHashes), name);
		if (obj != null)
		{
			SimHashes key = (SimHashes)obj;
			elementTable.TryGetValue((int)key, out value);
		}
		return value;
	}

	public static Element FindElementByHash(SimHashes hash)
	{
		Element value = null;
		elementTable.TryGetValue((int)hash, out value);
		return value;
	}

	public static int GetElementIndex(SimHashes hash)
	{
		getElementIndexHash = hash;
		return elements.FindIndex(getElementIndexCallback);
	}

	public static byte GetElementIndex(Tag element_tag)
	{
		byte result = byte.MaxValue;
		for (int i = 0; i < elements.Count; i++)
		{
			Element element = elements[i];
			if (element_tag == element.tag)
			{
				result = (byte)i;
				break;
			}
		}
		return result;
	}

	public static Element GetElement(Tag tag)
	{
		for (int i = 0; i < elements.Count; i++)
		{
			Element element = elements[i];
			if (tag == element.tag)
			{
				return element;
			}
		}
		return null;
	}

	public static SimHashes GetElementID(Tag tag)
	{
		for (int i = 0; i < elements.Count; i++)
		{
			Element element = elements[i];
			if (tag == element.tag)
			{
				return element.id;
			}
		}
		return SimHashes.Vacuum;
	}

	private static void SetupElementsTable()
	{
		if (elements == null)
		{
			SimHashes[] array = Enum.GetValues(typeof(SimHashes)) as SimHashes[];
			elements = new List<Element>();
			elementTable = new Dictionary<int, Element>();
			SimHashes[] array2 = array;
			foreach (SimHashes id in array2)
			{
				Element element = new Element();
				element.id = id;
				elements.Add(element);
				elementTable[(int)element.id] = element;
			}
		}
	}

	private static SimHashes GetID(int column, int row, string[,] grid, SimHashes defaultValue = SimHashes.Vacuum)
	{
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			Output.LogError($"Could not find element at loc [{column},{row}] grid is only [{grid.GetLength(0)},{grid.GetLength(1)}]");
			return defaultValue;
		}
		string text = grid[column, row];
		if (text == null || text == string.Empty)
		{
			return defaultValue;
		}
		object obj = null;
		try
		{
			obj = Enum.Parse(typeof(SimHashes), text);
		}
		catch (Exception ex)
		{
			Output.LogError($"Could not find element {text}: {ex.ToString()}");
			return defaultValue;
		}
		return (SimHashes)obj;
	}

	private static SpawnFXHashes GetSpawnFX(int column, int row, string[,] grid)
	{
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			Output.LogError($"Could not find SpawnFXHashes at loc [{column},{row}] grid is only [{grid.GetLength(0)},{grid.GetLength(1)}]");
			return SpawnFXHashes.None;
		}
		string text = grid[column, row];
		if (text == null || text == string.Empty)
		{
			return SpawnFXHashes.None;
		}
		object obj = null;
		try
		{
			obj = Enum.Parse(typeof(SpawnFXHashes), text);
		}
		catch (Exception ex)
		{
			Output.LogError($"Could not find FX {text}: {ex.ToString()}");
			return SpawnFXHashes.None;
		}
		return (SpawnFXHashes)obj;
	}

	private static Tag CreateMaterialCategoryTag(SimHashes element_id, Tag phaseTag, string materialCategoryField)
	{
		if (!string.IsNullOrEmpty(materialCategoryField))
		{
			Tag tag = TagManager.Create(materialCategoryField);
			if (!GameTags.MaterialCategories.Contains(tag) && !GameTags.IgnoredMaterialCategories.Contains(tag))
			{
				Debug.LogWarningFormat("Element {0} has category {1}, but that isn't in GameTags.MaterialCategores!", element_id, materialCategoryField);
			}
			return tag;
		}
		return phaseTag;
	}

	private static Tag[] CreateOreTags(Tag materialCategory, Tag phaseTag, string[] ore_tags_split)
	{
		List<Tag> list = new List<Tag>();
		if (ore_tags_split != null)
		{
			foreach (string text in ore_tags_split)
			{
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(TagManager.Create(text));
				}
			}
		}
		list.Add(phaseTag);
		if (materialCategory.IsValid && !list.Contains(materialCategory))
		{
			list.Add(materialCategory);
		}
		return list.ToArray();
	}

	private static void FinaliseElementsTable(ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		SimHashes[] array = Enum.GetValues(typeof(SimHashes)) as SimHashes[];
		foreach (SimHashes hash in array)
		{
			Element element = FindElementByHash(hash);
			if (element != null)
			{
				if (element.substance == null)
				{
					if ((UnityEngine.Object)substanceTable == (UnityEngine.Object)null)
					{
						element.substance = new Substance();
					}
					else
					{
						SetOrCreateSubstanceForElement(element.id, ref substanceList, substanceTable);
					}
				}
				if (element.thermalConductivity == 0f)
				{
					element.state |= Element.State.TemperatureInsulated;
				}
				if (element.strength == 0f)
				{
					element.state |= Element.State.Unbreakable;
				}
				if (element.IsSolid)
				{
					Element element2 = FindElementByHash(element.highTempTransitionTarget);
					if (element2 != null)
					{
						element.highTempTransition = element2;
					}
				}
				else if (element.IsLiquid)
				{
					Element element3 = FindElementByHash(element.highTempTransitionTarget);
					if (element3 != null)
					{
						element.highTempTransition = element3;
					}
					Element element4 = FindElementByHash(element.lowTempTransitionTarget);
					if (element4 != null)
					{
						element.lowTempTransition = element4;
					}
				}
				else if (element.IsGas)
				{
					Element element5 = FindElementByHash(element.lowTempTransitionTarget);
					if (element5 != null)
					{
						element.lowTempTransition = element5;
					}
				}
			}
		}
		IOrderedEnumerable<Element> source = from e in elements
		orderby (int)(e.state & Element.State.Solid) descending, e.id
		select e;
		elements = source.ToList();
		for (int j = 0; j < elements.Count; j++)
		{
			if (elements[j].substance != null)
			{
				elements[j].substance.idx = j;
			}
			elements[j].idx = (byte)j;
		}
	}
}
