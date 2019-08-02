using System.Collections.Generic;
using UnityEngine;

public class RadiationGridEmitter
{
	public int cell = -1;

	public LightShape shape;

	public float radius = 4f;

	public int intensity = 1;

	public float falloffRate = 0.5f;

	private List<int> litCells;

	public RadiationGridEmitter(int cell, List<int> lit_cells, int intensity, float radius, LightShape shape, float falloffRate = 0.5f)
	{
		this.cell = cell;
		this.radius = radius;
		this.intensity = intensity;
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
			int num3 = Mathf.Max(0, Grid.RadiationCount[num] + intensity / num2);
			Grid.RadiationCount[num] = num3;
			RadiationGridManager.previewLux[num] = num3;
		}
	}

	public void Remove()
	{
		for (int i = 0; i < litCells.Count; i++)
		{
			int num = litCells[i];
			int num2 = RadiationGridManager.CalculateFalloff(falloffRate, num, cell);
			Grid.RadiationCount[num] = Mathf.Max(0, Grid.RadiationCount[num] - intensity / num2);
			RadiationGridManager.previewLux[num] = 0;
		}
		litCells.Clear();
	}
}
