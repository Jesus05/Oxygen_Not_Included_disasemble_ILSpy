using System.Collections.Generic;
using UnityEngine;

public class FlowUtilityNetwork : UtilityNetwork
{
	public interface IItem
	{
		int Cell
		{
			get;
		}

		FlowUtilityNetwork Network
		{
			set;
		}

		Endpoint EndpointType
		{
			get;
		}

		ConduitType ConduitType
		{
			get;
		}

		GameObject GameObject
		{
			get;
		}
	}

	public class NetworkItem : IItem
	{
		private int cell;

		private FlowUtilityNetwork network;

		private Endpoint endpointType;

		private ConduitType conduitType;

		private GameObject parent;

		public Endpoint EndpointType => endpointType;

		public ConduitType ConduitType => conduitType;

		public int Cell => cell;

		public FlowUtilityNetwork Network
		{
			get
			{
				return network;
			}
			set
			{
				network = value;
			}
		}

		public GameObject GameObject => parent;

		public NetworkItem(ConduitType conduit_type, Endpoint endpoint_type, int cell, GameObject parent)
		{
			conduitType = conduit_type;
			endpointType = endpoint_type;
			this.cell = cell;
			this.parent = parent;
		}
	}

	public List<IItem> sources = new List<IItem>();

	public List<IItem> sinks = new List<IItem>();

	private List<IItem> conduits = new List<IItem>();

	public bool HasSinks => sinks.Count > 0;

	public int GetActiveCount()
	{
		return sinks.Count;
	}

	public override void AddItem(int cell, object generic_item)
	{
		IItem item = (IItem)generic_item;
		if (item != null)
		{
			switch (item.EndpointType)
			{
			case Endpoint.Conduit:
				break;
			case Endpoint.Source:
				if (!sources.Contains(item))
				{
					sources.Add(item);
					item.Network = this;
				}
				break;
			case Endpoint.Sink:
				if (!sinks.Contains(item))
				{
					sinks.Add(item);
					item.Network = this;
				}
				break;
			default:
				item.Network = this;
				break;
			}
		}
	}

	public override void Reset(UtilityNetworkGridNode[] grid)
	{
		for (int i = 0; i < sinks.Count; i++)
		{
			IItem item = sinks[i];
			item.Network = null;
			UtilityNetworkGridNode utilityNetworkGridNode = grid[item.Cell];
			utilityNetworkGridNode.networkIdx = -1;
			grid[item.Cell] = utilityNetworkGridNode;
		}
		for (int j = 0; j < sources.Count; j++)
		{
			IItem item2 = sources[j];
			item2.Network = null;
			UtilityNetworkGridNode utilityNetworkGridNode2 = grid[item2.Cell];
			utilityNetworkGridNode2.networkIdx = -1;
			grid[item2.Cell] = utilityNetworkGridNode2;
		}
		for (int k = 0; k < conduits.Count; k++)
		{
			IItem item3 = conduits[k];
			item3.Network = null;
			UtilityNetworkGridNode utilityNetworkGridNode3 = grid[item3.Cell];
			utilityNetworkGridNode3.networkIdx = -1;
			grid[item3.Cell] = utilityNetworkGridNode3;
		}
	}
}
