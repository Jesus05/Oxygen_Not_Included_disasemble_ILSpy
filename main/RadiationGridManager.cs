using System.Collections.Generic;
using UnityEngine;

public static class RadiationGridManager
{
	public static List<Tuple<int, int>> previewLightCells = new List<Tuple<int, int>>();

	public static int[] previewLux;

	public static int CalculateFalloff(float falloffRate, int cell, int origin)
	{
		return Mathf.Max(1, Mathf.RoundToInt(falloffRate * (float)Mathf.Max(Grid.GetCellDistance(origin, cell), 1)));
	}

	public static void Initialise()
	{
		previewLux = new int[Grid.CellCount];
	}

	public static void Shutdown()
	{
		previewLux = null;
		previewLightCells.Clear();
	}

	public static void DestroyPreview()
	{
		foreach (Tuple<int, int> previewLightCell in previewLightCells)
		{
			previewLux[previewLightCell.first] = 0;
		}
		previewLightCells.Clear();
	}

	public static void CreatePreview(int origin_cell, float radius, LightShape shape, int lux)
	{
		previewLightCells.Clear();
		ListPool<int, RadiationGridEmitter>.PooledList pooledList = ListPool<int, RadiationGridEmitter>.Allocate();
		pooledList.Add(origin_cell);
		DiscreteShadowCaster.GetVisibleCells(origin_cell, pooledList, (int)radius, shape);
		int num = 0;
		foreach (int item in pooledList)
		{
			if (Grid.IsValidCell(item))
			{
				num = lux / CalculateFalloff(0.5f, item, origin_cell);
				previewLightCells.Add(new Tuple<int, int>(item, num));
				previewLux[item] = num;
			}
		}
		pooledList.Recycle();
	}
}
