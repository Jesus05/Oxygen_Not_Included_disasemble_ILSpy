using System;

public class NavTableValidator
{
	public Action<int> onDirty;

	protected bool IsClear(int cell, CellOffset[] bounding_offsets, bool allow_forcefield_traversal)
	{
		foreach (CellOffset offset in bounding_offsets)
		{
			int cell2 = Grid.OffsetCell(cell, offset);
			if (!Grid.IsValidCell(cell2) || IsCellSolid(cell2, allow_forcefield_traversal))
			{
				return false;
			}
			int num = Grid.CellAbove(cell2);
			if (Grid.IsValidCell(num) && Grid.Element[num].IsUnstable)
			{
				return false;
			}
		}
		return true;
	}

	protected static bool IsCellSolid(int cell, bool allow_forcefield_traversal)
	{
		Grid.BuildFlags buildFlags = Grid.BuildMasks[cell] & (Grid.BuildFlags.ForceField | Grid.BuildFlags.Solid | Grid.BuildFlags.Impassable);
		if (buildFlags != 0)
		{
			return (buildFlags & (Grid.BuildFlags.Solid | Grid.BuildFlags.Impassable)) != 0 && ((buildFlags & Grid.BuildFlags.ForceField) == ~(Grid.BuildFlags.FakeFloor | Grid.BuildFlags.ForceField | Grid.BuildFlags.Foundation | Grid.BuildFlags.Solid | Grid.BuildFlags.PreviousSolid | Grid.BuildFlags.Impassable | Grid.BuildFlags.LiquidPumpFloor | Grid.BuildFlags.Door) || !allow_forcefield_traversal);
		}
		return false;
	}

	public virtual void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
	{
	}

	public virtual void Clear()
	{
	}
}
