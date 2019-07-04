public class BreathableCellQuery : PathFinderQuery
{
	private OxygenBreather breather;

	public BreathableCellQuery Reset(Brain brain)
	{
		breather = brain.GetComponent<OxygenBreather>();
		return this;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!breather.IsBreathableElementAtCell(cell, null))
		{
			return false;
		}
		return true;
	}
}
