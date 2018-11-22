using System.Collections.Generic;

[SkipSaveFileSerialization]
public class PathProber : KMonoBehaviour
{
	public const int InvalidHandle = -1;

	public const int InvalidIdx = -1;

	public const int InvalidCell = -1;

	public const int InvalidCost = -1;

	public int QueryId = 1;

	private PathGrid PathGrid;

	private PathFinder.PotentialList Potentials = new PathFinder.PotentialList();

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
		return PathGrid.GetCost(cell, QueryId);
	}

	public int GetNavigationCostIgnoreProberOffset(int cell, CellOffset[] offsets)
	{
		return PathGrid.GetCostIgnoreProberOffset(cell, offsets, QueryId);
	}

	public PathGrid GetPathGrid()
	{
		return PathGrid;
	}

	public void UpdateProbe(NavGrid nav_grid, int cell, NavType nav_type, PathFinderAbilities abilities, PathFinder.PotentialPath.Flags flags, bool new_query = true)
	{
		Potentials.Clear();
		if (new_query)
		{
			QueryId++;
		}
		PathGrid.SetRootCell(cell);
		bool is_cell_in_range = false;
		PathFinder.Cell cell_data = PathGrid.GetCell(cell, nav_type, QueryId, out is_cell_in_range);
		PathFinder.PotentialPath potential_path = new PathFinder.PotentialPath(cell, nav_type, flags);
		PathFinder.AddPotential(potential_path, Grid.InvalidCell, NavType.NumNavTypes, 0, 0, -1, Potentials, QueryId, PathGrid, ref cell_data);
		UpdateProbe(nav_grid, ref abilities, Potentials, QueryId);
	}

	private void UpdateProbe(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.PotentialList potentials, int query_id)
	{
		while (potentials.Count > 0)
		{
			KeyValuePair<int, PathFinder.PotentialPath> keyValuePair = potentials.Next();
			UpdateProbe(nav_grid, ref abilities, keyValuePair.Value, keyValuePair.Key, potentials, query_id);
		}
	}

	private void UpdateProbe(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.PotentialPath potential, int potential_cost, PathFinder.PotentialList potentials, int query_id)
	{
		bool is_cell_in_range;
		PathFinder.Cell cell = PathGrid.GetCell(potential, query_id, out is_cell_in_range);
		if (cell.cost == potential_cost)
		{
			PathFinder.AddPotentials(nav_grid.potentialScratchPad, potential, cell.cost, cell.underwaterCost, ref abilities, null, nav_grid.maxLinksPerCell, nav_grid.Links, potentials, query_id, PathGrid, cell.parent, cell.parentNavType);
		}
	}
}
