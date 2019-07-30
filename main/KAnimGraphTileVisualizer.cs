using KSerialization;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class KAnimGraphTileVisualizer : KMonoBehaviour, ISaveLoadable, IUtilityItem
{
	public enum ConnectionSource
	{
		Gas,
		Liquid,
		Electrical,
		Logic,
		Tube,
		Solid
	}

	[Serialize]
	private UtilityConnections _connections;

	public bool isPhysicalBuilding;

	public bool skipCleanup;

	public bool skipRefresh;

	public ConnectionSource connectionSource;

	[NonSerialized]
	public IUtilityNetworkMgr connectionManager;

	public UtilityConnections Connections
	{
		get
		{
			return _connections;
		}
		set
		{
			_connections = value;
			Trigger(-1041684577, _connections);
		}
	}

	public IUtilityNetworkMgr ConnectionManager
	{
		get
		{
			switch (connectionSource)
			{
			case ConnectionSource.Gas:
				return Game.Instance.gasConduitSystem;
			case ConnectionSource.Liquid:
				return Game.Instance.liquidConduitSystem;
			case ConnectionSource.Solid:
				return Game.Instance.solidConduitSystem;
			case ConnectionSource.Electrical:
				return Game.Instance.electricalConduitSystem;
			case ConnectionSource.Logic:
				return Game.Instance.logicCircuitSystem;
			case ConnectionSource.Tube:
				return Game.Instance.travelTubeSystem;
			default:
				return null;
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		connectionManager = ConnectionManager;
		int cell = Grid.PosToCell(base.transform.GetPosition());
		connectionManager.SetConnections(Connections, cell, isPhysicalBuilding);
		Building component = GetComponent<Building>();
		TileVisualizer.RefreshCell(cell, component.Def.TileLayer, component.Def.ReplacementLayer);
	}

	protected override void OnCleanUp()
	{
		if (connectionManager != null && !skipCleanup)
		{
			skipRefresh = true;
			int cell = Grid.PosToCell(base.transform.GetPosition());
			connectionManager.ClearCell(cell, isPhysicalBuilding);
			Building component = GetComponent<Building>();
			TileVisualizer.RefreshCell(cell, component.Def.TileLayer, component.Def.ReplacementLayer);
		}
	}

	[ContextMenu("Refresh")]
	public void Refresh()
	{
		if (connectionManager != null && !skipRefresh)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			Connections = connectionManager.GetConnections(cell, isPhysicalBuilding);
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				string text = connectionManager.GetVisualizerString(cell);
				BuildingUnderConstruction component2 = GetComponent<BuildingUnderConstruction>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component.HasAnimation(text + "_place"))
				{
					text += "_place";
				}
				if (text != null && text != string.Empty)
				{
					component.Play(text, KAnim.PlayMode.Once, 1f, 0f);
				}
			}
		}
	}

	public int GetNetworkID()
	{
		return GetNetwork()?.id ?? (-1);
	}

	private UtilityNetwork GetNetwork()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		return connectionManager.GetNetworkForDirection(cell, Direction.None);
	}

	public UtilityNetwork GetNetworkForDirection(Direction d)
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		return connectionManager.GetNetworkForDirection(cell, d);
	}

	public void UpdateConnections(UtilityConnections new_connections)
	{
		_connections = new_connections;
		if (connectionManager != null)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			connectionManager.SetConnections(new_connections, cell, isPhysicalBuilding);
		}
	}

	public KAnimGraphTileVisualizer GetNeighbour(Direction d)
	{
		KAnimGraphTileVisualizer result = null;
		Grid.PosToXY(base.transform.GetPosition(), out Vector2I xy);
		int num = -1;
		switch (d)
		{
		case Direction.Up:
			if (xy.y < Grid.HeightInCells - 1)
			{
				num = Grid.XYToCell(xy.x, xy.y + 1);
			}
			break;
		case Direction.Down:
			if (xy.y > 0)
			{
				num = Grid.XYToCell(xy.x, xy.y - 1);
			}
			break;
		case Direction.Left:
			if (xy.x > 0)
			{
				num = Grid.XYToCell(xy.x - 1, xy.y);
			}
			break;
		case Direction.Right:
			if (xy.x < Grid.WidthInCells - 1)
			{
				num = Grid.XYToCell(xy.x + 1, xy.y);
			}
			break;
		}
		if (num != -1)
		{
			ObjectLayer layer;
			switch (connectionSource)
			{
			case ConnectionSource.Gas:
				layer = ObjectLayer.GasConduitTile;
				break;
			case ConnectionSource.Liquid:
				layer = ObjectLayer.LiquidConduitTile;
				break;
			case ConnectionSource.Electrical:
				layer = ObjectLayer.WireTile;
				break;
			case ConnectionSource.Logic:
				layer = ObjectLayer.LogicWiresTiling;
				break;
			case ConnectionSource.Tube:
				layer = ObjectLayer.TravelTubeTile;
				break;
			case ConnectionSource.Solid:
				layer = ObjectLayer.SolidConduitTile;
				break;
			default:
				throw new ArgumentNullException("wtf");
			}
			GameObject gameObject = Grid.Objects[num, (int)layer];
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				result = gameObject.GetComponent<KAnimGraphTileVisualizer>();
			}
		}
		return result;
	}
}
