using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Matching : IMatching, IClearable, IGraph, IArcLookup
	{
		private readonly Dictionary<Node, Arc> matchedArc;

		private readonly HashSet<Arc> arcs;

		private int edgeCount;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Matching(IGraph graph)
		{
			Graph = graph;
			matchedArc = new Dictionary<Node, Arc>();
			arcs = new HashSet<Arc>();
			Clear();
		}

		public void Clear()
		{
			matchedArc.Clear();
			arcs.Clear();
			edgeCount = 0;
		}

		public void Enable(Arc arc, bool enabled)
		{
			if (enabled != arcs.Contains(arc))
			{
				Node node = Graph.U(arc);
				Node node2 = Graph.V(arc);
				if (enabled)
				{
					if (node == node2)
					{
						throw new ArgumentException("Matchings cannot have loop arcs.");
					}
					if (matchedArc.ContainsKey(node))
					{
						throw new ArgumentException("Node is already matched: " + node);
					}
					if (matchedArc.ContainsKey(node2))
					{
						throw new ArgumentException("Node is already matched: " + node2);
					}
					matchedArc[node] = arc;
					matchedArc[node2] = arc;
					arcs.Add(arc);
					if (Graph.IsEdge(arc))
					{
						edgeCount++;
					}
				}
				else
				{
					matchedArc.Remove(node);
					matchedArc.Remove(node2);
					arcs.Remove(arc);
					if (Graph.IsEdge(arc))
					{
						edgeCount--;
					}
				}
			}
		}

		public Arc MatchedArc(Node node)
		{
			Arc value;
			return (!matchedArc.TryGetValue(node, out value)) ? Arc.Invalid : value;
		}

		public Node U(Arc arc)
		{
			return Graph.U(arc);
		}

		public Node V(Arc arc)
		{
			return Graph.V(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return Graph.IsEdge(arc);
		}

		public IEnumerable<Node> Nodes()
		{
			return matchedArc.Keys;
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (filter != 0)
			{
				if (edgeCount != 0)
				{
					return from arc in arcs
					where IsEdge(arc)
					select arc;
				}
				return Enumerable.Empty<Arc>();
			}
			return arcs;
		}

		private bool YieldArc(Node u, ArcFilter filter, Arc arc)
		{
			return filter == ArcFilter.All || IsEdge(arc) || (filter == ArcFilter.Forward && U(arc) == u) || (filter == ArcFilter.Backward && V(arc) == u);
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			Arc arc = MatchedArc(u);
			if (arc != Arc.Invalid && YieldArc(u, filter, arc))
			{
				yield return arc;
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (u != v)
			{
				Arc arc = MatchedArc(u);
				if (arc != Arc.Invalid && arc == MatchedArc(v) && YieldArc(u, filter, arc))
				{
					yield return arc;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}

		public int NodeCount()
		{
			return matchedArc.Count;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return (filter != 0) ? edgeCount : arcs.Count;
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			Arc arc = MatchedArc(u);
			return (arc != Arc.Invalid && YieldArc(u, filter, arc)) ? 1 : 0;
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (!(u != v))
			{
				return 0;
			}
			Arc arc = MatchedArc(u);
			return (arc != Arc.Invalid && arc == MatchedArc(v) && YieldArc(u, filter, arc)) ? 1 : 0;
		}

		public bool HasNode(Node node)
		{
			return Graph.HasNode(node) && matchedArc.ContainsKey(node);
		}

		public bool HasArc(Arc arc)
		{
			return Graph.HasArc(arc) && arcs.Contains(arc);
		}
	}
}
