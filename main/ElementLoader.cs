using Klei;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementLoader
{
	public class ElementEntryCollection
	{
		public ElementEntry[] elements
		{
			get;
			set;
		}
	}

	public class ElementEntry
	{
		private string description_backing;

		public string elementId
		{
			get;
			set;
		}

		public float specificHeatCapacity
		{
			get;
			set;
		}

		public float thermalConductivity
		{
			get;
			set;
		}

		public float solidSurfaceAreaMultiplier
		{
			get;
			set;
		}

		public float liquidSurfaceAreaMultiplier
		{
			get;
			set;
		}

		public float gasSurfaceAreaMultiplier
		{
			get;
			set;
		}

		public float defaultMass
		{
			get;
			set;
		}

		public float defaultTemperature
		{
			get;
			set;
		}

		public float defaultPressure
		{
			get;
			set;
		}

		public float molarMass
		{
			get;
			set;
		}

		public float lightAbsorptionFactor
		{
			get;
			set;
		}

		public string lowTempTransitionTarget
		{
			get;
			set;
		}

		public float lowTemp
		{
			get;
			set;
		}

		public string highTempTransitionTarget
		{
			get;
			set;
		}

		public float highTemp
		{
			get;
			set;
		}

		public string lowTempTransitionOreId
		{
			get;
			set;
		}

		public float lowTempTransitionOreMassConversion
		{
			get;
			set;
		}

		public string highTempTransitionOreId
		{
			get;
			set;
		}

		public float highTempTransitionOreMassConversion
		{
			get;
			set;
		}

		public string sublimateId
		{
			get;
			set;
		}

		public string sublimateFx
		{
			get;
			set;
		}

		public string materialCategory
		{
			get;
			set;
		}

		public string[] tags
		{
			get;
			set;
		}

		public bool isDisabled
		{
			get;
			set;
		}

		public float strength
		{
			get;
			set;
		}

		public float maxMass
		{
			get;
			set;
		}

		public byte hardness
		{
			get;
			set;
		}

		public float toxicity
		{
			get;
			set;
		}

		public float liquidCompression
		{
			get;
			set;
		}

		public float speed
		{
			get;
			set;
		}

		public float minHorizontalFlow
		{
			get;
			set;
		}

		public float minVerticalFlow
		{
			get;
			set;
		}

		public string convertId
		{
			get;
			set;
		}

		public float flow
		{
			get;
			set;
		}

		public int buildMenuSort
		{
			get;
			set;
		}

		public Element.State state
		{
			get;
			set;
		}

		public string localizationID
		{
			get;
			set;
		}

		public string description
		{
			get
			{
				return description_backing ?? ("STRINGS.ELEMENTS." + elementId.ToString().ToUpper() + ".DESC");
			}
			set
			{
				description_backing = value;
			}
		}

		public ElementEntry()
		{
			lowTemp = 0f;
			highTemp = 10000f;
		}
	}

	public static List<Element> elements;

	public static Dictionary<int, Element> elementTable;

	private static string path = Application.streamingAssetsPath + "/elements/";

	private static readonly Color noColour = new Color(0f, 0f, 0f, 0f);

	public static List<ElementEntry> CollectElementsFromYAML()
	{
		List<ElementEntry> list = new List<ElementEntry>();
		ListPool<FileHandle, ElementLoader>.PooledList pooledList = ListPool<FileHandle, ElementLoader>.Allocate();
		FileSystem.GetFiles(FileSystem.Normalize(path), "*.yaml", pooledList);
		ListPool<YamlIO.Error, ElementLoader>.PooledList errors = ListPool<YamlIO.Error, ElementLoader>.Allocate();
		foreach (FileHandle item in pooledList)
		{
			FileHandle file = item;
			ElementEntryCollection elementEntryCollection = YamlIO.LoadFile<ElementEntryCollection>(file.full_path, delegate(YamlIO.Error error, bool force_log_as_warning)
			{
				error.file = file;
				errors.Add(error);
			}, null);
			if (elementEntryCollection != null)
			{
				list.AddRange(elementEntryCollection.elements);
			}
		}
		pooledList.Recycle();
		if ((UnityEngine.Object)Global.Instance != (UnityEngine.Object)null && Global.Instance.modManager != null)
		{
			Global.Instance.modManager.HandleErrors(errors);
		}
		errors.Recycle();
		return list;
	}

	public static void Load(ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		elements = new List<Element>();
		elementTable = new Dictionary<int, Element>();
		List<ElementEntry> list = CollectElementsFromYAML();
		foreach (ElementEntry item in list)
		{
			int num = Hash.SDBMLower(item.elementId);
			if (!elementTable.ContainsKey(num))
			{
				Element element = new Element();
				element.id = (SimHashes)num;
				element.name = Strings.Get(item.localizationID);
				element.nameUpperCase = element.name.ToUpper();
				element.description = Strings.Get(item.description);
				element.tag = TagManager.Create(item.elementId, element.name);
				CopyEntryToElement(item, element);
				elements.Add(element);
				elementTable[num] = element;
			}
		}
		foreach (Element element2 in elements)
		{
			if (!ManifestSubstanceForElement(element2, ref substanceList, substanceTable))
			{
				Debug.LogWarning("Missing substance for element: " + element2.id.ToString());
			}
		}
		FinaliseElementsTable(ref substanceList, substanceTable);
		WorldGen.SetupDefaultElements();
	}

	private static void CopyEntryToElement(ElementEntry entry, Element elem)
	{
		int num = Hash.SDBMLower(entry.elementId);
		elem.tag = TagManager.Create(entry.elementId.ToString());
		elem.specificHeatCapacity = entry.specificHeatCapacity;
		elem.thermalConductivity = entry.thermalConductivity;
		elem.molarMass = entry.molarMass;
		elem.strength = entry.strength;
		elem.disabled = entry.isDisabled;
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

	private static bool ManifestSubstanceForElement(Element elem, ref Hashtable substanceList, SubstanceTable substanceTable)
	{
		elem.substance = null;
		if (substanceList.ContainsKey(elem.id))
		{
			elem.substance = (substanceList[elem.id] as Substance);
			return false;
		}
		if ((UnityEngine.Object)substanceTable != (UnityEngine.Object)null)
		{
			elem.substance = substanceTable.GetSubstance(elem.id);
		}
		if (elem.substance == null)
		{
			elem.substance = new Substance();
			substanceTable.GetList().Add(elem.substance);
		}
		elem.substance.elementID = elem.id;
		elem.substance.renderedByWorld = elem.IsSolid;
		elem.substance.idx = substanceList.Count;
		if ((Color)elem.substance.uiColour == noColour)
		{
			int count = elements.Count;
			int idx = elem.substance.idx;
			elem.substance.uiColour = Color.HSVToRGB((float)idx / (float)count, 1f, 1f);
		}
		string text = UI.StripLinkFormatting(elem.name);
		elem.substance.name = text;
		if (Array.IndexOf((SimHashes[])Enum.GetValues(typeof(SimHashes)), elem.id) >= 0)
		{
			elem.substance.nameTag = GameTagExtensions.Create(elem.id);
		}
		else
		{
			elem.substance.nameTag = ((text == null) ? Tag.Invalid : TagManager.Create(text));
		}
		elem.substance.audioConfig = ElementsAudio.Instance.GetConfigForElement(elem.id);
		substanceList.Add(elem.id, elem.substance);
		return true;
	}

	public static Element FindElementByName(string name)
	{
		try
		{
			return FindElementByHash((SimHashes)Enum.Parse(typeof(SimHashes), name));
		}
		catch
		{
			return FindElementByHash((SimHashes)Hash.SDBMLower(name));
		}
	}

	public static Element FindElementByHash(SimHashes hash)
	{
		Element value = null;
		elementTable.TryGetValue((int)hash, out value);
		return value;
	}

	public static int GetElementIndex(SimHashes hash)
	{
		for (int i = 0; i != elements.Count; i++)
		{
			if (elements[i].id == hash)
			{
				return i;
			}
		}
		return -1;
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

	private static SimHashes GetID(int column, int row, string[,] grid, SimHashes defaultValue = SimHashes.Vacuum)
	{
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			Debug.LogError($"Could not find element at loc [{column},{row}] grid is only [{grid.GetLength(0)},{grid.GetLength(1)}]");
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
			Debug.LogError($"Could not find element {text}: {ex.ToString()}");
			return defaultValue;
		}
		return (SimHashes)obj;
	}

	private static SpawnFXHashes GetSpawnFX(int column, int row, string[,] grid)
	{
		if (column >= grid.GetLength(0) || row > grid.GetLength(1))
		{
			Debug.LogError($"Could not find SpawnFXHashes at loc [{column},{row}] grid is only [{grid.GetLength(0)},{grid.GetLength(1)}]");
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
			Debug.LogError($"Could not find FX {text}: {ex.ToString()}");
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
		foreach (Element element5 in elements)
		{
			if (element5 != null)
			{
				if (element5.substance == null)
				{
					if ((UnityEngine.Object)substanceTable == (UnityEngine.Object)null)
					{
						element5.substance = new Substance();
					}
					else
					{
						ManifestSubstanceForElement(element5, ref substanceList, substanceTable);
					}
				}
				Debug.Assert(element5.substance.nameTag.IsValid);
				if (element5.thermalConductivity == 0f)
				{
					element5.state |= Element.State.TemperatureInsulated;
				}
				if (element5.strength == 0f)
				{
					element5.state |= Element.State.Unbreakable;
				}
				if (element5.IsSolid)
				{
					Element element = FindElementByHash(element5.highTempTransitionTarget);
					if (element != null)
					{
						element5.highTempTransition = element;
					}
				}
				else if (element5.IsLiquid)
				{
					Element element2 = FindElementByHash(element5.highTempTransitionTarget);
					if (element2 != null)
					{
						element5.highTempTransition = element2;
					}
					Element element3 = FindElementByHash(element5.lowTempTransitionTarget);
					if (element3 != null)
					{
						element5.lowTempTransition = element3;
					}
				}
				else if (element5.IsGas)
				{
					Element element4 = FindElementByHash(element5.lowTempTransitionTarget);
					if (element4 != null)
					{
						element5.lowTempTransition = element4;
					}
				}
			}
		}
		IOrderedEnumerable<Element> source = from e in elements
		orderby (int)(e.state & Element.State.Solid) descending, e.id
		select e;
		elements = source.ToList();
		for (int i = 0; i < elements.Count; i++)
		{
			if (elements[i].substance != null)
			{
				elements[i].substance.idx = i;
			}
			elements[i].idx = (byte)i;
		}
	}
}
