using System.Collections.Generic;

public class Pathfinding : KMonoBehaviour
{
	public interface INavigationFeature
	{
		bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, int cost, PathFinderAbilities abilities);

		void ApplyTraversalToPath(Navigator agent, ref PathFinder.PotentialPath path, int from_cell);
	}

	private List<NavGrid> NavGrids = new List<NavGrid>();

	private Dictionary<int, INavigationFeature> NavigationFeatures = new Dictionary<int, INavigationFeature>();

	private int UpdateIdx;

	private bool navGridsHaveBeenFlushedOnLoad;

	public static Pathfinding Instance;

	public static void DestroyInstance()
	{
		Instance = null;
		OffsetTableTracker.OnPathfindingInvalidated();
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	public void AddNavGrid(NavGrid nav_grid)
	{
		NavGrids.Add(nav_grid);
	}

	public NavGrid GetNavGrid(string id)
	{
		foreach (NavGrid navGrid in NavGrids)
		{
			if (navGrid.id == id)
			{
				return navGrid;
			}
		}
		Debug.LogError("Could not find nav grid: " + id, null);
		return null;
	}

	public void ResetNavGrids()
	{
		foreach (NavGrid navGrid in NavGrids)
		{
			navGrid.InitializeGraph();
		}
	}

	public void FlushNavGridsOnLoad()
	{
		if (!navGridsHaveBeenFlushedOnLoad)
		{
			navGridsHaveBeenFlushedOnLoad = true;
			UpdateNavGrids(true);
		}
	}

	public void UpdateNavGrids(bool update_all = false)
	{
		update_all = true;
		if (update_all)
		{
			foreach (NavGrid navGrid in NavGrids)
			{
				navGrid.UpdateGraph();
			}
		}
		else
		{
			foreach (NavGrid navGrid2 in NavGrids)
			{
				if (navGrid2.updateEveryFrame)
				{
					navGrid2.UpdateGraph();
				}
			}
			NavGrids[UpdateIdx].UpdateGraph();
			UpdateIdx = (UpdateIdx + 1) % NavGrids.Count;
		}
	}

	public void RenderEveryTick()
	{
		foreach (NavGrid navGrid in NavGrids)
		{
			navGrid.DebugUpdate();
		}
	}

	public void AddDirtyNavGridCell(int cell)
	{
		foreach (NavGrid navGrid in NavGrids)
		{
			navGrid.AddDirtyCell(cell);
		}
	}

	public void RefreshNavCell(int cell)
	{
		HashSet<int> hashSet = new HashSet<int>();
		hashSet.Add(cell);
		foreach (NavGrid navGrid in NavGrids)
		{
			navGrid.UpdateGraph(hashSet);
		}
	}

	public void AddNavigationFeature(int cell, INavigationFeature feature)
	{
		NavigationFeatures[cell] = feature;
	}

	public void RemoveNavigationFeature(int cell, INavigationFeature feature)
	{
		NavigationFeatures.Remove(cell);
	}

	public INavigationFeature GetNavigationFeature(int cell)
	{
		INavigationFeature value = null;
		NavigationFeatures.TryGetValue(cell, out value);
		return value;
	}

	protected override void OnCleanUp()
	{
		NavGrids.Clear();
		OffsetTableTracker.OnPathfindingInvalidated();
	}
}
