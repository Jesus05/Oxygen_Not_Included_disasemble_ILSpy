using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class AlgaeHabitatConfig : IBuildingConfig
{
	public const string ID = "AlgaeHabitat";

	private const float ALGAE_RATE = 0.0300000012f;

	private const float WATER_RATE = 0.3f;

	private const float OXYGEN_RATE = 0.0400000028f;

	private const float CO2_RATE = 0.0003333333f;

	private const float ALGAE_CAPACITY = 90f;

	private const float WATER_CAPACITY = 360f;

	private static readonly List<Storage.StoredItemModifier> PollutedWaterStorageModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Seal
	};

	public static List<Tag> pollutedWaterFilter = new List<Tag>
	{
		ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag
	};

	public override BuildingDef CreateBuildingDef()
	{
		string id = "AlgaeHabitat";
		int width = 1;
		int height = 2;
		string anim = "algaefarm_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] fARMABLE = MATERIALS.FARMABLE;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, fARMABLE, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, tIER2, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.OxygenMap;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_bubbles", NOISE_POLLUTION.NOISY.TIER0);
		SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_algae_in", NOISE_POLLUTION.NOISY.TIER0);
		SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_algae_out", NOISE_POLLUTION.NOISY.TIER0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 360f;
		storage2.showInUI = true;
		storage2.SetDefaultStoredItemModifiers(PollutedWaterStorageModifiers);
		storage2.allowItemRemoval = false;
		storage2.storageFilters = pollutedWaterFilter;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Algae");
		manualDeliveryKG.capacity = 90f;
		manualDeliveryKG.refillMass = 18f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.OperateFetch.IdHash;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.SetStorage(storage);
		manualDeliveryKG2.requestedItemTag = new Tag("Water");
		manualDeliveryKG2.capacity = 360f;
		manualDeliveryKG2.refillMass = 72f;
		manualDeliveryKG2.allowPause = true;
		manualDeliveryKG2.choreTypeIDHash = Db.Get().ChoreTypes.OperateFetch.IdHash;
		KAnimFile[] overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_outhouse_kanim")
		};
		AlgaeHabitatEmpty algaeHabitatEmpty = go.AddOrGet<AlgaeHabitatEmpty>();
		algaeHabitatEmpty.workTime = 5f;
		algaeHabitatEmpty.overrideAnims = overrideAnims;
		algaeHabitatEmpty.workLayer = Grid.SceneLayer.BuildingFront;
		AlgaeHabitat algaeHabitat = go.AddOrGet<AlgaeHabitat>();
		algaeHabitat.lightBonusMultiplier = 1.1f;
		algaeHabitat.pressureSampleOffset = new CellOffset(0, 1);
		ElementConverter elementConverter = go.AddComponent<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
		{
			new ElementConverter.ConsumedElement(new Tag("Algae"), 0.0300000012f),
			new ElementConverter.ConsumedElement(new Tag("Water"), 0.3f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.0400000028f, SimHashes.Oxygen, 303.15f, false, 0f, 1f, false, 1f, byte.MaxValue, 0)
		};
		ElementConverter elementConverter2 = go.AddComponent<ElementConverter>();
		elementConverter2.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.290333331f, SimHashes.DirtyWater, 303.15f, true, 0f, 1f, false, 1f, byte.MaxValue, 0)
		};
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.consumptionRate = 0.0003333333f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 1f, 0f);
		elementConsumer.isRequired = false;
		PassiveElementConsumer passiveElementConsumer = go.AddComponent<PassiveElementConsumer>();
		passiveElementConsumer.elementToConsume = SimHashes.Water;
		passiveElementConsumer.consumptionRate = 1.2f;
		passiveElementConsumer.consumptionRadius = 1;
		passiveElementConsumer.showDescriptor = false;
		passiveElementConsumer.storeOnConsume = true;
		passiveElementConsumer.capacityKG = 360f;
		passiveElementConsumer.showInStatusPanel = false;
		go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		go.AddOrGet<AnimTileable>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
