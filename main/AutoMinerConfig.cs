using STRINGS;
using System;
using System.Runtime.CompilerServices;
using TUNING;
using UnityEngine;

public class AutoMinerConfig : IBuildingConfig
{
	public const string ID = "AutoMiner";

	private const int RANGE = 7;

	private const int X = -7;

	private const int Y = 0;

	private const int WIDTH = 16;

	private const int HEIGHT = 9;

	private const int VISION_OFFSET = 1;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};

	[CompilerGenerated]
	private static Func<int, bool> _003C_003Ef__mg_0024cache0;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AutoMiner", 2, 2, "auto_miner_kanim", 10, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFoundationRotatable, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "AutoMiner");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Operational>();
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<MiningSounds>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		AddVisualizer(go, true);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		AddVisualizer(go, false);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		AutoMiner autoMiner = go.AddOrGet<AutoMiner>();
		autoMiner.x = -7;
		autoMiner.y = 0;
		autoMiner.width = 16;
		autoMiner.height = 9;
		autoMiner.vision_offset = new CellOffset(0, 1);
		AddVisualizer(go, false);
	}

	private static void AddVisualizer(GameObject prefab, bool movable)
	{
		StationaryChoreRangeVisualizer stationaryChoreRangeVisualizer = prefab.AddOrGet<StationaryChoreRangeVisualizer>();
		stationaryChoreRangeVisualizer.x = -7;
		stationaryChoreRangeVisualizer.y = 0;
		stationaryChoreRangeVisualizer.width = 16;
		stationaryChoreRangeVisualizer.height = 9;
		stationaryChoreRangeVisualizer.vision_offset = new CellOffset(0, 1);
		stationaryChoreRangeVisualizer.movable = movable;
		stationaryChoreRangeVisualizer.blocking_tile_visible = false;
		KPrefabID component = prefab.GetComponent<KPrefabID>();
		component.instantiateFn += delegate(GameObject go)
		{
			go.GetComponent<StationaryChoreRangeVisualizer>().blocking_cb = AutoMiner.DigBlockingCB;
		};
	}
}
