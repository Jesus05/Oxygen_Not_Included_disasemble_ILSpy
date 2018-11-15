public class IdleCellQuery : PathFinderQuery
{
	private MinionBrain brain;

	private int targetCell;

	private int maxCost;

	public IdleCellQuery Reset(MinionBrain brain, int max_cost)
	{
		this.brain = brain;
		maxCost = max_cost;
		targetCell = Grid.InvalidCell;
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		SafeCellQuery.SafeFlags flags = SafeCellQuery.GetFlags(cell, brain);
		if ((flags & SafeCellQuery.SafeFlags.IsClear) != 0 && (flags & SafeCellQuery.SafeFlags.IsNotLadder) != 0 && (flags & SafeCellQuery.SafeFlags.IsNotTube) != 0 && (flags & SafeCellQuery.SafeFlags.HasSomeOxygen) != 0 && (flags & SafeCellQuery.SafeFlags.IsNotLiquid) != 0)
		{
			targetCell = cell;
		}
		return cost > maxCost;
	}

	public override int GetResultCell()
	{
		return targetCell;
	}
}
