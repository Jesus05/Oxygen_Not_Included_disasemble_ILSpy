using STRINGS;
using TUNING;
using UnityEngine;

public class LogicMemoryConfig : IBuildingConfig
{
	public static string ID = "LogicMemory";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[2]
	{
		new LogicPorts.Port(LogicMemory.SET_PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.SET_PORT_DESC, true, LogicPortSpriteType.Input),
		new LogicPorts.Port(LogicMemory.RESET_PORT_ID, new CellOffset(1, 0), STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.RESET_PORT_DESC, true, LogicPortSpriteType.ResetUpdate)
	};

	private static readonly LogicPorts.Port[] OUTPUT_PORTS = new LogicPorts.Port[1]
	{
		new LogicPorts.Port(LogicMemory.READ_PORT_ID, new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.READ_PORT_DESC, true, LogicPortSpriteType.Output)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string iD = ID;
		int width = 2;
		int height = 2;
		string anim = "logic_memory_kanim";
		int hitpoints = 10;
		float construction_time = 30f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(iD, width, height, anim, hitpoints, construction_time, tIER, rEFINED_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.NONE, nONE, 0.2f);
		buildingDef.Deprecated = false;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.WireBridges;
		buildingDef.ObjectLayer = ObjectLayer.LogicGates;
		SoundEventVolumeCache.instance.AddVolume("logic_memory_kanim", "PowerMemory_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("logic_memory_kanim", "PowerMemory_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS, OUTPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS, OUTPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		go.AddOrGet<LogicMemory>();
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS, OUTPUT_PORTS);
	}
}
