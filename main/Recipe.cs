using Klei;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class Recipe : IHasSortOrder
{
	[Serializable]
	[DebuggerDisplay("{tag} {amount}")]
	public class Ingredient
	{
		public Tag tag;

		public float amount;

		public Ingredient(string tag, float amount)
		{
			this.tag = TagManager.Create(tag);
			this.amount = amount;
		}

		public Ingredient(Tag tag, float amount)
		{
			this.tag = tag;
			this.amount = amount;
		}

		public List<Element> GetElementOptions()
		{
			List<Element> list = new List<Element>(ElementLoader.elements);
			list.RemoveAll((Element e) => !e.IsSolid);
			list.RemoveAll((Element e) => !e.HasTag(tag));
			return list;
		}
	}

	private string nameOverride;

	public string HotKey;

	public string Type;

	public List<Ingredient> Ingredients;

	public string recipeDescription;

	public Tag Result;

	public GameObject FabricationVisualizer;

	public SimHashes ResultElementOverride;

	public Sprite Icon;

	public Color IconColor = Color.white;

	public string[] fabricators;

	public float OutputUnits;

	public float FabricationTime;

	public string TechUnlock;

	public int sortOrder
	{
		get;
		set;
	}

	public string Name
	{
		get
		{
			return (nameOverride != null) ? nameOverride : Result.ProperName();
		}
		set
		{
			nameOverride = value;
		}
	}

	public string[] MaterialOptionNames
	{
		get
		{
			List<string> list = new List<string>();
			foreach (Element element in ElementLoader.elements)
			{
				if (Array.IndexOf(element.oreTags, Ingredients[0].tag) >= 0)
				{
					list.Add(element.id.ToString());
				}
			}
			return list.ToArray();
		}
	}

	public Recipe()
	{
	}

	public Recipe(string prefabId, float outputUnits = 1f, SimHashes elementOverride = (SimHashes)0, string nameOverride = null, string recipeDescription = null, int sortOrder = 0)
	{
		Debug.Assert(prefabId != null);
		Result = TagManager.Create(prefabId);
		ResultElementOverride = elementOverride;
		this.nameOverride = nameOverride;
		OutputUnits = outputUnits;
		Ingredients = new List<Ingredient>();
		this.recipeDescription = recipeDescription;
		this.sortOrder = sortOrder;
		FabricationVisualizer = null;
	}

	public Recipe SetFabricator(string fabricator, float fabricationTime)
	{
		fabricators = new string[1]
		{
			fabricator
		};
		FabricationTime = fabricationTime;
		RecipeManager.Get().Add(this);
		return this;
	}

	public Recipe SetFabricators(string[] fabricators, float fabricationTime)
	{
		this.fabricators = fabricators;
		FabricationTime = fabricationTime;
		RecipeManager.Get().Add(this);
		return this;
	}

	public Recipe SetIcon(Sprite Icon)
	{
		this.Icon = Icon;
		IconColor = Color.white;
		return this;
	}

	public Recipe SetIcon(Sprite Icon, Color IconColor)
	{
		this.Icon = Icon;
		this.IconColor = IconColor;
		return this;
	}

	public Recipe AddIngredient(Ingredient ingredient)
	{
		Ingredients.Add(ingredient);
		return this;
	}

	public Ingredient[] GetAllIngredients(IList<Tag> selectedTags)
	{
		List<Ingredient> list = new List<Ingredient>();
		for (int i = 0; i < Ingredients.Count; i++)
		{
			float amount = Ingredients[i].amount;
			if (i < selectedTags.Count)
			{
				list.Add(new Ingredient(selectedTags[i], amount));
			}
			else
			{
				list.Add(new Ingredient(Ingredients[i].tag, amount));
			}
		}
		return list.ToArray();
	}

	public Ingredient[] GetAllIngredients(IList<Element> selected_elements)
	{
		List<Ingredient> list = new List<Ingredient>();
		for (int i = 0; i < Ingredients.Count; i++)
		{
			int num = (int)Ingredients[i].amount;
			bool flag = false;
			if (i < selected_elements.Count)
			{
				Element element = selected_elements[i];
				if (element != null && element.HasTag(Ingredients[i].tag))
				{
					list.Add(new Ingredient(GameTagExtensions.Create(element.id), (float)num));
					flag = true;
				}
			}
			if (!flag)
			{
				list.Add(new Ingredient(Ingredients[i].tag, (float)num));
			}
		}
		return list.ToArray();
	}

	public GameObject Craft(Storage resource_storage, IList<Tag> selectedTags)
	{
		Ingredient[] allIngredients = GetAllIngredients(selectedTags);
		return CraftRecipe(resource_storage, allIngredients);
	}

	private GameObject CraftRecipe(Storage resource_storage, Ingredient[] ingredientTags)
	{
		SimUtil.DiseaseInfo a = SimUtil.DiseaseInfo.Invalid;
		float num = 0f;
		float num2 = 0f;
		foreach (Ingredient ingredient in ingredientTags)
		{
			GameObject gameObject = resource_storage.FindFirst(ingredient.tag);
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				Edible component = gameObject.GetComponent<Edible>();
				if ((bool)component)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, 0f - component.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.CRAFTED_USED, "{0}", component.GetProperName()), UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
				}
			}
			resource_storage.ConsumeAndGetDisease(ingredient, out SimUtil.DiseaseInfo disease_info, out float temperature);
			a = SimUtil.CalculateFinalDiseaseInfo(a, disease_info);
			num = SimUtil.CalculateFinalTemperature(num2, num, ingredient.amount, temperature);
			num2 += ingredient.amount;
		}
		GameObject prefab = Assets.GetPrefab(Result);
		GameObject gameObject2 = null;
		if ((UnityEngine.Object)prefab != (UnityEngine.Object)null)
		{
			gameObject2 = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, null, 0);
			PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
			gameObject2.GetComponent<KSelectable>().entityName = Name;
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				gameObject2.GetComponent<KPrefabID>().RemoveTag(TagManager.Create("Vacuum"));
				if (ResultElementOverride != 0)
				{
					if ((UnityEngine.Object)component2.GetComponent<ElementChunk>() != (UnityEngine.Object)null)
					{
						component2.SetElement(ResultElementOverride);
					}
					else
					{
						component2.ElementID = ResultElementOverride;
					}
				}
				component2.Temperature = num;
				component2.Units = OutputUnits;
			}
			Edible component3 = gameObject2.GetComponent<Edible>();
			if ((bool)component3)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component3.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.CRAFTED, "{0}", component3.GetProperName()), UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
			}
			gameObject2.SetActive(true);
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				component2.AddDisease(a.idx, a.count, "Recipe.CraftRecipe");
			}
			gameObject2.GetComponent<KMonoBehaviour>().Trigger(748399584, null);
		}
		return gameObject2;
	}

	public Element[] MaterialOptions()
	{
		List<Element> list = new List<Element>();
		foreach (Element element in ElementLoader.elements)
		{
			if (Array.IndexOf(element.oreTags, Ingredients[0].tag) >= 0)
			{
				list.Add(element);
			}
		}
		return list.ToArray();
	}

	public BuildingDef GetBuildingDef()
	{
		GameObject prefab = Assets.GetPrefab(Result);
		BuildingComplete component = prefab.GetComponent<BuildingComplete>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			return component.Def;
		}
		return null;
	}

	public Sprite GetUIIcon()
	{
		Sprite result = null;
		if ((UnityEngine.Object)Icon != (UnityEngine.Object)null)
		{
			result = Icon;
		}
		else
		{
			GameObject prefab = Assets.GetPrefab(Result);
			KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				result = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui", false);
			}
		}
		return result;
	}

	public Color GetUIColor()
	{
		return (!((UnityEngine.Object)Icon != (UnityEngine.Object)null)) ? Color.white : IconColor;
	}
}
