using TUNING;
using UnityEngine;

public class EthanolDistilleryConfig : IBuildingConfig
{
	public const string ID = "EthanolDistillery";

	public const float ORGANICS_CONSUME_PER_SECOND = 0.75f;

	public const float ORGANICS_STORAGE_AMOUNT = 450f;

	public const float WASTE_RATE = 0.333333343f;

	public const float SOLID_WASTE_RATE = 0.166666672f;

	public const float CO2_WASTE_RATE = 0.0833333358f;

	public const float ETHANOL_RATE = 0.5f;

	public const float OUTPUT_TEMPERATURE = 346.5f;

	public const float WASTE_OUTPUT_TEMPERATURE = 366.5f;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "EthanolDistillery";
		int width = 3;
		int height = 4;
		string anim = "algae_distillery_kanim";
		int hitpoints = 100;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 800f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, tIER2, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = new SimHashes[1]
		{
			SimHashes.Ethanol
		};
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = WoodLogConfig.TAG;
		manualDeliveryKG.capacity = 450f;
		manualDeliveryKG.refillMass = 112.5f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(WoodLogConfig.TAG, 0.75f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[3]
		{
			new ElementConverter.OutputElement(0.5f, SimHashes.Ethanol, 346.5f, true, 0f, 0.5f, false, 1f, byte.MaxValue, 0),
			new ElementConverter.OutputElement(0.166666672f, SimHashes.ToxicSand, 366.5f, false, 0f, 0.5f, false, 1f, byte.MaxValue, 0),
			new ElementConverter.OutputElement(0.0833333358f, SimHashes.CarbonDioxide, 366.5f, false, 0f, 0.5f, false, 1f, byte.MaxValue, 0)
		};
		AlgaeDistillery algaeDistillery = go.AddOrGet<AlgaeDistillery>();
		algaeDistillery.emitMass = 10f;
		algaeDistillery.emitTag = new Tag("ToxicSand");
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
