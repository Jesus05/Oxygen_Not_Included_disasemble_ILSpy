using System.Collections.Generic;
using UnityEngine;

public class ComplexRecipe
{
	public class RecipeElement
	{
		public Tag material;

		public float amount
		{
			get;
			private set;
		}

		public RecipeElement(Tag material, float amount)
		{
			this.material = material;
			this.amount = amount;
		}
	}

	public string id;

	public RecipeElement[] ingredients;

	public RecipeElement[] results;

	public float time;

	public GameObject FabricationVisualizer;

	public bool useResultAsDescription = false;

	public bool displayInputAndOutput = false;

	public string description;

	public List<Tag> fabricators;

	public int sortOrder = 0;

	public string requiredTech;

	public ComplexRecipe(string id, RecipeElement[] ingredients, RecipeElement[] results)
	{
		this.id = id;
		this.ingredients = ingredients;
		this.results = results;
		ComplexRecipeManager.Get().Add(this);
	}

	public float TotalResultUnits()
	{
		float num = 0f;
		RecipeElement[] array = results;
		foreach (RecipeElement recipeElement in array)
		{
			num += recipeElement.amount;
		}
		return num;
	}

	public bool IsRequiredTechUnlocked()
	{
		if (!string.IsNullOrEmpty(requiredTech))
		{
			Tech tech = Db.Get().Techs.Get(requiredTech);
			return tech.IsComplete();
		}
		return true;
	}

	public Sprite GetUIIcon()
	{
		Sprite result = null;
		Tag tag = (!useResultAsDescription) ? ingredients[0].material : results[0].material;
		GameObject prefab = Assets.GetPrefab(tag);
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		if ((Object)component != (Object)null)
		{
			result = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui", false);
		}
		return result;
	}

	public Color GetUIColor()
	{
		return Color.white;
	}

	public string GetUIName()
	{
		return (!useResultAsDescription) ? ingredients[0].material.ProperName() : results[0].material.ProperName();
	}
}
