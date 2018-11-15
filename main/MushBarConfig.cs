using STRINGS;
using TUNING;
using UnityEngine;

public class MushBarConfig : IEntityConfig
{
	public const string ID = "MushBar";

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("MushBar", ITEMS.FOOD.MUSHBAR.NAME, ITEMS.FOOD.MUSHBAR.DESC, 1f, false, Assets.GetAnim("mushbar_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.MUSHBAR);
		string prefabId = "MushBar";
		string recipeDescription = ITEMS.FOOD.MUSHBAR.RECIPEDESC;
		Recipe recipe = new Recipe(prefabId, 1f, (SimHashes)0, null, recipeDescription, 1);
		recipe.AddIngredient(new Recipe.Ingredient("Dirt", 75f));
		recipe.AddIngredient(new Recipe.Ingredient("Water", 75f));
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
