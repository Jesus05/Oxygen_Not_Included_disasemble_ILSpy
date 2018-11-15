using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BasicSingleHarvestPlantConfig : IEntityConfig
{
	public const string ID = "BasicSingleHarvestPlant";

	public const string SEED_ID = "BasicSingleHarvestPlantSeed";

	public const float DIRT_RATE = 0.0166666675f;

	public GameObject CreatePrefab()
	{
		string id = "BasicSingleHarvestPlant";
		string name = STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.NAME;
		string desc = STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("meallice_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.PENALTY.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingBack, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject template = gameObject;
		SimHashes[] safe_elements = new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		};
		EntityTemplates.ExtendEntityToBasicPlant(template, 218.15f, 283.15f, 303.15f, 398.15f, safe_elements, true, 0f, 0.15f, "BasicPlantFood", true, false);
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<KAnimControllerBase>().randomiseLoopedOffset = true;
		gameObject.AddOrGet<LoopingSounds>();
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		initialAnim = "BasicSingleHarvestPlantSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.BASICSINGLEHARVESTPLANT.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.BASICSINGLEHARVESTPLANT.DESC;
		anim = Assets.GetAnim("seed_meallice_kanim");
		int numberOfSeeds = 0;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list = list;
		id = STRINGS.CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.DOMESTICATEDDESC;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", numberOfSeeds, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, id, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, string.Empty);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Dirt,
				massConsumptionRate = 0.0166666675f
			}
		});
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "BasicSingleHarvestPlant_preview", Assets.GetAnim("meallice_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("meallice_kanim", "MealLice_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("meallice_kanim", "MealLice_LP", NOISE_POLLUTION.CREATURES.TIER4);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
