using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BasicFabricMaterialPlantConfig : IEntityConfig
{
	public static string ID = "BasicFabricPlant";

	public static string SEED_ID = "BasicFabricMaterialPlantSeed";

	public const float WATER_RATE = 0.266666681f;

	public GameObject CreatePrefab()
	{
		string iD = ID;
		string name = STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.NAME;
		string desc = STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("swampreed_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(iD, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingBack, 1, 3, tIER, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject template = gameObject;
		initialAnim = BasicFabricConfig.ID;
		SimHashes[] safe_elements = new SimHashes[5]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide,
			SimHashes.DirtyWater,
			SimHashes.Water
		};
		EntityTemplates.ExtendEntityToBasicPlant(template, 248.15f, 295.15f, 310.15f, 398.15f, safe_elements, false, 0f, 0.15f, initialAnim, false, true, true, true, 2400f);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.DirtyWater,
				massConsumptionRate = 0.266666681f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<KAnimControllerBase>().randomiseLoopedOffset = true;
		gameObject.AddOrGet<LoopingSounds>();
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		initialAnim = SEED_ID;
		desc = STRINGS.CREATURES.SPECIES.SEEDS.BASICFABRICMATERIALPLANT.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.BASICFABRICMATERIALPLANT.DESC;
		anim = Assets.GetAnim("seed_swampreed_kanim");
		int numberOfSeeds = 0;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.WaterSeed);
		list = list;
		iD = STRINGS.CREATURES.SPECIES.BASICFABRICMATERIALPLANT.DOMESTICATEDDESC;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", numberOfSeeds, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, iD, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, ID + "_preview", Assets.GetAnim("swampreed_kanim"), "place", 1, 3);
		SoundEventVolumeCache.instance.AddVolume("swampreed_kanim", "FabricPlant_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("swampreed_kanim", "FabricPlant_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
