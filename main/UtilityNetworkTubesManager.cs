using STRINGS;
using UnityEngine;

public class UtilityNetworkTubesManager : UtilityNetworkManager<TravelTubeNetwork, TravelTube>
{
	public UtilityNetworkTubesManager(int game_width, int game_height, int tile_layer)
		: base(game_width, game_height, tile_layer)
	{
	}

	public override bool CanAddConnection(UtilityConnections new_connection, int cell, bool is_physical_building, out string fail_reason)
	{
		return TestForUTurnLeft(cell, new_connection, is_physical_building, out fail_reason) && TestForUTurnRight(cell, new_connection, is_physical_building, out fail_reason) && TestForNoAdjacentBridge(cell, new_connection, out fail_reason);
	}

	public override void SetConnections(UtilityConnections connections, int cell, bool is_physical_building)
	{
		base.SetConnections(connections, cell, is_physical_building);
		Pathfinding.Instance.AddDirtyNavGridCell(cell);
	}

	private bool TestForUTurnLeft(int first_cell, UtilityConnections first_connection, bool is_physical_building, out string fail_reason)
	{
		int from_cell = first_cell;
		UtilityConnections direction = first_connection;
		int num = 1;
		for (int i = 0; i < 3; i++)
		{
			int num2 = direction.CellInDirection(from_cell);
			UtilityConnections utilityConnections = direction.LeftDirection();
			if (HasConnection(num2, utilityConnections, is_physical_building))
			{
				num++;
			}
			from_cell = num2;
			direction = utilityConnections;
		}
		fail_reason = UI.TOOLTIPS.HELP_TUBELOCATION_NO_UTURNS;
		return num <= 2;
	}

	private bool TestForUTurnRight(int first_cell, UtilityConnections first_connection, bool is_physical_building, out string fail_reason)
	{
		int from_cell = first_cell;
		UtilityConnections direction = first_connection;
		int num = 1;
		for (int i = 0; i < 3; i++)
		{
			int num2 = direction.CellInDirection(from_cell);
			UtilityConnections utilityConnections = direction.RightDirection();
			if (HasConnection(num2, utilityConnections, is_physical_building))
			{
				num++;
			}
			from_cell = num2;
			direction = utilityConnections;
		}
		fail_reason = UI.TOOLTIPS.HELP_TUBELOCATION_NO_UTURNS;
		return num <= 2;
	}

	private bool TestForNoAdjacentBridge(int cell, UtilityConnections connection, out string fail_reason)
	{
		UtilityConnections direction = connection.LeftDirection();
		UtilityConnections direction2 = connection.RightDirection();
		int cell2 = direction.CellInDirection(cell);
		int cell3 = direction2.CellInDirection(cell);
		GameObject gameObject = Grid.Objects[cell2, 9];
		GameObject gameObject2 = Grid.Objects[cell3, 9];
		fail_reason = UI.TOOLTIPS.HELP_TUBELOCATION_STRAIGHT_BRIDGES;
		return ((Object)gameObject == (Object)null || (Object)gameObject.GetComponent<TravelTubeBridge>() == (Object)null) && ((Object)gameObject2 == (Object)null || (Object)gameObject2.GetComponent<TravelTubeBridge>() == (Object)null);
	}

	private bool HasConnection(int cell, UtilityConnections connection, bool is_physical_building)
	{
		return (GetConnections(cell, is_physical_building) & connection) != (UtilityConnections)0;
	}
}
