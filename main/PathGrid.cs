public class PathGrid
{
	private struct ProberCell
	{
		public int cost;

		public int queryId;
	}

	private PathFinder.Cell[] Cells;

	private ProberCell[] ProberCells;

	private NavType[] ValidNavTypes;

	private int[] NavTypeTable;

	private int widthInCells;

	private int heightInCells;

	private bool applyOffset;

	private int rootX;

	private int rootY;

	private IGroupProber groupProber;

	public PathGrid(int width_in_cells, int height_in_cells, bool apply_offset, NavType[] valid_nav_types)
	{
		applyOffset = apply_offset;
		widthInCells = width_in_cells;
		heightInCells = height_in_cells;
		ValidNavTypes = valid_nav_types;
		int num = 0;
		NavTypeTable = new int[10];
		for (int i = 0; i < NavTypeTable.Length; i++)
		{
			NavTypeTable[i] = -1;
			for (int j = 0; j < ValidNavTypes.Length; j++)
			{
				if ((uint)ValidNavTypes[j] == (byte)i)
				{
					NavTypeTable[i] = num++;
					break;
				}
			}
		}
		Cells = new PathFinder.Cell[width_in_cells * height_in_cells * ValidNavTypes.Length];
		ProberCells = new ProberCell[width_in_cells * height_in_cells];
	}

	public void SetGroupProber(IGroupProber group_prober)
	{
		groupProber = group_prober;
	}

	public PathFinder.Cell GetCell(PathFinder.PotentialPath potential_path, int query_id, out bool is_cell_in_range)
	{
		return GetCell(potential_path.cell, potential_path.navType, query_id, out is_cell_in_range);
	}

	public PathFinder.Cell GetCell(int cell, NavType nav_type, int query_id, out bool is_cell_in_range)
	{
		int num = OffsetCell(cell);
		is_cell_in_range = IsValidOffsetCell(num);
		if (is_cell_in_range)
		{
			int num2 = NavTypeTable[(uint)nav_type];
			int num3 = num * ValidNavTypes.Length + num2;
			PathFinder.Cell result = Cells[num3];
			if (result.queryId != query_id)
			{
				PathFinder.Cell result2 = default(PathFinder.Cell);
				result2.cost = -1;
				return result2;
			}
			return result;
		}
		PathFinder.Cell result3 = default(PathFinder.Cell);
		result3.cost = -1;
		return result3;
	}

	public void SetCell(PathFinder.PotentialPath potential_path, ref PathFinder.Cell cell_data)
	{
		int num = OffsetCell(potential_path.cell);
		if (IsValidOffsetCell(num))
		{
			int num2 = NavTypeTable[(uint)potential_path.navType];
			int num3 = num * ValidNavTypes.Length + num2;
			Cells[num3] = cell_data;
			if (potential_path.navType != NavType.Tube)
			{
				ProberCell proberCell = ProberCells[num];
				if (cell_data.queryId != proberCell.queryId || cell_data.cost < proberCell.cost)
				{
					proberCell.queryId = cell_data.queryId;
					proberCell.cost = cell_data.cost;
					ProberCells[num] = proberCell;
					if (groupProber != null)
					{
						groupProber.SetProberCell(potential_path.cell);
					}
				}
			}
		}
	}

	public int GetCostIgnoreProberOffset(int cell, CellOffset[] offsets, int query_id)
	{
		int num = -1;
		foreach (CellOffset offset in offsets)
		{
			int num2 = Grid.OffsetCell(cell, offset);
			if (Grid.IsValidCell(num2))
			{
				ProberCell proberCell = ProberCells[num2];
				if (proberCell.queryId == query_id && (num == -1 || proberCell.cost < num))
				{
					num = proberCell.cost;
				}
			}
		}
		return num;
	}

	public int GetCost(int cell, int query_id)
	{
		int num = OffsetCell(cell);
		if (IsValidOffsetCell(num))
		{
			int result = -1;
			ProberCell proberCell = ProberCells[num];
			if (proberCell.queryId == query_id)
			{
				result = proberCell.cost;
			}
			return result;
		}
		return -1;
	}

	private bool IsValidOffsetCell(int offset_cell)
	{
		return !applyOffset || -1 != offset_cell;
	}

	private int OffsetCell(int cell)
	{
		if (!applyOffset)
		{
			return cell;
		}
		Grid.CellToXY(cell, out int x, out int y);
		if (x >= rootX && x < rootX + widthInCells && y >= rootY && y < rootY + heightInCells)
		{
			int num = x - rootX;
			int num2 = y - rootY;
			return num2 * widthInCells + num;
		}
		return -1;
	}

	public void SetRootCell(int root_cell)
	{
		if (applyOffset)
		{
			Grid.CellToXY(root_cell, out rootX, out rootY);
			rootX -= widthInCells / 2;
			rootY -= heightInCells / 2;
		}
	}
}
