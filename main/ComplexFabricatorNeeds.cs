using System;

public class ComplexFabricatorNeeds : KMonoBehaviour
{
	public static ComplexFabricatorNeeds Instance
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
		Components.ComplexFabricators.Register(OnAddRefinery, OnRemoveRefinery);
	}

	private void OnAddRefinery(ComplexFabricator refinery)
	{
		refinery.OnCreateMachineOrder = (Action<ComplexFabricator.MachineOrder>)Delegate.Combine(refinery.OnCreateMachineOrder, new Action<ComplexFabricator.MachineOrder>(OnCreateMachineOrder));
		refinery.OnMachineOrderCancelledOrComplete = (Action<ComplexFabricator.MachineOrder>)Delegate.Combine(refinery.OnMachineOrderCancelledOrComplete, new Action<ComplexFabricator.MachineOrder>(OnFinishMachineOrder));
	}

	private void OnRemoveRefinery(ComplexFabricator refinery)
	{
		refinery.OnCreateMachineOrder = (Action<ComplexFabricator.MachineOrder>)Delegate.Remove(refinery.OnCreateMachineOrder, new Action<ComplexFabricator.MachineOrder>(OnCreateMachineOrder));
		refinery.OnMachineOrderCancelledOrComplete = (Action<ComplexFabricator.MachineOrder>)Delegate.Remove(refinery.OnMachineOrderCancelledOrComplete, new Action<ComplexFabricator.MachineOrder>(OnFinishMachineOrder));
	}

	private void OnCreateMachineOrder(ComplexFabricator.MachineOrder order)
	{
		ComplexRecipe.RecipeElement[] ingredients = order.parentOrder.recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			MaterialNeeds.Instance.UpdateNeed(recipeElement.material, recipeElement.amount);
		}
	}

	private void OnFinishMachineOrder(ComplexFabricator.MachineOrder order)
	{
		ComplexRecipe.RecipeElement[] ingredients = order.parentOrder.recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			MaterialNeeds.Instance.UpdateNeed(recipeElement.material, 0f - recipeElement.amount);
		}
	}
}
