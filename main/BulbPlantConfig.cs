using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class BulbPlantConfig : IEntityConfig
{
	public const string ID = "BulbPlant";

	public const string SEED_ID = "BulbPlantSeed";

	public GameObject CreatePrefab()
	{
		string id = "BulbPlant";
		string name = CREATURES.SPECIES.BULBPLANT.NAME;
		string desc = CREATURES.SPECIES.BULBPLANT.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("potted_bulb_kanim");
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
		EntityTemplates.ExtendEntityToBasicPlant(template, 218.15f, 283.15f, 303.15f, 398.15f, safe_elements, true, 0f, 0.15f, null, true, false);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		initialAnim = "BulbPlantSeed";
		desc = CREATURES.SPECIES.SEEDS.BULBPLANT.NAME;
		name = CREATURES.SPECIES.SEEDS.BULBPLANT.DESC;
		anim = Assets.GetAnim("seed_potted_bulb_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 5, CREATURES.SPECIES.BULBPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "BulbPlant_preview", Assets.GetAnim("potted_bulb_kanim"), "place", 1, 1);
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
