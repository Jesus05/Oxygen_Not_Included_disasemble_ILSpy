using UnityEngine;

public class BuildingTemplates
{
	public static BuildingDef CreateBuildingDef(string id, int width, int height, string anim, int hitpoints, float construction_time, float[] construction_mass, string[] construction_materials, float melting_point, BuildLocationRule build_location_rule, EffectorValues decor, EffectorValues noise, float temperature_modification_mass_scale = 0.2f)
	{
		BuildingDef buildingDef = ScriptableObject.CreateInstance<BuildingDef>();
		buildingDef.PrefabID = id;
		buildingDef.InitDef();
		buildingDef.name = id;
		buildingDef.Mass = construction_mass;
		buildingDef.MassForTemperatureModification = construction_mass[0] * temperature_modification_mass_scale;
		buildingDef.WidthInCells = width;
		buildingDef.HeightInCells = height;
		buildingDef.HitPoints = hitpoints;
		buildingDef.ConstructionTime = construction_time;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.MaterialCategory = construction_materials;
		buildingDef.BaseMeltingPoint = melting_point;
		switch (build_location_rule)
		{
		case BuildLocationRule.Anywhere:
		case BuildLocationRule.Tile:
		case BuildLocationRule.Conduit:
		case BuildLocationRule.LogicBridge:
		case BuildLocationRule.WireBridge:
			buildingDef.ContinuouslyCheckFoundation = false;
			break;
		default:
			buildingDef.ContinuouslyCheckFoundation = true;
			break;
		}
		buildingDef.BuildLocationRule = build_location_rule;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.AnimFiles = new KAnimFile[1]
		{
			Assets.GetAnim(anim)
		};
		buildingDef.GenerateOffsets();
		buildingDef.BaseDecor = (float)decor.amount;
		buildingDef.BaseDecorRadius = (float)decor.radius;
		buildingDef.BaseNoisePollution = noise.amount;
		buildingDef.BaseNoisePollutionRadius = noise.radius;
		return buildingDef;
	}

	public static void CreateStandardBuildingDef(BuildingDef def)
	{
		def.Breakable = true;
	}

	public static void CreateElectricalBuildingDef(BuildingDef def)
	{
		CreateStandardBuildingDef(def);
		def.RequiresPowerInput = true;
		def.ViewMode = OverlayModes.Power.ID;
		def.AudioCategory = "HollowMetal";
	}

	public static void CreateRocketBuildingDef(BuildingDef def)
	{
		CreateStandardBuildingDef(def);
		def.Invincible = true;
		def.DefaultAnimState = "grounded";
	}

	public static void CreateMonumentBuildingDef(BuildingDef def)
	{
		CreateStandardBuildingDef(def);
		def.Invincible = true;
	}

	public static Storage CreateDefaultStorage(GameObject go, bool forceCreate = false)
	{
		Storage storage = (!forceCreate) ? go.AddOrGet<Storage>() : go.AddComponent<Storage>();
		storage.capacityKg = 2000f;
		return storage;
	}

	public static void CreateComplexFabricatorStorage(GameObject go, ComplexFabricator fabricator)
	{
		fabricator.inStorage = go.AddComponent<Storage>();
		fabricator.inStorage.capacityKg = 20000f;
		fabricator.inStorage.showInUI = true;
		fabricator.inStorage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
		fabricator.buildStorage = go.AddComponent<Storage>();
		fabricator.buildStorage.capacityKg = 20000f;
		fabricator.buildStorage.showInUI = true;
		fabricator.buildStorage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
		fabricator.outStorage = go.AddComponent<Storage>();
		fabricator.outStorage.capacityKg = 20000f;
		fabricator.outStorage.showInUI = true;
		fabricator.outStorage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
	}

	public static void DoPostConfigure(GameObject go)
	{
	}
}
