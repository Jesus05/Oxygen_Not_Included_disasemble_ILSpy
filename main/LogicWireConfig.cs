using TUNING;
using UnityEngine;

public class LogicWireConfig : IBuildingConfig
{
	public const string ID = "LogicWire";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "LogicWire";
		int width = 1;
		int height = 1;
		string anim = "logic_wires_kanim";
		int hitpoints = 10;
		float construction_time = 3f;
		float[] tIER_TINY = BUILDINGS.CONSTRUCTION_MASS_KG.TIER_TINY;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER_TINY, rEFINED_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER0, nONE, 0.2f);
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.ObjectLayer = ObjectLayer.LogicWires;
		buildingDef.TileLayer = ObjectLayer.LogicWiresTiling;
		buildingDef.SceneLayer = Grid.SceneLayer.LogicWires;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.isKAnimTile = true;
		buildingDef.isUtility = true;
		buildingDef.DragBuild = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, "LogicWire");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Logic;
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		Constructable component = go.GetComponent<Constructable>();
		component.isDiggingRequired = false;
		component.choreTags = GameTags.ChoreTypes.WiringChores;
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Logic;
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicWire>();
	}
}
