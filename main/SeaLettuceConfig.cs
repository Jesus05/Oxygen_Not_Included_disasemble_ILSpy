using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SeaLettuceConfig : IEntityConfig
{
	public static string ID = "SeaLettuce";

	public const float WATER_RATE = 0.008333334f;

	public const float FERTILIZATION_RATE = 0.000833333354f;

	public GameObject CreatePrefab()
	{
		string iD = ID;
		string name = STRINGS.CREATURES.SPECIES.SEALETTUCE.NAME;
		string desc = STRINGS.CREATURES.SPECIES.SEALETTUCE.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("sea_lettuce_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(iD, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingBack, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, null, 308.15f);
		GameObject template = gameObject;
		mass = 248.15f;
		float temperature_warning_low = 295.15f;
		float temperature_warning_high = 338.15f;
		float temperature_lethal_high = 398.15f;
		bool pressure_sensitive = false;
		SimHashes[] safe_elements = new SimHashes[3]
		{
			SimHashes.Water,
			SimHashes.SaltWater,
			SimHashes.Brine
		};
		EntityTemplates.ExtendEntityToBasicPlant(template, mass, temperature_warning_low, temperature_warning_high, temperature_lethal_high, safe_elements, pressure_sensitive, 0f, 0.15f, "Lettuce", true, true, true, true, 2400f);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.SaltWater.CreateTag(),
				massConsumptionRate = 0.008333334f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.BleachStone.CreateTag(),
				massConsumptionRate = 0.000833333354f
			}
		});
		gameObject.GetComponent<DrowningMonitor>().canDrownToDeath = false;
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<KAnimControllerBase>().randomiseLoopedOffset = true;
		gameObject.AddOrGet<LoopingSounds>();
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		initialAnim = ID + "Seed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.SEALETTUCE.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.SEALETTUCE.DESC;
		anim = Assets.GetAnim("seed_sealettuce_kanim");
		int numberOfSeeds = 0;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.WaterSeed);
		list = list;
		iD = STRINGS.CREATURES.SPECIES.SEALETTUCE.DOMESTICATEDDESC;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", numberOfSeeds, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, iD, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, ID + "_preview", Assets.GetAnim("sea_lettuce_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("sea_lettuce_kanim", "SeaLettuce_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("sea_lettuce_kanim", "SeaLettuce_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
