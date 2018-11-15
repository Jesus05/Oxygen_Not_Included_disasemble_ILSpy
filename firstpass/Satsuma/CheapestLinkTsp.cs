using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class CheapestLinkTsp<TNode> : ITsp<TNode>
	{
		private List<TNode> tour;

		public IList<TNode> Nodes
		{
			get;
			private set;
		}

		public Func<TNode, TNode, double> Cost
		{
			get;
			private set;
		}

		public IEnumerable<TNode> Tour => tour;

		public double TourCost
		{
			get;
			private set;
		}

		public CheapestLinkTsp(IList<TNode> nodes, Func<TNode, TNode, double> cost)
		{
			Nodes = nodes;
			Cost = cost;
			tour = new List<TNode>();
			Run();
		}

		private void Run()
		{
			CompleteGraph graph = new CompleteGraph(Nodes.Count, Directedness.Undirected);
			Func<Arc, double> cost = (Arc arc) => Cost(Nodes[graph.GetNodeIndex(graph.U(arc))], Nodes[graph.GetNodeIndex(graph.V(arc))]);
			Kruskal<double> kruskal = new Kruskal<double>(graph, cost, (Node _) => 2);
			kruskal.Run();
			Dictionary<Node, Arc> dictionary = new Dictionary<Node, Arc>();
			Dictionary<Node, Arc> dictionary2 = new Dictionary<Node, Arc>();
			foreach (Arc item in kruskal.Forest)
			{
				Node key = graph.U(item);
				((!dictionary.ContainsKey(key)) ? dictionary : dictionary2)[key] = item;
				Node key2 = graph.V(item);
				((!dictionary.ContainsKey(key2)) ? dictionary : dictionary2)[key2] = item;
			}
			foreach (Node item2 in graph.Nodes())
			{
				if (kruskal.Degree[item2] == 1)
				{
					Arc arc2 = Arc.Invalid;
					Node node = item2;
					while (true)
					{
						tour.Add(Nodes[graph.GetNodeIndex(node)]);
						if (arc2 != Arc.Invalid && kruskal.Degree[node] == 1)
						{
							break;
						}
						Arc arc3 = dictionary[node];
						arc2 = ((!(arc3 != arc2)) ? dictionary2[node] : arc3);
						node = graph.Other(arc2, node);
					}
					tour.Add(Nodes[graph.GetNodeIndex(item2)]);
					break;
				}
			}
			TourCost = TspUtils.GetTourCost<TNode>((IEnumerable<TNode>)tour, Cost);
		}
	}
}
