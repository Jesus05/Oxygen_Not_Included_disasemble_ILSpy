using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BasicCureConfig : IEntityConfig
{
	public const string ID = "BasicCure";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("BasicCure", ITEMS.PILLS.BASICCURE.NAME, ITEMS.PILLS.BASICCURE.DESC, 1f, true, Assets.GetAnim("pill_foodpoisoning_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.BASICCURE);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Carbon.CreateTag(), 1f),
			new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("BasicCure", 1f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(id, array, array2);
		complexRecipe.time = 50f;
		complexRecipe.description = ITEMS.PILLS.BASICCURE.RECIPEDESC;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag>
		{
			"Apothecary"
		};
		complexRecipe.sortOrder = 10;
		recipe = complexRecipe;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
