using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct JetPackNavMask
{
	public JetPackNavMask(Navigator navigator)
	{
	}

	public bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id)
	{
		if (path.navType == NavType.Hover)
		{
			bool flag = path.HasFlag(PathFinder.PotentialPath.Flags.HasJetPack);
			bool flag2 = agent.HasTag(GameTags.JetSuitOutOfFuel);
			return flag && !flag2;
		}
		return true;
	}
}
