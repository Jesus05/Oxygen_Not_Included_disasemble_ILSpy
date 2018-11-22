using System;
using UnityEngine;

public class EggCracker : KMonoBehaviour
{
	[MyCmpReq]
	private ComplexFabricator refinery;

	[MyCmpReq]
	private ComplexFabricatorWorkable workable;

	private KBatchedAnimTracker tracker;

	private GameObject display_egg;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ComplexFabricatorWorkable complexFabricatorWorkable = workable;
		complexFabricatorWorkable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(complexFabricatorWorkable.OnWorkableEventCB, new Action<Workable.WorkableEvent>(OnWorkableEvent));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		UnityEngine.Object.Destroy(tracker);
		tracker = null;
	}

	private void OnWorkableEvent(Workable.WorkableEvent e)
	{
		switch (e)
		{
		case Workable.WorkableEvent.WorkStarted:
		{
			ComplexFabricator.MachineOrder currentMachineOrder = refinery.CurrentMachineOrder;
			if (currentMachineOrder != null)
			{
				ComplexRecipe.RecipeElement[] ingredients = currentMachineOrder.parentOrder.recipe.ingredients;
				if (ingredients.Length > 0)
				{
					ComplexRecipe.RecipeElement recipeElement = ingredients[0];
					display_egg = refinery.buildStorage.FindFirst(recipeElement.material);
					PositionActiveEgg();
				}
			}
			break;
		}
		case Workable.WorkableEvent.WorkCompleted:
			if ((bool)display_egg)
			{
				KBatchedAnimController component = display_egg.GetComponent<KBatchedAnimController>();
				component.Play("hatching_pst", KAnim.PlayMode.Once, 1f, 0f);
			}
			break;
		case Workable.WorkableEvent.WorkStopped:
			UnityEngine.Object.Destroy(tracker);
			tracker = null;
			display_egg = null;
			break;
		}
	}

	private void PositionActiveEgg()
	{
		if ((bool)display_egg)
		{
			KBatchedAnimController component = display_egg.GetComponent<KBatchedAnimController>();
			component.enabled = true;
			component.SetSceneLayer(Grid.SceneLayer.BuildingUse);
			KSelectable component2 = display_egg.GetComponent<KSelectable>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				component2.enabled = true;
			}
			tracker = display_egg.AddComponent<KBatchedAnimTracker>();
			tracker.symbol = "snapto_egg";
		}
	}
}
