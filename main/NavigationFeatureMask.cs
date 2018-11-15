using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct NavigationFeatureMask
{
	public NavigationFeatureMask(Navigator navigator)
	{
	}

	public bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, int cost, int transition_id, PathFinderAbilities abilities)
	{
		return Pathfinding.Instance.GetNavigationFeature(from_cell)?.IsTraversable(agent, path, from_cell, cost, abilities) ?? true;
	}

	public void ApplyTraversalToPath(Navigator agent, ref PathFinder.PotentialPath path, int from_cell)
	{
		Pathfinding.Instance.GetNavigationFeature(from_cell)?.ApplyTraversalToPath(agent, ref path, from_cell);
	}
}
