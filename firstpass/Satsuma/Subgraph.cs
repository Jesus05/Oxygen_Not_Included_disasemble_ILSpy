using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Subgraph : IGraph, IArcLookup
	{
		private IGraph graph;

		private bool defaultNodeEnabled;

		private HashSet<Node> nodeExceptions = new HashSet<Node>();

		private bool defaultArcEnabled;

		private HashSet<Arc> arcExceptions = new HashSet<Arc>();

		public Subgraph(IGraph graph)
		{
			this.graph = graph;
			EnableAllNodes(true);
			EnableAllArcs(true);
		}

		public void EnableAllNodes(bool enabled)
		{
			defaultNodeEnabled = enabled;
			nodeExceptions.Clear();
		}

		public void EnableAllArcs(bool enabled)
		{
			defaultArcEnabled = enabled;
			arcExceptions.Clear();
		}

		public void Enable(Node node, bool enabled)
		{
			if (defaultNodeEnabled != enabled)
			{
				nodeExceptions.Add(node);
			}
			else
			{
				nodeExceptions.Remove(node);
			}
		}

		public void Enable(Arc arc, bool enabled)
		{
			if (defaultArcEnabled != enabled)
			{
				arcExceptions.Add(arc);
			}
			else
			{
				arcExceptions.Remove(arc);
			}
		}

		public bool IsEnabled(Node node)
		{
			return defaultNodeEnabled ^ nodeExceptions.Contains(node);
		}

		public bool IsEnabled(Arc arc)
		{
			return defaultArcEnabled ^ arcExceptions.Contains(arc);
		}

		public Node U(Arc arc)
		{
			return graph.U(arc);
		}

		public Node V(Arc arc)
		{
			return graph.V(arc);
		}

		public bool IsEdge(Arc arc)
		{
			return graph.IsEdge(arc);
		}

		private IEnumerable<Node> NodesInternal()
		{
			foreach (Node item in graph.Nodes())
			{
				if (IsEnabled(item))
				{
					yield return item;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			yield break;
			IL_00d4:
			/*Error near IL_00d5: Unexpected return in MoveNext()*/;
		}

		public IEnumerable<Node> Nodes()
		{
			if (nodeExceptions.Count == 0)
			{
				if (defaultNodeEnabled)
				{
					return graph.Nodes();
				}
				return Enumerable.Empty<Node>();
			}
			return NodesInternal();
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			foreach (Arc item in graph.Arcs(filter))
			{
				if (IsEnabled(item) && IsEnabled(graph.U(item)) && IsEnabled(graph.V(item)))
				{
					yield return item;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			yield break;
			IL_0126:
			/*Error near IL_0127: Unexpected return in MoveNext()*/;
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (IsEnabled(u))
			{
				foreach (Arc item in graph.Arcs(u, filter))
				{
					if (IsEnabled(item) && IsEnabled(graph.Other(item, u)))
					{
						yield return item;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_0127:
			/*Error near IL_0128: Unexpected return in MoveNext()*/;
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (IsEnabled(u) && IsEnabled(v))
			{
				foreach (Arc item in graph.Arcs(u, v, filter))
				{
					if (IsEnabled(item))
					{
						yield return item;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_0117:
			/*Error near IL_0118: Unexpected return in MoveNext()*/;
		}

		public int NodeCount()
		{
			return (!defaultNodeEnabled) ? nodeExceptions.Count : (graph.NodeCount() - nodeExceptions.Count);
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (nodeExceptions.Count == 0 && filter == ArcFilter.All)
			{
				return defaultNodeEnabled ? ((!defaultArcEnabled) ? arcExceptions.Count : (graph.ArcCount(ArcFilter.All) - arcExceptions.Count)) : 0;
			}
			return Arcs(filter).Count();
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
			return graph.HasNode(node) && IsEnabled(node);
		}

		public bool HasArc(Arc arc)
		{
			return graph.HasArc(arc) && IsEnabled(arc);
		}
	}
}
