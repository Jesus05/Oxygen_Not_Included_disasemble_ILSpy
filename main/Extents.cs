using System;

public struct Extents
{
	public int x;

	public int y;

	public int width;

	public int height;

	public Extents(int x, int y, int width, int height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public Extents(int cell, int radius)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		x = num - radius;
		y = num2 - radius;
		width = radius * 2 + 1;
		height = radius * 2 + 1;
	}

	public Extents(int center_x, int center_y, int radius)
	{
		x = center_x - radius;
		y = center_y - radius;
		width = radius * 2 + 1;
		height = radius * 2 + 1;
	}

	public Extents(int cell, CellOffset[] offsets)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		int num3 = num;
		int num4 = num2;
		foreach (CellOffset offset in offsets)
		{
			int val = 0;
			int val2 = 0;
			int cell2 = Grid.OffsetCell(cell, offset);
			Grid.CellToXY(cell2, out val, out val2);
			num = Math.Min(num, val);
			num2 = Math.Min(num2, val2);
			num3 = Math.Max(num3, val);
			num4 = Math.Max(num4, val2);
		}
		x = num;
		y = num2;
		width = num3 - num + 1;
		height = num4 - num2 + 1;
	}

	public Extents(int cell, CellOffset[] offsets, Orientation orientation)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		int num3 = num;
		int num4 = num2;
		for (int i = 0; i < offsets.Length; i++)
		{
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offsets[i], orientation);
			int val = 0;
			int val2 = 0;
			int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
			Grid.CellToXY(cell2, out val, out val2);
			num = Math.Min(num, val);
			num2 = Math.Min(num2, val2);
			num3 = Math.Max(num3, val);
			num4 = Math.Max(num4, val2);
		}
		x = num;
		y = num2;
		width = num3 - num + 1;
		height = num4 - num2 + 1;
	}

	public Extents(int cell, CellOffset[][] offset_table)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		int num3 = num;
		int num4 = num2;
		foreach (CellOffset[] array in offset_table)
		{
			int val = 0;
			int val2 = 0;
			int cell2 = Grid.OffsetCell(cell, array[0]);
			Grid.CellToXY(cell2, out val, out val2);
			num = Math.Min(num, val);
			num2 = Math.Min(num2, val2);
			num3 = Math.Max(num3, val);
			num4 = Math.Max(num4, val2);
		}
		x = num;
		y = num2;
		width = num3 - num + 1;
		height = num4 - num2 + 1;
	}

	public static Extents OneCell(int cell)
	{
		Grid.CellToXY(cell, out int num, out int num2);
		return new Extents(num, num2, 1, 1);
	}

	public bool Contains(Vector2I pos)
	{
		return x <= pos.x && pos.x < x + width && y <= pos.y && pos.y < y + height;
	}
}
