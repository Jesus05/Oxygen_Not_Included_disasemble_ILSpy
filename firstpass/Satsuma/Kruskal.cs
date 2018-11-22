using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Kruskal<TCost> where TCost : IComparable<TCost>
	{
		private IEnumerator<Arc> arcEnumerator;

		private int arcsToGo;

		private DisjointSet<Node> components;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Func<Arc, TCost> Cost
		{
			get;
			private set;
		}

		public Func<Node, int> MaxDegree
		{
			get;
			private set;
		}

		public HashSet<Arc> Forest
		{
			get;
			private set;
		}

		public Dictionary<Node, int> Degree
		{
			get;
			private set;
		}

		public Kruskal(IGraph graph, Func<Arc, TCost> cost, Func<Node, int> maxDegree = null)
		{
			Graph = graph;
			Cost = cost;
			MaxDegree = maxDegree;
			Forest = new HashSet<Arc>();
			Degree = new Dictionary<Node, int>();
			foreach (Node item in Graph.Nodes())
			{
				Degree[item] = 0;
			}
			List<Arc> list = Enumerable.ToList<Arc>(Graph.Arcs(ArcFilter.All));
			list.Sort((Arc a, Arc b) => Cost(a).CompareTo(Cost(b)));
			arcEnumerator = list.GetEnumerator();
			arcsToGo = Graph.NodeCount() - new ConnectedComponents(Graph, ConnectedComponents.Flags.None).Count;
			components = new DisjointSet<Node>();
		}

		public bool Step()
		{
			if (arcsToGo > 0 && arcEnumerator != null && arcEnumerator.MoveNext())
			{
				AddArc(arcEnumerator.Current);
				return true;
			}
			arcEnumerator = null;
			return false;
		}

		public void Run()
		{
			while (Step())
			{
			}
		}

		public bool AddArc(Arc arc)
		{
			Node node = Graph.U(arc);
			if (MaxDegree != null && Degree[node] >= MaxDegree(node))
			{
				return false;
			}
			DisjointSetSet<Node> a = components.WhereIs(node);
			Node node2 = Graph.V(arc);
			if (MaxDegree != null && Degree[node2] >= MaxDegree(node2))
			{
				return false;
			}
			DisjointSetSet<Node> b = components.WhereIs(node2);
			if (!(a == b))
			{
				Forest.Add(arc);
				components.Union(a, b);
				Node key;
				Dictionary<Node, int> degree;
				(degree = Degree)[key = node] = degree[key] + 1;
				Node key2;
				(degree = Degree)[key2 = node2] = degree[key2] + 1;
				arcsToGo--;
				return true;
			}
			return false;
		}
	}
}
