using System.Collections.Generic;
using UnityEngine;

public class HarvestTool : DragTool
{
	public GameObject Placer;

	public static HarvestTool Instance;

	public Texture2D[] visualizerTextures;

	private Dictionary<string, ToolParameterMenu.ToggleState> options = new Dictionary<string, ToolParameterMenu.ToggleState>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		options.Add("HARVEST_WHEN_READY", ToolParameterMenu.ToggleState.On);
		options.Add("DO_NOT_HARVEST", ToolParameterMenu.ToggleState.Off);
		viewMode = SimViewMode.HarvestWhenReady;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (Grid.IsValidCell(cell))
		{
			foreach (Harvestable item in Components.Harvestables.Items)
			{
				OccupyArea area = item.area;
				if (Grid.PosToCell(item) == cell || ((Object)area != (Object)null && area.CheckIsOccupying(cell)))
				{
					if (options["HARVEST_WHEN_READY"] == ToolParameterMenu.ToggleState.On)
					{
						item.SetHarvestWhenReady(true);
					}
					else if (options["DO_NOT_HARVEST"] == ToolParameterMenu.ToggleState.On)
					{
						item.SetHarvestWhenReady(false);
					}
					Prioritizable component = item.GetComponent<Prioritizable>();
					if ((Object)component != (Object)null)
					{
						component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
					}
				}
			}
		}
	}

	public void Update()
	{
		MeshRenderer componentInChildren = visualizer.GetComponentInChildren<MeshRenderer>();
		if ((Object)componentInChildren != (Object)null)
		{
			if (options["HARVEST_WHEN_READY"] == ToolParameterMenu.ToggleState.On)
			{
				componentInChildren.material.mainTexture = visualizerTextures[0];
			}
			else if (options["DO_NOT_HARVEST"] == ToolParameterMenu.ToggleState.On)
			{
				componentInChildren.material.mainTexture = visualizerTextures[1];
			}
		}
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		base.OnLeftClickUp(cursor_pos);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.Show(true);
		ToolMenu.Instance.toolParameterMenu.PopulateMenu(options);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(false);
		ToolMenu.Instance.toolParameterMenu.ClearMenu();
	}
}
