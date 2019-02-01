public abstract class PathFinderAbilities
{
	private Navigator navigator;

	protected int prefabInstanceID;

	public PathFinderAbilities(Navigator navigator)
	{
		this.navigator = navigator;
	}

	public void Refresh()
	{
		prefabInstanceID = navigator.gameObject.GetComponent<KPrefabID>().InstanceID;
		Refresh(navigator);
	}

	protected abstract void Refresh(Navigator navigator);

	public abstract bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, int underwater_cost);
}
