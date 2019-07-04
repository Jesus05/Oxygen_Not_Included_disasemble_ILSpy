public class CellCostQuery : PathFinderQuery
{
	private int targetCell;

	private int maxCost;

	public int resultCost
	{
		get;
		private set;
	}

	public void Reset(int target_cell, int max_cost)
	{
		targetCell = target_cell;
		maxCost = max_cost;
		resultCost = -1;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (cost <= maxCost)
		{
			if (cell != targetCell)
			{
				return false;
			}
			resultCost = cost;
			return true;
		}
		return true;
	}
}
