using TUNING;
using UnityEngine;

public class WoodGasGeneratorConfig : IBuildingConfig
{
	public const string ID = "WoodGasGenerator";

	private const float BRANCHES_PER_GENERATOR = 12f;

	public const float CONSUMPTION_RATE = 1.71428573f;

	private const float WOOD_PER_REFILL = 514.2857f;

	private const SimHashes EXHAUST_ELEMENT_GAS = SimHashes.CarbonDioxide;

	private const SimHashes EXHAUST_ELEMENT_GAS2 = SimHashes.Syngas;

	public const float WASTE_RATE = 0.1f;

	public const float CO2_EXHAUST_RATE = 0.171428576f;

	private const int WIDTH = 2;

	private const int HEIGHT = 2;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "WoodGasGenerator";
		int width = 2;
		int height = 2;
		string anim = "generatorwood_kanim";
		int hitpoints = 100;
		float construction_time = 120f;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		EffectorValues tIER = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, aLL_METALS, 2400f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tIER, 0.2f);
		buildingDef.GeneratorWattageRating = 300f;
		buildingDef.GeneratorBaseCapacity = 20000f;
		buildingDef.ExhaustKilowattsWhenActive = 8f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerOutputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_1_1);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_1_1);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_1_1);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		float max_stored_mass = 1028.57141f;
		go.AddOrGet<LoopingSounds>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = WoodLogConfig.TAG;
		manualDeliveryKG.capacity = 514.2857f;
		manualDeliveryKG.refillMass = 257.142853f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.powerDistributionOrder = 8;
		energyGenerator.ignoreBatteryRefillPercent = true;
		energyGenerator.hasMeter = true;
		EnergyGenerator.Formula formula = default(EnergyGenerator.Formula);
		formula.inputs = new EnergyGenerator.InputItem[1]
		{
			new EnergyGenerator.InputItem(WoodLogConfig.TAG, 1.71428573f, max_stored_mass)
		};
		formula.outputs = new EnergyGenerator.OutputItem[1]
		{
			new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.171428576f, false, new CellOffset(0, 1), 0f)
		};
		energyGenerator.formula = formula;
		Tinkerable.MakePowerTinkerable(go);
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
