using STRINGS;
using TUNING;
using UnityEngine;

public class VitaminSupplementConfig : IEntityConfig
{
	public const string ID = "VitaminSupplement";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("VitaminSupplement", ITEMS.PILLS.VITAMINSUPPLEMENT.NAME, ITEMS.PILLS.VITAMINSUPPLEMENT.DESC, 1f, true, Assets.GetAnim("pill_2_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.VITAMINSUPPLEMENT);
		string prefabId = "VitaminSupplement";
		string nameOverride = ITEMS.PILLS.VITAMINSUPPLEMENT.NAME;
		string recipeDescription = ITEMS.PILLS.VITAMINSUPPLEMENT.RECIPEDESC;
		Recipe recipe = new Recipe(prefabId, 1f, (SimHashes)0, nameOverride, recipeDescription, 10).SetFabricator("Apothecary", 40f);
		recipe.AddIngredient(new Recipe.Ingredient("Carbon", 100f));
		recipe.AddIngredient(new Recipe.Ingredient(SwampLilyFlowerConfig.ID, 1f));
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
