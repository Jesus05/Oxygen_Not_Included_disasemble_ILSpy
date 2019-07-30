using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MushroomPlantConfig : IEntityConfig
{
	public const float FERTILIZATION_RATE = 0.006666667f;

	public const string ID = "MushroomPlant";

	public const string SEED_ID = "MushroomSeed";

	public GameObject CreatePrefab()
	{
		string id = "MushroomPlant";
		string name = STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.NAME;
		string desc = STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("fungusplant_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject template = gameObject;
		mass = 228.15f;
		float temperature_warning_low = 278.15f;
		float temperature_warning_high = 308.15f;
		float temperature_lethal_high = 398.15f;
		SimHashes[] safe_elements = new SimHashes[1]
		{
			SimHashes.CarbonDioxide
		};
		initialAnim = MushroomConfig.ID;
		EntityTemplates.ExtendEntityToBasicPlant(template, mass, temperature_warning_low, temperature_warning_high, temperature_lethal_high, safe_elements, true, 0f, 0.15f, initialAnim, true, true, true, true, 2400f);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.SlimeMold,
				massConsumptionRate = 0.006666667f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		IlluminationVulnerable illuminationVulnerable = gameObject.AddOrGet<IlluminationVulnerable>();
		illuminationVulnerable.SetPrefersDarkness(true);
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		initialAnim = "MushroomSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.MUSHROOMPLANT.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.MUSHROOMPLANT.DESC;
		anim = Assets.GetAnim("seed_fungusplant_kanim");
		int numberOfSeeds = 0;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list = list;
		id = STRINGS.CREATURES.SPECIES.MUSHROOMPLANT.DOMESTICATEDDESC;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", numberOfSeeds, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, id, EntityTemplates.CollisionShape.CIRCLE, 0.33f, 0.33f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "MushroomPlant_preview", Assets.GetAnim("fungusplant_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
