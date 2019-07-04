using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GasGrassConfig : IEntityConfig
{
	public const string ID = "GasGrass";

	public const string SEED_ID = "GasGrassSeed";

	public const float FERTILIZATION_RATE = 0.000833333354f;

	public GameObject CreatePrefab()
	{
		string id = "GasGrass";
		string name = STRINGS.CREATURES.SPECIES.GASGRASS.NAME;
		string desc = STRINGS.CREATURES.SPECIES.GASGRASS.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("gassygrass_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER3;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 3, tIER, default(EffectorValues), SimHashes.Creature, null, 255f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 0f, 348.15f, 373.15f, null, true, 0f, 0.15f, "GasGrassHarvested", true, true, true, true, 2400f);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Chlorine,
				massConsumptionRate = 0.000833333354f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		HarvestDesignatable harvestDesignatable = gameObject.AddOrGet<HarvestDesignatable>();
		harvestDesignatable.defaultHarvestStateWhenPlanted = false;
		CropSleepingMonitor.Def def = gameObject.AddOrGetDef<CropSleepingMonitor.Def>();
		def.lightIntensityThreshold = 20000f;
		def.prefersDarkness = false;
		GameObject plant = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		initialAnim = "GasGrassSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.GASGRASS.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.GASGRASS.DESC;
		anim = Assets.GetAnim("seed_gassygrass_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(plant, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, STRINGS.CREATURES.SPECIES.GASGRASS.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.2f, 0.2f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "GasGrass_preview", Assets.GetAnim("gassygrass_kanim"), "place", 1, 1);
		SoundEventVolumeCache.instance.AddVolume("gassygrass_kanim", "GasGrass_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("gassygrass_kanim", "GasGrass_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
