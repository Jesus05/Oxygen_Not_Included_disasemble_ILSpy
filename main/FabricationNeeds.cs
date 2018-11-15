using System;

public class FabricationNeeds : KMonoBehaviour
{
	public static FabricationNeeds Instance
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
		Components.Fabricators.Register(OnAddFabricator, OnRemoveFabricator);
	}

	private void OnAddFabricator(Fabricator fabricator)
	{
		fabricator.OnCreateOrder = (Action<Fabricator.UserOrder>)Delegate.Combine(fabricator.OnCreateOrder, new Action<Fabricator.UserOrder>(OnCreateOrder));
		fabricator.OnOrderCancelledOrComplete = (Action<Fabricator.UserOrder>)Delegate.Combine(fabricator.OnOrderCancelledOrComplete, new Action<Fabricator.UserOrder>(OnFinishOrder));
	}

	private void OnRemoveFabricator(Fabricator fabricator)
	{
		fabricator.OnCreateOrder = (Action<Fabricator.UserOrder>)Delegate.Remove(fabricator.OnCreateOrder, new Action<Fabricator.UserOrder>(OnCreateOrder));
		fabricator.OnOrderCancelledOrComplete = (Action<Fabricator.UserOrder>)Delegate.Remove(fabricator.OnOrderCancelledOrComplete, new Action<Fabricator.UserOrder>(OnFinishOrder));
	}

	private void OnCreateOrder(Fabricator.UserOrder order)
	{
		Recipe.Ingredient[] allIngredients = order.recipe.GetAllIngredients(order.orderTags);
		foreach (Recipe.Ingredient ingredient in allIngredients)
		{
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, ingredient.amount);
		}
	}

	private void OnFinishOrder(Fabricator.UserOrder order)
	{
		Recipe.Ingredient[] allIngredients = order.recipe.GetAllIngredients(order.orderTags);
		foreach (Recipe.Ingredient ingredient in allIngredients)
		{
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, 0f - ingredient.amount);
		}
	}
}
