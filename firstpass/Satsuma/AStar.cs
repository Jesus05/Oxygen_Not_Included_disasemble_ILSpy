using System;

namespace Satsuma
{
	public sealed class AStar
	{
		private Dijkstra dijkstra;

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

		public Func<Node, double> Heuristic
		{
			get;
			private set;
		}

		public AStar(IGraph graph, Func<Arc, double> cost, Func<Node, double> heuristic)
		{
			Graph = graph;
			Cost = cost;
			Heuristic = heuristic;
			dijkstra = new Dijkstra(Graph, (Arc arc) => Cost(arc) - Heuristic(Graph.U(arc)) + Heuristic(Graph.V(arc)), DijkstraMode.Sum);
		}

		private Node CheckTarget(Node node)
		{
			if (node != Node.Invalid && Heuristic(node) != 0.0)
			{
				throw new ArgumentException("Heuristic is nonzero for a target");
			}
			return node;
		}

		public void AddSource(Node node)
		{
			dijkstra.AddSource(node, Heuristic(node));
		}

		public Node RunUntilReached(Node target)
		{
			return CheckTarget(dijkstra.RunUntilFixed(target));
		}

		public Node RunUntilReached(Func<Node, bool> isTarget)
		{
			return CheckTarget(dijkstra.RunUntilFixed(isTarget));
		}

		public double GetDistance(Node node)
		{
			CheckTarget(node);
			return (!dijkstra.Fixed(node)) ? double.PositiveInfinity : dijkstra.GetDistance(node);
		}

		public IPath GetPath(Node node)
		{
			CheckTarget(node);
			if (!dijkstra.Fixed(node))
			{
				return null;
			}
			return dijkstra.GetPath(node);
		}
	}
}
