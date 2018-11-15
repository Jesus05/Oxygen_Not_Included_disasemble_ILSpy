using UnityEngine;

public class ClearTool : DragTool
{
	public static ClearTool Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		interceptNumberKeysForPriority = true;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		GameObject gameObject = Grid.Objects[cell, 3];
		if (!((Object)gameObject == (Object)null))
		{
			ObjectLayerListItem objectLayerListItem = gameObject.GetComponent<Pickupable>().objectLayerListItem;
			while (objectLayerListItem != null)
			{
				GameObject gameObject2 = objectLayerListItem.gameObject;
				objectLayerListItem = objectLayerListItem.nextItem;
				if (!((Object)gameObject2 == (Object)null) && !((Object)gameObject2.GetComponent<MinionIdentity>() != (Object)null))
				{
					Clearable component = gameObject2.GetComponent<Clearable>();
					if (component.isClearable)
					{
						gameObject2.GetComponent<Clearable>().MarkForClear(false);
						Prioritizable component2 = gameObject2.GetComponent<Prioritizable>();
						if ((Object)component2 != (Object)null)
						{
							component2.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
						}
					}
				}
			}
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.Show(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(false);
	}
}
