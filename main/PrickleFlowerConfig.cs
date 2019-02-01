using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class PrickleFlowerConfig : IEntityConfig
{
	public const float WATER_RATE = 0.0333333351f;

	public const string ID = "PrickleFlower";

	public const string SEED_ID = "PrickleFlowerSeed";

	public GameObject CreatePrefab()
	{
		string id = "PrickleFlower";
		string name = STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.NAME;
		string desc = STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("bristleblossom_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject template = gameObject;
		mass = 218.15f;
		float temperature_warning_low = 278.15f;
		float temperature_warning_high = 303.15f;
		float temperature_lethal_high = 398.15f;
		SimHashes[] safe_elements = new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		};
		initialAnim = PrickleFruitConfig.ID;
		EntityTemplates.ExtendEntityToBasicPlant(template, mass, temperature_warning_low, temperature_warning_high, temperature_lethal_high, safe_elements, true, 0f, 0.15f, initialAnim, true, true);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Water,
				massConsumptionRate = 0.0333333351f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		IlluminationVulnerable illuminationVulnerable = gameObject.AddOrGet<IlluminationVulnerable>();
		illuminationVulnerable.SetPrefersDarkness(false);
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		initialAnim = "PrickleFlowerSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.DESC;
		anim = Assets.GetAnim("seed_bristleblossom_kanim");
		int numberOfSeeds = 0;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list = list;
		id = STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DOMESTICATEDDESC;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", numberOfSeeds, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, id, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "PrickleFlower_preview", Assets.GetAnim("bristleblossom_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_grow", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<PrimaryElement>().Temperature = 288.15f;
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
