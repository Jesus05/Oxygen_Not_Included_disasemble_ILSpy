using STRINGS;
using TUNING;
using UnityEngine;

public class GenericPillConfig : IEntityConfig
{
	public const string ID = "GenericPill";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("GenericPill", ITEMS.PILLS.PLACEBO.NAME, ITEMS.PILLS.PLACEBO.DESC, 1f, true, Assets.GetAnim("pill_1_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.GENERICPILL);
		string prefabId = "GenericPill";
		string nameOverride = ITEMS.PILLS.PLACEBO.NAME;
		string recipeDescription = ITEMS.PILLS.PLACEBO.RECIPEDESC;
		Recipe recipe = new Recipe(prefabId, 1f, (SimHashes)0, nameOverride, recipeDescription, 1).SetFabricator("Apothecary", 40f);
		recipe.AddIngredient(new Recipe.Ingredient("Water", 100f));
		recipe.AddIngredient(new Recipe.Ingredient("Sand", 100f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
