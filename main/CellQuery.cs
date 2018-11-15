public class CellQuery : PathFinderQuery
{
	private int targetCell;

	public CellQuery Reset(int target_cell)
	{
		targetCell = target_cell;
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		return cell == targetCell;
	}
}
