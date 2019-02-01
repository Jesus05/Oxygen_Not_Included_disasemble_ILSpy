using System;

public static class UtilityConnectionsExtensions
{
	public static UtilityConnections InverseDirection(this UtilityConnections direction)
	{
		switch (direction)
		{
		case UtilityConnections.Up:
			return UtilityConnections.Down;
		case UtilityConnections.Down:
			return UtilityConnections.Up;
		case UtilityConnections.Left:
			return UtilityConnections.Right;
		case UtilityConnections.Right:
			return UtilityConnections.Left;
		default:
			throw new ArgumentException("Unexpected enum value: " + direction, "direction");
		}
	}

	public static UtilityConnections LeftDirection(this UtilityConnections direction)
	{
		switch (direction)
		{
		case UtilityConnections.Up:
			return UtilityConnections.Left;
		case UtilityConnections.Left:
			return UtilityConnections.Down;
		case UtilityConnections.Down:
			return UtilityConnections.Right;
		case UtilityConnections.Right:
			return UtilityConnections.Up;
		default:
			throw new ArgumentException("Unexpected enum value: " + direction, "direction");
		}
	}

	public static UtilityConnections RightDirection(this UtilityConnections direction)
	{
		switch (direction)
		{
		case UtilityConnections.Up:
			return UtilityConnections.Right;
		case UtilityConnections.Right:
			return UtilityConnections.Down;
		case UtilityConnections.Down:
			return UtilityConnections.Left;
		case UtilityConnections.Left:
			return UtilityConnections.Up;
		default:
			throw new ArgumentException("Unexpected enum value: " + direction, "direction");
		}
	}

	public static int CellInDirection(this UtilityConnections direction, int from_cell)
	{
		switch (direction)
		{
		case UtilityConnections.Up:
			return from_cell + Grid.WidthInCells;
		case UtilityConnections.Down:
			return from_cell - Grid.WidthInCells;
		case UtilityConnections.Left:
			return from_cell - 1;
		case UtilityConnections.Right:
			return from_cell + 1;
		default:
			throw new ArgumentException("Unexpected enum value: " + direction, "direction");
		}
	}

	public static UtilityConnections DirectionFromToCell(int from_cell, int to_cell)
	{
		if (to_cell != from_cell - 1)
		{
			if (to_cell != from_cell + 1)
			{
				if (to_cell != from_cell + Grid.WidthInCells)
				{
					if (to_cell != from_cell - Grid.WidthInCells)
					{
						return (UtilityConnections)0;
					}
					return UtilityConnections.Down;
				}
				return UtilityConnections.Up;
			}
			return UtilityConnections.Right;
		}
		return UtilityConnections.Left;
	}
}
