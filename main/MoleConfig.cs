using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MoleConfig : IEntityConfig
{
	public const string ID = "Mole";

	public const string BASE_TRAIT_ID = "MoleBaseTrait";

	public const string EGG_ID = "MoleEgg";

	private static float MIN_POOP_SIZE_IN_CALORIES = 2400000f;

	private static float CALORIES_PER_KG_OF_DIRT = 1000f;

	public static int EGG_SORT_ORDER = 800;

	public static GameObject CreateMole(string id, string name, string desc, string anim_file, bool is_baby = false)
	{
		GameObject gameObject = BaseMoleConfig.BaseMole(id, name, STRINGS.CREATURES.SPECIES.MOLE.DESC, "MoleBaseTrait", anim_file, is_baby);
		gameObject.AddTag(GameTags.Creatures.Digger);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, MoleTuning.PEN_SIZE_PER_CREATURE, 100f);
		Trait trait = Db.Get().CreateTrait("MoleBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, MoleTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - MoleTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		List<Tag> list = new List<Tag>();
		list.Add(SimHashes.Regolith.CreateTag());
		list.Add(SimHashes.Dirt.CreateTag());
		list.Add(SimHashes.IronOre.CreateTag());
		List<Diet.Info> list2 = BaseMoleConfig.SimpleOreDiet(list, CALORIES_PER_KG_OF_DIRT, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL);
		Diet diet = new Diet(list2.ToArray());
		CreatureCalorieMonitor.Def def = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = MIN_POOP_SIZE_IN_CALORIES;
		SolidConsumerMonitor.Def def2 = gameObject.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
		OvercrowdingMonitor.Def def3 = gameObject.AddOrGetDef<OvercrowdingMonitor.Def>();
		def3.spaceRequiredPerCreature = 0;
		return gameObject;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CreateMole("Mole", STRINGS.CREATURES.SPECIES.MOLE.NAME, STRINGS.CREATURES.SPECIES.MOLE.DESC, "driller_kanim", false);
		GameObject prefab = gameObject;
		string eggId = "MoleEgg";
		string eggName = STRINGS.CREATURES.SPECIES.MOLE.EGG_NAME;
		string eggDesc = STRINGS.CREATURES.SPECIES.MOLE.DESC;
		string egg_anim = "egg_driller_kanim";
		float eGG_MASS = MoleTuning.EGG_MASS;
		string baby_id = "MoleBaby";
		float fertility_cycles = 60.0000038f;
		float incubation_cycles = 20f;
		int eGG_SORT_ORDER = EGG_SORT_ORDER;
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, eggId, eggName, eggDesc, egg_anim, eGG_MASS, baby_id, fertility_cycles, incubation_cycles, MoleTuning.EGG_CHANCES_BASE, eGG_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		SetSpawnNavType(inst);
	}

	public static void SetSpawnNavType(GameObject inst)
	{
		int cell = Grid.PosToCell(inst);
		Navigator component = inst.GetComponent<Navigator>();
		if ((Object)component != (Object)null)
		{
			if (Grid.IsSolidCell(cell))
			{
				component.SetCurrentNavType(NavType.Solid);
				inst.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.FXFront));
				KBatchedAnimController component2 = inst.GetComponent<KBatchedAnimController>();
				component2.SetSceneLayer(Grid.SceneLayer.FXFront);
			}
			else
			{
				KBatchedAnimController component3 = inst.GetComponent<KBatchedAnimController>();
				component3.SetSceneLayer(Grid.SceneLayer.Creatures);
			}
		}
	}
}
