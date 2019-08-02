using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LeafyPlantConfig : IEntityConfig
{
	public const string ID = "LeafyPlant";

	public const string SEED_ID = "LeafyPlantSeed";

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;

	public GameObject CreatePrefab()
	{
		string id = "LeafyPlant";
		string name = STRINGS.CREATURES.SPECIES.LEAFYPLANT.NAME;
		string desc = STRINGS.CREATURES.SPECIES.LEAFYPLANT.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("potted_leafy_kanim");
		string initialAnim = "grow_seed";
		EffectorValues pOSITIVE_DECOR_EFFECT = POSITIVE_DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 1, pOSITIVE_DECOR_EFFECT, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject template = gameObject;
		SimHashes[] safe_elements = new SimHashes[5]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide,
			SimHashes.ChlorineGas,
			SimHashes.Hydrogen
		};
		EntityTemplates.ExtendEntityToBasicPlant(template, 288f, 293.15f, 323.15f, 373f, safe_elements, true, 0f, 0.15f, null, true, false, true, true, 2400f);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		initialAnim = "LeafyPlantSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.LEAFYPLANT.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.LEAFYPLANT.DESC;
		anim = Assets.GetAnim("seed_potted_leafy_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 7, STRINGS.CREATURES.SPECIES.LEAFYPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "LeafyPlant_preview", Assets.GetAnim("potted_leafy_kanim"), "place", 1, 1);
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
