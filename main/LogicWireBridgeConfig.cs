using STRINGS;
using TUNING;
using UnityEngine;

public class LogicWireBridgeConfig : IBuildingConfig
{
	public const string ID = "LogicWireBridge";

	public static readonly HashedString BRIDGE_LOGIC_IO_ID = new HashedString("BRIDGE_LOGIC_IO");

	public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[2]
	{
		LogicPorts.Port.InputPort(BRIDGE_LOGIC_IO_ID, new CellOffset(-1, 0), STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_PORT_DESC, false),
		LogicPorts.Port.InputPort(BRIDGE_LOGIC_IO_ID, new CellOffset(1, 0), STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_PORT_DESC, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "LogicWireBridge";
		int width = 3;
		int height = 1;
		string anim = "logic_bridge_kanim";
		int hitpoints = 30;
		float construction_time = 3f;
		float[] tIER_TINY = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER_TINY;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.LogicBridge;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER_TINY, rEFINED_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, nONE, 0.2f);
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.ObjectLayer = ObjectLayer.LogicGates;
		buildingDef.SceneLayer = Grid.SceneLayer.LogicWireBridges;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, "LogicWireBridge");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		LogicUtilityNetworkLink logicUtilityNetworkLink = AddNetworkLink(go);
		logicUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		Constructable component = go.GetComponent<Constructable>();
		component.choreTags = GameTags.ChoreTypes.WiringChores;
		LogicUtilityNetworkLink logicUtilityNetworkLink = AddNetworkLink(go);
		logicUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicUtilityNetworkLink logicUtilityNetworkLink = AddNetworkLink(go);
		logicUtilityNetworkLink.visualizeOnly = false;
		go.AddOrGet<BuildingCellVisualizer>();
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	private LogicUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		LogicUtilityNetworkLink logicUtilityNetworkLink = go.AddOrGet<LogicUtilityNetworkLink>();
		logicUtilityNetworkLink.link1 = new CellOffset(-1, 0);
		logicUtilityNetworkLink.link2 = new CellOffset(1, 0);
		return logicUtilityNetworkLink;
	}
}
