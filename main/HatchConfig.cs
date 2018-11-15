using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class HatchConfig : IEntityConfig
{
	public const string ID = "Hatch";

	public const string BASE_TRAIT_ID = "HatchBaseTrait";

	public const string EGG_ID = "HatchEgg";

	private const SimHashes EMIT_ELEMENT = SimHashes.Carbon;

	private static float KG_ORE_EATEN_PER_CYCLE = 140f;

	private static float CALORIES_PER_KG_OF_ORE = HatchTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 25f;

	public static int EGG_SORT_ORDER = 0;

	public static GameObject CreateHatch(string id, string name, string desc, string anim_file, bool is_baby)
	{
		bool is_baby2 = is_baby;
		GameObject prefab = BaseHatchConfig.BaseHatch(id, name, desc, anim_file, "HatchBaseTrait", is_baby2, null);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, HatchTuning.PEN_SIZE_PER_CREATURE, 100f);
		Trait trait = Db.Get().CreateTrait("HatchBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, HatchTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - HatchTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		List<Diet.Info> list = BaseHatchConfig.BasicRockDiet(SimHashes.Carbon.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f);
		list.AddRange(BaseHatchConfig.FoodDiet(SimHashes.Carbon.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0f));
		return BaseHatchConfig.SetupDiet(prefab, list, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG);
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreateHatch("Hatch", STRINGS.CREATURES.SPECIES.HATCH.NAME, STRINGS.CREATURES.SPECIES.HATCH.DESC, "hatch_kanim", false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "HatchEgg", STRINGS.CREATURES.SPECIES.HATCH.EGG_NAME, STRINGS.CREATURES.SPECIES.HATCH.DESC, "egg_hatch_kanim", HatchTuning.EGG_MASS, "HatchBaby", 60.0000038f, 20f, HatchTuning.EGG_CHANCES_BASE, EGG_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
