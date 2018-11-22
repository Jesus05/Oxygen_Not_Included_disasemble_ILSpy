using TUNING;
using UnityEngine;

public class FarmTileConfig : IBuildingConfig
{
	public const string ID = "FarmTile";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "FarmTile";
		int width = 1;
		int height = 1;
		string anim = "farmtilerotating_kanim";
		int hitpoints = 100;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] fARMABLE = MATERIALS.FARMABLE;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Tile;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, fARMABLE, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Overheatable = false;
		buildingDef.IsFoundation = true;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.SceneLayer = Grid.SceneLayer.TileFront;
		buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
		buildingDef.PermittedRotations = PermittedRotations.FlipV;
		buildingDef.isSolidTile = false;
		buildingDef.DragBuild = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.notifyOnMelt = true;
		go.AddOrGet<TileTemperature>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.occupyingObjectRelativePosition = new Vector3(0f, 1f, 0f);
		plantablePlot.AddDepositTag(GameTags.CropSeed);
		plantablePlot.AddDepositTag(GameTags.WaterSeed);
		plantablePlot.SetFertilizationFlags(true, false);
		CopyBuildingSettings copyBuildingSettings = go.AddOrGet<CopyBuildingSettings>();
		copyBuildingSettings.copyGroupTag = GameTags.Farm;
		go.AddOrGet<AnimTileable>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RemoveLoopingSounds(go);
		SetUpFarmPlotTags(go);
	}

	public static void SetUpFarmPlotTags(GameObject go)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.prefabSpawnFn += delegate(GameObject inst)
		{
			Rotatable component2 = inst.GetComponent<Rotatable>();
			PlantablePlot component3 = inst.GetComponent<PlantablePlot>();
			switch (component2.GetOrientation())
			{
			case Orientation.NumRotations:
				break;
			case Orientation.Neutral:
			case Orientation.FlipH:
				component3.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Top);
				break;
			case Orientation.R180:
			case Orientation.FlipV:
				component3.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Bottom);
				break;
			case Orientation.R90:
			case Orientation.R270:
				component3.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Side);
				break;
			}
		};
	}
}
