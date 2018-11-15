using UnityEngine;

public class AttackTool : DragTool
{
	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		Vector2 regularizedPos = GetRegularizedPos(Vector2.Min(downPos, upPos), true);
		Vector2 regularizedPos2 = GetRegularizedPos(Vector2.Max(downPos, upPos), false);
		MarkForAttack(regularizedPos, regularizedPos2, true);
	}

	public static void MarkForAttack(Vector2 min, Vector2 max, bool mark)
	{
		foreach (FactionAlignment item in Components.FactionAlignments.Items)
		{
			Vector2 vector = Grid.PosToXY(item.transform.GetPosition());
			if (vector.x >= min.x && vector.x < max.x && vector.y >= min.y && vector.y < max.y)
			{
				if (mark)
				{
					if (FactionManager.Instance.GetDisposition(FactionManager.FactionID.Duplicant, item.Alignment) != 0)
					{
						item.SetPlayerTargeted(true);
					}
				}
				else
				{
					item.gameObject.Trigger(2127324410, null);
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
