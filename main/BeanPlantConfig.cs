using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BeanPlantConfig : IEntityConfig
{
	public const string ID = "BeanPlant";

	public const string SEED_ID = "BeanPlantSeed";

	public const float FERTILIZATION_RATE = 0.00166666671f;

	public const float WATER_RATE = 0.0583333336f;

	public GameObject CreatePrefab()
	{
		string id = "BeanPlant";
		string name = STRINGS.CREATURES.SPECIES.BEAN_PLANT.NAME;
		string desc = STRINGS.CREATURES.SPECIES.BEAN_PLANT.DESC;
		float mass = 2f;
		KAnimFile anim = Assets.GetAnim("beanplant_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, null, 258.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 198.15f, 248.15f, 273.15f, 323.15f, null, true, 0f, 0.15f, "BeanPlantSeed", true, true, true, true, 2400f);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = tag,
				massConsumptionRate = 0.0583333336f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Phosphorite,
				massConsumptionRate = 0.00166666671f
			}
		});
		PressureVulnerable pressureVulnerable = gameObject.AddOrGet<PressureVulnerable>();
		PressureVulnerable pressureVulnerable2 = pressureVulnerable;
		mass = 0.025f;
		float pressureLethalLow = 0f;
		SimHashes[] safeAtmospheres = new SimHashes[1]
		{
			SimHashes.CarbonDioxide
		};
		pressureVulnerable2.Configure(mass, pressureLethalLow, 10f, 30f, safeAtmospheres);
		UprootedMonitor component = gameObject.GetComponent<UprootedMonitor>();
		component.monitorCell = new CellOffset(0, -1);
		gameObject.AddOrGet<StandardCropPlant>();
		GameObject plant = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		initialAnim = "BeanPlantSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.BEAN_PLANT.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.BEAN_PLANT.DESC;
		anim = Assets.GetAnim("seed_beanplant_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(plant, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, STRINGS.CREATURES.SPECIES.BEAN_PLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.3f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "BeanPlant_preview", Assets.GetAnim("beanplant_kanim"), "place", 1, 3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
