using TUNING;
using UnityEngine;

public class SolidConduitConfig : IBuildingConfig
{
	public const string ID = "SolidConduit";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "SolidConduit";
		int width = 1;
		int height = 1;
		string anim = "utilities_conveyor_kanim";
		int hitpoints = 10;
		float construction_time = 3f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] rAW_METALS = MATERIALS.RAW_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, nONE, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
		buildingDef.ObjectLayer = ObjectLayer.SolidConduit;
		buildingDef.TileLayer = ObjectLayer.SolidConduitTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementSolidConduit;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = 0f;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.SceneLayer = Grid.SceneLayer.SolidConduits;
		buildingDef.isKAnimTile = true;
		buildingDef.isUtility = true;
		buildingDef.DragBuild = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidConduit");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<SolidConduit>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Solid;
		kAnimGraphTileVisualizer.isPhysicalBuilding = false;
		Constructable component = go.GetComponent<Constructable>();
		component.choreTags = GameTags.ChoreTypes.ConveyorChores;
		component.requiredRolePerk = RoleManager.rolePerks.ConveyorBuild.id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
		KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Solid;
		kAnimGraphTileVisualizer.isPhysicalBuilding = true;
		LiquidConduitConfig.CommonConduitPostConfigureComplete(go);
	}
}
