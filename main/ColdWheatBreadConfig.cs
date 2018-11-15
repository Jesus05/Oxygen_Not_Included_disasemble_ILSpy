using STRINGS;
using TUNING;
using UnityEngine;

public class ColdWheatBreadConfig : IEntityConfig
{
	public const string ID = "ColdWheatBread";

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("ColdWheatBread", ITEMS.FOOD.COLDWHEATBREAD.NAME, ITEMS.FOOD.COLDWHEATBREAD.DESC, 1f, false, Assets.GetAnim("frostbread_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.COLD_WHEAT_BREAD);
		string prefabId = "ColdWheatBread";
		string recipeDescription = ITEMS.FOOD.COLDWHEATBREAD.RECIPEDESC;
		Recipe recipe = new Recipe(prefabId, 1f, (SimHashes)0, null, recipeDescription, 50).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient("ColdWheatSeed", 3f));
		return template;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
