using TUNING;
using UnityEngine;

public class AirFilterConfig : IBuildingConfig
{
	public const string ID = "AirFilter";

	public const float DIRTY_AIR_CONSUMPTION_RATE = 0.1f;

	private const float SAND_CONSUMPTION_RATE = 0.13333334f;

	private const float REFILL_RATE = 2400f;

	private const float SAND_STORAGE_AMOUNT = 320.000031f;

	private const float CLAY_PER_LOAD = 10f;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "AirFilter";
		int width = 1;
		int height = 1;
		string anim = "co2filter_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] rAW_MINERALS = MATERIALS.RAW_MINERALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_MINERALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, tIER2, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = SimViewMode.OxygenMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		storage.capacityKg = 200f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.ContaminatedOxygen;
		elementConsumer.consumptionRate = 0.1f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
		elementConsumer.isRequired = false;
		elementConsumer.storeOnConsume = true;
		elementConsumer.showDescriptor = false;
		ElementDropper elementDropper = go.AddComponent<ElementDropper>();
		elementDropper.emitMass = 10f;
		elementDropper.emitTag = new Tag("Clay");
		elementDropper.emitOffset = new Vector3(0f, 0f, 0f);
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
		{
			new ElementConverter.ConsumedElement(new Tag("Filter"), 0.13333334f),
			new ElementConverter.ConsumedElement(new Tag("ContaminatedOxygen"), 0.1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[2]
		{
			new ElementConverter.OutputElement(0.143333346f, SimHashes.Clay, 0f, true, 0f, 0.5f, false, 0.25f, byte.MaxValue, 0),
			new ElementConverter.OutputElement(0.0899999961f, SimHashes.Oxygen, 0f, false, 0f, 0f, false, 0.75f, byte.MaxValue, 0)
		};
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Filter");
		manualDeliveryKG.capacity = 320.000031f;
		manualDeliveryKG.refillMass = 32.0000038f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.OperateFetch.IdHash;
		AirFilter airFilter = go.AddOrGet<AirFilter>();
		airFilter.filterTag = new Tag("Filter");
		go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<ActiveController.Def>();
	}
}
