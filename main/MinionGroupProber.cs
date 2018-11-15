using UnityEngine;

public class MinionGroupProber : KMonoBehaviour, IGroupProber
{
	private static MinionGroupProber Instance;

	private int[] proberCells;

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
			proberCells[i] = -10000;
		}
	}

	public bool IsReachable(Workable workable)
	{
		int cell = Grid.PosToCell(workable);
		return IsReachable(cell, workable.GetOffsets());
	}

	public bool IsReachable(int cell, int current_frame)
	{
		if (Grid.IsValidCell(cell))
		{
			int count = Components.LiveMinionIdentities.Count;
			int num = current_frame - proberCells[cell];
			if (num <= count)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsReachable(int cell)
	{
		return IsReachable(cell, Time.frameCount);
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

	public void SetProberCell(int cell)
	{
		proberCells[cell] = Time.frameCount;
	}
}
