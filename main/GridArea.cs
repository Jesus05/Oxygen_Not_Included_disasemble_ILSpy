using System;
using UnityEngine;

public struct GridArea
{
	private Vector2I min;

	private Vector2I max;

	private int MinCell;

	private int MaxCell;

	public Vector2I Min => min;

	public Vector2I Max => max;

	public void SetArea(int cell, int width, int height)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2I vector2I2 = new Vector2I(vector2I.x + width, vector2I.y + height);
		SetExtents(vector2I.x, vector2I.y, vector2I2.x, vector2I2.y);
	}

	public void SetExtents(int min_x, int min_y, int max_x, int max_y)
	{
		min.x = Math.Max(min_x, 0);
		min.y = Math.Max(min_y, 0);
		max.x = Math.Min(max_x, Grid.WidthInCells);
		max.y = Math.Min(max_y, Grid.HeightInCells);
		MinCell = Grid.XYToCell(min.x, min.y);
		MaxCell = Grid.XYToCell(max.x, max.y);
	}

	public bool Contains(int cell)
	{
		if (cell >= MinCell && cell < MaxCell)
		{
			int num = cell % Grid.WidthInCells;
			int num2 = num;
			Vector2I vector2I = Min;
			int result;
			if (num2 >= vector2I.x)
			{
				int num3 = num;
				Vector2I vector2I2 = Max;
				result = ((num3 < vector2I2.x) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			return (byte)result != 0;
		}
		return false;
	}

	public bool Contains(int x, int y)
	{
		return x >= min.x && x < max.x && y >= min.y && y < max.y;
	}

	public bool Contains(Vector3 pos)
	{
		return (float)min.x <= pos.x && pos.x < (float)max.x && (float)min.y <= pos.y && pos.y <= (float)max.y;
	}

	public void RunIfInside(int cell, Action<int> action)
	{
		if (Contains(cell))
		{
			action(cell);
		}
	}

	public void Run(Action<int> action)
	{
		for (int i = min.y; i < max.y; i++)
		{
			for (int j = min.x; j < max.x; j++)
			{
				int obj = Grid.XYToCell(j, i);
				action(obj);
			}
		}
	}

	public void RunOnDifference(GridArea subtract_area, Action<int> action)
	{
		for (int i = min.y; i < max.y; i++)
		{
			for (int j = min.x; j < max.x; j++)
			{
				if (!subtract_area.Contains(j, i))
				{
					int obj = Grid.XYToCell(j, i);
					action(obj);
				}
			}
		}
	}

	public int GetCellCount()
	{
		return (max.x - min.x) * (max.y - min.y);
	}
}
