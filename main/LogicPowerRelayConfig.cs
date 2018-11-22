using STRINGS;
using TUNING;
using UnityEngine;

public class LogicPowerRelayConfig : IBuildingConfig
{
	public static string ID = "LogicPowerRelay";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICPOWERRELAY.LOGIC_PORT_DESC, true)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string iD = ID;
		int width = 1;
		int height = 1;
		string anim = "switchpowershutoff_kanim";
		int hitpoints = 10;
		float construction_time = 30f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(iD, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, nONE, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		OperationalControlledSwitch operationalControlledSwitch = go.AddOrGet<OperationalControlledSwitch>();
		operationalControlledSwitch.objectLayer = ObjectLayer.Wire;
	}
}
