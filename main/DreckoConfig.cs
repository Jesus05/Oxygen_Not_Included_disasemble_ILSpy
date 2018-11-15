using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DreckoConfig : IEntityConfig
{
	public const string ID = "Drecko";

	public const string BASE_TRAIT_ID = "DreckoBaseTrait";

	public const string EGG_ID = "DreckoEgg";

	public static Tag POOP_ELEMENT = SimHashes.Phosphorite.CreateTag();

	public static Tag EMIT_ELEMENT = BasicFabricConfig.ID;

	private static float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 2f;

	private static float CALORIES_PER_DAY_OF_PLANT_EATEN = DreckoTuning.STANDARD_CALORIES_PER_CYCLE / DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;

	private static float KG_POOP_PER_DAY_OF_PLANT = 5f;

	private static float MIN_POOP_SIZE_IN_KG = 1.5f;

	private static float MIN_POOP_SIZE_IN_CALORIES = CALORIES_PER_DAY_OF_PLANT_EATEN * MIN_POOP_SIZE_IN_KG / KG_POOP_PER_DAY_OF_PLANT;

	public static float SCALE_GROWTH_TIME_IN_CYCLES = 8f;

	public static float FIBER_PER_CYCLE = 0.25f;

	public static int EGG_SORT_ORDER = 800;

	public static GameObject CreateDrecko(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BaseDreckoConfig.BaseDrecko(id, name, desc, anim_file, "DreckoBaseTrait", is_baby, "fbr_", 308.15f, 363.15f);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, DreckoTuning.PEN_SIZE_PER_CREATURE, 150f);
		Trait trait = Db.Get().CreateTrait("DreckoBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, DreckoTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - DreckoTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name, false, false, true));
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add("SpiceVine".ToTag());
		hashSet.Add(SwampLilyConfig.ID.ToTag());
		hashSet.Add("BasicSingleHarvestPlant".ToTag());
		Diet.Info[] infos = new Diet.Info[1]
		{
			new Diet.Info(hashSet, POOP_ELEMENT, CALORIES_PER_DAY_OF_PLANT_EATEN, KG_POOP_PER_DAY_OF_PLANT, null, 0f, false)
		};
		Diet diet = new Diet(infos);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = MIN_POOP_SIZE_IN_CALORIES;
		SolidConsumerMonitor.Def def2 = prefab.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
		ScaleGrowthMonitor.Def def3 = prefab.AddOrGetDef<ScaleGrowthMonitor.Def>();
		def3.defaultGrowthRate = 1f / SCALE_GROWTH_TIME_IN_CYCLES / 600f;
		def3.dropMass = FIBER_PER_CYCLE * SCALE_GROWTH_TIME_IN_CYCLES;
		def3.itemDroppedOnShear = EMIT_ELEMENT;
		def3.levelCount = 6;
		def3.targetAtmosphere = SimHashes.Hydrogen;
		return prefab;
	}

	public virtual GameObject CreatePrefab()
	{
		GameObject gameObject = CreateDrecko("Drecko", CREATURES.SPECIES.DRECKO.NAME, CREATURES.SPECIES.DRECKO.DESC, "drecko_kanim", false);
		GameObject prefab = gameObject;
		string eggId = "DreckoEgg";
		string eggName = CREATURES.SPECIES.DRECKO.EGG_NAME;
		string eggDesc = CREATURES.SPECIES.DRECKO.DESC;
		string egg_anim = "egg_drecko_kanim";
		float eGG_MASS = DreckoTuning.EGG_MASS;
		string baby_id = "DreckoBaby";
		float fertility_cycles = 90f;
		float incubation_cycles = 30f;
		int eGG_SORT_ORDER = EGG_SORT_ORDER;
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, eggId, eggName, eggDesc, egg_anim, eGG_MASS, baby_id, fertility_cycles, incubation_cycles, DreckoTuning.EGG_CHANCES_BASE, eGG_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
