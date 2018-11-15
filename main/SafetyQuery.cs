public class SafetyQuery : PathFinderQuery
{
	private int targetCell;

	private int targetCost;

	private int targetConditions;

	private int maxCost;

	private SafetyChecker checker;

	private KMonoBehaviour cmp;

	private SafetyChecker.Context context;

	public SafetyQuery(SafetyChecker checker, KMonoBehaviour cmp, int max_cost)
	{
		this.checker = checker;
		this.cmp = cmp;
		maxCost = max_cost;
	}

	public void Reset()
	{
		targetCell = PathFinder.InvalidCell;
		targetCost = 2147483647;
		targetConditions = 0;
		context = new SafetyChecker.Context(cmp);
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		bool all_conditions_met = false;
		int safetyConditions = checker.GetSafetyConditions(cell, cost, context, out all_conditions_met);
		if (safetyConditions != 0 && (safetyConditions > targetConditions || (safetyConditions == targetConditions && cost < targetCost)))
		{
			targetCell = cell;
			targetConditions = safetyConditions;
			targetCost = cost;
			if (all_conditions_met)
			{
				return true;
			}
		}
		if (cost >= maxCost)
		{
			return true;
		}
		return false;
	}

	public override int GetResultCell()
	{
		return targetCell;
	}
}
