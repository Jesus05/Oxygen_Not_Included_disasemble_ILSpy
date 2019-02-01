using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class EggConfig
{
	public static GameObject CreateEgg(string id, string name, string desc, Tag creature_id, string anim, float mass, int egg_sort_order, float base_incubation_rate)
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, name, desc, mass, true, Assets.GetAnim(anim), "idle", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.8f, true, 0, SimHashes.Creature, null);
		gameObject.AddOrGet<KBoxCollider2D>().offset = new Vector2f(0f, 0.36f);
		Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
		pickupable.sortOrder = SORTORDER.EGGS + egg_sort_order;
		gameObject.AddOrGet<Effects>();
		KPrefabID kPrefabID = gameObject.AddOrGet<KPrefabID>();
		kPrefabID.AddTag(GameTags.Egg);
		kPrefabID.AddTag(GameTags.IncubatableEgg);
		kPrefabID.AddTag(GameTags.PedestalDisplayable);
		IncubationMonitor.Def def = gameObject.AddOrGetDef<IncubationMonitor.Def>();
		def.spawnedCreature = creature_id;
		def.baseIncubationRate = base_incubation_rate;
		OvercrowdingMonitor.Def def2 = gameObject.AddOrGetDef<OvercrowdingMonitor.Def>();
		def2.spaceRequiredPerCreature = 0;
		Object.Destroy(gameObject.GetComponent<EntitySplitter>());
		Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		string arg = string.Format(STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RESULT_DESCRIPTION, name);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(id, 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("RawEgg", 0.5f * mass),
			new ComplexRecipe.RecipeElement("EggShell", 0.5f * mass)
		};
		string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID(id, "RawEgg");
		string text = ComplexRecipeManager.MakeRecipeID(id, array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(text, array, array2);
		complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, name, arg);
		complexRecipe.fabricators = new List<Tag>
		{
			"EggCracker"
		};
		complexRecipe.time = 5f;
		ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, text);
		return gameObject;
	}
}
