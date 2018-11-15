using STRINGS;
using TUNING;
using UnityEngine;

public class BasicPlantBarConfig : IEntityConfig
{
	public const string ID = "BasicPlantBar";

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("BasicPlantBar", ITEMS.FOOD.BASICPLANTBAR.NAME, ITEMS.FOOD.BASICPLANTBAR.DESC, 1f, false, Assets.GetAnim("liceloaf_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.BASICPLANTBAR);
		string prefabId = "BasicPlantBar";
		float outputUnits = 1f;
		string recipeDescription = ITEMS.FOOD.BASICPLANTBAR.RECIPEDESC;
		Recipe recipe = new Recipe(prefabId, outputUnits, (SimHashes)0, null, recipeDescription, 2);
		recipe.AddIngredient(new Recipe.Ingredient("BasicPlantFood", 2f));
		recipe.AddIngredient(new Recipe.Ingredient("Water", 50f));
		recipe.FabricationVisualizer = MushBarConfig.CreateFabricationVisualizer(template);
		recipe.SetFabricator("MicrobeMusher", FOOD.RECIPES.STANDARD_COOK_TIME);
		return template;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
