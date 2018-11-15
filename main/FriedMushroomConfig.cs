using STRINGS;
using TUNING;
using UnityEngine;

public class FriedMushroomConfig : IEntityConfig
{
	public const string ID = "FriedMushroom";

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("FriedMushroom", ITEMS.FOOD.FRIEDMUSHROOM.NAME, ITEMS.FOOD.FRIEDMUSHROOM.DESC, 1f, false, Assets.GetAnim("funguscapfried_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, true, SimHashes.Creature, null);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.FRIED_MUSHROOM);
		string prefabId = "FriedMushroom";
		string recipeDescription = ITEMS.FOOD.FRIEDMUSHROOM.RECIPEDESC;
		Recipe recipe = new Recipe(prefabId, 1f, (SimHashes)0, null, recipeDescription, 20).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient(MushroomConfig.ID, 1f));
		return template;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
