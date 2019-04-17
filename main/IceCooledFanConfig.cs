using TUNING;
using UnityEngine;

public class IceCooledFanConfig : IBuildingConfig
{
	public const string ID = "IceCooledFan";

	private float COOLING_RATE = 32f;

	private float TARGET_TEMPERATURE = 278.15f;

	private float ICE_CAPACITY = 50f;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "IceCooledFan";
		int width = 2;
		int height = 2;
		string anim = "fanice_kanim";
		int hitpoints = 30;
		float construction_time = 10f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, aLL_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, tIER2, 0.2f);
		buildingDef.ExhaustKilowattsWhenActive = 0f - COOLING_RATE;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Temperature.ID;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddComponent<Storage>();
		storage.capacityKg = 50f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 50f;
		storage2.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		MinimumOperatingTemperature minimumOperatingTemperature = go.AddOrGet<MinimumOperatingTemperature>();
		minimumOperatingTemperature.minimumTemperature = 273.15f;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		IceCooledFan iceCooledFan = go.AddOrGet<IceCooledFan>();
		iceCooledFan.coolingRate = COOLING_RATE;
		iceCooledFan.targetTemperature = TARGET_TEMPERATURE;
		iceCooledFan.iceStorage = storage;
		iceCooledFan.liquidStorage = storage2;
		iceCooledFan.minCooledTemperature = 278.15f;
		iceCooledFan.minEnvironmentMass = 0.25f;
		iceCooledFan.minCoolingRange = new Vector2I(-2, 0);
		iceCooledFan.maxCoolingRange = new Vector2I(2, 4);
		iceCooledFan.consumptionTag = GameTags.IceOre;
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = GameTags.IceOre;
		manualDeliveryKG.capacity = ICE_CAPACITY;
		manualDeliveryKG.refillMass = ICE_CAPACITY * 0.2f;
		manualDeliveryKG.minimumMass = 10f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		IceCooledFanWorkable iceCooledFanWorkable = go.AddOrGet<IceCooledFanWorkable>();
		iceCooledFanWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_icefan_kanim")
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
