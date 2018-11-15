using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TUNING;
using UnityEngine;

public static class BaseHatchConfig
{
	[CompilerGenerated]
	private static Func<int, int> _003C_003Ef__mg_0024cache0;

	public static GameObject BaseHatch(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix = null)
	{
		float mass = 100f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string initialAnim = "idle_loop";
		EffectorValues tIER = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.Creatures, 1, 1, tIER, default(EffectorValues), SimHashes.Creature, null, 293f);
		string navGridName = "HatchNavGrid";
		if (is_baby)
		{
			navGridName = "HatchBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, navGridName, NavType.Floor, 32, 2f, "Meat", 2, true, false, 283.15f, 293.15f, 243.15f, 343.15f);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByPrefix(Assets.GetAnim(anim_file), symbolOverridePrefix, 0);
		}
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<BurrowMonitor.Def>();
		WorldSpawnableMonitor.Def def = gameObject.AddOrGetDef<WorldSpawnableMonitor.Def>();
		def.adjustSpawnLocationCb = AdjustSpawnLocationCB;
		ThreatMonitor.Def def2 = gameObject.AddOrGetDef<ThreatMonitor.Def>();
		def2.fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0f);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_idle", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "Hatch_footstep", NOISE_POLLUTION.CREATURES.TIER1);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_land", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_chew", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_hurt", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_die", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_emerge", NOISE_POLLUTION.CREATURES.TIER6);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_hide", NOISE_POLLUTION.CREATURES.TIER6);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.GroundBased);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		bool condition = !is_baby;
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new ExitBurrowStates.Def(), condition)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Burrowed, true, "idle_mound", STRINGS.CREATURES.STATUSITEMS.BURROWED.NAME, STRINGS.CREATURES.STATUSITEMS.BURROWED.TOOLTIP), condition)
			.Add(new GrowUpStates.Def(), true)
			.Add(new TrappedStates.Def(), true)
			.Add(new IncubatingStates.Def(), true)
			.Add(new BaggedStates.Def(), true)
			.Add(new FallStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new DrowningStates.Def(), true)
			.Add(new DebugGoToStates.Def(), true)
			.Add(new FleeStates.Def(), true)
			.Add(new AttackStates.Def(), condition)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def(), true)
			.Add(new FixedCaptureStates.Def(), true)
			.Add(new RanchedStates.Def(), true)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.WantsToEnterBurrow, false, "hide", STRINGS.CREATURES.STATUSITEMS.BURROWING.NAME, STRINGS.CREATURES.STATUSITEMS.BURROWING.TOOLTIP), condition)
			.Add(new LayEggStates.Def(), true)
			.Add(new EatStates.Def(), true)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true)
			.Add(new CallAdultStates.Def(), true)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true);
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.HatchSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static List<Diet.Info> BasicRockDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Sand.CreateTag());
		hashSet.Add(SimHashes.SandStone.CreateTag());
		hashSet.Add(SimHashes.Clay.CreateTag());
		hashSet.Add(SimHashes.CrushedRock.CreateTag());
		hashSet.Add(SimHashes.Dirt.CreateTag());
		hashSet.Add(SimHashes.SedimentaryRock.CreateTag());
		List<Diet.Info> list = new List<Diet.Info>();
		list.Add(new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false));
		return list;
	}

	public static List<Diet.Info> HardRockDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.SedimentaryRock.CreateTag());
		hashSet.Add(SimHashes.IgneousRock.CreateTag());
		hashSet.Add(SimHashes.Obsidian.CreateTag());
		hashSet.Add(SimHashes.Granite.CreateTag());
		List<Diet.Info> list = new List<Diet.Info>();
		list.Add(new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false));
		return list;
	}

	public static List<Diet.Info> MetalDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		List<Diet.Info> list = new List<Diet.Info>();
		list.Add(new Diet.Info(new HashSet<Tag>(new Tag[1]
		{
			SimHashes.Cuprite.CreateTag()
		}), (!(poopTag == GameTags.Metal)) ? poopTag : SimHashes.Copper.CreateTag(), caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false));
		list.Add(new Diet.Info(new HashSet<Tag>(new Tag[1]
		{
			SimHashes.GoldAmalgam.CreateTag()
		}), (!(poopTag == GameTags.Metal)) ? poopTag : SimHashes.Gold.CreateTag(), caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false));
		list.Add(new Diet.Info(new HashSet<Tag>(new Tag[1]
		{
			SimHashes.IronOre.CreateTag()
		}), (!(poopTag == GameTags.Metal)) ? poopTag : SimHashes.Iron.CreateTag(), caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false));
		list.Add(new Diet.Info(new HashSet<Tag>(new Tag[1]
		{
			SimHashes.Wolframite.CreateTag()
		}), (!(poopTag == GameTags.Metal)) ? poopTag : SimHashes.Tungsten.CreateTag(), caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false));
		return list;
	}

	public static List<Diet.Info> VeggieDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Dirt.CreateTag());
		hashSet.Add(SimHashes.SlimeMold.CreateTag());
		hashSet.Add(SimHashes.Algae.CreateTag());
		hashSet.Add(SimHashes.Fertilizer.CreateTag());
		hashSet.Add(SimHashes.ToxicSand.CreateTag());
		List<Diet.Info> list = new List<Diet.Info>();
		list.Add(new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false));
		return list;
	}

	public static List<Diet.Info> FoodDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		List<Diet.Info> list = new List<Diet.Info>();
		foreach (EdiblesManager.FoodInfo item in FOOD.FOOD_TYPES_LIST)
		{
			if (item.CaloriesPerUnit > 0f)
			{
				HashSet<Tag> hashSet = new HashSet<Tag>();
				hashSet.Add(new Tag(item.Id));
				list.Add(new Diet.Info(hashSet, poopTag, item.CaloriesPerUnit, producedConversionRate, diseaseId, diseasePerKgProduced, false));
			}
		}
		return list;
	}

	public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> diet_infos, float referenceCaloriesPerKg, float minPoopSizeInKg)
	{
		Diet diet = new Diet(diet_infos.ToArray());
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = referenceCaloriesPerKg * minPoopSizeInKg;
		SolidConsumerMonitor.Def def2 = prefab.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
		return prefab;
	}

	private static int AdjustSpawnLocationCB(int cell)
	{
		while (!Grid.Solid[cell])
		{
			int num = Grid.CellBelow(cell);
			if (!Grid.IsValidCell(cell))
			{
				break;
			}
			cell = num;
		}
		return cell;
	}
}
