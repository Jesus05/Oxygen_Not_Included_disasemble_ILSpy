using System.Collections.Generic;
using System.Diagnostics;

public class MinionGroupProber : KMonoBehaviour, IGroupProber
{
	private static MinionGroupProber Instance;

	private List<PathGrid> pathGrids;

	private List<int> pendingPathGridRemovals;

	private int[] proberCells;

	private const int InvalidIndex = -1;

	private const int ProxyIndex = -2;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public static MinionGroupProber Get()
	{
		return Instance;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		proberCells = new int[Grid.CellCount];
		for (int i = 0; i < proberCells.Length; i++)
		{
			proberCells[i] = -1;
		}
		pathGrids = new List<PathGrid>();
		pendingPathGridRemovals = new List<int>();
	}

	public bool IsReachable(Workable workable)
	{
		int cell = Grid.PosToCell(workable);
		return IsReachable(cell, workable.GetOffsets());
	}

	public bool IsReachable(int cell)
	{
		if (Grid.IsValidCell(cell))
		{
			LaunderCells();
			int num = proberCells[cell];
			switch (num)
			{
			case -1:
				return false;
			case -2:
				return true;
			default:
				DebugUtil.Assert(num < pathGrids.Count);
				return pathGrids[num].GetCost(cell) != -1;
			}
		}
		return false;
	}

	public bool IsReachable(int cell, CellOffset[] offsets)
	{
		if (Grid.IsValidCell(cell))
		{
			int num = offsets.Length;
			for (int i = 0; i < num; i++)
			{
				int cell2 = Grid.OffsetCell(cell, offsets[i]);
				if (IsReachable(cell2))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetProberCell(int cell, PathGrid pathGrid)
	{
		int num = pathGrids.IndexOf(pathGrid);
		if (num == -1)
		{
			lock (pathGrids)
			{
				pathGrids.Add(pathGrid);
				num = pathGrids.Count - 1;
			}
		}
		proberCells[cell] = num;
	}

	public void ProxyProberCell(int cell, bool set)
	{
		proberCells[cell] = ((!set) ? (-1) : (-2));
	}

	public bool ReleasePathGrid(PathGrid pathGrid)
	{
		int num = pathGrids.IndexOf(pathGrid);
		if (num == -1)
		{
			return false;
		}
		pendingPathGridRemovals.Add(num);
		return true;
	}

	private void LaunderCells()
	{
		if (pendingPathGridRemovals.Count != 0)
		{
			pendingPathGridRemovals.Sort();
			for (int i = 0; i != proberCells.Length; i++)
			{
				for (int num = pendingPathGridRemovals.Count - 1; num != -1; num--)
				{
					if (pendingPathGridRemovals[num] <= proberCells[i])
					{
						proberCells[i] -= num + 1;
						break;
					}
				}
			}
			for (int num2 = pendingPathGridRemovals.Count - 1; num2 != -1; num2--)
			{
				pathGrids.RemoveAt(pendingPathGridRemovals[num2]);
			}
			pendingPathGridRemovals.Clear();
		}
	}

	[Conditional("MINION_GROUP_PROBER_UNIT_TESTS")]
	private void TestLaunderCells()
	{
		List<PathGrid> list = new List<PathGrid>();
		for (int i = 0; i != 10; i++)
		{
			PathGrid pathGrid = new PathGrid(10, 10, false, new NavType[1]);
			list.Add(pathGrid);
			pathGrid.SetGroupProber(this);
			pathGrid.BeginUpdate(0, false);
			pathGrid.EndUpdate(true);
		}
		PathFinder.Cell cell = default(PathFinder.Cell);
		cell.cost = 10;
		cell.queryId = 1;
		PathFinder.Cell cell_data = cell;
		for (int j = 0; j != 100; j++)
		{
			list[j % list.Count].SetCell(new PathFinder.PotentialPath
			{
				cell = j
			}, ref cell_data);
		}
		int num = 0;
		for (int k = 0; k != 100; k++)
		{
			if (IsReachable(k))
			{
				num++;
			}
		}
		DebugUtil.Assert(num == 100);
		ReleasePathGrid(list[5]);
		DebugUtil.Assert(pendingPathGridRemovals.Count == 1);
		list[5] = null;
		num = 0;
		for (int l = 0; l != 100; l++)
		{
			if (IsReachable(l))
			{
				num++;
			}
		}
		DebugUtil.Assert(pendingPathGridRemovals.Count == 0);
		DebugUtil.Assert(num == 90);
		ReleasePathGrid(list[9]);
		DebugUtil.Assert(pendingPathGridRemovals.Count == 1);
		list[9] = null;
		num = 0;
		for (int m = 0; m != 100; m++)
		{
			if (IsReachable(m))
			{
				num++;
			}
		}
		DebugUtil.Assert(pendingPathGridRemovals.Count == 0);
		DebugUtil.Assert(num == 80);
		ReleasePathGrid(list[0]);
		DebugUtil.Assert(pendingPathGridRemovals.Count == 1);
		list[0] = null;
		num = 0;
		for (int n = 0; n != 100; n++)
		{
			if (IsReachable(n))
			{
				num++;
			}
		}
		DebugUtil.Assert(pendingPathGridRemovals.Count == 0);
		DebugUtil.Assert(num == 70);
		ReleasePathGrid(list[1]);
		ReleasePathGrid(list[3]);
		ReleasePathGrid(list[7]);
		DebugUtil.Assert(pendingPathGridRemovals.Count == 3);
		list[1] = null;
		list[3] = null;
		list[7] = null;
		num = 0;
		for (int num2 = 0; num2 != 100; num2++)
		{
			if (IsReachable(num2))
			{
				num++;
			}
		}
		DebugUtil.Assert(pendingPathGridRemovals.Count == 0);
		DebugUtil.Assert(num == 40);
		foreach (PathGrid item in list)
		{
			ReleasePathGrid(item);
		}
		LaunderCells();
	}
}
