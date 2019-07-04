// ColdWheatConfig
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ColdWheatConfig : IEntityConfig
{
	public const string ID = "ColdWheat";

	public const string SEED_ID = "ColdWheatSeed";

	public const float FERTILIZATION_RATE = 0.008333334f;

	public const float WATER_RATE = 0.0333333351f;

	public GameObject CreatePrefab()
	{
		string id = "ColdWheat";
		string name = STRINGS.CREATURES.SPECIES.COLDWHEAT.NAME;
		string desc = STRINGS.CREATURES.SPECIES.COLDWHEAT.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("coldwheat_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 1, tIER, default(EffectorValues), SimHashes.Creature, null, 255f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 118.149994f, 218.15f, 278.15f, 358.15f, new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, true, 0f, 0.15f, "ColdWheatSeed", true, true, true, true);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Dirt,
				massConsumptionRate = 0.008333334f
			}
		});
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Water,
				massConsumptionRate = 0.0333333351f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		GameObject plant = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.DigOnly;
		initialAnim = "ColdWheatSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.COLDWHEAT.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.COLDWHEAT.DESC;
		anim = Assets.GetAnim("seed_coldwheat_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list = list;
		id = STRINGS.CREATURES.SPECIES.COLDWHEAT.DOMESTICATEDDESC;
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(plant, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, id, EntityTemplates.CollisionShape.CIRCLE, 0.2f, 0.2f, null, "", true);
		EntityTemplates.ExtendEntityToFood(gameObject2, FOOD.FOOD_TYPES.COLD_WHEAT_SEED);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject2, "ColdWheat_preview", Assets.GetAnim("coldwheat_kanim"), "place", 1, 1);
		SoundEventVolumeCache.instance.AddVolume("coldwheat_kanim", "ColdWheat_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("coldwheat_kanim", "ColdWheat_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}