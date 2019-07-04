public class CellVisibility
{
	private int MinX;

	private int MinY;

	private int MaxX;

	private int MaxY;

	public CellVisibility()
	{
		Grid.GetVisibleExtents(out MinX, out MinY, out MaxX, out MaxY);
	}

	public bool IsVisible(int cell)
	{
		int num = Grid.CellColumn(cell);
		if (num >= MinX && num <= MaxX)
		{
			int num2 = Grid.CellRow(cell);
			if (num2 >= MinY && num2 <= MaxY)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
