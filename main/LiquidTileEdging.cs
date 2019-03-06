using System;
using UnityEngine;

public class LiquidTileEdging
{
	private void Update()
	{
		Grid.GetVisibleExtents(out int min_x, out int min_y, out int max_x, out int max_y);
		min_x = Math.Max(0, min_x);
		min_y = Math.Max(0, min_y);
		min_x = Mathf.Min(min_x, Grid.WidthInCells - 1);
		min_y = Mathf.Min(min_y, Grid.HeightInCells - 1);
		max_x = Mathf.CeilToInt((float)max_x);
		max_y = Mathf.CeilToInt((float)max_y);
		max_x = Mathf.Max(max_x, 0);
		max_y = Mathf.Max(max_y, 0);
		max_x = Mathf.Min(max_x, Grid.WidthInCells - 1);
		max_y = Mathf.Min(max_y, Grid.HeightInCells - 1);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = min_y; i < max_y; i++)
		{
			for (int j = min_x; j < max_x; j++)
			{
				int num4 = i * Grid.WidthInCells + j;
				Element element = Grid.Element[num4];
				if (element.IsSolid)
				{
					num++;
				}
				else if (element.IsLiquid)
				{
					num2++;
				}
				else
				{
					num3++;
				}
			}
		}
	}
}
