using System.Collections.Generic;
using System.Diagnostics;

public class CellEventLogger : EventLogger<CellEventInstance, CellEvent>
{
	public static CellEventLogger Instance;

	public CellSolidEvent SimMessagesSolid;

	public CellSolidEvent SimCellOccupierDestroy;

	public CellSolidEvent SimCellOccupierForceSolid;

	public CellSolidEvent SimCellOccupierSolidChanged;

	public CellElementEvent DoorOpen;

	public CellElementEvent DoorClose;

	public CellElementEvent Excavator;

	public CellElementEvent DebugTool;

	public CellElementEvent SandBoxTool;

	public CellElementEvent TemplateLoader;

	public CellElementEvent Scenario;

	public CellElementEvent SimCellOccupierOnSpawn;

	public CellElementEvent SimCellOccupierDestroySelf;

	public CellElementEvent WorldGapManager;

	public CellElementEvent ReceiveElementChanged;

	public CellElementEvent ObjectSetSimOnSpawn;

	public CellElementEvent DecompositionDirtyWater;

	public CellCallbackEvent SendCallback;

	public CellCallbackEvent ReceiveCallback;

	public CellDigEvent Dig;

	public CellAddRemoveSubstanceEvent WorldDamageDelayedSpawnFX;

	public CellAddRemoveSubstanceEvent SublimatesEmit;

	public CellAddRemoveSubstanceEvent OxygenModifierSimUpdate;

	public CellAddRemoveSubstanceEvent LiquidChunkOnStore;

	public CellAddRemoveSubstanceEvent FallingWaterAddToSim;

	public CellAddRemoveSubstanceEvent ExploderOnSpawn;

	public CellAddRemoveSubstanceEvent ExhaustSimUpdate;

	public CellAddRemoveSubstanceEvent ElementConsumerSimUpdate;

	public CellAddRemoveSubstanceEvent ElementChunkTransition;

	public CellAddRemoveSubstanceEvent OxyrockEmit;

	public CellAddRemoveSubstanceEvent BleachstoneEmit;

	public CellAddRemoveSubstanceEvent UnstableGround;

	public CellAddRemoveSubstanceEvent ConduitFlowEmptyConduit;

	public CellAddRemoveSubstanceEvent ConduitConsumerWrongElement;

	public CellAddRemoveSubstanceEvent OverheatableMeltingDown;

	public CellAddRemoveSubstanceEvent FabricatorProduceMelted;

	public CellAddRemoveSubstanceEvent PumpSimUpdate;

	public CellAddRemoveSubstanceEvent WallPumpSimUpdate;

	public CellAddRemoveSubstanceEvent Vomit;

	public CellAddRemoveSubstanceEvent Tears;

	public CellAddRemoveSubstanceEvent Pee;

	public CellAddRemoveSubstanceEvent AlgaeHabitat;

	public CellAddRemoveSubstanceEvent CO2FilterOxygen;

	public CellAddRemoveSubstanceEvent ToiletEmit;

	public CellAddRemoveSubstanceEvent ElementEmitted;

	public CellAddRemoveSubstanceEvent Mop;

	public CellAddRemoveSubstanceEvent OreMelted;

	public CellAddRemoveSubstanceEvent ConstructTile;

	public CellAddRemoveSubstanceEvent Dumpable;

	public CellAddRemoveSubstanceEvent Cough;

	public CellAddRemoveSubstanceEvent Meteor;

	public CellModifyMassEvent CO2ManagerFixedUpdate;

	public CellModifyMassEvent EnvironmentConsumerFixedUpdate;

	public CellModifyMassEvent ExcavatorShockwave;

	public CellModifyMassEvent OxygenBreatherSimUpdate;

	public CellModifyMassEvent CO2ScrubberSimUpdate;

	public CellModifyMassEvent RiverSourceSimUpdate;

	public CellModifyMassEvent RiverTerminusSimUpdate;

	public CellModifyMassEvent DebugToolModifyMass;

