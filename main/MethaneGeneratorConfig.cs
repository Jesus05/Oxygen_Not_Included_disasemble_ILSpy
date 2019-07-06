using TUNING;
using UnityEngine;

public class MethaneGeneratorConfig : IBuildingConfig
{
	public const string ID = "MethaneGenerator";

	public const float FUEL_CONSUMPTION_RATE = 0.09f;

	private const float CO2_RATIO = 0.25f;

	public const float WATER_OUTPUT_TEMPERATURE = 313.15f;

	private const int WIDTH = 4;

	private const int HEIGHT = 3;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "MethaneGenerator";
		int width = 4;
		int height = 3;
		string anim = "generatormethane_kanim";
		int hitpoints = 100;
		float construction_time = 120f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] rAW_METALS = MATERIALS.RAW_METALS;
		float melting_point = 2400f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER2, tIER2, 0.2f);
		buildingDef.GeneratorWattageRating = 800f;
		buildingDef.GeneratorBaseCapacity = 1000f;
		buildingDef.ExhaustKilowattsWhenActive = 2f;
		buildingDef.SelfHeatKilowattsWhenActive = 8f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(2, 2);
		buildingDef.PowerOutputOffset = new CellOffset(0, 0);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<LoopingSounds>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 50f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 0.900000036f;
		conduitConsumer.capacityTag = GameTags.CombustibleGas;
		conduitConsumer.capacityKG = 0.900000036f;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.powerDistributionOrder = 8;
		energyGenerator.ignoreBatteryRefillPercent = true;
		EnergyGenerator.Formula formula = default(EnergyGenerator.Formula);
		formula.inputs = new EnergyGenerator.InputItem[1]
		{
			new EnergyGenerator.InputItem(GameTags.CombustibleGas, 0.09f, 0.900000036f)
		};
		formula.outputs = new EnergyGenerator.OutputItem[2]
		{
			new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.0675f, false, new CellOffset(1, 1), 313.15f),
			new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.0225f, true, new CellOffset(0, 2), 383.15f)
		};
		energyGenerator.formula = formula;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Gas;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[2]
		{
			SimHashes.Methane,
			SimHashes.Syngas
		};
		Tinkerable.MakePowerTinkerable(go);
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
