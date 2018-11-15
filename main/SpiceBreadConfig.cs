using STRINGS;
using TUNING;
using UnityEngine;

public class SpiceBreadConfig : IEntityConfig
{
	public const string ID = "SpiceBread";

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("SpiceBread", ITEMS.FOOD.SPICEBREAD.NAME, ITEMS.FOOD.SPICEBREAD.DESC, 1f, false, Assets.GetAnim("pepperbread_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, true, SimHashes.Creature, null);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.SPICEBREAD);
		string prefabId = "SpiceBread";
		string recipeDescription = ITEMS.FOOD.SPICEBREAD.RECIPEDESC;
		Recipe recipe = new Recipe(prefabId, 1f, (SimHashes)0, null, recipeDescription, 100).SetFabricator("CookingStation", FOOD.RECIPES.STANDARD_COOK_TIME);
		recipe.AddIngredient(new Recipe.Ingredient("ColdWheatSeed", 10f));
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