	public CellModifyMassEvent EnergyGeneratorModifyMass;

	public CellSolidFilterEvent SolidFilterEvent;

	public Dictionary<int, int> CallbackToCellMap = new Dictionary<int, int>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	[Conditional("ENABLE_CELL_EVENT_LOGGER")]
	public void LogCallbackSend(int cell, int callback_id)
	{
		if (callback_id != -1)
		{
			CallbackToCellMap[callback_id] = cell;
		}
	}

	[Conditional("ENABLE_CELL_EVENT_LOGGER")]
	public void LogCallbackReceive(int callback_id)
	{
		int value = Grid.InvalidCell;
		if (!CallbackToCellMap.TryGetValue(callback_id, out value))
		{
			return;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		SimMessagesSolid = (AddEvent(new CellSolidEvent("SimMessageSolid", "Sim Message", false, true)) as CellSolidEvent);
		SimCellOccupierDestroy = (AddEvent(new CellSolidEvent("SimCellOccupierClearSolid", "Sim Cell Occupier Destroy", false, true)) as CellSolidEvent);
		SimCellOccupierForceSolid = (AddEvent(new CellSolidEvent("SimCellOccupierForceSolid", "Sim Cell Occupier Force Solid", false, true)) as CellSolidEvent);
		SimCellOccupierSolidChanged = (AddEvent(new CellSolidEvent("SimCellOccupierSolidChanged", "Sim Cell Occupier Solid Changed", false, true)) as CellSolidEvent);
		DoorOpen = (AddEvent(new CellElementEvent("DoorOpen", "Door Open", true, true)) as CellElementEvent);
		DoorClose = (AddEvent(new CellElementEvent("DoorClose", "Door Close", true, true)) as CellElementEvent);
		Excavator = (AddEvent(new CellElementEvent("Excavator", "Excavator", true, true)) as CellElementEvent);
		DebugTool = (AddEvent(new CellElementEvent("DebugTool", "Debug Tool", true, true)) as CellElementEvent);
		SandBoxTool = (AddEvent(new CellElementEvent("SandBoxTool", "Sandbox Tool", true, true)) as CellElementEvent);
		TemplateLoader = (AddEvent(new CellElementEvent("TemplateLoader", "Template Loader", true, true)) as CellElementEvent);
		Scenario = (AddEvent(new CellElementEvent("Scenario", "Scenario", true, true)) as CellElementEvent);
		SimCellOccupierOnSpawn = (AddEvent(new CellElementEvent("SimCellOccupierOnSpawn", "Sim Cell Occupier OnSpawn", true, true)) as CellElementEvent);
		SimCellOccupierDestroySelf = (AddEvent(new CellElementEvent("SimCellOccupierDestroySelf", "Sim Cell Occupier Destroy Self", true, true)) as CellElementEvent);
		WorldGapManager = (AddEvent(new CellElementEvent("WorldGapManager", "World Gap Manager", true, true)) as CellElementEvent);
		ReceiveElementChanged = (AddEvent(new CellElementEvent("ReceiveElementChanged", "Sim Message", false, false)) as CellElementEvent);
		ObjectSetSimOnSpawn = (AddEvent(new CellElementEvent("ObjectSetSimOnSpawn", "Object set sim on spawn", true, true)) as CellElementEvent);
		DecompositionDirtyWater = (AddEvent(new CellElementEvent("DecompositionDirtyWater", "Decomposition dirty water", true, true)) as CellElementEvent);
		SendCallback = (AddEvent(new CellCallbackEvent("SendCallback", true, true)) as CellCallbackEvent);
		ReceiveCallback = (AddEvent(new CellCallbackEvent("ReceiveCallback", false, true)) as CellCallbackEvent);
		Dig = (AddEvent(new CellDigEvent(true)) as CellDigEvent);
		WorldDamageDelayedSpawnFX = (AddEvent(new CellAddRemoveSubstanceEvent("WorldDamageDelayedSpawnFX", "World Damage Delayed Spawn FX", false)) as CellAddRemoveSubstanceEvent);
		OxygenModifierSimUpdate = (AddEvent(new CellAddRemoveSubstanceEvent("OxygenModifierSimUpdate", "Oxygen Modifier SimUpdate", false)) as CellAddRemoveSubstanceEvent);
		LiquidChunkOnStore = (AddEvent(new CellAddRemoveSubstanceEvent("LiquidChunkOnStore", "Liquid Chunk On Store", false)) as CellAddRemoveSubstanceEvent);
		FallingWaterAddToSim = (AddEvent(new CellAddRemoveSubstanceEvent("FallingWaterAddToSim", "Falling Water Add To Sim", false)) as CellAddRemoveSubstanceEvent);
		ExploderOnSpawn = (AddEvent(new CellAddRemoveSubstanceEvent("ExploderOnSpawn", "Exploder OnSpawn", false)) as CellAddRemoveSubstanceEvent);
		ExhaustSimUpdate = (AddEvent(new CellAddRemoveSubstanceEvent("ExhaustSimUpdate", "Exhaust SimUpdate", false)) as CellAddRemoveSubstanceEvent);
		ElementConsumerSimUpdate = (AddEvent(new CellAddRemoveSubstanceEvent("ElementConsumerSimUpdate", "Element Consumer SimUpdate", false)) as CellAddRemoveSubstanceEvent);
		SublimatesEmit = (AddEvent(new CellAddRemoveSubstanceEvent("SublimatesEmit", "Sublimates Emit", false)) as CellAddRemoveSubstanceEvent);
		Mop = (AddEvent(new CellAddRemoveSubstanceEvent("Mop", "Mop", false)) as CellAddRemoveSubstanceEvent);
		OreMelted = (AddEvent(new CellAddRemoveSubstanceEvent("OreMelted", "Ore Melted", false)) as CellAddRemoveSubstanceEvent);
		ConstructTile = (AddEvent(new CellAddRemoveSubstanceEvent("ConstructTile", "ConstructTile", false)) as CellAddRemoveSubstanceEvent);
		Dumpable = (AddEvent(new CellAddRemoveSubstanceEvent("Dympable", "Dumpable", false)) as CellAddRemoveSubstanceEvent);
		Cough = (AddEvent(new CellAddRemoveSubstanceEvent("Cough", "Cough", false)) as CellAddRemoveSubstanceEvent);
		Meteor = (AddEvent(new CellAddRemoveSubstanceEvent("Meteor", "Meteor", false)) as CellAddRemoveSubstanceEvent);
		ElementChunkTransition = (AddEvent(new CellAddRemoveSubstanceEvent("ElementChunkTransition", "Element Chunk Transition", false)) as CellAddRemoveSubstanceEvent);
		OxyrockEmit = (AddEvent(new CellAddRemoveSubstanceEvent("OxyrockEmit", "Oxyrock Emit", false)) as CellAddRemoveSubstanceEvent);
		BleachstoneEmit = (AddEvent(new CellAddRemoveSubstanceEvent("BleachstoneEmit", "Bleachstone Emit", false)) as CellAddRemoveSubstanceEvent);
		UnstableGround = (AddEvent(new CellAddRemoveSubstanceEvent("UnstableGround", "Unstable Ground", false)) as CellAddRemoveSubstanceEvent);
		ConduitFlowEmptyConduit = (AddEvent(new CellAddRemoveSubstanceEvent("ConduitFlowEmptyConduit", "Conduit Flow Empty Conduit", false)) as CellAddRemoveSubstanceEvent);
		ConduitConsumerWrongElement = (AddEvent(new CellAddRemoveSubstanceEvent("ConduitConsumerWrongElement", "Conduit Consumer Wrong Element", false)) as CellAddRemoveSubstanceEvent);
		OverheatableMeltingDown = (AddEvent(new CellAddRemoveSubstanceEvent("OverheatableMeltingDown", "Overheatable MeltingDown", false)) as CellAddRemoveSubstanceEvent);
		FabricatorProduceMelted = (AddEvent(new CellAddRemoveSubstanceEvent("FabricatorProduceMelted", "Fabricator Produce Melted", false)) as CellAddRemoveSubstanceEvent);
		PumpSimUpdate = (AddEvent(new CellAddRemoveSubstanceEvent("PumpSimUpdate", "Pump SimUpdate", false)) as CellAddRemoveSubstanceEvent);
		WallPumpSimUpdate = (AddEvent(new CellAddRemoveSubstanceEvent("WallPumpSimUpdate", "Wall Pump SimUpdate", false)) as CellAddRemoveSubstanceEvent);
		Vomit = (AddEvent(new CellAddRemoveSubstanceEvent("Vomit", "Vomit", false)) as CellAddRemoveSubstanceEvent);
		Tears = (AddEvent(new CellAddRemoveSubstanceEvent("Tears", "Tears", false)) as CellAddRemoveSubstanceEvent);
		Pee = (AddEvent(new CellAddRemoveSubstanceEvent("Pee", "Pee", false)) as CellAddRemoveSubstanceEvent);
		AlgaeHabitat = (AddEvent(new CellAddRemoveSubstanceEvent("AlgaeHabitat", "AlgaeHabitat", false)) as CellAddRemoveSubstanceEvent);
		CO2FilterOxygen = (AddEvent(new CellAddRemoveSubstanceEvent("CO2FilterOxygen", "CO2FilterOxygen", false)) as CellAddRemoveSubstanceEvent);
		ToiletEmit = (AddEvent(new CellAddRemoveSubstanceEvent("ToiletEmit", "ToiletEmit", false)) as CellAddRemoveSubstanceEvent);
		ElementEmitted = (AddEvent(new CellAddRemoveSubstanceEvent("ElementEmitted", "Element Emitted", false)) as CellAddRemoveSubstanceEvent);
		CO2ManagerFixedUpdate = (AddEvent(new CellModifyMassEvent("CO2ManagerFixedUpdate", "CO2Manager FixedUpdate", false)) as CellModifyMassEvent);
		EnvironmentConsumerFixedUpdate = (AddEvent(new CellModifyMassEvent("EnvironmentConsumerFixedUpdate", "EnvironmentConsumer FixedUpdate", false)) as CellModifyMassEvent);
		ExcavatorShockwave = (AddEvent(new CellModifyMassEvent("ExcavatorShockwave", "Excavator Shockwave", false)) as CellModifyMassEvent);
		OxygenBreatherSimUpdate = (AddEvent(new CellModifyMassEvent("OxygenBreatherSimUpdate", "Oxygen Breather SimUpdate", false)) as CellModifyMassEvent);
		CO2ScrubberSimUpdate = (AddEvent(new CellModifyMassEvent("CO2ScrubberSimUpdate", "CO2Scrubber SimUpdate", false)) as CellModifyMassEvent);
		RiverSourceSimUpdate = (AddEvent(new CellModifyMassEvent("RiverSourceSimUpdate", "RiverSource SimUpdate", false)) as CellModifyMassEvent);
		RiverTerminusSimUpdate = (AddEvent(new CellModifyMassEvent("RiverTerminusSimUpdate", "RiverTerminus SimUpdate", false)) as CellModifyMassEvent);
		DebugToolModifyMass = (AddEvent(new CellModifyMassEvent("DebugToolModifyMass", "DebugTool ModifyMass", false)) as CellModifyMassEvent);
		EnergyGeneratorModifyMass = (AddEvent(new CellModifyMassEvent("EnergyGeneratorModifyMass", "EnergyGenerator ModifyMass", false)) as CellModifyMassEvent);
		SolidFilterEvent = (AddEvent(new CellSolidFilterEvent("SolidFilterEvent", true)) as CellSolidFilterEvent);
	}
}
