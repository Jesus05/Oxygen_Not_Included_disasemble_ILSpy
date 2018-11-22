using TUNING;
using UnityEngine;

public class LogicTemperatureSensorConfig : IBuildingConfig
{
	public static string ID = "LogicTemperatureSensor";

	public override BuildingDef CreateBuildingDef()
	{
		string iD = ID;
		int width = 1;
		int height = 1;
		string anim = "switchthermal_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(iD, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER0, nONE, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
		LogicTemperatureSensor logicTemperatureSensor = go.AddOrGet<LogicTemperatureSensor>();
		logicTemperatureSensor.manuallyControlled = false;
		logicTemperatureSensor.minTemp = 0f;
		logicTemperatureSensor.maxTemp = 573.15f;
	}
}
