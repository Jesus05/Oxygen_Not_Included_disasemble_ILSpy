using System.Diagnostics;

public class MinionPathFinderAbilities : PathFinderAbilities
{
	public int maxUnderwaterCost;

	private AccessControlNavMask accessControlNavMask;

	private TravelTubeNavMask travelTubeNavMask;

	private JetPackNavMask jetPackNavMask;

	private NavigationFeatureMask navigationFeatureMask;

	private IdleNavMask idleNavMask;

	public MinionPathFinderAbilities(Navigator navigator)
		: base(navigator)
	{
		accessControlNavMask = new AccessControlNavMask(navigator);
		travelTubeNavMask = new TravelTubeNavMask(navigator);
		jetPackNavMask = new JetPackNavMask(navigator);
		navigationFeatureMask = new NavigationFeatureMask(navigator);
		idleNavMask = default(IdleNavMask);
	}

	public override void Refresh()
	{
		int cell = Grid.PosToCell(base.navigator);
		if (PathFinder.IsSubmerged(cell))
		{
			maxUnderwaterCost = 2147483647;
		}
		else
		{
			maxUnderwaterCost = (int)Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup(base.navigator).GetTotalValue();
		}
	}

	public void SetIdleNavMaskEnabled(bool enabled)
	{
		idleNavMask.enabled = enabled;
	}

	public override bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, int underwater_cost)
	{
		if (accessControlNavMask.IsTraversable(base.navigator, path, from_cell, cost, transition_id))
		{
			if (travelTubeNavMask.IsTraversable(base.navigator, path, from_cell, from_nav_type, cost, transition_id))
			{
				if (jetPackNavMask.IsTraversable(base.navigator, path, from_cell, from_nav_type, cost, transition_id))
				{
					if (navigationFeatureMask.IsTraversable(base.navigator, path, from_cell, cost, transition_id, this))
					{
						if (idleNavMask.IsTraversable(base.navigator, path, from_cell, cost, transition_id))
						{
							if (!path.HasFlag(PathFinder.PotentialPath.Flags.HasAtmoSuit) && !path.HasFlag(PathFinder.PotentialPath.Flags.HasJetPack) && path.navType != NavType.Tube && underwater_cost > maxUnderwaterCost)
							{
								return false;
							}
							navigationFeatureMask.ApplyTraversalToPath(base.navigator, ref path, from_cell);
							return true;
						}
						return false;
					}
					return false;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	[Conditional("ENABLE_NAVIGATION_MASK_PROFILING")]
	private void BeginSample(string region_name)
	{
	}

	[Conditional("ENABLE_NAVIGATION_MASK_PROFILING")]
	private void EndSample()
	{
	}
}
