using System.Collections.Generic;
using UnityEngine;

public class EmptyPipeTool : FilteredDragTool
{
	public static EmptyPipeTool Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		for (int i = 0; i < 37; i++)
		{
			if (IsActiveLayer((ObjectLayer)i))
			{
				GameObject gameObject = Grid.Objects[cell, i];
				if (!((Object)gameObject == (Object)null))
				{
					EmptyConduitWorkable component = gameObject.GetComponent<EmptyConduitWorkable>();
					if (!((Object)component == (Object)null))
					{
						component.MarkForEmptying();
						Prioritizable component2 = gameObject.GetComponent<Prioritizable>();
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

	protected override void GetDefaultFilters(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		filters.Add(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.On);
		filters.Add(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.On);
		filters.Add(ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, ToolParameterMenu.ToggleState.On);
	}
}
