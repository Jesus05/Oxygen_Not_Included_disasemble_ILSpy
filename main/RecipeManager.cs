using System.Collections.Generic;
using UnityEngine;

public class RecipeManager
{
	private static RecipeManager _Instance;

	public List<Recipe> recipes = new List<Recipe>();

	public static RecipeManager Get()
	{
		if (_Instance == null)
		{
			_Instance = new RecipeManager();
		}
		return _Instance;
	}

	public static void DestroyInstance()
	{
		_Instance = null;
	}

	public void Add(Recipe recipe)
	{
		recipes.Add(recipe);
		if ((Object)recipe.FabricationVisualizer != (Object)null)
		{
			Object.DontDestroyOnLoad(recipe.FabricationVisualizer);
		}
	}
}
