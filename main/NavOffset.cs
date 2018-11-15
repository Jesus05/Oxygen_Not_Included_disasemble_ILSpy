public struct NavOffset
{
	public NavType navType;

	public CellOffset offset;

	public NavOffset(NavType nav_type, int x, int y)
	{
		navType = nav_type;
		offset.x = x;
		offset.y = y;
	}
}
