using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class CrabConfig : IEntityConfig
{
	public const string ID = "Crab";

	public const string BASE_TRAIT_ID = "CrabBaseTrait";

	public const string EGG_ID = "CrabEgg";

	private const SimHashes EMIT_ELEMENT = SimHashes.Sand;

	private static float KG_ORE_EATEN_PER_CYCLE = 140f;

	private static float CALORIES_PER_KG_OF_ORE = CrabTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 25f;

	public static int EGG_SORT_ORDER = 0;

	public static GameObject CreateCrab(string id, string name, string desc, string anim_file, bool is_baby, string deathDropID)
	{
		bool is_baby2 = is_baby;
		GameObject prefab = BaseCrabConfig.BaseCrab(id, name, desc, anim_file, "CrabBaseTrait", is_baby2, null, deathDropID);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, CrabTuning.PEN_SIZE_PER_CREATURE, 100f);
		Trait trait = Db.Get().CreateTrait("CrabBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, CrabTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - CrabTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		List<Diet.Info> diet_infos = BaseCrabConfig.BasicDiet(SimHashes.Sand.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f);
		return BaseCrabConfig.SetupDiet(prefab, diet_infos, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG);
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreateCrab("Crab", STRINGS.CREATURES.SPECIES.CRAB.NAME, STRINGS.CREATURES.SPECIES.CRAB.DESC, "pincher_kanim", false, "CrabShell");
		prefab = EntityTemplates.ExtendEntityToFertileCreature(prefab, "CrabEgg", STRINGS.CREATURES.SPECIES.CRAB.EGG_NAME, STRINGS.CREATURES.SPECIES.CRAB.DESC, "egg_pincher_kanim", CrabTuning.EGG_MASS, "CrabBaby", 60.0000038f, 20f, CrabTuning.EGG_CHANCES_BASE, EGG_SORT_ORDER, true, false, true, 1f);
		EggProtectionMonitor.Def def = prefab.AddOrGetDef<EggProtectionMonitor.Def>();
		def.allyTags = new Tag[1]
		{
			GameTags.Creatures.CrabFriend
		};
		return prefab;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
