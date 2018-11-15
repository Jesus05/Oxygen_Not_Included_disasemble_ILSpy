using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TUNING;
using UnityEngine;

public static class BasePuftConfig
{
	[CompilerGenerated]
	private static IdleStates.Def.IdleAnimCallback _003C_003Ef__mg_0024cache0;

	public static GameObject BasePuft(string id, string name, string desc, string traitId, string anim_file, bool is_baby, string symbol_override_prefix)
	{
		float mass = 50f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string initialAnim = "idle_loop";
		EffectorValues tIER = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.Creatures, 1, 1, tIER, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, traitId, "FlyerNavGrid1x1", NavType.Hover, 32, 2f, "Meat", 1, true, true, 302f, 318f, 243.15f, 343.15f);
		if (!string.IsNullOrEmpty(symbol_override_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByPrefix(Assets.GetAnim(anim_file), symbol_override_prefix, 0);
		}
		gameObject.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Flyer);
		gameObject.AddOrGet<LoopingSounds>();
		LureableMonitor.Def def = gameObject.AddOrGetDef<LureableMonitor.Def>();
		def.lures = new Tag[1]
		{
			GameTags.SlimeMold
		};
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<SubmergedMonitor.Def>();
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_idle", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_air_intake", NOISE_POLLUTION.CREATURES.TIER4);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_toot", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_air_inflated", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_die", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("puft_kanim", "Puft_voice_hurt", NOISE_POLLUTION.CREATURES.TIER5);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, false);
		string inhaleSound = "Puft_air_intake";
		if (is_baby)
		{
			inhaleSound = "PuftBaby_air_intake";
		}
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new GrowUpStates.Def(), true)
			.Add(new IncubatingStates.Def(), true)
			.Add(new BaggedStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new DebugGoToStates.Def(), true)
			.Add(new SubmergedStates.Def(), true)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true)
			.Add(new FixedCaptureStates.Def(), true)
			.Add(new RanchedStates.Def(), true)
			.Add(new UpTopPoopStates.Def(), true)
			.Add(new LayEggStates.Def(), true)
			.Add(new InhaleStates.Def
			{
				inhaleSound = inhaleSound
			}, true)
			.Add(new MoveToLureStates.Def(), true)
			.Add(new CallAdultStates.Def(), true)
			.PopInterruptGroup()
			.Add(new IdleStates.Def
			{
				customIdleAnim = new IdleStates.Def.IdleAnimCallback(BasePuftConfig.CustomIdleAnim)
			}, true);
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.PuftSpecies, symbol_override_prefix);
		return gameObject;
	}

	public static GameObject SetupDiet(GameObject prefab, Tag consumed_tag, Tag producedTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced, float minPoopSizeInKg)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(consumed_tag);
		Diet.Info[] infos = new Diet.Info[1]
		{
			new Diet.Info(hashSet, producedTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false)
		};
		Diet diet = new Diet(infos);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = minPoopSizeInKg * caloriesPerKg;
		GasAndLiquidConsumerMonitor.Def def2 = prefab.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
		def2.diet = diet;
		return prefab;
	}

	private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
	{
		CreatureCalorieMonitor.Instance sMI = smi.GetSMI<CreatureCalorieMonitor.Instance>();
		return (sMI == null || !sMI.stomach.IsReadyToPoop()) ? "idle_loop" : "idle_loop_full";
	}

	public static void OnSpawn(GameObject inst)
	{
		Navigator component = inst.GetComponent<Navigator>();
		component.transitionDriver.overrideLayers.Add(new FullPuftTransitionLayer(component));
	}
}
