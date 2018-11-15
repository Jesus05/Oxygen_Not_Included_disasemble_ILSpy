public class NavGraph
{
	private struct NavGraphEdge
	{
		public NavType startNavType;

		public NavType endNavType;

		public int startCell;

		public int endCell;
	}

	private HandleVector<NavGraphEdge>.Handle[] grid;

	private HandleVector<NavGraphEdge> edges;

	public NavGraph(int cell_count, NavGrid nav_grid)
	{
		grid = new HandleVector<NavGraphEdge>.Handle[nav_grid.maxLinksPerCell * cell_count];
		for (int i = 0; i < grid.Length; i++)
		{
			grid[i] = HandleVector<NavGraphEdge>.InvalidHandle;
		}
		edges = new HandleVector<NavGraphEdge>(cell_count);
		for (int j = 0; j < cell_count; j++)
		{
			int num = j * nav_grid.maxLinksPerCell;
			NavGrid.Link link = nav_grid.Links[num];
			while (link.link != NavGrid.InvalidHandle)
			{
				NavGraphEdge item = new NavGraphEdge
				{
					startNavType = link.startNavType,
					endNavType = link.endNavType,
					endCell = link.link,
					startCell = j
				};
				HandleVector<NavGraphEdge>.Handle handle = edges.Add(item);
				grid[num] = handle;
				num++;
				link = nav_grid.Links[num];
			}
			grid[num] = HandleVector<NavGraphEdge>.InvalidHandle;
		}
	}

	public void Cleanup()
	{
	}
}
