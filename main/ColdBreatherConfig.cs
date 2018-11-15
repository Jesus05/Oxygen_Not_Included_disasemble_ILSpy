using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ColdBreatherConfig : IEntityConfig
{
	public const string ID = "ColdBreather";

	public static readonly Tag TAG = TagManager.Create("ColdBreather");

	public const float FERTILIZATION_RATE = 0.0333333351f;

	public const SimHashes FERTILIZER = SimHashes.Phosphorite;

	public const float TEMP_DELTA = -5f;

	public const float CONSUMPTION_RATE = 1f;

	public const string SEED_ID = "ColdBreatherSeed";

	public static readonly Tag SEED_TAG = TagManager.Create("ColdBreatherSeed");

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ColdBreather", STRINGS.CREATURES.SPECIES.COLDBREATHER.NAME, STRINGS.CREATURES.SPECIES.COLDBREATHER.DESC, 400f, Assets.GetAnim("coldbreather_kanim"), "grow_seed", Grid.SceneLayer.BuildingFront, 1, 2, DECOR.BONUS.TIER1, NOISE_POLLUTION.NOISY.TIER2, SimHashes.Creature, null, 293f);
		gameObject.AddOrGet<ReceptacleMonitor>();
		gameObject.AddOrGet<EntombVulnerable>();
		gameObject.AddOrGet<WiltCondition>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Uprootable>();
		gameObject.AddOrGet<UprootedMonitor>();
		gameObject.AddOrGet<DrowningMonitor>();
		TemperatureVulnerable temperatureVulnerable = gameObject.AddOrGet<TemperatureVulnerable>();
		temperatureVulnerable.Configure(213.15f, 183.15f, 368.15f, 463.15f);
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		ColdBreather coldBreather = gameObject.AddOrGet<ColdBreather>();
		coldBreather.deltaEmitTemperature = -5f;
		coldBreather.emitOffsetCell = new Vector3(0f, 1f);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		Storage storage = BuildingTemplates.CreateDefaultStorage(gameObject, false);
		storage.showInUI = false;
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.storeOnConsume = true;
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.capacityKG = 2f;
		elementConsumer.consumptionRate = 1f;
		elementConsumer.consumptionRadius = 1;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		SimTemperatureTransfer component = gameObject.GetComponent<SimTemperatureTransfer>();
		component.SurfaceArea = 10f;
		component.Thickness = 0.001f;
		GameObject plant = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		string id = "ColdBreatherSeed";
		string name = STRINGS.CREATURES.SPECIES.SEEDS.COLDBREATHER.NAME;
		string desc = STRINGS.CREATURES.SPECIES.SEEDS.COLDBREATHER.DESC;
		KAnimFile anim = Assets.GetAnim("seed_coldbreather_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(plant, productionType, id, name, desc, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, STRINGS.CREATURES.SPECIES.COLDBREATHER.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, string.Empty);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "ColdBreather_preview", Assets.GetAnim("coldbreather_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("coldbreather_kanim", "ColdBreather_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("coldbreather_kanim", "ColdBreather_intake", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
