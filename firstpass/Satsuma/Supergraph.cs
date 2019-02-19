using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public class Supergraph : IBuildableGraph, IDestroyableGraph, IGraph, IClearable, IArcLookup
	{
		private class NodeAllocator : IdAllocator
		{
			public Supergraph Parent;

			protected override bool IsAllocated(long id)
			{
				return Parent.HasNode(new Node(id));
			}
		}

		private class ArcAllocator : IdAllocator
		{
			public Supergraph Parent;

			protected override bool IsAllocated(long id)
			{
				return Parent.HasArc(new Arc(id));
			}
		}

		private class ArcProperties
		{
			public Node U
			{
				get;
				private set;
			}

			public Node V
			{
				get;
				private set;
			}

			public bool IsEdge
			{
				get;
				private set;
			}

			public ArcProperties(Node u, Node v, bool isEdge)
			{
				U = u;
				V = v;
				IsEdge = isEdge;
			}
		}

		private IGraph graph;

		private NodeAllocator nodeAllocator;

		private ArcAllocator arcAllocator;

		private HashSet<Node> nodes;

		private HashSet<Arc> arcs;

		private Dictionary<Arc, ArcProperties> arcProperties;

		private HashSet<Arc> edges;

		private Dictionary<Node, List<Arc>> nodeArcs_All;

		private Dictionary<Node, List<Arc>> nodeArcs_Edge;

		private Dictionary<Node, List<Arc>> nodeArcs_Forward;

		private Dictionary<Node, List<Arc>> nodeArcs_Backward;

		private static readonly List<Arc> EmptyArcList = new List<Arc>();

		public Supergraph(IGraph graph)
		{
			this.graph = graph;
			nodeAllocator = new NodeAllocator
			{
				Parent = this
			};
			arcAllocator = new ArcAllocator
			{
				Parent = this
			};
			nodes = new HashSet<Node>();
			arcs = new HashSet<Arc>();
			arcProperties = new Dictionary<Arc, ArcProperties>();
			edges = new HashSet<Arc>();
			nodeArcs_All = new Dictionary<Node, List<Arc>>();
			nodeArcs_Edge = new Dictionary<Node, List<Arc>>();
			nodeArcs_Forward = new Dictionary<Node, List<Arc>>();
			nodeArcs_Backward = new Dictionary<Node, List<Arc>>();
		}

		public void Clear()
		{
			nodeAllocator.Rewind();
			arcAllocator.Rewind();
			nodes.Clear();
			arcs.Clear();
			arcProperties.Clear();
			edges.Clear();
			nodeArcs_All.Clear();
			nodeArcs_Edge.Clear();
			nodeArcs_Forward.Clear();
			nodeArcs_Backward.Clear();
		}

		public Node AddNode()
		{
			if (NodeCount() == 2147483647)
			{
				throw new InvalidOperationException("Error: too many nodes!");
			}
			Node node = new Node(nodeAllocator.Allocate());
			nodes.Add(node);
			return node;
		}

		public Arc AddArc(Node u, Node v, Directedness directedness)
		{
			if (ArcCount(ArcFilter.All) == 2147483647)
			{
				throw new InvalidOperationException("Error: too many arcs!");
			}
			Arc arc = new Arc(arcAllocator.Allocate());
			arcs.Add(arc);
			bool flag = directedness == Directedness.Undirected;
			arcProperties[arc] = new ArcProperties(u, v, flag);
			Utils.MakeEntry(nodeArcs_All, u).Add(arc);
			Utils.MakeEntry(nodeArcs_Forward, u).Add(arc);
			Utils.MakeEntry(nodeArcs_Backward, v).Add(arc);
			if (flag)
			{
				edges.Add(arc);
				Utils.MakeEntry(nodeArcs_Edge, u).Add(arc);
			}
			if (v != u)
			{
				Utils.MakeEntry(nodeArcs_All, v).Add(arc);
				if (flag)
				{
					Utils.MakeEntry(nodeArcs_Edge, v).Add(arc);
					Utils.MakeEntry(nodeArcs_Forward, v).Add(arc);
					Utils.MakeEntry(nodeArcs_Backward, u).Add(arc);
				}
			}
			return arc;
		}

		public bool DeleteNode(Node node)
		{
			if (!nodes.Remove(node))
			{
				return false;
			}
			Func<Arc, bool> condition = (Arc a) => U(a) == node || V(a) == node;
			Utils.RemoveAll(arcs, condition);
			Utils.RemoveAll(edges, condition);
			Utils.RemoveAll(arcProperties, condition);
			nodeArcs_All.Remove(node);
			nodeArcs_Edge.Remove(node);
			nodeArcs_Forward.Remove(node);
			nodeArcs_Backward.Remove(node);
			return true;
		}

		public bool DeleteArc(Arc arc)
		{
			if (!arcs.Remove(arc))
			{
				return false;
			}
			ArcProperties arcProperties = this.arcProperties[arc];
			this.arcProperties.Remove(arc);
			Utils.RemoveLast(nodeArcs_All[arcProperties.U], arc);
			Utils.RemoveLast(nodeArcs_Forward[arcProperties.U], arc);
			Utils.RemoveLast(nodeArcs_Backward[arcProperties.V], arc);
			if (arcProperties.IsEdge)
			{
				edges.Remove(arc);
				Utils.RemoveLast(nodeArcs_Edge[arcProperties.U], arc);
			}
			if (arcProperties.V != arcProperties.U)
			{
				Utils.RemoveLast(nodeArcs_All[arcProperties.V], arc);
				if (arcProperties.IsEdge)
				{
					Utils.RemoveLast(nodeArcs_Edge[arcProperties.V], arc);
					Utils.RemoveLast(nodeArcs_Forward[arcProperties.V], arc);
					Utils.RemoveLast(nodeArcs_Backward[arcProperties.U], arc);
				}
			}
			return true;
		}

		public Node U(Arc arc)
		{
			if (arcProperties.TryGetValue(arc, out ArcProperties value))
			{
				return value.U;
			}
			return graph.U(arc);
		}

		public Node V(Arc arc)
		{
			if (arcProperties.TryGetValue(arc, out ArcProperties value))
			{
				return value.V;
			}
			return graph.V(arc);
		}

		public bool IsEdge(Arc arc)
		{
			if (arcProperties.TryGetValue(arc, out ArcProperties value))
			{
				return value.IsEdge;
			}
			return graph.IsEdge(arc);
		}

		private HashSet<Arc> ArcsInternal(ArcFilter filter)
		{
			return (filter != 0) ? edges : arcs;
		}

		private List<Arc> ArcsInternal(Node v, ArcFilter filter)
		{
			List<Arc> value;
			switch (filter)
			{
			case ArcFilter.All:
				nodeArcs_All.TryGetValue(v, out value);
				break;
			case ArcFilter.Edge:
				nodeArcs_Edge.TryGetValue(v, out value);
				break;
			case ArcFilter.Forward:
				nodeArcs_Forward.TryGetValue(v, out value);
				break;
			default:
				nodeArcs_Backward.TryGetValue(v, out value);
				break;
			}
			return value ?? EmptyArcList;
		}

		public IEnumerable<Node> Nodes()
		{
			return (graph != null) ? nodes.Concat(graph.Nodes()) : nodes;
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			return (graph != null) ? ArcsInternal(filter).Concat(graph.Arcs(filter)) : ArcsInternal(filter);
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (graph == null || nodes.Contains(u))
			{
				return ArcsInternal(u, filter);
			}
			return ArcsInternal(u, filter).Concat(graph.Arcs(u, filter));
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			foreach (Arc item in ArcsInternal(u, filter))
			{
				if (this.Other(item, u) == v)
				{
					yield return item;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			if (graph != null && !nodes.Contains(u) && !nodes.Contains(v))
			{
				using (IEnumerator<Arc> enumerator2 = graph.Arcs(u, v, filter).GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						Arc arc = enumerator2.Current;
						yield return arc;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_01d5:
			/*Error near IL_01d6: Unexpected return in MoveNext()*/;
		}

		public int NodeCount()
		{
			return nodes.Count + ((graph != null) ? graph.NodeCount() : 0);
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return ArcsInternal(filter).Count + ((graph != null) ? graph.ArcCount(filter) : 0);
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			return ArcsInternal(u, filter).Count + ((graph != null && !nodes.Contains(u)) ? graph.ArcCount(u, filter) : 0);
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			int num = 0;
			foreach (Arc item in ArcsInternal(u, filter))
			{
				if (this.Other(item, u) == v)
				{
					num++;
				}
			}
			return num + ((graph != null && !nodes.Contains(u) && !nodes.Contains(v)) ? graph.ArcCount(u, v, filter) : 0);
		}

		public bool HasNode(Node node)
		{
			return nodes.Contains(node) || (graph != null && graph.HasNode(node));
		}

		public bool HasArc(Arc arc)
		{
			return arcs.Contains(arc) || (graph != null && graph.HasArc(arc));
		}
	}
}
