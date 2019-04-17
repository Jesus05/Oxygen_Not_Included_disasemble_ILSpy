public class OffsetTableTracker : OffsetTracker
{
	private readonly CellOffset[][] table;

	public HandleVector<int>.Handle solidPartitionerEntry;

	public HandleVector<int>.Handle validNavCellChangedPartitionerEntry;

	private static NavGrid navGridImpl;

	private KMonoBehaviour cmp;

	private static NavGrid navGrid
	{
		get
		{
			if (navGridImpl == null)
			{
				navGridImpl = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
			}
			Debug.Assert(navGridImpl == Pathfinding.Instance.GetNavGrid("MinionNavGrid"), "Cached NavGrid reference is invalid");
			return navGridImpl;
		}
	}

	public OffsetTableTracker(CellOffset[][] table, KMonoBehaviour cmp)
	{
		this.table = table;
		this.cmp = cmp;
	}

	protected override void UpdateCell(int previous_cell, int current_cell)
	{
		if (previous_cell != current_cell)
		{
			base.UpdateCell(previous_cell, current_cell);
			if (!solidPartitionerEntry.IsValid())
			{
				Extents extents = new Extents(current_cell, table);
				extents.height += 2;
				extents.y--;
				solidPartitionerEntry = GameScenePartitioner.Instance.Add("OffsetTableTracker.UpdateCell", cmp.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, OnCellChanged);
				validNavCellChangedPartitionerEntry = GameScenePartitioner.Instance.Add("OffsetTableTracker.UpdateCell", cmp.gameObject, extents, GameScenePartitioner.Instance.validNavCellChangedLayer, OnCellChanged);
			}
			else
			{
				GameScenePartitioner.Instance.UpdatePosition(solidPartitionerEntry, current_cell);
				GameScenePartitioner.Instance.UpdatePosition(validNavCellChangedPartitionerEntry, current_cell);
			}
			offsets = null;
		}
	}

	private static bool IsValidRow(int current_cell, CellOffset[] row)
	{
		for (int i = 1; i < row.Length; i++)
		{
			int num = Grid.OffsetCell(current_cell, row[i]);
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			if (Grid.Solid[num])
			{
				return false;
			}
		}
		return true;
	}

	private unsafe void UpdateOffsets(int cell, CellOffset[][] table)
	{
		Debug.Assert(table.Length <= 192, $"validRowIndices[{192}] isn't big enough < {table.Length}");
		int* ptr = stackalloc int[192];
		int num = 0;
		if (Grid.IsValidCell(cell))
		{
			for (int i = 0; i < table.Length; i++)
			{
				CellOffset[] array = table[i];
				int cell2 = Grid.OffsetCell(cell, array[0]);
				for (int j = 0; j < navGrid.ValidNavTypes.Length; j++)
				{
					NavType navType = navGrid.ValidNavTypes[j];
					if (navType != NavType.Tube && navGrid.NavTable.IsValid(cell2, navType) && IsValidRow(cell, array))
					{
						ptr[num] = i;
						num++;
						break;
					}
				}
			}
		}
		if (offsets == null || offsets.Length != num)
		{
			offsets = new CellOffset[num];
		}
		for (int k = 0; k != num; k++)
		{
			offsets[k] = table[ptr[k]][0];
		}
	}

	protected override void UpdateOffsets(int current_cell)
	{
		base.UpdateOffsets(current_cell);
		UpdateOffsets(current_cell, table);
	}

	private void OnCellChanged(object data)
	{
		offsets = null;
	}

	public override void Clear()
	{
		GameScenePartitioner.Instance.Free(ref solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref validNavCellChangedPartitionerEntry);
	}

	public static void OnPathfindingInvalidated()
	{
		navGridImpl = null;
	}
}
