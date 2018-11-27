using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SwampLilyConfig : IEntityConfig
{
	public static string ID = "SwampLily";

	public const string SEED_ID = "SwampLilySeed";

	public GameObject CreatePrefab()
	{
		string id = "SwampLily";
		string name = STRINGS.CREATURES.SPECIES.SWAMPLILY.NAME;
		string desc = STRINGS.CREATURES.SPECIES.SWAMPLILY.DESC;
		float mass = 1f;
		KAnimFile anim = Assets.GetAnim("swamplily_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingBack, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, null, 328.15f);
		GameObject template = gameObject;
		mass = 258.15f;
		float temperature_warning_low = 308.15f;
		float temperature_warning_high = 358.15f;
		float temperature_lethal_high = 448.15f;
		SimHashes[] safe_elements = new SimHashes[1]
		{
			SimHashes.ChlorineGas
		};
		initialAnim = SwampLilyFlowerConfig.ID;
		EntityTemplates.ExtendEntityToBasicPlant(template, mass, temperature_warning_low, temperature_warning_high, temperature_lethal_high, safe_elements, true, 0f, 0.15f, initialAnim, true, true);
		gameObject.AddOrGet<StandardCropPlant>();
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		initialAnim = "SwampLilySeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.SWAMPLILY.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.SWAMPLILY.DESC;
		anim = Assets.GetAnim("seed_swampLily_kanim");
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list.Add(GameTags.BuildingFiber);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, STRINGS.CREATURES.SPECIES.SWAMPLILY.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, ID + "_preview", Assets.GetAnim("swamplily_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("swamplily_kanim", "SwampLily_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("swamplily_kanim", "SwampLily_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("swamplily_kanim", "SwampLily_death", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("swamplily_kanim", "SwampLily_death_bloom", NOISE_POLLUTION.CREATURES.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, ID);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
