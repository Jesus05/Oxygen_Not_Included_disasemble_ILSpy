using System.Collections.Generic;

public class Pathfinding : KMonoBehaviour
{
	private List<NavGrid> NavGrids = new List<NavGrid>();

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
		Debug.LogError("Could not find nav grid: " + id);
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

	protected override void OnCleanUp()
	{
		NavGrids.Clear();
		OffsetTableTracker.OnPathfindingInvalidated();
	}
}
