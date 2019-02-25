using STRINGS;
using UnityEngine;

public class CaptureTool : DragTool
{
	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		Vector2 regularizedPos = GetRegularizedPos(Vector2.Min(downPos, upPos), true);
		Vector2 regularizedPos2 = GetRegularizedPos(Vector2.Max(downPos, upPos), false);
		MarkForCapture(regularizedPos, regularizedPos2, true);
	}

	public static void MarkForCapture(Vector2 min, Vector2 max, bool mark)
	{
		foreach (Capturable item in Components.Capturables.Items)
		{
			Vector2 vector = Grid.PosToXY(item.transform.GetPosition());
			if (vector.x >= min.x && vector.x < max.x && vector.y >= min.y && vector.y < max.y)
			{
				if (item.allowCapture)
				{
					PrioritySetting lastSelectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
					item.MarkForCapture(mark, lastSelectedPriority);
				}
				else if (mark)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.TOOLS.CAPTURE.NOT_CAPTURABLE, null, item.transform.GetPosition(), 1.5f, false, false);
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
