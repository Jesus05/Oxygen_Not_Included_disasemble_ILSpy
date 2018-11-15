using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Prim<TCost> where TCost : IComparable<TCost>
	{
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

		public HashSet<Arc> Forest
		{
			get;
			private set;
		}

		public Prim(IGraph graph, Func<Arc, TCost> cost)
		{
			Graph = graph;
			Cost = cost;
			Forest = new HashSet<Arc>();
			Run();
		}

		private void Run()
		{
			Forest.Clear();
			PriorityQueue<Node, TCost> priorityQueue = new PriorityQueue<Node, TCost>();
			HashSet<Node> hashSet = new HashSet<Node>();
			Dictionary<Node, Arc> dictionary = new Dictionary<Node, Arc>();
			ConnectedComponents connectedComponents = new ConnectedComponents(Graph, ConnectedComponents.Flags.CreateComponents);
			foreach (HashSet<Node> component in connectedComponents.Components)
			{
				Node node = Enumerable.First<Node>((IEnumerable<Node>)component);
				hashSet.Add(node);
				foreach (Arc item in Graph.Arcs(node, ArcFilter.All))
				{
					Node node2 = Graph.Other(item, node);
					dictionary[node2] = item;
					priorityQueue[node2] = Cost(item);
				}
			}
			connectedComponents = null;
			while (priorityQueue.Count != 0)
			{
				Node node3 = priorityQueue.Peek();
				priorityQueue.Pop();
				hashSet.Add(node3);
				Forest.Add(dictionary[node3]);
				foreach (Arc item2 in Graph.Arcs(node3, ArcFilter.All))
				{
					Node node4 = Graph.Other(item2, node3);
					if (!hashSet.Contains(node4))
					{
						TCost value = Cost(item2);
						if (value.CompareTo(priorityQueue[node4]) < 0)
						{
							priorityQueue[node4] = value;
							dictionary[node4] = item2;
						}
					}
				}
			}
		}
	}
}
