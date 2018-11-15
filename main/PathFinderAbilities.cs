public class PathFinderAbilities
{
	public Navigator navigator
	{
		get;
		private set;
	}

	public PathFinderAbilities(Navigator navigator)
	{
		this.navigator = navigator;
	}

	public virtual void Refresh()
	{
	}

	public virtual bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, int underwater_cost)
	{
		return true;
	}
}
