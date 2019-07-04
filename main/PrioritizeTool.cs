using System.Collections.Generic;
using UnityEngine;

public class PrioritizeTool : FilteredDragTool
{
	public GameObject Placer;

	public static PrioritizeTool Instance;

	public Texture2D[] cursors;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		interceptNumberKeysForPriority = true;
		Instance = this;
		visualizer = Util.KInstantiate(visualizer, null, null);
		viewMode = OverlayModes.Priorities.ID;
		Game.Instance.prioritizableRenderer.currentTool = this;
	}

	public override string GetFilterLayerFromGameObject(GameObject input)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if ((bool)input.GetComponent<Diggable>())
		{
			flag = true;
		}
		if ((bool)input.GetComponent<Constructable>() || ((bool)input.GetComponent<Deconstructable>() && input.GetComponent<Deconstructable>().IsMarkedForDeconstruction()))
		{
			flag2 = true;
		}
		if ((bool)input.GetComponent<Clearable>() || (bool)input.GetComponent<Moppable>() || (bool)input.GetComponent<StorageLocker>())
		{
			flag3 = true;
		}
		if (!flag2)
		{
			if (!flag)
			{
				if (!flag3)
				{
					return ToolParameterMenu.FILTERLAYERS.OPERATE;
				}
				return ToolParameterMenu.FILTERLAYERS.CLEAN;
			}
			return ToolParameterMenu.FILTERLAYERS.DIG;
		}
		return ToolParameterMenu.FILTERLAYERS.CONSTRUCTION;
	}

	protected override void GetDefaultFilters(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		filters.Add(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
		filters.Add(ToolParameterMenu.FILTERLAYERS.CONSTRUCTION, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.DIG, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.CLEAN, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.OPERATE, ToolParameterMenu.ToggleState.Off);
	}

	private bool TryPrioritizeGameObject(GameObject target, PrioritySetting priority)
	{
		string filterLayerFromGameObject = GetFilterLayerFromGameObject(target);
		if (IsActiveLayer(filterLayerFromGameObject))
		{
			Prioritizable component = target.GetComponent<Prioritizable>();
			if ((Object)component != (Object)null && component.showIcon && component.IsPrioritizable())
			{
				component.SetMasterPriority(priority);
				return true;
			}
		}
		return false;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		PrioritySetting lastSelectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
		int num = 0;
		for (int i = 0; i < 39; i++)
		{
			GameObject gameObject = Grid.Objects[cell, i];
			if ((Object)gameObject != (Object)null)
			{
				if ((bool)gameObject.GetComponent<Pickupable>())
				{
					ObjectLayerListItem objectLayerListItem = gameObject.GetComponent<Pickupable>().objectLayerListItem;
					while (objectLayerListItem != null)
					{
						GameObject gameObject2 = objectLayerListItem.gameObject;
						objectLayerListItem = objectLayerListItem.nextItem;
						if (!((Object)gameObject2 == (Object)null) && !((Object)gameObject2.GetComponent<MinionIdentity>() != (Object)null) && TryPrioritizeGameObject(gameObject2, lastSelectedPriority))
						{
							num++;
						}
					}
				}
				else if (TryPrioritizeGameObject(gameObject, lastSelectedPriority))
				{
					num++;
				}
			}
		}
		if (num > 0)
		{
			PriorityScreen.PlayPriorityConfirmSound(lastSelectedPriority);
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.ShowDiagram(true);
		ToolMenu.Instance.PriorityScreen.Show(true);
		ToolMenu.Instance.PriorityScreen.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(false);
		ToolMenu.Instance.PriorityScreen.ShowDiagram(false);
		ToolMenu.Instance.PriorityScreen.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void Update()
	{
		PrioritySetting lastSelectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
		int num = 0;
		if (lastSelectedPriority.priority_class >= PriorityScreen.PriorityClass.high)
		{
			num += 9;
		}
		if (lastSelectedPriority.priority_class >= PriorityScreen.PriorityClass.topPriority)
		{
			num = num;
		}
		num += lastSelectedPriority.priority_value;
		Texture2D mainTexture = cursors[num - 1];
		MeshRenderer componentInChildren = visualizer.GetComponentInChildren<MeshRenderer>();
		if ((Object)componentInChildren != (Object)null)
		{
			componentInChildren.material.mainTexture = mainTexture;
		}
	}
}
