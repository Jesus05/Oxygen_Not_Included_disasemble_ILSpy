using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LightBugPurpleConfig : IEntityConfig
{
	public const string ID = "LightBugPurple";

	public const string BASE_TRAIT_ID = "LightBugPurpleBaseTrait";

	public const string EGG_ID = "LightBugPurpleEgg";

	private static float KG_ORE_EATEN_PER_CYCLE = 1f;

	private static float CALORIES_PER_KG_OF_ORE = LightBugTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	public static int EGG_SORT_ORDER = LightBugConfig.EGG_SORT_ORDER + 2;

	public static GameObject CreateLightBug(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BaseLightBugConfig.BaseLightBug(id, name, desc, anim_file, "LightBugPurpleBaseTrait", LIGHT2D.LIGHTBUG_COLOR_PURPLE, DECOR.BONUS.TIER6, is_baby, "prp_");
		EntityTemplates.ExtendEntityToWildCreature(prefab, LightBugTuning.PEN_SIZE_PER_CREATURE, 25f);
		Trait trait = Db.Get().CreateTrait("LightBugPurpleBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, LightBugTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - LightBugTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name, false, false, true));
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(TagManager.Create("FriedMushroom"));
		hashSet.Add(TagManager.Create("GrilledPrickleFruit"));
		hashSet.Add(TagManager.Create(SpiceNutConfig.ID));
		hashSet.Add(TagManager.Create("SpiceBread"));
		hashSet.Add(SimHashes.Phosphorite.CreateTag());
		return BaseLightBugConfig.SetupDiet(prefab, hashSet, Tag.Invalid, CALORIES_PER_KG_OF_ORE);
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CreateLightBug("LightBugPurple", STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_PURPLE.NAME, STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_PURPLE.DESC, "lightbug_kanim", false);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "LightBugPurpleEgg", STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_PURPLE.EGG_NAME, STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_PURPLE.DESC, "egg_lightbug_kanim", LightBugTuning.EGG_MASS, "LightBugPurpleBaby", 15.000001f, 5f, LightBugTuning.EGG_CHANCES_PURPLE, EGG_SORT_ORDER, true, false, true, 1f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BaseLightBugConfig.SetupLoopingSounds(inst);
	}
}
