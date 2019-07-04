using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class ContractedGraph : IGraph, IArcLookup
	{
		private IGraph graph;

		private DisjointSet<Node> nodeGroups;

		private int unionCount;

		public ContractedGraph(IGraph graph)
		{
			this.graph = graph;
			nodeGroups = new DisjointSet<Node>();
			Reset();
		}

		public void Reset()
		{
			nodeGroups.Clear();
			unionCount = 0;
		}

		public Node Merge(Node u, Node v)
		{
			DisjointSetSet<Node> a = nodeGroups.WhereIs(u);
			DisjointSetSet<Node> disjointSetSet = nodeGroups.WhereIs(v);
			if (!a.Equals(disjointSetSet))
			{
				unionCount++;
				return nodeGroups.Union(a, disjointSetSet).Representative;
			}
			return a.Representative;
		}

		public Node Contract(Arc arc)
		{
			return Merge(graph.U(arc), graph.V(arc));
		}

		public Node U(Arc arc)
		{
			return nodeGroups.WhereIs(graph.U(arc)).Representative;
		}

		public Node V(Arc arc)
		{
			return nodeGroups.WhereIs(graph.V(arc)).Representative;
		}

		public bool IsEdge(Arc arc)
		{
			return graph.IsEdge(arc);
		}

		public IEnumerable<Node> Nodes()
		{
			foreach (Node item in graph.Nodes())
			{
				if (nodeGroups.WhereIs(item).Representative == item)
				{
					yield return item;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			yield break;
			IL_00ee:
			/*Error near IL_00ef: Unexpected return in MoveNext()*/;
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			return graph.Arcs(filter);
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			DisjointSetSet<Node> x = nodeGroups.WhereIs(u);
			foreach (Node item in nodeGroups.Elements(x))
			{
				foreach (Arc item2 in graph.Arcs(item, filter))
				{
					if (!(U(item2) == V(item2)) || (filter != 0 && !IsEdge(item2)) || graph.U(item2) == item)
					{
						yield return item2;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_01e2:
			/*Error near IL_01e3: Unexpected return in MoveNext()*/;
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			foreach (Arc item in Arcs(u, filter))
			{
				if (this.Other(item, u) == v)
				{
					yield return item;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			yield break;
			IL_00ee:
			/*Error near IL_00ef: Unexpected return in MoveNext()*/;
		}

		public int NodeCount()
		{
			return graph.NodeCount() - unionCount;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return graph.ArcCount(filter);
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
			return node == nodeGroups.WhereIs(node).Representative;
		}

		public bool HasArc(Arc arc)
		{
			return graph.HasArc(arc);
		}
	}
}
