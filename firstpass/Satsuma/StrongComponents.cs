using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class StrongComponents
	{
		[Flags]
		public enum Flags
		{
			None = 0x0,
			CreateComponents = 0x1
		}

		private class ForwardDfs : Dfs
		{
			public List<Node> ReverseExitOrder;

			protected override void Start(out Direction direction)
			{
				direction = Direction.Forward;
				ReverseExitOrder = new List<Node>();
			}

			protected override bool NodeExit(Node node, Arc arc)
			{
				ReverseExitOrder.Add(node);
				return true;
			}

			protected override void StopSearch()
			{
				ReverseExitOrder.Reverse();
			}
		}

		private class BackwardDfs : Dfs
		{
			public StrongComponents Parent;

			protected override void Start(out Direction direction)
			{
				direction = Direction.Backward;
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
					Parent.Components[Parent.Components.Count - 1].Add(node);
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

		public StrongComponents(IGraph graph, Flags flags = Flags.None)
		{
			Graph = graph;
			if ((flags & Flags.CreateComponents) != 0)
			{
				Components = new List<HashSet<Node>>();
			}
			ForwardDfs forwardDfs = new ForwardDfs();
			forwardDfs.Run(graph, null);
			BackwardDfs backwardDfs = new BackwardDfs
			{
				Parent = this
			};
			backwardDfs.Run(graph, forwardDfs.ReverseExitOrder);
		}
	}
}
