using STRINGS;
using TUNING;
using UnityEngine;

public class GeneratorConfig : IBuildingConfig
{
	public const string ID = "Generator";

	private const float COAL_BURN_RATE = 1f;

	private const float COAL_CAPACITY = 600f;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "Generator";
		int width = 3;
		int height = 3;
		string anim = "generatorphos_kanim";
		int hitpoints = 100;
		float construction_time = 120f;
		float[] tIER = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 2400f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tIER2, 0.2f);
		buildingDef.GeneratorWattageRating = 600f;
		buildingDef.GeneratorBaseCapacity = 20000f;
		buildingDef.ExhaustKilowattsWhenActive = 8f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.formula = EnergyGenerator.CreateSimpleFormula(SimHashes.Carbon, 1f, 600f, SimHashes.Void, 0f, true);
		energyGenerator.meterOffset = Meter.Offset.Behind;
		energyGenerator.SetSliderValue(50f, 0);
		energyGenerator.powerDistributionOrder = 9;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 600f;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Coal");
		manualDeliveryKG.capacity = storage.capacityKg;
		manualDeliveryKG.refillMass = 100f;
		manualDeliveryKG.choreTags = new Tag[1]
		{
			GameTags.ChoreTypes.Power
		};
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;
		BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
		buildingElementEmitter.emitRate = 0.02f;
		buildingElementEmitter.temperature = 310f;
		buildingElementEmitter.element = SimHashes.CarbonDioxide;
		buildingElementEmitter.modifierOffset = new Vector2(1f, 2f);
		Tinkerable.MakePowerTinkerable(go);
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
		GeneratedBuildings.RegisterLogicPorts(go, INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
