using STRINGS;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TUNING;
using UnityEngine;

public static class BaseMoleConfig
{
	private static readonly string[] SolidIdleAnims = new string[4]
	{
		"idle1",
		"idle2",
		"idle3",
		"idle4"
	};

	[CompilerGenerated]
	private static IdleStates.Def.IdleAnimCallback _003C_003Ef__mg_0024cache0;

	public static GameObject BaseMole(string id, string name, string desc, string traitId, string anim_file, bool is_baby)
	{
		float mass = 25f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string initialAnim = "idle_loop";
		EffectorValues nONE = TUNING.BUILDINGS.DECOR.NONE;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.Creatures, 1, 1, nONE, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, "DiggerNavGrid", NavType.Floor, 32, 2f, "Meat", 10, true, false, 123.149994f, 673.15f, 73.1499939f, 773.15f);
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<DiggerMonitor.Def>();
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true);
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def(), true).Add(new AnimInterruptStates.Def(), true).Add(new FallStates.Def(), true)
			.Add(new StunnedStates.Def(), true)
			.Add(new DiggerStates.Def(MoleTuning.DEPTH_TO_HIDE), true)
			.Add(new GrowUpStates.Def(), true)
			.Add(new TrappedStates.Def(), true)
			.Add(new IncubatingStates.Def(), true)
			.Add(new BaggedStates.Def(), true)
			.Add(new DebugGoToStates.Def(), true)
			.Add(new FleeStates.Def(), true)
			.Add(new AttackStates.Def(), !is_baby)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def(), true)
			.Add(new RanchedStates.Def(), true)
			.Add(new LayEggStates.Def(), true)
			.Add(new CreatureSleepStates.Def(), true)
			.Add(new EatStates.Def(), true)
			.Add(new NestingPoopState.Def((!is_baby) ? SimHashes.Regolith.CreateTag() : Tag.Invalid), true)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true)
			.PopInterruptGroup()
			.Add(new IdleStates.Def
			{
				customIdleAnim = new IdleStates.Def.IdleAnimCallback(BaseMoleConfig.CustomIdleAnim)
			}, true);
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.MoleSpecies, null);
		return gameObject;
	}

	public static List<Diet.Info> SimpleOreDiet(List<Tag> elementTags, float caloriesPerKg, float producedConversionRate)
	{
		List<Diet.Info> list = new List<Diet.Info>();
		foreach (Tag elementTag in elementTags)
		{
			list.Add(new Diet.Info(new HashSet<Tag>
			{
				elementTag
			}, elementTag, caloriesPerKg, producedConversionRate, null, 0f, true));
		}
		return list;
	}

	private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
	{
		int cell = Grid.PosToCell(smi.master.gameObject);
		if (Grid.IsSolidCell(cell))
		{
			int num = Random.Range(0, SolidIdleAnims.Length);
			return SolidIdleAnims[num];
		}
		return "idle_loop";
	}
}
