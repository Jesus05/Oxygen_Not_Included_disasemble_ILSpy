using TUNING;
using UnityEngine;

public class GasConduitConfig : IBuildingConfig
{
	public const string ID = "GasConduit";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "GasConduit";
		int width = 1;
		int height = 1;
		string anim = "utilities_gas_kanim";
		int hitpoints = 10;
		float construction_time = 3f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] rAW_MINERALS = MATERIALS.RAW_MINERALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_MINERALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.GasConduits.ID;
		buildingDef.ObjectLayer = ObjectLayer.GasConduit;
		buildingDef.TileLayer = ObjectLayer.GasConduitTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementGasConduit;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = 0f;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.SceneLayer = Grid.SceneLayer.GasConduits;
		buildingDef.isKAnimTile = true;
		buildingDef.isUtility = true;
		buildingDef.DragBuild = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, buildingDef.PrefabID);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		Conduit conduit = go.AddOrGet<Conduit>();
		conduit.type = ConduitType.Gas;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
		go.AddComponent<EmptyConduitWorkable>();
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Gas;
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
		LiquidConduitConfig.CommonConduitPostConfigureComplete(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Gas;
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
	}
}
