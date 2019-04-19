using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedCureConfig : IEntityConfig
{
	public const string ID = "AdvancedCure";

	public static ComplexRecipe recipe;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("AdvancedCure", ITEMS.PILLS.ADVANCEDCURE.NAME, ITEMS.PILLS.ADVANCEDCURE.DESC, 1f, true, Assets.GetAnim("vial_spore_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.MedicalSupplies);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[2]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Tungsten.CreateTag(), 1f),
			new ComplexRecipe.RecipeElement("LightBugOrangeEgg", 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[1]
		{
			new ComplexRecipe.RecipeElement("AdvancedCure", 1f)
		};
		string id = ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(id, array, array2);
		complexRecipe.time = 200f;
		complexRecipe.description = ITEMS.PILLS.ADVANCEDCURE.RECIPEDESC;
		complexRecipe.useResultAsDescription = true;
		complexRecipe.fabricators = new List<Tag>
		{
			"Apothecary"
		};
		complexRecipe.sortOrder = 20;
		complexRecipe.requiredTech = "MedicineIV";
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
