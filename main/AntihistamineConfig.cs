using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class AntihistamineConfig : IEntityConfig
{
	public const string ID = "Antihistamine";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("Antihistamine", ITEMS.PILLS.ANTIHISTAMINE.NAME, ITEMS.PILLS.ANTIHISTAMINE.DESC, 1f, true, Assets.GetAnim("pill_allergies_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.ANTIHISTAMINE);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement("PrickleFlowerSeed", 1f),
			new ComplexRecipe.RecipeElement(SimHashes.Dirt.CreateTag(), 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("Antihistamine", 1f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(id, array, array2);
		complexRecipe.time = 100f;
		complexRecipe.description = ITEMS.PILLS.ANTIHISTAMINE.RECIPEDESC;
		complexRecipe.useResultAsDescription = true;
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
