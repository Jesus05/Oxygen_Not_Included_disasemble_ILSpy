using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class RedirectedGraph : IGraph, IArcLookup
	{
		public enum Direction
		{
			Forward,
			Backward,
			Edge
		}

		private IGraph graph;

		private Func<Arc, Direction> getDirection;

		public RedirectedGraph(IGraph graph, Func<Arc, Direction> getDirection)
		{
			this.graph = graph;
			this.getDirection = getDirection;
		}

		public Node U(Arc arc)
		{
			return (getDirection(arc) != Direction.Backward) ? graph.U(arc) : graph.V(arc);
		}

		public Node V(Arc arc)
		{
			return (getDirection(arc) != Direction.Backward) ? graph.V(arc) : graph.U(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return getDirection(arc) == Direction.Edge;
		}

		public IEnumerable<Node> Nodes()
		{
			return graph.Nodes();
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			return (filter != 0) ? (from x in graph.Arcs(ArcFilter.All)
			where getDirection(x) == Direction.Edge
			select x) : graph.Arcs(ArcFilter.All);
		}

		private IEnumerable<Arc> FilterArcs(Node u, IEnumerable<Arc> arcs, ArcFilter filter)
		{
			switch (filter)
			{
			case ArcFilter.All:
				return arcs;
			case ArcFilter.Edge:
				return from x in arcs
				where getDirection(x) == Direction.Edge
				select x;
			case ArcFilter.Forward:
				return arcs.Where(delegate(Arc x)
				{
					switch (getDirection(x))
					{
					case Direction.Forward:
						return U(x) == u;
					case Direction.Backward:
						return V(x) == u;
					default:
						return true;
					}
				});
			default:
				return arcs.Where(delegate(Arc x)
				{
					switch (getDirection(x))
					{
					case Direction.Forward:
						return V(x) == u;
					case Direction.Backward:
						return U(x) == u;
					default:
						return true;
					}
				});
			}
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return FilterArcs(u, graph.Arcs(u, ArcFilter.All), filter);
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return FilterArcs(u, graph.Arcs(u, v, ArcFilter.All), filter);
		}

		public int NodeCount()
		{
			return graph.NodeCount();
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return (filter != 0) ? Arcs(filter).Count() : graph.ArcCount(ArcFilter.All);
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
			return graph.HasNode(node);
		}

		public bool HasArc(Arc arc)
		{
			return graph.HasArc(arc);
		}
	}
}
