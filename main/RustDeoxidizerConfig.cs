using TUNING;
using UnityEngine;

public class RustDeoxidizerConfig : IBuildingConfig
{
	public const string ID = "RustDeoxidizer";

	private const float RUST_KG_CONSUMPTION_RATE = 0.75f;

	private const float SALT_KG_CONSUMPTION_RATE = 0.25f;

	private const float RUST_KG_PER_REFILL = 585f;

	private const float SALT_KG_PER_REFILL = 195f;

	private const float TOTAL_CONSUMPTION_RATE = 1f;

	private const float IRON_CONVERSION_RATIO = 0.4f;

	private const float OXYGEN_CONVERSION_RATIO = 0.57f;

	private const float CHLORINE_CONVERSION_RATIO = 0.0300000012f;

	public const float OXYGEN_TEMPERATURE = 348.15f;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "RustDeoxidizer";
		int width = 2;
		int height = 2;
		string anim = "rust_deoxidizer_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 800f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, tIER2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.ViewMode = OverlayModes.Oxygen.ID;
		buildingDef.AudioCategory = "HollowMetal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		RustDeoxidizer rustDeoxidizer = go.AddOrGet<RustDeoxidizer>();
		rustDeoxidizer.maxMass = 1.8f;
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Rust");
		manualDeliveryKG.capacity = 585f;
		manualDeliveryKG.refillMass = 193.05f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.SetStorage(storage);
		manualDeliveryKG2.requestedItemTag = new Tag("Salt");
		manualDeliveryKG2.capacity = 195f;
		manualDeliveryKG2.refillMass = 64.3500061f;
		manualDeliveryKG2.allowPause = true;
		manualDeliveryKG2.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
		{
			new ElementConverter.ConsumedElement(new Tag("Rust"), 0.75f),
			new ElementConverter.ConsumedElement(new Tag("Salt"), 0.25f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[3]
		{
			new ElementConverter.OutputElement(0.57f, SimHashes.Oxygen, 348.15f, false, false, 0f, 1f, 1f, byte.MaxValue, 0),
			new ElementConverter.OutputElement(0.0300000012f, SimHashes.Chlorine, 348.15f, false, false, 0f, 1f, 1f, byte.MaxValue, 0),
			new ElementConverter.OutputElement(0.4f, SimHashes.IronOre, 348.15f, false, true, 0f, 1f, 1f, byte.MaxValue, 0)
		};
		ElementDropper elementDropper = go.AddComponent<ElementDropper>();
		elementDropper.emitMass = 24f;
		elementDropper.emitTag = SimHashes.IronOre.CreateTag();
		elementDropper.emitOffset = new Vector3(0f, 1f, 0f);
		Prioritizable.AddRef(go);
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
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
