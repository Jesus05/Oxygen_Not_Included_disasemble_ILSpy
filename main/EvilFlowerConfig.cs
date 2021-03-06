using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class EvilFlowerConfig : IEntityConfig
{
	public const string ID = "EvilFlower";

	public const string SEED_ID = "EvilFlowerSeed";

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER7;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER5;

	public GameObject CreatePrefab()
	{
		string id = "EvilFlower";
		string name = STRINGS.CREATURES.SPECIES.EVILFLOWER.NAME;
		string desc = STRINGS.CREATURES.SPECIES.EVILFLOWER.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("potted_evilflower_kanim");
		string initialAnim = "grow_seed";
		EffectorValues pOSITIVE_DECOR_EFFECT = POSITIVE_DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 1, pOSITIVE_DECOR_EFFECT, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 168.15f, 258.15f, 513.15f, 563.15f, new SimHashes[1]
		{
			SimHashes.CarbonDioxide
		}, true, 0f, 0.15f, null, true, false, true, true, 2400f);
		EvilFlower evilFlower = gameObject.AddOrGet<EvilFlower>();
		evilFlower.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		evilFlower.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		GameObject plant = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		initialAnim = "EvilFlowerSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.EVILFLOWER.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.EVILFLOWER.DESC;
		anim = Assets.GetAnim("seed_potted_evilflower_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(plant, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 5, STRINGS.CREATURES.SPECIES.EVILFLOWER.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.4f, 0.4f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "EvilFlower_preview", Assets.GetAnim("potted_evilflower_kanim"), "place", 1, 1);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		DiseaseDropper.Def def = gameObject.AddOrGetDef<DiseaseDropper.Def>();
		def.diseaseIdx = Db.Get().Diseases.GetIndex("ZombieSpores");
		def.emitFrequency = 1f;
		def.averageEmitPerSecond = 1000;
		def.singleEmitQuantity = 100000;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
