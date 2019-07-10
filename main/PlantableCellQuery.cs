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
							else
							{
								if (Grid.Element[num] != plantElement)
								{
									return false;
								}
								if (CountNearbyPlants(num, 6) > 2)
								{
									return false;
								}
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

	private static int CountNearbyPlants(int cell, int radius)
	{
		int x = 0;
		int y = 0;
		Grid.PosToXY(Grid.CellToPos(cell), out x, out y);
		int num = radius * 2;
		x -= radius;
		y -= radius;
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(x, y, num, num, GameScenePartitioner.Instance.plants, pooledList);
		int num2 = 0;
		foreach (ScenePartitionerEntry item in pooledList)
		{
			KPrefabID kPrefabID = (KPrefabID)item.obj;
			if (!(bool)kPrefabID.GetComponent<TreeBud>())
			{
				num2++;
			}
		}
		pooledList.Recycle();
		return num2;
	}
}
