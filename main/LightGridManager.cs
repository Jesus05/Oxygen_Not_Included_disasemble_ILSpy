using System.Collections.Generic;
using UnityEngine;

public static class LightGridManager
{
	public class LightGridEmitter
	{
		public int cell = -1;

		public LightShape shape;

		public float radius = 4f;

		public int intensity = 1;

		public Color colour = Color.white;

		public float falloffRate = 0.5f;

		private List<int> litCells;

		public LightGridEmitter(int cell, List<int> lit_cells, int intensity, float radius, Color colour, LightShape shape, float falloffRate = 0.5f)
		{
			this.cell = cell;
			this.radius = radius;
			this.intensity = intensity;
			this.colour = colour;
			this.shape = shape;
			litCells = lit_cells;
			this.falloffRate = falloffRate;
		}

		public void Add()
		{
			Remove();
			DiscreteShadowCaster.GetVisibleCells(cell, litCells, (int)radius, shape);
			for (int i = 0; i < litCells.Count; i++)
			{
				int num = litCells[i];
				int num2 = Mathf.Max(1, Mathf.RoundToInt(falloffRate * (float)Mathf.Max(Grid.GetCellDistance(num, cell), 1)));
				int num3 = Mathf.Max(0, Grid.LightCount[num] + intensity / num2);
				Grid.LightCount[num] = num3;
				previewLux[num] = num3;
			}
		}

		public void Remove()
		{
			for (int i = 0; i < litCells.Count; i++)
			{
				int num = litCells[i];
				int num2 = CalculateFalloff(falloffRate, num, cell);
				Grid.LightCount[num] = Mathf.Max(0, Grid.LightCount[num] - intensity / num2);
				previewLux[num] = 0;
			}
			litCells.Clear();
		}
	}

	public static List<Tuple<int, int>> previewLightCells = new List<Tuple<int, int>>();

	public static int[] previewLux;

	private static int CalculateFalloff(float falloffRate, int cell, int origin)
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
		ListPool<int, LightGridEmitter>.PooledList pooledList = ListPool<int, LightGridEmitter>.Allocate();
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
