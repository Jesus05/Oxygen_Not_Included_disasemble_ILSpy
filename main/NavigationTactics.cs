public static class NavigationTactics
{
	public static NavTactic ReduceTravelDistance = new NavTactic(0, 0, 1, 4);

	public static NavTactic Range_2_AvoidOverlaps = new NavTactic(2, 6, 12, 1);

	public static NavTactic Range_3_ProhibitOverlap = new NavTactic(3, 6, 9999, 1);
}
