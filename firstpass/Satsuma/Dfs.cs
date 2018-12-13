using System.Collections.Generic;

namespace Satsuma
{
	public abstract class Dfs
	{
		public enum Direction
		{
			Undirected,
			Forward,
			Backward
		}

		private HashSet<Node> traversed;

		private ArcFilter arcFilter;

		protected IGraph Graph
		{
			get;
			private set;
		}

		protected int Level
		{
			get;
			private set;
		}

		public void Run(IGraph graph, IEnumerable<Node> roots = null)
		{
			Graph = graph;
			Start(out Direction direction);
			switch (direction)
			{
			case Direction.Undirected:
				arcFilter = ArcFilter.All;
				break;
			case Direction.Forward:
				arcFilter = ArcFilter.Forward;
				break;
			default:
				arcFilter = ArcFilter.Backward;
				break;
			}
			traversed = new HashSet<Node>();
			foreach (Node item in roots ?? Graph.Nodes())
			{
				if (!traversed.Contains(item))
				{
					Level = 0;
					if (!Traverse(item, Arc.Invalid))
					{
						break;
					}
				}
			}
			traversed = null;
			StopSearch();
		}

		private bool Traverse(Node node, Arc arc)
		{
			traversed.Add(node);
			if (!NodeEnter(node, arc))
			{
				return false;
			}
			foreach (Arc item in Graph.Arcs(node, arcFilter))
			{
				if (!(item == arc))
				{
					Node node2 = Graph.Other(item, node);
					if (traversed.Contains(node2))
					{
						if (!BackArc(node, item))
						{
							return false;
						}
					}
					else
					{
						Level++;
						if (!Traverse(node2, item))
						{
							return false;
						}
						Level--;
					}
				}
			}
			return NodeExit(node, arc);
		}

		protected abstract void Start(out Direction direction);

		protected virtual bool NodeEnter(Node node, Arc arc)
		{
			return true;
		}

		protected virtual bool NodeExit(Node node, Arc arc)
		{
			return true;
		}

		protected virtual bool BackArc(Node node, Arc arc)
		{
			return true;
		}

		protected virtual void StopSearch()
		{
		}
	}
}
