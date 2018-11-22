using TUNING;
using UnityEngine;

public class LiquidConduitConfig : IBuildingConfig
{
	public const string ID = "LiquidConduit";

	public static void CommonConduitPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RemoveLoopingSounds(go);
	}

	public override BuildingDef CreateBuildingDef()
	{
		string id = "LiquidConduit";
		int width = 1;
		int height = 1;
		string anim = "utilities_liquid_kanim";
		int hitpoints = 10;
		float construction_time = 3f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] pLUMBABLE = MATERIALS.PLUMBABLE;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, pLUMBABLE, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.ObjectLayer = ObjectLayer.LiquidConduit;
		buildingDef.TileLayer = ObjectLayer.LiquidConduitTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementLiquidConduit;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.SceneLayer = Grid.SceneLayer.LiquidConduits;
		buildingDef.isKAnimTile = true;
		buildingDef.isUtility = true;
		buildingDef.DragBuild = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidConduit");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		Conduit conduit = go.AddOrGet<Conduit>();
		conduit.type = ConduitType.Liquid;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
		go.AddComponent<EmptyConduitWorkable>();
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
		CommonConduitPostConfigureComplete(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
	}
}
