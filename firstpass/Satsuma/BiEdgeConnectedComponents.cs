using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class BiEdgeConnectedComponents
	{
		[Flags]
		public enum Flags
		{
			None = 0x0,
			CreateComponents = 0x1,
			CreateBridges = 0x2
		}

		public IGraph Graph
		{
			get;
			private set;
		}

		public int Count
		{
			get;
			private set;
		}

		public List<HashSet<Node>> Components
		{
			get;
			private set;
		}

		public HashSet<Arc> Bridges
		{
			get;
			private set;
		}

		public BiEdgeConnectedComponents(IGraph graph, Flags flags = Flags.None)
		{
			Graph = graph;
			BridgeDfs bridgeDfs = new BridgeDfs();
			bridgeDfs.Run(graph, null);
			Count = bridgeDfs.ComponentCount;
			if ((flags & Flags.CreateBridges) != 0)
			{
				Bridges = bridgeDfs.Bridges;
			}
			if ((flags & Flags.CreateComponents) != 0)
			{
				Subgraph subgraph = new Subgraph(graph);
				foreach (Arc bridge in bridgeDfs.Bridges)
				{
					subgraph.Enable(bridge, false);
				}
				Components = new ConnectedComponents(subgraph, ConnectedComponents.Flags.CreateComponents).Components;
			}
		}
	}
}
