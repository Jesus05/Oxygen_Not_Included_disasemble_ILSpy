internal struct IdleNavMask
{
	public bool enabled;

	public IdleNavMask(Navigator navigator)
	{
		enabled = false;
	}

	public bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, int cost, int transition_id)
	{
		if (!enabled)
		{
			return true;
		}
		if (Grid.PreventIdleTraversal[path.cell] || Grid.PreventIdleTraversal[from_cell])
		{
			return false;
		}
		return true;
	}
}
