using System;

public class LiquidFetchMask
{
	private bool[] isLiquidAvailable;

	private CellOffset maxOffset;

	public LiquidFetchMask(CellOffset[][] offset_table)
	{
		for (int i = 0; i < offset_table.Length; i++)
		{
			for (int j = 0; j < offset_table[i].Length; j++)
			{
				maxOffset.x = Math.Max(maxOffset.x, Math.Abs(offset_table[i][j].x));
				maxOffset.y = Math.Max(maxOffset.y, Math.Abs(offset_table[i][j].y));
			}
		}
		isLiquidAvailable = new bool[Grid.CellCount];
		for (int k = 0; k < Grid.CellCount; k++)
		{
			RefreshCell(k);
		}
	}

	private void RefreshCell(int cell)
	{
		CellOffset offset = Grid.GetOffset(cell);
		for (int i = Math.Max(0, offset.y - maxOffset.y); i < Grid.HeightInCells && i < offset.y + maxOffset.y; i++)
		{
			for (int j = Math.Max(0, offset.x - maxOffset.x); j < Grid.WidthInCells && j < offset.x + maxOffset.x; j++)
			{
				if (Grid.Element[Grid.XYToCell(j, i)].IsLiquid)
				{
					isLiquidAvailable[cell] = true;
					return;
				}
			}
		}
		isLiquidAvailable[cell] = false;
	}

	public void MarkDirty(int cell)
	{
		RefreshCell(cell);
	}

	public bool IsLiquidAvailable(int cell)
	{
		return isLiquidAvailable[cell];
	}

	public void Destroy()
	{
		isLiquidAvailable = null;
	}
}
