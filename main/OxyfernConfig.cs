using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class OxyfernConfig : IEntityConfig
{
	public const string ID = "Oxyfern";

	public const string SEED_ID = "OxyfernSeed";

	public const float WATER_CONSUMPTION_RATE = 0.0316666663f;

	public const float FERTILIZATION_RATE = 0.006666667f;

	public const float CO2_RATE = 0.000625000044f;

	private const float CONVERSION_RATIO = 50f;

	public const float OXYGEN_RATE = 0.0312500037f;

	public GameObject CreatePrefab()
	{
		string id = "Oxyfern";
		string name = STRINGS.CREATURES.SPECIES.OXYFERN.NAME;
		string desc = STRINGS.CREATURES.SPECIES.OXYFERN.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("oxy_fern_kanim");
		string initialAnim = "idle_full";
		EffectorValues tIER = DECOR.PENALTY.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingBack, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, null, 293f);
		gameObject.AddOrGet<ReceptacleMonitor>();
		gameObject.AddOrGet<EntombVulnerable>();
		gameObject.AddOrGet<WiltCondition>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Uprootable>();
		gameObject.AddOrGet<UprootedMonitor>();
		gameObject.AddOrGet<DrowningMonitor>();
		TemperatureVulnerable temperatureVulnerable = gameObject.AddOrGet<TemperatureVulnerable>();
		temperatureVulnerable.Configure(273.15f, 253.15f, 313.15f, 373.15f);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = tag,
				massConsumptionRate = 0.0316666663f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Dirt,
				massConsumptionRate = 0.006666667f
			}
		});
		gameObject.AddOrGet<Oxyfern>();
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		PressureVulnerable pressureVulnerable = gameObject.AddOrGet<PressureVulnerable>();
		PressureVulnerable pressureVulnerable2 = pressureVulnerable;
		mass = 0.025f;
		float pressureLethalLow = 0f;
		SimHashes[] safeAtmospheres = new SimHashes[1]
		{
			SimHashes.CarbonDioxide
		};
		pressureVulnerable2.Configure(mass, pressureLethalLow, 10f, 30f, safeAtmospheres);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			PressureVulnerable component2 = inst.GetComponent<PressureVulnerable>();
			component2.safe_atmospheres.Add(ElementLoader.FindElementByHash(SimHashes.CarbonDioxide));
		};
		gameObject.AddOrGet<LoopingSounds>();
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.showInUI = false;
		storage.capacityKg = 1f;
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.showInStatusPanel = false;
		elementConsumer.storeOnConsume = true;
		elementConsumer.storage = storage;
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.configuration = ElementConsumer.Configuration.Element;
		elementConsumer.consumptionRadius = 2;
		elementConsumer.EnableConsumption(true);
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		elementConsumer.consumptionRate = 0.000156250011f;
		ElementConverter elementConverter = gameObject.AddOrGet<ElementConverter>();
		elementConverter.OutputMultiplier = 50f;
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(1960575215.ToString().ToTag(), 0.000625000044f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[1]
		{
			new ElementConverter.OutputElement(0.0312500037f, SimHashes.Oxygen, 0f, false, 0f, 1f, true, 0.75f, byte.MaxValue, 0)
		};
		GameObject plant = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		initialAnim = "OxyfernSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.OXYFERN.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.OXYFERN.DESC;
		anim = Assets.GetAnim("seed_oxyfern_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(plant, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, STRINGS.CREATURES.SPECIES.OXYFERN.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "Oxyfern_preview", Assets.GetAnim("oxy_fern_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("oxy_fern_kanim", "MealLice_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("oxy_fern_kanim", "MealLice_LP", NOISE_POLLUTION.CREATURES.TIER4);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<Oxyfern>().SetConsumptionRate();
	}
}
