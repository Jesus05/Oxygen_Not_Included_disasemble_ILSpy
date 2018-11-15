using STRINGS;
using TUNING;
using UnityEngine;

public class CookedMeatConfig : IEntityConfig
{
	public const string ID = "CookedMeat";

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("CookedMeat", ITEMS.FOOD.COOKEDMEAT.NAME, ITEMS.FOOD.COOKEDMEAT.DESC, 1f, false, Assets.GetAnim("barbeque_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.COOKEDMEAT);
		string prefabId = "CookedMeat";
		string recipeDescription = ITEMS.FOOD.COOKEDMEAT.RECIPEDESC;
		Recipe recipe = new Recipe(prefabId, 1f, (SimHashes)0, null, recipeDescription, 21).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient("Meat", 2f));
		recipe.AddIngredient(new Recipe.Ingredient(SpiceNutConfig.ID, 1f));
		return template;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
