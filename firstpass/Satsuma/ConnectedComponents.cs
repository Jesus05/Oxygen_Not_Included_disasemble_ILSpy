using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class ConnectedComponents
	{
		[Flags]
		public enum Flags
		{
			None = 0x0,
			CreateComponents = 0x1
		}

		private class MyDfs : Dfs
		{
			public ConnectedComponents Parent;

			protected override void Start(out Direction direction)
			{
				direction = Direction.Undirected;
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					Parent.Count++;
					if (Parent.Components != null)
					{
						Parent.Components.Add(new HashSet<Node>
						{
							node
						});
					}
				}
				else if (Parent.Components != null)
				{
					Parent.Components[Parent.Count - 1].Add(node);
				}
				return true;
			}
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

		public ConnectedComponents(IGraph graph, Flags flags = Flags.None)
		{
			Graph = graph;
			if ((flags & Flags.CreateComponents) != 0)
			{
				Components = new List<HashSet<Node>>();
			}
			MyDfs myDfs = new MyDfs();
			myDfs.Parent = this;
			myDfs.Run(graph, null);
		}
	}
}
