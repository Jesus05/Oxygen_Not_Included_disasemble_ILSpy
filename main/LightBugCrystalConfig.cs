using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LightBugCrystalConfig : IEntityConfig
{
	public const string ID = "LightBugCrystal";

	public const string BASE_TRAIT_ID = "LightBugCrystalBaseTrait";

	public const string EGG_ID = "LightBugCrystalEgg";

	private static float KG_ORE_EATEN_PER_CYCLE = 1f;

	private static float CALORIES_PER_KG_OF_ORE = LightBugTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	public static int EGG_SORT_ORDER = LightBugConfig.EGG_SORT_ORDER + 7;

	public static GameObject CreateLightBug(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BaseLightBugConfig.BaseLightBug(id, name, desc, anim_file, "LightBugCrystalBaseTrait", LIGHT2D.LIGHTBUG_COLOR_CRYSTAL, DECOR.BONUS.TIER8, is_baby, "cry_");
		EntityTemplates.ExtendEntityToWildCreature(prefab, LightBugTuning.PEN_SIZE_PER_CREATURE, 75f);
		Trait trait = Db.Get().CreateTrait("LightBugCrystalBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, LightBugTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - LightBugTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name, false, false, true));
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(TagManager.Create("CookedMeat"));
		hashSet.Add(SimHashes.Diamond.CreateTag());
		prefab = BaseLightBugConfig.SetupDiet(prefab, hashSet, Tag.Invalid, CALORIES_PER_KG_OF_ORE);
		LureableMonitor.Def def = prefab.AddOrGetDef<LureableMonitor.Def>();
		def.lures = new Tag[1]
		{
			SimHashes.Diamond.CreateTag()
		};
		return prefab;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CreateLightBug("LightBugCrystal", STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_CRYSTAL.NAME, STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_CRYSTAL.DESC, "lightbug_kanim", false);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "LightBugCrystalEgg", STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_CRYSTAL.EGG_NAME, STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_CRYSTAL.DESC, "egg_lightbug_kanim", LightBugTuning.EGG_MASS, "LightBugCrystalBaby", 45f, 15f, LightBugTuning.EGG_CHANCES_CRYSTAL, EGG_SORT_ORDER, true, false, true, 1f);
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
