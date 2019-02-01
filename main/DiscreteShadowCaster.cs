using System;
using System.Collections.Generic;

public static class DiscreteShadowCaster
{
	public enum Octant
	{
		N_NW,
		N_NE,
		E_NE,
		E_SE,
		S_SE,
		S_SW,
		W_SW,
		W_NW
	}

	public static void GetVisibleCells(int cell, List<int> visiblePoints, int range, LightShape shape)
	{
		visiblePoints.Add(cell);
		Vector2I cellPos = Grid.CellToXY(cell);
		switch (shape)
		{
		case LightShape.Circle:
			ScanOctant(cellPos, range, 1, Octant.N_NW, 1.0, 0.0, visiblePoints);
			ScanOctant(cellPos, range, 1, Octant.N_NE, 1.0, 0.0, visiblePoints);
			ScanOctant(cellPos, range, 1, Octant.E_NE, 1.0, 0.0, visiblePoints);
			ScanOctant(cellPos, range, 1, Octant.E_SE, 1.0, 0.0, visiblePoints);
			ScanOctant(cellPos, range, 1, Octant.S_SE, 1.0, 0.0, visiblePoints);
			ScanOctant(cellPos, range, 1, Octant.S_SW, 1.0, 0.0, visiblePoints);
			ScanOctant(cellPos, range, 1, Octant.W_SW, 1.0, 0.0, visiblePoints);
			ScanOctant(cellPos, range, 1, Octant.W_NW, 1.0, 0.0, visiblePoints);
			break;
		case LightShape.Cone:
			ScanOctant(cellPos, range, 1, Octant.S_SE, 1.0, 0.0, visiblePoints);
			ScanOctant(cellPos, range, 1, Octant.S_SW, 1.0, 0.0, visiblePoints);
			break;
		}
	}

	private static bool DoesOcclude(int x, int y)
	{
		int num = Grid.XYToCell(x, y);
		return Grid.IsValidCell(num) && !Grid.Transparent[num] && Grid.Solid[num];
	}

