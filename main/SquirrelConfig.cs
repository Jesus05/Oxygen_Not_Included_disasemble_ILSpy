using Klei.AI;
using STRINGS;
using UnityEngine;

public class SquirrelConfig : IEntityConfig
{
	public const string ID = "Squirrel";

	public const string BASE_TRAIT_ID = "SquirrelBaseTrait";

	public const string EGG_ID = "SquirrelEgg";

	public const float OXYGEN_RATE = 0.0234375037f;

	public const float BABY_OXYGEN_RATE = 0.0117187519f;

	private const SimHashes EMIT_ELEMENT = SimHashes.Dirt;

	private static float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 0.4f;

	private static float CALORIES_PER_DAY_OF_PLANT_EATEN = SquirrelTuning.STANDARD_CALORIES_PER_CYCLE / DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;

	private static float KG_POOP_PER_DAY_OF_PLANT = 50f;

	private static float MIN_POOP_SIZE_KG = 40f;

	private static float MIN_POOP_SIZE_CALORIES = CALORIES_PER_DAY_OF_PLANT_EATEN * MIN_POOP_SIZE_KG / KG_POOP_PER_DAY_OF_PLANT;

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
		Diet.Info[] diet_infos = BaseSquirrelConfig.BasicWoodDiet(SimHashes.Dirt.CreateTag(), CALORIES_PER_DAY_OF_PLANT_EATEN, KG_POOP_PER_DAY_OF_PLANT, null, 0f);
		return BaseSquirrelConfig.SetupDiet(prefab, diet_infos, MIN_POOP_SIZE_KG);
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreateSquirrel("Squirrel", CREATURES.SPECIES.SQUIRREL.NAME, CREATURES.SPECIES.SQUIRREL.DESC, "squirrel_kanim", false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "SquirrelEgg", CREATURES.SPECIES.SQUIRREL.EGG_NAME, CREATURES.SPECIES.SQUIRREL.DESC, "egg_squirrel_kanim", SquirrelTuning.EGG_MASS, "SquirrelBaby", 60.0000038f, 20f, SquirrelTuning.EGG_CHANCES_BASE, EGG_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
