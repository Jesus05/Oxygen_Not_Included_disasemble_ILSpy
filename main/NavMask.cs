public class NavMask
{
	public virtual bool IsTraversable(PathFinder.PotentialPath path, int from_cell, int cost, int transition_id, PathFinderAbilities abilities)
	{
		return true;
	}

	public virtual void ApplyTraversalToPath(ref PathFinder.PotentialPath path, int from_cell)
	{
	}
}
