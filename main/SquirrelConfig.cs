using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SquirrelConfig : IEntityConfig
{
	public const string ID = "Squirrel";

	public const string BASE_TRAIT_ID = "SquirrelBaseTrait";

	public const string EGG_ID = "SquirrelEgg";

	public const float OXYGEN_RATE = 0.0234375037f;

	public const float BABY_OXYGEN_RATE = 0.0117187519f;

	private const SimHashes EMIT_ELEMENT = SimHashes.Dirt;

	private static float KG_ORE_EATEN_PER_CYCLE = 0.33f;

	private static float CALORIES_PER_KG_OF_ORE = SquirrelTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 25f;

	public static int EGG_SORT_ORDER = 0;

	public static GameObject CreateSquirrel(string id, string name, string desc, string anim_file, bool is_baby)
	{
		bool is_baby2 = is_baby;
		GameObject prefab = BaseSquirrelConfig.BaseSquirrel(id, name, desc, anim_file, "SquirrelBaseTrait", is_baby2, null);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, SquirrelTuning.PEN_SIZE_PER_CREATURE, 100f);
		Trait trait = Db.Get().CreateTrait("SquirrelBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, SquirrelTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - SquirrelTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		List<Diet.Info> diet_infos = BaseSquirrelConfig.BasicWoodDiet(SimHashes.Dirt.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0f);
		return BaseSquirrelConfig.SetupDiet(prefab, diet_infos, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG);
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreateSquirrel("Squirrel", STRINGS.CREATURES.SPECIES.SQUIRREL.NAME, STRINGS.CREATURES.SPECIES.SQUIRREL.DESC, "squirrel_kanim", false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "SquirrelEgg", STRINGS.CREATURES.SPECIES.SQUIRREL.EGG_NAME, STRINGS.CREATURES.SPECIES.SQUIRREL.DESC, "egg_squirrel_kanim", SquirrelTuning.EGG_MASS, "SquirrelBaby", 60.0000038f, 20f, SquirrelTuning.EGG_CHANCES_BASE, EGG_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
