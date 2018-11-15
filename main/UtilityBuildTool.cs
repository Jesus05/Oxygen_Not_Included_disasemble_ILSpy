public class UtilityBuildTool : BaseUtilityBuildTool
{
	public static UtilityBuildTool Instance;

	private int lastPathHead = -1;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		base.OnPrefabInit();
		canChangeDragAxis = false;
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
						UtilityConnections utilityConnections = UtilityConnectionsExtensions.DirectionFromToCell(cell, cell2);
						if (utilityConnections != 0)
						{
							UtilityConnections new_connection = utilityConnections.InverseDirection();
							if (conduitMgr.CanAddConnection(utilityConnections, cell, false, out string fail_reason) && conduitMgr.CanAddConnection(new_connection, cell2, false, out fail_reason))
							{
								conduitMgr.AddConnection(utilityConnections, cell, false);
								conduitMgr.AddConnection(new_connection, cell2, false);
							}
							else if (i == path.Count - 1 && lastPathHead != i)
							{
								PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, fail_reason, null, Grid.CellToPosCCC(cell2, (Grid.SceneLayer)0), 1.5f, false, false);
							}
						}
					}
				}
			}
			lastPathHead = path.Count - 1;
		}
	}
}
