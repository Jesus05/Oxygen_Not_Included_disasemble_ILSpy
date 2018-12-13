using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ComplexRecipeManager
{
	private static ComplexRecipeManager _Instance;

	public List<ComplexRecipe> recipes = new List<ComplexRecipe>();

	private Dictionary<string, string> obsoleteIDMapping = new Dictionary<string, string>();

	public static ComplexRecipeManager Get()
	{
		if (_Instance == null)
		{
			_Instance = new ComplexRecipeManager();
		}
		return _Instance;
	}

	public static void DestroyInstance()
	{
		_Instance = null;
	}

	public static string MakeObsoleteRecipeID(string fabricator, Tag signatureElement)
	{
		return fabricator + "_" + signatureElement;
	}

	public static string MakeRecipeID(string fabricator, IList<ComplexRecipe.RecipeElement> inputs, IList<ComplexRecipe.RecipeElement> outputs)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(fabricator);
		stringBuilder.Append("_I");
		foreach (ComplexRecipe.RecipeElement input in inputs)
		{
			stringBuilder.Append("_");
			stringBuilder.Append(input.material.ToString());
		}
		stringBuilder.Append("_O");
		foreach (ComplexRecipe.RecipeElement output in outputs)
		{
			stringBuilder.Append("_");
			stringBuilder.Append(output.material.ToString());
		}
		return stringBuilder.ToString();
	}

	public void Add(ComplexRecipe recipe)
	{
		foreach (ComplexRecipe recipe2 in recipes)
		{
			if (recipe2.id == recipe.id)
			{
				Output.LogError($"DUPLICATE RECIPE ID! '{recipe.id}' is being added to the recipe manager multiple times. This will result in the failure to save/load certain queued recipes at fabricators.");
			}
		}
		recipes.Add(recipe);
		if ((Object)recipe.FabricationVisualizer != (Object)null)
		{
			Object.DontDestroyOnLoad(recipe.FabricationVisualizer);
		}
	}

	public ComplexRecipe GetRecipe(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		return recipes.Find((ComplexRecipe r) => r.id == id);
	}

	public void AddObsoleteIDMapping(string obsolete_id, string new_id)
	{
		obsoleteIDMapping[obsolete_id] = new_id;
	}

	public ComplexRecipe GetObsoleteRecipe(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		ComplexRecipe result = null;
		string value = null;
		if (obsoleteIDMapping.TryGetValue(id, out value))
		{
			result = GetRecipe(value);
		}
		return result;
	}
}
