using Klei.AI;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class BaseLightBugConfig
{
	public static GameObject BaseLightBug(string id, string name, string desc, string anim_file, string traitId, Color lightColor, EffectorValues decor, bool is_baby, string symbolOverridePrefix = null)
	{
		float mass = 5f;
		KAnimFile anim = Assets.GetAnim(anim_file);
		string initialAnim = "idle_loop";
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.Creatures, 1, 1, decor, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject template = gameObject;
		FactionManager.FactionID faction = FactionManager.FactionID.Prey;
		string navGridName = "FlyerNavGrid1x1";
		NavType navType = NavType.Hover;
		mass = 2f;
		string onDeathDropID = "Meat";
		int onDeathDropCount = 0;
		float fREEZING_ = CREATURES.TEMPERATURE.FREEZING_1;
		float hOT_ = CREATURES.TEMPERATURE.HOT_1;
		EntityTemplates.ExtendEntityToBasicCreature(template, faction, traitId, navGridName, navType, 32, mass, onDeathDropID, onDeathDropCount, true, true, fREEZING_, hOT_, CREATURES.TEMPERATURE.FREEZING_2, CREATURES.TEMPERATURE.HOT_2);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByPrefix(Assets.GetAnim(anim_file), symbolOverridePrefix, 0);
		}
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Flyer);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		gameObject.AddOrGet<LoopingSounds>();
		LureableMonitor.Def def = gameObject.AddOrGetDef<LureableMonitor.Def>();
		def.lures = new Tag[1]
		{
			GameTags.Phosphorite
		};
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<SubmergedMonitor.Def>();
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, false, false);
		if (is_baby)
		{
			KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
			component2.animWidth = 0.5f;
			component2.animHeight = 0.5f;
		}
		if (lightColor != Color.black)
		{
			Light2D light2D = gameObject.AddOrGet<Light2D>();
			light2D.Color = lightColor;
			light2D.overlayColour = LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
			light2D.Range = 5f;
			light2D.Angle = 0f;
			light2D.Direction = LIGHT2D.LIGHTBUG_DIRECTION;
			light2D.Offset = LIGHT2D.LIGHTBUG_OFFSET;
			light2D.shape = LightShape.Circle;
			light2D.drawOverlay = true;
			light2D.Lux = 1800;
			gameObject.AddOrGet<LightSymbolTracker>().targetSymbol = "snapTo_light_locator";
			gameObject.AddOrGetDef<CreatureLightToggleController.Def>();
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
			.Add(new LayEggStates.Def(), true)
			.Add(new EatStates.Def(), true)
			.Add(new MoveToLureStates.Def(), true)
			.Add(new CallAdultStates.Def(), true)
			.PopInterruptGroup()
			.Add(new IdleStates.Def(), true);
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.LightBugSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static GameObject SetupDiet(GameObject prefab, HashSet<Tag> consumed_tags, Tag producedTag, float caloriesPerKg)
	{
		Diet.Info[] infos = new Diet.Info[1]
		{
			new Diet.Info(consumed_tags, producedTag, caloriesPerKg, 1f, null, 0f, false)
		};
		Diet diet = new Diet(infos);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		SolidConsumerMonitor.Def def2 = prefab.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
		return prefab;
	}

	public static void SetupLoopingSounds(GameObject inst)
	{
		LoopingSounds component = inst.GetComponent<LoopingSounds>();
		component.StartSound(GlobalAssets.GetSound("ShineBug_wings_LP", false));
	}
}
