using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class PathGraph : IPath, IGraph, IArcLookup
	{
		public enum Topology
		{
			Path,
			Cycle
		}

		private readonly int nodeCount;

		private readonly bool isCycle;

		private readonly bool directed;

		public Node FirstNode => (nodeCount <= 0) ? Node.Invalid : new Node(1L);

		public Node LastNode => (nodeCount <= 0) ? Node.Invalid : new Node(isCycle ? 1 : nodeCount);

		public PathGraph(int nodeCount, Topology topology, Directedness directedness)
		{
			this.nodeCount = nodeCount;
			isCycle = (topology == Topology.Cycle);
			directed = (directedness == Directedness.Directed);
		}

		public Node GetNode(int index)
		{
			return new Node(1L + (long)index);
		}

		public int GetNodeIndex(Node node)
		{
			return (int)(node.Id - 1);
		}

		public Arc NextArc(Node node)
		{
			if (!isCycle && node.Id == nodeCount)
			{
				return Arc.Invalid;
			}
			return new Arc(node.Id);
		}

		public Arc PrevArc(Node node)
		{
			if (node.Id == 1)
			{
				return (!isCycle) ? Arc.Invalid : new Arc(nodeCount);
			}
			return new Arc(node.Id - 1);
		}

		public Node U(Arc arc)
		{
			return new Node(arc.Id);
		}

		public Node V(Arc arc)
		{
			return new Node((arc.Id != nodeCount) ? (arc.Id + 1) : 1);
		}

		public bool IsEdge(Arc arc)
		{
			return !directed;
		}

		public IEnumerable<Node> Nodes()
		{
			int i = 1;
			if (i <= nodeCount)
			{
				yield return new Node(i);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (!directed || filter != ArcFilter.Edge)
			{
				int j = 1;
				int i = ArcCountInternal();
				if (j <= i)
				{
					yield return new Arc(j);
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return this.ArcsHelper(u, filter);
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return from arc in Arcs(u, filter)
			where this.Other(arc, u) == v
			select arc;
		}

		public int NodeCount()
		{
			return nodeCount;
		}

		private int ArcCountInternal()
		{
			return (nodeCount != 0) ? ((!isCycle) ? (nodeCount - 1) : nodeCount) : 0;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return (!directed || filter != ArcFilter.Edge) ? ArcCountInternal() : 0;
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			return Arcs(u, filter).Count();
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return Arcs(u, v, filter).Count();
		}

		public bool HasNode(Node node)
		{
			return node.Id >= 1 && node.Id <= nodeCount;
		}

		public bool HasArc(Arc arc)
		{
			return arc.Id >= 1 && arc.Id <= ArcCountInternal();
		}
	}
}
