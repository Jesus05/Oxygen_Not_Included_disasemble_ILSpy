using System.Collections.Generic;
using UnityEngine;

public class CancelTool : FilteredDragTool
{
	public static CancelTool Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void GetDefaultFilters(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		base.GetDefaultFilters(filters);
		filters.Add(ToolParameterMenu.FILTERLAYERS.CLEANANDCLEAR, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.DIGPLACER, ToolParameterMenu.ToggleState.Off);
	}

	protected override string GetConfirmSound()
	{
		return "Tile_Confirm_NegativeTool";
	}

	protected override string GetDragSound()
	{
		return "Tile_Drag_NegativeTool";
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		for (int i = 0; i < 39; i++)
		{
			GameObject gameObject = Grid.Objects[cell, i];
			if ((Object)gameObject != (Object)null)
			{
				string filterLayerFromGameObject = GetFilterLayerFromGameObject(gameObject);
				if (IsActiveLayer(filterLayerFromGameObject))
				{
					gameObject.Trigger(2127324410, null);
				}
			}
		}
	}

	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		Vector2 regularizedPos = GetRegularizedPos(Vector2.Min(downPos, upPos), true);
		Vector2 regularizedPos2 = GetRegularizedPos(Vector2.Max(downPos, upPos), false);
		AttackTool.MarkForAttack(regularizedPos, regularizedPos2, false);
		CaptureTool.MarkForCapture(regularizedPos, regularizedPos2, false);
	}
}
