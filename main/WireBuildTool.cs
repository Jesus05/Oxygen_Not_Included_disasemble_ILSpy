public class WireBuildTool : BaseUtilityBuildTool
{
	public static WireBuildTool Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		base.OnPrefabInit();
		viewMode = SimViewMode.PowerMap;
	}

	protected override void ApplyPathToConduitSystem()
	{
		if (path.Count >= 2)
		{
			for (int i = 1; i < path.Count; i++)
			{
				PathNode pathNode = path[i - 1];
				if (pathNode.valid)
				{
					PathNode pathNode2 = path[i];
					if (pathNode2.valid)
					{
						PathNode pathNode3 = path[i - 1];
						int cell = pathNode3.cell;
						PathNode pathNode4 = path[i];
						int cell2 = pathNode4.cell;
						int from_cell = cell;
						PathNode pathNode5 = path[i];
						UtilityConnections utilityConnections = UtilityConnectionsExtensions.DirectionFromToCell(from_cell, pathNode5.cell);
						if (utilityConnections != 0)
						{
							UtilityConnections new_connection = utilityConnections.InverseDirection();
							conduitMgr.AddConnection(utilityConnections, cell, false);
							conduitMgr.AddConnection(new_connection, cell2, false);
						}
					}
				}
			}
		}
	}
}
