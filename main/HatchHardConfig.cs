using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class HatchHardConfig : IEntityConfig
{
	public const string ID = "HatchHard";

	public const string BASE_TRAIT_ID = "HatchHardBaseTrait";

	public const string EGG_ID = "HatchHardEgg";

	private const SimHashes EMIT_ELEMENT = SimHashes.Carbon;

	private static float KG_ORE_EATEN_PER_CYCLE = 140f;

	private static float CALORIES_PER_KG_OF_ORE = HatchTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 25f;

	public static int EGG_SORT_ORDER = HatchConfig.EGG_SORT_ORDER + 2;

	public static GameObject CreateHatch(string id, string name, string desc, string anim_file, bool is_baby)
	{
		bool is_baby2 = is_baby;
		GameObject prefab = BaseHatchConfig.BaseHatch(id, name, desc, anim_file, "HatchHardBaseTrait", is_baby2, "hvy_");
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, HatchTuning.PEN_SIZE_PER_CREATURE, 100f);
		Trait trait = Db.Get().CreateTrait("HatchHardBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, HatchTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - HatchTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 200f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		List<Diet.Info> list = BaseHatchConfig.HardRockDiet(SimHashes.Carbon.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f);
		list.AddRange(BaseHatchConfig.MetalDiet(SimHashes.Carbon.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_1, null, 0f));
		return BaseHatchConfig.SetupDiet(prefab, list, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG);
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreateHatch("HatchHard", STRINGS.CREATURES.SPECIES.HATCH.VARIANT_HARD.NAME, STRINGS.CREATURES.SPECIES.HATCH.VARIANT_HARD.DESC, "hatch_kanim", false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "HatchHardEgg", STRINGS.CREATURES.SPECIES.HATCH.VARIANT_HARD.EGG_NAME, STRINGS.CREATURES.SPECIES.HATCH.VARIANT_HARD.DESC, "egg_hatch_kanim", HatchTuning.EGG_MASS, "HatchHardBaby", 60.0000038f, 20f, HatchTuning.EGG_CHANCES_HARD, EGG_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
