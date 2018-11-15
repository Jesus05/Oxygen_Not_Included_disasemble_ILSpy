using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class MooConfig : IEntityConfig
{
	public const string ID = "Moo";

	public const string BASE_TRAIT_ID = "MooBaseTrait";

	public const SimHashes CONSUME_ELEMENT = SimHashes.Carbon;

	public static Tag POOP_ELEMENT = SimHashes.Methane.CreateTag();

	private static float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 2f;

	private static float CALORIES_PER_DAY_OF_PLANT_EATEN = MooTuning.STANDARD_CALORIES_PER_CYCLE / DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;

	private static float KG_POOP_PER_DAY_OF_PLANT = 5f;

	private static float MIN_POOP_SIZE_IN_KG = 1.5f;

	private static float MIN_POOP_SIZE_IN_CALORIES = CALORIES_PER_DAY_OF_PLANT_EATEN * MIN_POOP_SIZE_IN_KG / KG_POOP_PER_DAY_OF_PLANT;

	public static GameObject CreateMoo(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseMooConfig.BaseMoo(id, name, CREATURES.SPECIES.MOO.DESC, "MooBaseTrait", anim_file, is_baby, null);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, MooTuning.PEN_SIZE_PER_CREATURE, 75f);
		Trait trait = Db.Get().CreateTrait("MooBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, MooTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - MooTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name, false, false, true));
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add("GasGrass".ToTag());
		Diet.Info[] infos = new Diet.Info[1]
		{
			new Diet.Info(hashSet, POOP_ELEMENT, CALORIES_PER_DAY_OF_PLANT_EATEN, KG_POOP_PER_DAY_OF_PLANT, null, 0f, false)
		};
		Diet diet = new Diet(infos);
		CreatureCalorieMonitor.Def def = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = MIN_POOP_SIZE_IN_CALORIES;
		SolidConsumerMonitor.Def def2 = gameObject.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
		return gameObject;
	}

	public GameObject CreatePrefab()
	{
		return CreateMoo("Moo", CREATURES.SPECIES.MOO.NAME, CREATURES.SPECIES.MOO.DESC, "gassy_moo_kanim", false);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BaseMooConfig.OnSpawn(inst);
	}
}
