using System;
using System.Collections.Generic;

public class FabricatorIngredientStatusManager : KMonoBehaviour, ISim1000ms
{
	private KSelectable selectable;

	private ComplexFabricator fabricator;

	private Dictionary<ComplexRecipe, Guid> statusItems = new Dictionary<ComplexRecipe, Guid>();

	private Dictionary<ComplexRecipe, Dictionary<Tag, float>> recipeRequiredResourceBalances = new Dictionary<ComplexRecipe, Dictionary<Tag, float>>();

	private List<ComplexRecipe> deadOrderKeys = new List<ComplexRecipe>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		selectable = GetComponent<KSelectable>();
		fabricator = GetComponent<ComplexFabricator>();
		InitializeBalances();
	}

	private void InitializeBalances()
	{
		ComplexRecipe[] recipes = fabricator.GetRecipes();
		foreach (ComplexRecipe complexRecipe in recipes)
		{
			recipeRequiredResourceBalances.Add(complexRecipe, new Dictionary<Tag, float>());
			ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				recipeRequiredResourceBalances[complexRecipe].Add(recipeElement.material, 0f);
			}
		}
	}

	public void Sim1000ms(float dt)
	{
		RefreshStatusItems();
	}

	private void RefreshStatusItems()
	{
		foreach (KeyValuePair<ComplexRecipe, Guid> statusItem in statusItems)
		{
			ComplexFabricator.UserOrder userOrder = fabricator.GetUserOrders().Find((ComplexFabricator.UserOrder match) => match.recipe == statusItem.Key);
			if (userOrder == null)
			{
				deadOrderKeys.Add(statusItem.Key);
			}
		}
		foreach (ComplexRecipe deadOrderKey in deadOrderKeys)
		{
			recipeRequiredResourceBalances[deadOrderKey].Clear();
			ComplexRecipe.RecipeElement[] ingredients = deadOrderKey.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				recipeRequiredResourceBalances[deadOrderKey].Add(recipeElement.material, 0f);
			}
			selectable.RemoveStatusItem(statusItems[deadOrderKey], false);
			statusItems.Remove(deadOrderKey);
		}
		deadOrderKeys.Clear();
		foreach (ComplexFabricator.UserOrder userOrder2 in fabricator.GetUserOrders())
		{
			bool flag = false;
			ComplexRecipe.RecipeElement[] ingredients2 = userOrder2.recipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement2 in ingredients2)
			{
				float newBalance = fabricator.inStorage.GetAmountAvailable(recipeElement2.material) + fabricator.buildStorage.GetAmountAvailable(recipeElement2.material) + WorldInventory.Instance.GetAmount(recipeElement2.material) - recipeElement2.amount;
				flag = (flag || ChangeRecipeRequiredResourceBalance(userOrder2.recipe, recipeElement2.material, newBalance) || (statusItems.ContainsKey(userOrder2.recipe) && fabricator.GetRecipeQueueCount(userOrder2.recipe) == 0));
			}
			if (flag)
			{
				if (statusItems.ContainsKey(userOrder2.recipe))
				{
					selectable.RemoveStatusItem(statusItems[userOrder2.recipe], false);
					statusItems.Remove(userOrder2.recipe);
				}
				if (fabricator.GetRecipeQueueCount(userOrder2.recipe) > 0)
				{
					foreach (float value2 in recipeRequiredResourceBalances[userOrder2.recipe].Values)
					{
						if (value2 < 0f)
						{
							Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
							foreach (KeyValuePair<Tag, float> item in recipeRequiredResourceBalances[userOrder2.recipe])
							{
								if (item.Value < 0f)
								{
									dictionary.Add(item.Key, 0f - item.Value);
								}
							}
							Guid value = selectable.AddStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, dictionary);
							statusItems.Add(userOrder2.recipe, value);
							break;
						}
					}
				}
			}
		}
	}

	private bool ChangeRecipeRequiredResourceBalance(ComplexRecipe recipe, Tag tag, float newBalance)
	{
		bool result = false;
		if (recipeRequiredResourceBalances[recipe][tag] >= 0f != newBalance >= 0f)
		{
			result = true;
		}
		recipeRequiredResourceBalances[recipe][tag] = newBalance;
		return result;
	}
}
