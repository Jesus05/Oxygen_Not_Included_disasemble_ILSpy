using STRINGS;
using TUNING;
using UnityEngine;

public class LogicTimeOfDaySensorConfig : IBuildingConfig
{
	public static string ID = "LogicTimeOfDaySensor";

	public static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICTIMEOFDAYSENSOR.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICTIMEOFDAYSENSOR.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICTIMEOFDAYSENSOR.LOGIC_PORT_INACTIVE, false);

	public override BuildingDef CreateBuildingDef()
	{
		string iD = ID;
		int width = 1;
		int height = 1;
		string anim = "switchtimeofday_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(iD, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, nONE, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		SoundEventVolumeCache.instance.AddVolume("switchgaspressure_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchgaspressure_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		GeneratedBuildings.RegisterLogicPorts(go, OUTPUT_PORT);
		LogicTimeOfDaySensor logicTimeOfDaySensor = go.AddOrGet<LogicTimeOfDaySensor>();
		logicTimeOfDaySensor.manuallyControlled = false;
	}
}