	private static void ScanOctant(Vector2I cellPos, int range, int depth, Octant octant, double startSlope, double endSlope, List<int> visiblePoints)
	{
		int num = range * range;
		int num2 = 0;
		int num3 = 0;
		switch (octant)
		{
		case Octant.S_SW:
			num3 = cellPos.y - depth;
			if (num3 < 0)
			{
				return;
			}
			num2 = cellPos.x - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num2 < 0)
			{
				num2 = 0;
			}
			for (; GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, false) >= endSlope; num2++)
			{
				if (GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DoesOcclude(num2, num3))
					{
						if (num2 - 1 >= 0 && !DoesOcclude(num2 - 1, num3) && !DoesOcclude(num2 - 1, num3 + 1))
						{
							double slope = GetSlope((double)num2 - 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, false);
							ScanOctant(cellPos, range, depth + 1, octant, startSlope, slope, visiblePoints);
						}
					}
					else
					{
						if (num2 - 1 >= 0 && DoesOcclude(num2 - 1, num3))
						{
							startSlope = GetSlope((double)num2 - 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, false);
						}
						if (!DoesOcclude(num2, num3 + 1) && !visiblePoints.Contains(Grid.XYToCell(num2, num3)))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
			}
			num2--;
			break;
		case Octant.S_SE:
			num3 = cellPos.y - depth;
			if (num3 < 0)
			{
				return;
			}
			num2 = cellPos.x + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num2 >= Grid.WidthInCells)
			{
				num2 = Grid.WidthInCells - 1;
			}
			while (GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, false) <= endSlope)
			{
				if (GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DoesOcclude(num2, num3))
					{
						if (num2 + 1 < Grid.WidthInCells && !DoesOcclude(num2 + 1, num3) && !DoesOcclude(num2 + 1, num3 + 1))
						{
							double slope3 = GetSlope((double)num2 + 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, false);
							ScanOctant(cellPos, range, depth + 1, octant, startSlope, slope3, visiblePoints);
						}
					}
					else
					{
						if (num2 + 1 < Grid.WidthInCells && DoesOcclude(num2 + 1, num3))
						{
							startSlope = 0.0 - GetSlope((double)num2 + 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, false);
						}
						if (!DoesOcclude(num2, num3 + 1) && !visiblePoints.Contains(Grid.XYToCell(num2, num3)))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num2--;
			}
			num2++;
			break;
		case Octant.E_SE:
			num2 = cellPos.x + depth;
			if (num2 >= Grid.WidthInCells)
			{
				return;
			}
			num3 = cellPos.y - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num3 < 0)
			{
				num3 = 0;
			}
			for (; GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, true) <= endSlope; num3++)
			{
				if (GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DoesOcclude(num2, num3))
					{
						if (num3 - 1 >= 0 && !DoesOcclude(num2, num3 - 1) && !DoesOcclude(num2 - 1, num3 - 1))
						{
							ScanOctant(cellPos, range, depth + 1, octant, startSlope, GetSlope((double)num2 - 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, true), visiblePoints);
						}
					}
					else
					{
						if (num3 - 1 >= 0 && DoesOcclude(num2, num3 - 1))
						{
							startSlope = 0.0 - GetSlope((double)num2 + 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, true);
						}
						if (!DoesOcclude(num2 - 1, num3) && !visiblePoints.Contains(Grid.XYToCell(num2, num3)))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
			}
			num3--;
			break;
		case Octant.E_NE:
			num2 = cellPos.x + depth;
			if (num2 >= Grid.WidthInCells)
			{
				return;
			}
			num3 = cellPos.y + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num3 >= Grid.HeightInCells)
			{
				num3 = Grid.HeightInCells - 1;
			}
			while (GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, true) >= endSlope)
			{
				if (GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DoesOcclude(num2, num3))
					{
						if (num3 + 1 < Grid.HeightInCells && !DoesOcclude(num2, num3 + 1) && !DoesOcclude(num2 - 1, num3 + 1))
						{
							ScanOctant(cellPos, range, depth + 1, octant, startSlope, GetSlope((double)num2 - 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, true), visiblePoints);
						}
					}
					else
					{
						if (num3 + 1 < Grid.HeightInCells && DoesOcclude(num2, num3 + 1))
						{
							startSlope = GetSlope((double)num2 + 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, true);
						}
						if (!DoesOcclude(num2 - 1, num3) && !visiblePoints.Contains(Grid.XYToCell(num2, num3)))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num3--;
			}
			num3++;
			break;
		case Octant.N_NE:
			num3 = cellPos.y + depth;
			if (num3 >= Grid.HeightInCells)
			{
				return;
			}
			num2 = cellPos.x + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num2 >= Grid.WidthInCells)
			{
				num2 = Grid.WidthInCells - 1;
			}
			while (GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, false) >= endSlope)
			{
				if (GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DoesOcclude(num2, num3))
					{
						if (num2 + 1 < Grid.HeightInCells && !DoesOcclude(num2 + 1, num3) && !DoesOcclude(num2 + 1, num3 - 1))
						{
							double slope2 = GetSlope((double)num2 + 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, false);
							ScanOctant(cellPos, range, depth + 1, octant, startSlope, slope2, visiblePoints);
						}
					}
					else
					{
						if (num2 + 1 < Grid.HeightInCells && DoesOcclude(num2 + 1, num3))
						{
							startSlope = GetSlope((double)num2 + 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, false);
						}
						if (!DoesOcclude(num2, num3 - 1) && !visiblePoints.Contains(Grid.XYToCell(num2, num3)))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num2--;
			}
			num2++;
			break;
		case Octant.N_NW:
			num3 = cellPos.y + depth;
			if (num3 >= Grid.HeightInCells)
			{
				return;
			}
			num2 = cellPos.x - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num2 < 0)
			{
				num2 = 0;
			}
			for (; GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, false) <= endSlope; num2++)
			{
				if (GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DoesOcclude(num2, num3))
					{
						if (num2 - 1 >= 0 && !DoesOcclude(num2 - 1, num3) && !DoesOcclude(num2 - 1, num3 - 1))
						{
							ScanOctant(cellPos, range, depth + 1, octant, startSlope, GetSlope((double)num2 - 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, false), visiblePoints);
						}
					}
					else
					{
						if (num2 - 1 >= 0 && DoesOcclude(num2 - 1, num3))
						{
							startSlope = 0.0 - GetSlope((double)num2 - 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, false);
						}
						if (!DoesOcclude(num2, num3 - 1) && !visiblePoints.Contains(Grid.XYToCell(num2, num3)))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
			}
			num2--;
			break;
		case Octant.W_NW:
			num2 = cellPos.x - depth;
			if (num2 < 0)
			{
				return;
			}
			num3 = cellPos.y + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num3 >= Grid.HeightInCells)
			{
				num3 = Grid.HeightInCells - 1;
			}
			while (GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, true) <= endSlope)
			{
				if (GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DoesOcclude(num2, num3))
					{
						if (num3 + 1 < Grid.HeightInCells && !DoesOcclude(num2, num3 + 1) && !DoesOcclude(num2 + 1, num3 + 1))
						{
							ScanOctant(cellPos, range, depth + 1, octant, startSlope, GetSlope((double)num2 + 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, true), visiblePoints);
						}
					}
					else
					{
						if (num3 + 1 < Grid.HeightInCells && DoesOcclude(num2, num3 + 1))
						{
							startSlope = 0.0 - GetSlope((double)num2 - 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, true);
						}
						if (!DoesOcclude(num2 + 1, num3) && !visiblePoints.Contains(Grid.XYToCell(num2, num3)))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num3--;
			}
			num3++;
			break;
		case Octant.W_SW:
			num2 = cellPos.x - depth;
			if (num2 < 0)
			{
				return;
			}
			num3 = cellPos.y - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num3 < 0)
			{
				num3 = 0;
			}
			for (; GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, true) >= endSlope; num3++)
			{
				if (GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DoesOcclude(num2, num3))
					{
						if (num3 - 1 >= 0 && !DoesOcclude(num2, num3 - 1) && !DoesOcclude(num2 + 1, num3 - 1))
						{
							ScanOctant(cellPos, range, depth + 1, octant, startSlope, GetSlope((double)num2 + 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, true), visiblePoints);
						}
					}
					else
					{
						if (num3 - 1 >= 0 && DoesOcclude(num2, num3 - 1))
						{
							startSlope = GetSlope((double)num2 - 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, true);
						}
						if (!DoesOcclude(num2 + 1, num3) && !visiblePoints.Contains(Grid.XYToCell(num2, num3)))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
			}
			num3--;
			break;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		else if (num2 >= Grid.WidthInCells)
		{
			num2 = Grid.WidthInCells - 1;
		}
		if (num3 < 0)
		{
			num3 = 0;
		}
		else if (num3 >= Grid.HeightInCells)
		{
			num3 = Grid.HeightInCells - 1;
		}
		if ((depth < range) & !DoesOcclude(num2, num3))
		{
			ScanOctant(cellPos, range, depth + 1, octant, startSlope, endSlope, visiblePoints);
		}
	}

	private static double GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert)
	{
		if (!pInvert)
		{
			return (pX1 - pX2) / (pY1 - pY2);
		}
		return (pY1 - pY2) / (pX1 - pX2);
	}

	private static int GetVisDistance(int pX1, int pY1, int pX2, int pY2)
	{
		return (pX1 - pX2) * (pX1 - pX2) + (pY1 - pY2) * (pY1 - pY2);
	}
}
