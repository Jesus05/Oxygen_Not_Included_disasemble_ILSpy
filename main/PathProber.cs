using System.Collections.Generic;

[SkipSaveFileSerialization]
public class PathProber : KMonoBehaviour
{
	public const int InvalidHandle = -1;

	public const int InvalidIdx = -1;

	public const int InvalidCell = -1;

	public const int InvalidCost = -1;

	private PathGrid PathGrid;

	private PathFinder.PotentialList Potentials = new PathFinder.PotentialList();

	public int updateCount = -1;

	private const int updateCountThreshold = 25;

	private PathFinder.PotentialScratchPad scratchPad;

	public int potentialCellsPerUpdate = -1;

	protected override void OnCleanUp()
	{
		if (PathGrid != null)
		{
			PathGrid.OnCleanUp();
		}
		base.OnCleanUp();
	}

	public void SetGroupProber(IGroupProber group_prober)
	{
		PathGrid.SetGroupProber(group_prober);
	}

	public void SetValidNavTypes(NavType[] nav_types, int max_probing_radius)
	{
		if (max_probing_radius != 0)
		{
			PathGrid = new PathGrid(max_probing_radius * 2, max_probing_radius * 2, true, nav_types);
		}
		else
		{
			PathGrid = new PathGrid(Grid.WidthInCells, Grid.HeightInCells, false, nav_types);
		}
	}

	public int GetCost(int cell)
	{
		return PathGrid.GetCost(cell);
	}

	public int GetNavigationCostIgnoreProberOffset(int cell, CellOffset[] offsets)
	{
		return PathGrid.GetCostIgnoreProberOffset(cell, offsets);
	}

	public PathGrid GetPathGrid()
	{
		return PathGrid;
	}

	public void UpdateProbe(NavGrid nav_grid, int cell, NavType nav_type, PathFinderAbilities abilities, PathFinder.PotentialPath.Flags flags)
	{
		if (scratchPad == null)
		{
			scratchPad = new PathFinder.PotentialScratchPad(nav_grid.maxLinksPerCell);
		}
		bool flag = updateCount == -1;
		bool flag2 = Potentials.Count == 0 || flag;
		PathGrid.BeginUpdate(cell, !flag2);
		bool is_cell_in_range;
		if (flag2)
		{
			updateCount = 0;
			PathFinder.Cell cell_data = PathGrid.GetCell(cell, nav_type, out is_cell_in_range);
			PathFinder.PotentialPath potential_path = new PathFinder.PotentialPath(cell, nav_type, flags);
			PathFinder.AddPotential(potential_path, Grid.InvalidCell, NavType.NumNavTypes, 0, 0, -1, Potentials, PathGrid, ref cell_data);
		}
		int num = (potentialCellsPerUpdate > 0 && !flag) ? potentialCellsPerUpdate : 2147483647;
		updateCount++;
		while (Potentials.Count > 0 && num > 0)
		{
			KeyValuePair<int, PathFinder.PotentialPath> keyValuePair = Potentials.Next();
			num--;
			PathFinder.Cell cell2 = PathGrid.GetCell(keyValuePair.Value, out is_cell_in_range);
			if (cell2.cost == keyValuePair.Key)
			{
				PathFinder.AddPotentials(scratchPad, keyValuePair.Value, cell2.cost, cell2.underwaterCost, ref abilities, null, nav_grid.maxLinksPerCell, nav_grid.Links, Potentials, PathGrid, cell2.parent, cell2.parentNavType);
			}
		}
		bool flag3 = Potentials.Count == 0;
		PathGrid.EndUpdate(flag3);
		if (flag3 && updateCount > 25)
		{
			KProfiler.AddEvent("PathProberUpdateCountExceeded");
		}
	}
}
