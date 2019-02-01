using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class Bipartition
	{
		[Flags]
		public enum Flags
		{
			None = 0x0,
			CreateRedNodes = 0x1,
			CreateBlueNodes = 0x2
		}

		private class MyDfs : Dfs
		{
			public Bipartition Parent;

			private HashSet<Node> redNodes;

			protected override void Start(out Direction direction)
			{
				direction = Direction.Undirected;
				Parent.Bipartite = true;
				redNodes = (Parent.RedNodes ?? new HashSet<Node>());
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if ((base.Level & 1) == 0)
				{
					redNodes.Add(node);
				}
				else if (Parent.BlueNodes != null)
				{
					Parent.BlueNodes.Add(node);
				}
				return true;
			}

			protected override bool BackArc(Node node, Arc arc)
			{
				Node item = base.Graph.Other(arc, node);
				if (redNodes.Contains(node) != redNodes.Contains(item))
				{
					return true;
				}
				Parent.Bipartite = false;
				if (Parent.RedNodes != null)
				{
					Parent.RedNodes.Clear();
				}
				if (Parent.BlueNodes != null)
				{
					Parent.BlueNodes.Clear();
				}
				return false;
			}
		}

		public IGraph Graph
		{
			get;
			private set;
		}

		public bool Bipartite
		{
			get;
			private set;
		}

		public HashSet<Node> RedNodes
		{
			get;
			private set;
		}

		public HashSet<Node> BlueNodes
		{
			get;
			private set;
		}

		public Bipartition(IGraph graph, Flags flags = Flags.None)
		{
			Graph = graph;
			if ((flags & Flags.CreateRedNodes) != 0)
			{
				RedNodes = new HashSet<Node>();
			}
			if ((flags & Flags.CreateBlueNodes) != 0)
			{
				BlueNodes = new HashSet<Node>();
			}
			MyDfs myDfs = new MyDfs();
			myDfs.Parent = this;
			myDfs.Run(graph, null);
		}
	}
}
