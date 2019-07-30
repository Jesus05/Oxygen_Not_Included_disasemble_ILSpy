using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BulbPlantConfig : IEntityConfig
{
	public const string ID = "BulbPlant";

	public const string SEED_ID = "BulbPlantSeed";

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER1;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;

	public GameObject CreatePrefab()
	{
		string id = "BulbPlant";
		string name = STRINGS.CREATURES.SPECIES.BULBPLANT.NAME;
		string desc = STRINGS.CREATURES.SPECIES.BULBPLANT.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("potted_bulb_kanim");
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
		EntityTemplates.ExtendEntityToBasicPlant(template, 288f, 293.15f, 313.15f, 333.15f, safe_elements, true, 0f, 0.15f, null, true, false, true, true, 2400f);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = NEGATIVE_DECOR_EFFECT;
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		initialAnim = "BulbPlantSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.BULBPLANT.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.BULBPLANT.DESC;
		anim = Assets.GetAnim("seed_potted_bulb_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 6, STRINGS.CREATURES.SPECIES.BULBPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.4f, 0.4f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "BulbPlant_preview", Assets.GetAnim("potted_bulb_kanim"), "place", 1, 1);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		DiseaseDropper.Def def = gameObject.AddOrGetDef<DiseaseDropper.Def>();
		def.diseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.PollenGerms.id);
		def.singleEmitQuantity = 0;
		def.averageEmitPerSecond = 5000;
		def.emitFrequency = 5f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
