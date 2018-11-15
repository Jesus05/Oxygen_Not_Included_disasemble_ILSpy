using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class HatchMetalConfig : IEntityConfig
{
	public const string ID = "HatchMetal";

	public const string BASE_TRAIT_ID = "HatchMetalBaseTrait";

	public const string EGG_ID = "HatchMetalEgg";

	private static float KG_ORE_EATEN_PER_CYCLE = 100f;

	private static float CALORIES_PER_KG_OF_ORE = HatchTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 10f;

	public static int EGG_SORT_ORDER = HatchConfig.EGG_SORT_ORDER + 3;

	public static readonly TagBits METAL_ORE_TAGS = new TagBits(new Tag[4]
	{
		SimHashes.Cuprite.CreateTag(),
		SimHashes.GoldAmalgam.CreateTag(),
		SimHashes.IronOre.CreateTag(),
		SimHashes.Wolframite.CreateTag()
	});

	public static GameObject CreateHatch(string id, string name, string desc, string anim_file, bool is_baby)
	{
		bool is_baby2 = is_baby;
		GameObject prefab = BaseHatchConfig.BaseHatch(id, name, desc, anim_file, "HatchMetalBaseTrait", is_baby2, "mtl_");
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, HatchTuning.PEN_SIZE_PER_CREATURE, 100f);
		Trait trait = Db.Get().CreateTrait("HatchMetalBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, HatchTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - HatchTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 400f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		List<Diet.Info> diet_infos = BaseHatchConfig.MetalDiet(GameTags.Metal, CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0f);
		return BaseHatchConfig.SetupDiet(prefab, diet_infos, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG);
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreateHatch("HatchMetal", STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.NAME, STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.DESC, "hatch_kanim", false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "HatchMetalEgg", STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.EGG_NAME, STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.DESC, "egg_hatch_kanim", HatchTuning.EGG_MASS, "HatchMetalBaby", 60.0000038f, 20f, HatchTuning.EGG_CHANCES_METAL, EGG_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
