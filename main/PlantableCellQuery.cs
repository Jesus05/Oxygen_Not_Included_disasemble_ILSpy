using System.Collections.Generic;
using UnityEngine;

public class PlantableCellQuery : PathFinderQuery
{
	public List<int> result_cells = new List<int>();

	private Element plantElement = ElementLoader.FindElementByHash(SimHashes.Dirt);

	private PlantableSeed seed;

	private int max_results;

	public PlantableCellQuery Reset(PlantableSeed seed, int max_results)
	{
		this.seed = seed;
		this.max_results = max_results;
		result_cells.Clear();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!result_cells.Contains(cell) && CheckValidPlotCell(seed, cell))
		{
			result_cells.Add(cell);
		}
		return result_cells.Count >= max_results;
	}

	private bool CheckValidPlotCell(PlantableSeed seed, int cell)
	{
		if (Grid.IsValidCell(cell))
		{
			int num = (seed.Direction != SingleEntityReceptacle.ReceptacleDirection.Bottom) ? Grid.CellBelow(cell) : Grid.CellAbove(cell);
			if (Grid.IsValidCell(num))
			{
				if (Grid.Solid[num])
				{
					if (!(bool)Grid.Objects[cell, 5])
					{
						if (!(bool)Grid.Objects[cell, 1])
						{
							GameObject gameObject = Grid.Objects[num, 1];
							if ((bool)gameObject)
							{
								PlantablePlot component = gameObject.GetComponent<PlantablePlot>();
								if ((Object)component == (Object)null)
								{
									return false;
								}
								if (component.Direction != seed.Direction)
								{
									return false;
								}
								if ((Object)component.Occupant != (Object)null)
								{
									return false;
								}
							}
							else if (Grid.Element[num] != plantElement)
							{
								return false;
							}
							return true;
						}
						return false;
					}
					return false;
				}
				return false;
			}
			return false;
		}
		return false;
	}
}
