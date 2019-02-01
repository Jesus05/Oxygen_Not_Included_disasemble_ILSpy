using System;
using System.Collections.Generic;

namespace Satsuma
{
	public class BiNodeConnectedComponents
	{
		[Flags]
		public enum Flags
		{
			None = 0x0,
			CreateComponents = 0x1,
			CreateCutvertices = 0x2
		}

		private class BlockDfs : LowpointDfs
		{
			public BiNodeConnectedComponents Parent;

			private Stack<Node> blockStack;

			private bool oneNodeComponent;

			protected override void Start(out Direction direction)
			{
				base.Start(out direction);
				if (Parent.Components != null)
				{
					blockStack = new Stack<Node>();
				}
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (base.NodeEnter(node, arc))
				{
					if (Parent.Cutvertices != null && arc == Arc.Invalid)
					{
						Parent.Cutvertices[node] = -1;
					}
					if (Parent.Components != null)
					{
						blockStack.Push(node);
					}
					oneNodeComponent = (arc == Arc.Invalid);
					return true;
				}
				return false;
			}

			protected override bool NodeExit(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					if (oneNodeComponent)
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
					if (Parent.Cutvertices != null && Parent.Cutvertices[node] == 0)
					{
						Parent.Cutvertices.Remove(node);
					}
					if (Parent.Components != null)
					{
						blockStack.Clear();
					}
				}
				else
				{
					Node node2 = base.Graph.Other(arc, node);
					if (lowpoint[node] >= base.Level - 1)
					{
						if (Parent.Cutvertices != null)
						{
							Parent.Cutvertices[node2] = (Parent.Cutvertices.TryGetValue(node2, out int value) ? value : 0) + 1;
						}
						Parent.Count++;
						if (Parent.Components != null)
						{
							HashSet<Node> hashSet = new HashSet<Node>();
							Node node3;
							do
							{
								node3 = blockStack.Pop();
								hashSet.Add(node3);
							}
							while (!(node3 == node));
							hashSet.Add(node2);
							Parent.Components.Add(hashSet);
						}
					}
				}
				return base.NodeExit(node, arc);
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

		public Dictionary<Node, int> Cutvertices
		{
			get;
			private set;
		}

		public BiNodeConnectedComponents(IGraph graph, Flags flags = Flags.None)
		{
			Graph = graph;
			if ((flags & Flags.CreateComponents) != 0)
			{
				Components = new List<HashSet<Node>>();
			}
			if ((flags & Flags.CreateCutvertices) != 0)
			{
				Cutvertices = new Dictionary<Node, int>();
			}
			BlockDfs blockDfs = new BlockDfs();
			blockDfs.Parent = this;
			blockDfs.Run(graph, null);
		}
	}
}
