using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilityNetworkManager<NetworkType, ItemType> : IUtilityNetworkMgr where NetworkType : UtilityNetwork, new()where ItemType : MonoBehaviour
{
	private Dictionary<int, object> items = new Dictionary<int, object>();

	private Dictionary<int, object> endpoints = new Dictionary<int, object>();

	private Dictionary<int, int> links = new Dictionary<int, int>();

	private List<UtilityNetwork> networks;

	private HashSet<int> visitedCells;

	private Action<IList<UtilityNetwork>, ICollection<int>> onNetworksRebuilt;

	private Queue<int> queued = new Queue<int>();

	protected UtilityNetworkGridNode[] visualGrid;

	private UtilityNetworkGridNode[] stashedVisualGrid;

	protected UtilityNetworkGridNode[] physicalGrid;

	protected HashSet<int> physicalNodes;

	protected HashSet<int> visualNodes;

	private bool dirty;

	private int tileLayer = -1;

	public bool IsDirty => dirty;

	public UtilityNetworkManager(int game_width, int game_height, int tile_layer)
	{
		tileLayer = tile_layer;
		networks = new List<UtilityNetwork>();
		Initialize(game_width, game_height);
	}

	public void Initialize(int game_width, int game_height)
	{
		networks.Clear();
		physicalGrid = new UtilityNetworkGridNode[game_width * game_height];
		visualGrid = new UtilityNetworkGridNode[game_width * game_height];
		stashedVisualGrid = new UtilityNetworkGridNode[game_width * game_height];
		physicalNodes = new HashSet<int>();
		visualNodes = new HashSet<int>();
		visitedCells = new HashSet<int>();
		for (int i = 0; i < visualGrid.Length; i++)
		{
			visualGrid[i] = new UtilityNetworkGridNode
			{
				networkIdx = -1,
				connections = (UtilityConnections)0
			};
			physicalGrid[i] = new UtilityNetworkGridNode
			{
				networkIdx = -1,
				connections = (UtilityConnections)0
			};
		}
	}

	public void Update()
	{
		if (dirty)
		{
			dirty = false;
			for (int i = 0; i < networks.Count; i++)
			{
				networks[i].Reset(physicalGrid);
			}
			networks.Clear();
			RebuildNetworks(tileLayer, false);
			RebuildNetworks(tileLayer, true);
			if (onNetworksRebuilt != null)
			{
				onNetworksRebuilt(networks, GetNodes(true));
			}
		}
	}

	protected UtilityNetworkGridNode[] GetGrid(bool is_physical_building)
	{
		return (!is_physical_building) ? visualGrid : physicalGrid;
	}

	private HashSet<int> GetNodes(bool is_physical_building)
	{
		return (!is_physical_building) ? visualNodes : physicalNodes;
	}

	public void ClearCell(int cell, bool is_physical_building)
	{
		if (!Game.IsQuitting())
		{
			UtilityNetworkGridNode[] grid = GetGrid(is_physical_building);
			HashSet<int> nodes = GetNodes(is_physical_building);
			UtilityConnections connections = grid[cell].connections;
			grid[cell].connections = (UtilityConnections)0;
			Vector2I vector2I = Grid.CellToXY(cell);
			if (vector2I.x > 1 && (connections & UtilityConnections.Left) != 0)
			{
				grid[Grid.CellLeft(cell)].connections &= ~UtilityConnections.Right;
			}
			if (vector2I.x < Grid.WidthInCells - 1 && (connections & UtilityConnections.Right) != 0)
			{
				grid[Grid.CellRight(cell)].connections &= ~UtilityConnections.Left;
			}
			if (vector2I.y > 1 && (connections & UtilityConnections.Down) != 0)
			{
				grid[Grid.CellBelow(cell)].connections &= ~UtilityConnections.Up;
			}
			if (vector2I.y < Grid.HeightInCells - 1 && (connections & UtilityConnections.Up) != 0)
			{
				grid[Grid.CellAbove(cell)].connections &= ~UtilityConnections.Down;
			}
			nodes.Remove(cell);
			if (is_physical_building)
			{
				dirty = true;
				ClearCell(cell, false);
			}
		}
	}

	private void QueueCellForVisit(UtilityNetworkGridNode[] grid, int dest_cell, UtilityConnections direction)
	{
		if (Grid.IsValidCell(dest_cell) && !visitedCells.Contains(dest_cell) && (direction == (UtilityConnections)0 || (grid[dest_cell].connections & direction.InverseDirection()) != 0))
		{
			GameObject x = Grid.Objects[dest_cell, tileLayer];
			if ((UnityEngine.Object)x != (UnityEngine.Object)null)
			{
				visitedCells.Add(dest_cell);
				queued.Enqueue(dest_cell);
			}
		}
	}

	public void ForceRebuildNetworks()
	{
		dirty = true;
	}

	public void AddToNetworks(int cell, object item, bool is_endpoint)
	{
		dirty = true;
		if (item != null)
		{
			if (is_endpoint)
			{
				if (endpoints.ContainsKey(cell))
				{
					DebugUtil.LogWarningArgs("Cell", cell, "already has a utility network endpoint assigned. Adding", item.ToString(), "will stomp previous endpoint");
				}
				endpoints[cell] = item;
			}
			else
			{
				if (items.ContainsKey(cell))
				{
					DebugUtil.LogWarningArgs("Cell", cell, "already has a utility network connector assigned. Adding", item.ToString(), "will stomp previous item");
				}
				items[cell] = item;
			}
		}
	}

	private unsafe void Reconnect(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		int* ptr = stackalloc int[4];
		int* ptr2 = stackalloc int[4];
		int* ptr3 = stackalloc int[4];
		int num = 0;
		if (vector2I.y < Grid.HeightInCells - 1)
		{
			ptr[num] = Grid.CellAbove(cell);
			ptr2[num] = 4;
			ptr3[num] = 8;
			num++;
		}
		if (vector2I.y > 1)
		{
			ptr[num] = Grid.CellBelow(cell);
			ptr2[num] = 8;
			ptr3[num] = 4;
			num++;
		}
		if (vector2I.x > 1)
		{
			ptr[num] = Grid.CellLeft(cell);
			ptr2[num] = 1;
			ptr3[num] = 2;
			num++;
		}
		if (vector2I.x < Grid.WidthInCells - 1)
		{
			ptr[num] = Grid.CellRight(cell);
			ptr2[num] = 2;
			ptr3[num] = 1;
			num++;
		}
		UtilityConnections connections = physicalGrid[cell].connections;
		UtilityConnections connections2 = visualGrid[cell].connections;
		for (int i = 0; i < num; i++)
		{
			int num2 = ptr[i];
			UtilityConnections utilityConnections = (UtilityConnections)ptr2[i];
			UtilityConnections utilityConnections2 = (UtilityConnections)ptr3[i];
			if ((connections & utilityConnections) != 0)
			{
				if (physicalNodes.Contains(num2))
				{
					physicalGrid[num2].connections |= utilityConnections2;
				}
				if (visualNodes.Contains(num2))
				{
					visualGrid[num2].connections |= utilityConnections2;
				}
			}
			else if ((connections2 & utilityConnections) != 0 && (physicalNodes.Contains(num2) || visualNodes.Contains(num2)))
			{
				visualGrid[num2].connections |= utilityConnections2;
			}
		}
	}

	public void RemoveFromNetworks(int cell, object item, bool is_endpoint)
	{
		if (!Game.IsQuitting())
		{
			dirty = true;
			if (item != null)
			{
				if (is_endpoint)
				{
					endpoints.Remove(cell);
					int networkIdx = physicalGrid[cell].networkIdx;
					if (networkIdx != -1)
					{
						networks[networkIdx].RemoveItem(cell, item);
					}
				}
				else
				{
					int networkIdx2 = physicalGrid[cell].networkIdx;
					physicalGrid[cell].connections = (UtilityConnections)0;
					physicalGrid[cell].networkIdx = -1;
					items.Remove(cell);
					Disconnect(cell);
					if (endpoints.TryGetValue(cell, out object value) && networkIdx2 != -1)
					{
						networks[networkIdx2].DisconnectItem(cell, value);
					}
				}
			}
		}
	}

	private unsafe void Disconnect(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		int num = 0;
		int* ptr = stackalloc int[4];
		int* ptr2 = stackalloc int[4];
		if (vector2I.y < Grid.HeightInCells - 1)
		{
			ptr[num] = Grid.CellAbove(cell);
			ptr2[num] = -9;
			num++;
		}
		if (vector2I.y > 1)
		{
			ptr[num] = Grid.CellBelow(cell);
			ptr2[num] = -5;
			num++;
		}
		if (vector2I.x > 1)
		{
			ptr[num] = Grid.CellLeft(cell);
			ptr2[num] = -3;
			num++;
		}
		if (vector2I.x < Grid.WidthInCells - 1)
		{
			ptr[num] = Grid.CellRight(cell);
			ptr2[num] = -2;
			num++;
		}
		for (int i = 0; i < num; i++)
		{
			int num2 = ptr[i];
			int num3 = ptr2[i];
			int connections = (int)physicalGrid[num2].connections & num3;
			physicalGrid[num2].connections = (UtilityConnections)connections;
		}
	}

	private unsafe void RebuildNetworks(int layer, bool is_physical)
	{
		UtilityNetworkGridNode[] grid = GetGrid(is_physical);
		HashSet<int> nodes = GetNodes(is_physical);
		visitedCells.Clear();
		queued.Clear();
		int* ptr = stackalloc int[4];
		int* ptr2 = stackalloc int[4];
		foreach (int item in nodes)
		{
			UtilityNetworkGridNode utilityNetworkGridNode = grid[item];
			if (!visitedCells.Contains(item))
			{
				queued.Enqueue(item);
				visitedCells.Add(item);
				NetworkType val = new NetworkType();
				val.id = networks.Count;
				networks.Add(val);
				while (queued.Count > 0)
				{
					int num = queued.Dequeue();
					utilityNetworkGridNode = grid[num];
					object value = null;
					object value2 = null;
					if (is_physical)
					{
						if (items.TryGetValue(num, out value))
						{
							if (value is IDisconnectable)
							{
								IDisconnectable disconnectable = value as IDisconnectable;
								if (disconnectable.IsDisconnected())
								{
									continue;
								}
							}
							if (value != null)
							{
								val.AddItem(num, value);
							}
						}
						if (endpoints.TryGetValue(num, out value2) && value2 != null)
						{
							val.AddItem(num, value2);
						}
					}
					grid[num].networkIdx = val.id;
					if (value != null && value2 != null)
					{
						val.ConnectItem(num, value2);
					}
					Vector2I vector2I = Grid.CellToXY(num);
					int num2 = 0;
					if (vector2I.x >= 0)
					{
						ptr[num2] = Grid.CellLeft(num);
						ptr2[num2] = 1;
						num2++;
					}
					if (vector2I.x < Grid.WidthInCells)
					{
						ptr[num2] = Grid.CellRight(num);
						ptr2[num2] = 2;
						num2++;
					}
					if (vector2I.y >= 0)
					{
						ptr[num2] = Grid.CellBelow(num);
						ptr2[num2] = 8;
						num2++;
					}
					if (vector2I.y < Grid.HeightInCells)
					{
						ptr[num2] = Grid.CellAbove(num);
						ptr2[num2] = 4;
						num2++;
					}
					for (int i = 0; i < num2; i++)
					{
						int num3 = ptr2[i];
						if (((int)utilityNetworkGridNode.connections & num3) != 0)
						{
							int dest_cell = ptr[i];
							QueueCellForVisit(grid, dest_cell, (UtilityConnections)num3);
						}
					}
					if (links.TryGetValue(num, out int value3))
					{
						QueueCellForVisit(grid, value3, (UtilityConnections)0);
					}
				}
			}
		}
	}

	public UtilityNetwork GetNetworkByID(int id)
	{
		UtilityNetwork result = null;
		if (0 <= id && id < networks.Count)
		{
			result = networks[id];
		}
		return result;
	}

	public UtilityNetwork GetNetworkForCell(int cell)
	{
		UtilityNetwork result = null;
		if (Grid.IsValidCell(cell) && 0 <= physicalGrid[cell].networkIdx && physicalGrid[cell].networkIdx < networks.Count)
		{
			result = networks[physicalGrid[cell].networkIdx];
		}
		return result;
	}

	public UtilityNetwork GetNetworkForDirection(int cell, Direction direction)
	{
		cell = Grid.GetCellInDirection(cell, direction);
		if (!Grid.IsValidCell(cell))
		{
			return null;
		}
		UtilityNetworkGridNode[] grid = GetGrid(true);
		UtilityNetworkGridNode utilityNetworkGridNode = grid[cell];
		UtilityNetwork result = null;
		if (utilityNetworkGridNode.networkIdx != -1 && utilityNetworkGridNode.networkIdx < networks.Count)
		{
			result = networks[utilityNetworkGridNode.networkIdx];
		}
		return result;
	}

	private UtilityConnections GetNeighboursAsConnections(int cell, HashSet<int> nodes)
	{
		UtilityConnections utilityConnections = (UtilityConnections)0;
		Vector2I vector2I = Grid.CellToXY(cell);
		if (vector2I.x > 1 && nodes.Contains(Grid.CellLeft(cell)))
		{
			utilityConnections |= UtilityConnections.Left;
		}
		if (vector2I.x < Grid.WidthInCells - 1 && nodes.Contains(Grid.CellRight(cell)))
		{
			utilityConnections |= UtilityConnections.Right;
		}
		if (vector2I.y > 1 && nodes.Contains(Grid.CellBelow(cell)))
		{
			utilityConnections |= UtilityConnections.Down;
		}
		if (vector2I.y < Grid.HeightInCells - 1 && nodes.Contains(Grid.CellAbove(cell)))
		{
			utilityConnections |= UtilityConnections.Up;
		}
		return utilityConnections;
	}

	public virtual void SetConnections(UtilityConnections connections, int cell, bool is_physical_building)
	{
		HashSet<int> nodes = GetNodes(is_physical_building);
		nodes.Add(cell);
		visualGrid[cell].connections = connections;
		if (is_physical_building)
		{
			dirty = true;
			UtilityConnections connections2 = (!is_physical_building) ? connections : (connections & GetNeighboursAsConnections(cell, nodes));
			physicalGrid[cell].connections = connections2;
		}
		Reconnect(cell);
	}

	public UtilityConnections GetConnections(int cell, bool is_physical_building)
	{
		UtilityNetworkGridNode[] grid = GetGrid(is_physical_building);
		UtilityConnections utilityConnections = grid[cell].connections;
		if (!is_physical_building)
		{
			grid = GetGrid(true);
			utilityConnections |= grid[cell].connections;
		}
		return utilityConnections;
	}

	public UtilityConnections GetDisplayConnections(int cell)
	{
		UtilityConnections utilityConnections = (UtilityConnections)0;
		UtilityNetworkGridNode[] grid = GetGrid(false);
		utilityConnections |= grid[cell].connections;
		grid = GetGrid(true);
		return utilityConnections | grid[cell].connections;
	}

	public virtual bool CanAddConnection(UtilityConnections new_connection, int cell, bool is_physical_building, out string fail_reason)
	{
		fail_reason = null;
		return true;
	}

	public void AddConnection(UtilityConnections new_connection, int cell, bool is_physical_building)
	{
		if (CanAddConnection(new_connection, cell, is_physical_building, out string _))
		{
			if (is_physical_building)
			{
				dirty = true;
			}
			UtilityNetworkGridNode[] grid = GetGrid(is_physical_building);
			UtilityConnections connections = grid[cell].connections;
			grid[cell].connections = (connections | new_connection);
		}
	}

	public void StashVisualGrids()
	{
		Array.Copy(visualGrid, stashedVisualGrid, visualGrid.Length);
	}

	public void UnstashVisualGrids()
	{
		Array.Copy(stashedVisualGrid, visualGrid, visualGrid.Length);
	}

	public string GetVisualizerString(int cell)
	{
		UtilityConnections displayConnections = GetDisplayConnections(cell);
		return GetVisualizerString(displayConnections);
	}

	public string GetVisualizerString(UtilityConnections connections)
	{
		string text = string.Empty;
		if ((connections & UtilityConnections.Left) != 0)
		{
			text += "L";
		}
		if ((connections & UtilityConnections.Right) != 0)
		{
			text += "R";
		}
		if ((connections & UtilityConnections.Up) != 0)
		{
			text += "U";
		}
		if ((connections & UtilityConnections.Down) != 0)
		{
			text += "D";
		}
		if (text == string.Empty)
		{
			text = "None";
		}
		return text;
	}

	public object GetEndpoint(int cell)
	{
		object value = null;
		endpoints.TryGetValue(cell, out value);
		return value;
	}

	public void AddLink(int cell1, int cell2)
	{
		links[cell1] = cell2;
		links[cell2] = cell1;
		dirty = true;
	}

	public void RemoveLink(int cell1, int cell2)
	{
		links.Remove(cell1);
		links.Remove(cell2);
		dirty = true;
	}

	public void AddNetworksRebuiltListener(Action<IList<UtilityNetwork>, ICollection<int>> listener)
	{
		onNetworksRebuilt = (Action<IList<UtilityNetwork>, ICollection<int>>)Delegate.Combine(onNetworksRebuilt, listener);
	}

	public void RemoveNetworksRebuiltListener(Action<IList<UtilityNetwork>, ICollection<int>> listener)
	{
		onNetworksRebuilt = (Action<IList<UtilityNetwork>, ICollection<int>>)Delegate.Remove(onNetworksRebuilt, listener);
	}

	public IList<UtilityNetwork> GetNetworks()
	{
		return networks;
	}
}
