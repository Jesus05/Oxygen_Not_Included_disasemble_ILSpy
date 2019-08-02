using STRINGS;
using TUNING;
using UnityEngine;

public class MushBarConfig : IEntityConfig
{
	public const string ID = "MushBar";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("MushBar", ITEMS.FOOD.MUSHBAR.NAME, ITEMS.FOOD.MUSHBAR.DESC, 1f, false, Assets.GetAnim("mushbar_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.MUSHBAR);
		ComplexRecipeManager.Get().GetRecipe(recipe.id).FabricationVisualizer = CreateFabricationVisualizer(template);
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
		Object.DontDestroyOnLoad(gameObject);
		return gameObject;
	}
}
