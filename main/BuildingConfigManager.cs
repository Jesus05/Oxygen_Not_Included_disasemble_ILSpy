using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingConfigManager : KMonoBehaviour
{
	public static BuildingConfigManager Instance;

	private GameObject baseTemplate;

	private Dictionary<IBuildingConfig, BuildingDef> configTable = new Dictionary<IBuildingConfig, BuildingDef>();

	private string[] NonBuildableBuildings = new string[2]
	{
		"Headquarters",
		"POIBunkerExteriorDoor"
	};

	private HashSet<Type> defaultKComponents = new HashSet<Type>();

	private HashSet<Type> defaultBuildingCompleteKComponents = new HashSet<Type>();

	private Dictionary<Type, HashSet<Tag>> ignoredDefaultKComponents = new Dictionary<Type, HashSet<Tag>>();

	private Dictionary<Tag, HashSet<Type>> buildingCompleteKComponents = new Dictionary<Tag, HashSet<Type>>();

	protected override void OnPrefabInit()
	{
		Instance = this;
		baseTemplate = new GameObject("BuildingTemplate");
		baseTemplate.SetActive(false);
		baseTemplate.AddComponent<KPrefabID>();
		baseTemplate.AddComponent<KSelectable>();
		baseTemplate.AddComponent<Modifiers>();
		baseTemplate.AddComponent<PrimaryElement>();
		baseTemplate.AddComponent<BuildingComplete>();
		baseTemplate.AddComponent<StateMachineController>();
		baseTemplate.AddComponent<Deconstructable>();
		baseTemplate.AddComponent<SaveLoadRoot>();
		baseTemplate.AddComponent<OccupyArea>();
		baseTemplate.AddComponent<DecorProvider>();
		baseTemplate.AddComponent<Operational>();
		baseTemplate.AddComponent<BuildingEnabledButton>();
		baseTemplate.AddComponent<Prioritizable>();
		baseTemplate.AddComponent<BuildingHP>();
		baseTemplate.AddComponent<LoopingSounds>();
		defaultBuildingCompleteKComponents.Add(typeof(RequiresFoundation));
	}

	public static string GetUnderConstructionName(string name)
	{
		return name + "UnderConstruction";
	}

	public void RegisterBuilding(IBuildingConfig config)
	{
		BuildingDef buildingDef = config.CreateBuildingDef();
		configTable[config] = buildingDef;
		GameObject gameObject = UnityEngine.Object.Instantiate(baseTemplate);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.GetComponent<KPrefabID>().PrefabTag = buildingDef.Tag;
		gameObject.name = buildingDef.PrefabID + "Template";
		gameObject.GetComponent<Building>().Def = buildingDef;
		gameObject.GetComponent<OccupyArea>().OccupiedCellsOffsets = buildingDef.PlacementOffsets;
		if (buildingDef.Deprecated)
		{
			gameObject.GetComponent<KPrefabID>().AddTag(GameTags.DeprecatedContent);
		}
		config.ConfigureBuildingTemplate(gameObject, buildingDef.Tag);
		buildingDef.BuildingComplete = BuildingLoader.Instance.CreateBuildingComplete(gameObject, buildingDef);
		bool flag = true;
		for (int i = 0; i < NonBuildableBuildings.Length; i++)
		{
			if (buildingDef.PrefabID == NonBuildableBuildings[i])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			buildingDef.BuildingUnderConstruction = BuildingLoader.Instance.CreateBuildingUnderConstruction(buildingDef);
			buildingDef.BuildingUnderConstruction.name = GetUnderConstructionName(buildingDef.BuildingUnderConstruction.name);
			buildingDef.BuildingPreview = BuildingLoader.Instance.CreateBuildingPreview(buildingDef);
			GameObject buildingPreview = buildingDef.BuildingPreview;
			buildingPreview.name += "Preview";
		}
		buildingDef.PostProcess();
		config.DoPostConfigureComplete(buildingDef.BuildingComplete);
		if (flag)
		{
			config.DoPostConfigurePreview(buildingDef, buildingDef.BuildingPreview);
			config.DoPostConfigureUnderConstruction(buildingDef.BuildingUnderConstruction);
		}
		Assets.AddBuildingDef(buildingDef);
	}

	public void ConfigurePost()
	{
		foreach (KeyValuePair<IBuildingConfig, BuildingDef> item in configTable)
		{
			item.Key.ConfigurePost(item.Value);
		}
	}

	public void IgnoreDefaultKComponent(Type type_to_ignore, Tag building_tag)
	{
		if (!ignoredDefaultKComponents.TryGetValue(type_to_ignore, out HashSet<Tag> value))
		{
			value = new HashSet<Tag>();
			ignoredDefaultKComponents[type_to_ignore] = value;
		}
		value.Add(building_tag);
	}

	private bool IsIgnoredDefaultKComponent(Tag building_tag, Type type)
	{
		bool result = false;
		if (ignoredDefaultKComponents.TryGetValue(type, out HashSet<Tag> value) && value.Contains(building_tag))
		{
			result = true;
		}
		return result;
	}

	public void AddBuildingCompleteKComponents(GameObject go, Tag prefab_tag)
	{
		foreach (Type defaultBuildingCompleteKComponent in defaultBuildingCompleteKComponents)
		{
			if (!IsIgnoredDefaultKComponent(prefab_tag, defaultBuildingCompleteKComponent))
			{
				GameComps.GetKComponentManager(defaultBuildingCompleteKComponent).Add(go);
			}
		}
		if (buildingCompleteKComponents.TryGetValue(prefab_tag, out HashSet<Type> value))
		{
			foreach (Type item in value)
			{
				GameComps.GetKComponentManager(item).Add(go);
			}
		}
	}

	public void DestroyBuildingCompleteKComponents(GameObject go, Tag prefab_tag)
	{
		foreach (Type defaultBuildingCompleteKComponent in defaultBuildingCompleteKComponents)
		{
			if (!IsIgnoredDefaultKComponent(prefab_tag, defaultBuildingCompleteKComponent))
			{
				GameComps.GetKComponentManager(defaultBuildingCompleteKComponent).Remove(go);
			}
		}
		if (buildingCompleteKComponents.TryGetValue(prefab_tag, out HashSet<Type> value))
		{
			foreach (Type item in value)
			{
				GameComps.GetKComponentManager(item).Remove(go);
			}
		}
	}

	public void AddDefaultBuildingCompleteKComponent(Type kcomponent_type)
	{
		defaultKComponents.Add(kcomponent_type);
	}

	public void AddBuildingCompleteKComponent(Tag prefab_tag, Type kcomponent_type)
	{
		if (!buildingCompleteKComponents.TryGetValue(prefab_tag, out HashSet<Type> value))
		{
			value = new HashSet<Type>();
			buildingCompleteKComponents[prefab_tag] = value;
		}
		value.Add(kcomponent_type);
	}
}
