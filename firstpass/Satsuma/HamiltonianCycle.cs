using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class HamiltonianCycle
	{
		public IGraph Graph
		{
			get;
			private set;
		}

		public IPath Cycle
		{
			get;
			private set;
		}

		public HamiltonianCycle(IGraph graph)
		{
			Graph = graph;
			Cycle = null;
			Run();
		}

		private void Run()
		{
			Func<Node, Node, double> cost = (Node u, Node v) => (double)(Graph.Arcs(u, v, ArcFilter.Forward).Any() ? 1 : 10);
			IEnumerable<Node> enumerable = null;
			double num = (double)Graph.NodeCount();
			InsertionTsp<Node> insertionTsp = new InsertionTsp<Node>(Graph.Nodes(), cost, TspSelectionRule.Farthest);
			insertionTsp.Run();
			if (insertionTsp.TourCost == num)
			{
				enumerable = insertionTsp.Tour;
			}
			else
			{
				Opt2Tsp<Node> opt2Tsp = new Opt2Tsp<Node>(cost, insertionTsp.Tour, insertionTsp.TourCost);
				opt2Tsp.Run();
				if (opt2Tsp.TourCost == num)
				{
					enumerable = opt2Tsp.Tour;
				}
			}
			if (enumerable == null)
			{
				Cycle = null;
			}
			else
			{
				Path path = new Path(Graph);
				if (enumerable.Any())
				{
					Node node = Node.Invalid;
					foreach (Node item in enumerable)
					{
						if (node == Node.Invalid)
						{
							path.Begin(item);
						}
						else
						{
							path.AddLast(Graph.Arcs(node, item, ArcFilter.Forward).First());
						}
						node = item;
					}
					path.AddLast(Graph.Arcs(node, enumerable.First(), ArcFilter.Forward).First());
				}
				Cycle = path;
			}
		}
	}
}
