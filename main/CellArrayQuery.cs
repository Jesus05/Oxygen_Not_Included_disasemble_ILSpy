public class CellArrayQuery : PathFinderQuery
{
	private int[] targetCells;

	public CellArrayQuery Reset(int[] target_cells)
	{
		targetCells = target_cells;
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		for (int i = 0; i < targetCells.Length; i++)
		{
			if (targetCells[i] == cell)
			{
				return true;
			}
		}
		return false;
	}
}
