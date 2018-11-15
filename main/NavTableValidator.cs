using System;

public class NavTableValidator
{
	public Action<int> onDirty;

	protected bool IsClear(int cell, CellOffset[] bounding_offsets, ushort[] grid_bit_fields, bool allow_forcefield_traversal)
	{
		foreach (CellOffset offset in bounding_offsets)
		{
			int cell2 = Grid.OffsetCell(cell, offset);
			if (!Grid.IsValidCell(cell2) || IsCellSolid(grid_bit_fields, cell2, allow_forcefield_traversal))
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

	protected static bool IsCellSolid(ushort[] grid_bit_fields, int cell, bool allow_forcefield_traversal)
	{
		ushort num = grid_bit_fields[cell];
		bool flag = (num & 0x20) != 0;
		bool flag2 = (num & 4) != 0;
		bool flag3 = (num & 0x100) != 0;
		return (flag || flag3) && (!flag2 || !allow_forcefield_traversal);
	}

	public virtual void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
	{
	}

	public virtual void Clear()
	{
	}
}
