using STRINGS;
using TUNING;
using UnityEngine;

public class FruitCakeConfig : IEntityConfig
{
	public const string ID = "FruitCake";

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("FruitCake", ITEMS.FOOD.FRUITCAKE.NAME, ITEMS.FOOD.FRUITCAKE.DESC, 1f, false, Assets.GetAnim("fruitcake_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.FRUITCAKE);
		string prefabId = "FruitCake";
		string recipeDescription = ITEMS.FOOD.FRUITCAKE.RECIPEDESC;
		Recipe recipe = new Recipe(prefabId, 1f, (SimHashes)0, null, recipeDescription, 3);
		recipe.AddIngredient(new Recipe.Ingredient("ColdWheatSeed", 5f));
		recipe.AddIngredient(new Recipe.Ingredient(PrickleFruitConfig.ID, 1f));
		recipe.FabricationVisualizer = CreateFabricationVisualizer(template);
		recipe.SetFabricator("MicrobeMusher", FOOD.RECIPES.STANDARD_COOK_TIME);
		return template;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static GameObject CreateFabricationVisualizer(GameObject result)
	{
		KBatchedAnimController component = result.GetComponent<KBatchedAnimController>();
		GameObject gameObject = new GameObject();
		gameObject.name = result.name + "Visualizer";
		gameObject.SetActive(false);
		gameObject.transform.SetLocalPosition(Vector3.zero);
		KBatchedAnimController kBatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kBatchedAnimController.AnimFiles = component.AnimFiles;
		kBatchedAnimController.initialAnim = "fabricating";
		kBatchedAnimController.isMovable = true;
		KBatchedAnimTracker kBatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
		kBatchedAnimTracker.symbol = new HashedString("meter_ration");
		kBatchedAnimTracker.offset = Vector3.zero;
		kBatchedAnimTracker.skipInitialDisable = true;
		return gameObject;
	}
}
