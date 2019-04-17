using TUNING;
using UnityEngine;

public class PetroleumGeneratorConfig : IBuildingConfig
{
	public const string ID = "PetroleumGenerator";

	public const float CONSUMPTION_RATE = 2f;

	private const SimHashes INPUT_ELEMENT = SimHashes.Petroleum;

	private const SimHashes EXHAUST_ELEMENT_GAS = SimHashes.CarbonDioxide;

	private const SimHashes EXHAUST_ELEMENT_LIQUID = SimHashes.DirtyWater;

	public const float EFFICIENCY_RATE = 0.5f;

	public const float EXHAUST_GAS_RATE = 0.5f;

	public const float EXHAUST_LIQUID_RATE = 0.75f;

	private const int WIDTH = 3;

	private const int HEIGHT = 4;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "PetroleumGenerator";
		int width = 3;
		int height = 4;
		string anim = "generatorpetrol_kanim";
		int hitpoints = 100;
		float construction_time = 480f;
		string[] construction_materials = new string[2]
		{
			"Metal",
			"Plastic"
		};
		EffectorValues tIER = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
		}, construction_materials, 2400f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tIER, 0.2f);
		buildingDef.GeneratorWattageRating = 2000f;
		buildingDef.GeneratorBaseCapacity = 2000f;
		buildingDef.ExhaustKilowattsWhenActive = 4f;
		buildingDef.SelfHeatKilowattsWhenActive = 16f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.InputConduitType = ConduitType.Liquid;
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
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Storage>();
		BuildingDef def = go.GetComponent<Building>().Def;
		float num = 20f;
		go.AddOrGet<LoopingSounds>();
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = def.InputConduitType;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = SimHashes.Petroleum.CreateTag();
		conduitConsumer.capacityKG = num;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.powerDistributionOrder = 8;
		energyGenerator.ignoreBatteryRefillPercent = true;
		energyGenerator.hasMeter = true;
		EnergyGenerator.Formula formula = default(EnergyGenerator.Formula);
		formula.inputs = new EnergyGenerator.InputItem[1]
		{
			new EnergyGenerator.InputItem(SimHashes.Petroleum.CreateTag(), 2f, num)
		};
		formula.outputs = new EnergyGenerator.OutputItem[2]
		{
			new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.5f, false, new CellOffset(0, 3), 0f),
			new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.75f, false, new CellOffset(1, 1), 0f)
		};
		energyGenerator.formula = formula;
		Tinkerable.MakePowerTinkerable(go);
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
