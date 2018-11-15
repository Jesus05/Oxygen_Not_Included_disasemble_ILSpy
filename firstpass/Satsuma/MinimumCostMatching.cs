using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class MinimumCostMatching
	{
		public IGraph Graph
		{
			get;
			private set;
		}

		public Func<Node, bool> IsRed
		{
			get;
			private set;
		}

		public Func<Arc, double> Cost
		{
			get;
			private set;
		}

		public int MinimumMatchingSize
		{
			get;
			private set;
		}

		public int MaximumMatchingSize
		{
			get;
			private set;
		}

		public IMatching Matching
		{
			get;
			private set;
		}

		public MinimumCostMatching(IGraph graph, Func<Node, bool> isRed, Func<Arc, double> cost, int minimumMatchingSize = 0, int maximumMatchingSize = int.MaxValue)
		{
			Graph = graph;
			IsRed = isRed;
			Cost = cost;
			MinimumMatchingSize = minimumMatchingSize;
			MaximumMatchingSize = maximumMatchingSize;
			Run();
		}

		private void Run()
		{
			RedirectedGraph graph = new RedirectedGraph(Graph, (Arc x) => (!IsRed(Graph.U(x))) ? RedirectedGraph.Direction.Backward : RedirectedGraph.Direction.Forward);
			Supergraph supergraph = new Supergraph(graph);
			Node node = supergraph.AddNode();
			Node node2 = supergraph.AddNode();
			foreach (Node item in Graph.Nodes())
			{
				if (IsRed(item))
				{
					supergraph.AddArc(node, item, Directedness.Directed);
				}
				else
				{
					supergraph.AddArc(item, node2, Directedness.Directed);
				}
			}
			Arc reflow = supergraph.AddArc(node2, node, Directedness.Directed);
			NetworkSimplex networkSimplex = new NetworkSimplex(supergraph, (Arc x) => (x == reflow) ? MinimumMatchingSize : 0, (Arc x) => (!(x == reflow)) ? 1 : MaximumMatchingSize, null, (Arc x) => (!Graph.HasArc(x)) ? 0.0 : Cost(x));
			networkSimplex.Run();
			if (networkSimplex.State == SimplexState.Optimal)
			{
				Matching matching = new Matching(Graph);
				foreach (Arc item2 in networkSimplex.UpperBoundArcs.Concat(from kv in networkSimplex.Forest
				where kv.Value == 1
				select kv.Key))
				{
					if (Graph.HasArc(item2))
					{
						matching.Enable(item2, true);
					}
				}
				Matching = matching;
			}
		}
	}
}
