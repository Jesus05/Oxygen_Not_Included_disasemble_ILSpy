using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class BellmanFord
	{
		private const string NegativeCycleMessage = "A negative cycle was found.";

		private readonly Dictionary<Node, double> distance;

		private readonly Dictionary<Node, Arc> parentArc;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Func<Arc, double> Cost
		{
			get;
			private set;
		}

		public IPath NegativeCycle
		{
			get;
			private set;
		}

		public IEnumerable<Node> ReachedNodes => parentArc.Keys;

		public BellmanFord(IGraph graph, Func<Arc, double> cost, IEnumerable<Node> sources)
		{
			Graph = graph;
			Cost = cost;
			distance = new Dictionary<Node, double>();
			parentArc = new Dictionary<Node, Arc>();
			foreach (Node source in sources)
			{
				distance[source] = 0.0;
				parentArc[source] = Arc.Invalid;
			}
			Run();
		}

		private void Run()
		{
			for (int num = Graph.NodeCount(); num > 0; num--)
			{
				foreach (Arc item in Graph.Arcs(ArcFilter.All))
				{
					Node node = Graph.U(item);
					Node node2 = Graph.V(item);
					double num2 = GetDistance(node);
					double num3 = GetDistance(node2);
					double num4 = Cost(item);
					if (Graph.IsEdge(item))
					{
						if (num2 > num3)
						{
							Node node3 = node;
							node = node2;
							node2 = node3;
							double num5 = num2;
							num2 = num3;
							num3 = num5;
						}
						if (!double.IsPositiveInfinity(num2) && num4 < 0.0)
						{
							Path path = new Path(Graph);
							path.Begin(node);
							path.AddLast(item);
							path.AddLast(item);
							NegativeCycle = path;
							return;
						}
					}
					if (num2 + num4 < num3)
					{
						distance[node2] = num2 + num4;
						parentArc[node2] = item;
						if (num == 0)
						{
							Node node4 = node;
							for (int num6 = Graph.NodeCount() - 1; num6 > 0; num6--)
							{
								node4 = Graph.Other(parentArc[node4], node4);
							}
							Path path2 = new Path(Graph);
							path2.Begin(node4);
							Node node5 = node4;
							do
							{
								Arc arc = parentArc[node5];
								path2.AddFirst(arc);
								node5 = Graph.Other(arc, node5);
							}
							while (!(node5 == node4));
							NegativeCycle = path2;
							return;
						}
					}
				}
			}
		}

		public bool Reached(Node node)
		{
			return parentArc.ContainsKey(node);
		}

		public double GetDistance(Node node)
		{
			if (NegativeCycle != null)
			{
				throw new InvalidOperationException("A negative cycle was found.");
			}
			double value;
			return (!distance.TryGetValue(node, out value)) ? double.PositiveInfinity : value;
		}

		public Arc GetParentArc(Node node)
		{
			if (NegativeCycle != null)
			{
				throw new InvalidOperationException("A negative cycle was found.");
			}
			Arc value;
			return (!parentArc.TryGetValue(node, out value)) ? Arc.Invalid : value;
		}

		public IPath GetPath(Node node)
		{
			if (NegativeCycle != null)
			{
				throw new InvalidOperationException("A negative cycle was found.");
			}
			if (Reached(node))
			{
				Path path = new Path(Graph);
				path.Begin(node);
				while (true)
				{
					Arc arc = GetParentArc(node);
					if (arc == Arc.Invalid)
					{
						break;
					}
					path.AddFirst(arc);
					node = Graph.Other(arc, node);
				}
				return path;
			}
			return null;
		}
	}
}
