using UnityEngine;

public class OffsetTracker
{
	public static bool isExecutingWithinJob;

	protected CellOffset[] offsets;

	private int previousCell = Grid.InvalidCell;

	public virtual CellOffset[] GetOffsets(int current_cell)
	{
		if (current_cell != previousCell)
		{
			UpdateCell(previousCell, current_cell);
			previousCell = current_cell;
		}
		if (offsets == null)
		{
			UpdateOffsets(previousCell);
		}
		return offsets;
	}

	public void ForceRefresh()
	{
		int cell = previousCell;
		previousCell = Grid.InvalidCell;
		Refresh(cell);
	}

	public void Refresh(int cell)
	{
		GetOffsets(cell);
	}

	protected virtual void UpdateCell(int previous_cell, int current_cell)
	{
	}

	protected virtual void UpdateOffsets(int current_cell)
	{
	}

	public virtual void Clear()
	{
	}

	public virtual void DebugDrawExtents()
	{
	}

	public void DebugDrawOffsets(int cell)
	{
		CellOffset[] array = GetOffsets(cell);
		foreach (CellOffset offset in array)
		{
			int cell2 = Grid.OffsetCell(cell, offset);
			Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
			Gizmos.DrawCube(Grid.CellToPosCCC(cell2, Grid.SceneLayer.Move), new Vector3(1f, 1f, 1f));
		}
	}
}
