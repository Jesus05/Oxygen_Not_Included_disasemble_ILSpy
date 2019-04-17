using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class IntermediateBoosterConfig : IEntityConfig
{
	public const string ID = "IntermediateBooster";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("IntermediateBooster", ITEMS.PILLS.INTERMEDIATEBOOSTER.NAME, ITEMS.PILLS.INTERMEDIATEBOOSTER.DESC, 1f, true, Assets.GetAnim("pill_3_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.INTERMEDIATEBOOSTER);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("IntermediateBooster", 1f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(id, array, array2);
		complexRecipe.time = 100f;
		complexRecipe.description = ITEMS.PILLS.INTERMEDIATEBOOSTER.RECIPEDESC;
		complexRecipe.useResultAsDescription = true;
		complexRecipe.fabricators = new List<Tag>
		{
			"Apothecary"
		};
		complexRecipe.sortOrder = 5;
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
