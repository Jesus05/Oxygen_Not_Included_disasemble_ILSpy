using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class TopologicalOrder
	{
		[Flags]
		public enum Flags
		{
			None = 0x0,
			CreateOrder = 0x1
		}

		private class MyDfs : Dfs
		{
			public TopologicalOrder Parent;

			private HashSet<Node> exited;

			protected override void Start(out Direction direction)
			{
				direction = Direction.Forward;
				Parent.Acyclic = true;
				exited = new HashSet<Node>();
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (arc != Arc.Invalid && base.Graph.IsEdge(arc))
				{
					Parent.Acyclic = false;
					return false;
				}
				return true;
			}

			protected override bool NodeExit(Node node, Arc arc)
			{
				if (Parent.Order != null)
				{
					Parent.Order.Add(node);
				}
				exited.Add(node);
				return true;
			}

			protected override bool BackArc(Node node, Arc arc)
			{
				Node item = base.Graph.Other(arc, node);
				if (!exited.Contains(item))
				{
					Parent.Acyclic = false;
					return false;
				}
				return true;
			}

			protected override void StopSearch()
			{
				if (Parent.Order != null)
				{
					if (Parent.Acyclic)
					{
						Parent.Order.Reverse();
					}
					else
					{
						Parent.Order.Clear();
					}
				}
			}
		}

		public IGraph Graph
		{
			get;
			private set;
		}

		public bool Acyclic
		{
			get;
			private set;
		}

		public List<Node> Order
		{
			get;
			private set;
		}

		public TopologicalOrder(IGraph graph, Flags flags = Flags.None)
		{
			Graph = graph;
			if ((flags & Flags.CreateOrder) != 0)
			{
				Order = new List<Node>();
			}
			MyDfs myDfs = new MyDfs();
			myDfs.Parent = this;
			myDfs.Run(graph, null);
		}
	}
}
