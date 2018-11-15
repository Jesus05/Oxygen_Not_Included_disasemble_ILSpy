using System;

public class RefineryNeeds : KMonoBehaviour
{
	public static RefineryNeeds Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		Components.Refineries.Register(OnAddRefinery, OnRemoveRefinery);
	}

	private void OnAddRefinery(Refinery refinery)
	{
		refinery.OnCreateOrder = (Action<Refinery.UserOrder>)Delegate.Combine(refinery.OnCreateOrder, new Action<Refinery.UserOrder>(OnCreateOrder));
		refinery.OnOrderCancelledOrComplete = (Action<Refinery.UserOrder>)Delegate.Combine(refinery.OnOrderCancelledOrComplete, new Action<Refinery.UserOrder>(OnFinishOrder));
	}

	private void OnRemoveRefinery(Refinery refinery)
	{
		refinery.OnCreateOrder = (Action<Refinery.UserOrder>)Delegate.Remove(refinery.OnCreateOrder, new Action<Refinery.UserOrder>(OnCreateOrder));
		refinery.OnOrderCancelledOrComplete = (Action<Refinery.UserOrder>)Delegate.Remove(refinery.OnOrderCancelledOrComplete, new Action<Refinery.UserOrder>(OnFinishOrder));
	}

	private void OnCreateOrder(Refinery.UserOrder order)
	{
		ComplexRecipe.RecipeElement[] ingredients = order.recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			MaterialNeeds.Instance.UpdateNeed(recipeElement.material, recipeElement.amount);
		}
	}

	private void OnFinishOrder(Refinery.UserOrder order)
	{
		ComplexRecipe.RecipeElement[] ingredients = order.recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			MaterialNeeds.Instance.UpdateNeed(recipeElement.material, 0f - recipeElement.amount);
		}
	}
}
