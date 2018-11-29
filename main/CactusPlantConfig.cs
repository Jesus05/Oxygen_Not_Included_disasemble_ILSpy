using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class CactusPlantConfig : IEntityConfig
{
	public const string ID = "CactusPlant";

	public const string SEED_ID = "CactusPlantSeed";

	public GameObject CreatePrefab()
	{
		string id = "CactusPlant";
		string name = CREATURES.SPECIES.CACTUSPLANT.NAME;
		string desc = CREATURES.SPECIES.CACTUSPLANT.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("potted_cactus_kanim");
		string initialAnim = "grow_seed";
		EffectorValues effectorValues = default(EffectorValues);
		effectorValues.amount = 1;
		effectorValues.radius = 5;
		EffectorValues decor = effectorValues;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 1, decor, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject template = gameObject;
		SimHashes[] safe_elements = new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		};
		EntityTemplates.ExtendEntityToBasicPlant(template, 200f, 273.15f, 373.15f, 400f, safe_elements, false, 0f, 0.15f, null, true, false);
		gameObject.AddOrGet<PrickleGrass>();
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		initialAnim = "CactusPlantSeed";
		desc = CREATURES.SPECIES.SEEDS.CACTUSPLANT.NAME;
		name = CREATURES.SPECIES.SEEDS.CACTUSPLANT.DESC;
		anim = Assets.GetAnim("seed_potted_cactus_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 5, CREATURES.SPECIES.CACTUSPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "CactusPlant_preview", Assets.GetAnim("potted_cactus_kanim"), "place", 1, 1);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
