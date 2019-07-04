using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class PrickleGrassConfig : IEntityConfig
{
	public const string ID = "PrickleGrass";

	public const string SEED_ID = "PrickleGrassSeed";

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;

	public GameObject CreatePrefab()
	{
		string id = "PrickleGrass";
		string name = STRINGS.CREATURES.SPECIES.PRICKLEGRASS.NAME;
		string desc = STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("bristlebriar_kanim");
		string initialAnim = "grow_seed";
		EffectorValues pOSITIVE_DECOR_EFFECT = POSITIVE_DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 1, pOSITIVE_DECOR_EFFECT, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject template = gameObject;
		SimHashes[] safe_elements = new SimHashes[3]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		};
		EntityTemplates.ExtendEntityToBasicPlant(template, 218.15f, 283.15f, 303.15f, 398.15f, safe_elements, true, 0f, 0.15f, null, true, false, true, true, 2400f);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		initialAnim = "PrickleGrassSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.DESC;
		anim = Assets.GetAnim("seed_bristlebriar_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 5, STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "PrickleGrass_preview", Assets.GetAnim("bristlebriar_kanim"), "place", 1, 1);
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
