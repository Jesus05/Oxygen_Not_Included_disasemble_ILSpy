using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class Preflow : IFlow<double>
	{
		private Dictionary<Arc, double> flow;

		private Arc artificialArc;

		private double U;

		private double CapacityMultiplier;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Func<Arc, double> Capacity
		{
			get;
			private set;
		}

		public Node Source
		{
			get;
			private set;
		}

		public Node Target
		{
			get;
			private set;
		}

		public double FlowSize
		{
			get;
			private set;
		}

		public double Error
		{
			get;
			private set;
		}

		public IEnumerable<KeyValuePair<Arc, double>> NonzeroArcs => from kv in flow
		where kv.Value != 0.0
		select kv;

		public Preflow(IGraph graph, Func<Arc, double> capacity, Node source, Node target)
		{
			Graph = graph;
			Capacity = capacity;
			Source = source;
			Target = target;
			flow = new Dictionary<Arc, double>();
			Dijkstra dijkstra = new Dijkstra(Graph, (Arc a) => 0.0 - Capacity(a), DijkstraMode.Maximum);
			dijkstra.AddSource(Source);
			dijkstra.RunUntilFixed(Target);
			double num = 0.0 - dijkstra.GetDistance(Target);
			if (double.IsPositiveInfinity(num))
			{
				FlowSize = double.PositiveInfinity;
				Error = 0.0;
				Node node = Target;
				Node invalid = Node.Invalid;
				while (node != Source)
				{
					Arc parentArc = dijkstra.GetParentArc(node);
					flow[parentArc] = double.PositiveInfinity;
					invalid = Graph.Other(parentArc, node);
					node = invalid;
				}
			}
			else
			{
				if (double.IsNegativeInfinity(num))
				{
					num = 0.0;
				}
				U = (double)Graph.ArcCount(ArcFilter.All) * num;
				double num2 = 0.0;
				foreach (Arc item in Graph.Arcs(Source, ArcFilter.Forward))
				{
					if (Graph.Other(item, Source) != Source)
					{
						num2 += Capacity(item);
						if (num2 > U)
						{
							break;
						}
					}
				}
				U = Math.Min(U, num2);
				double num3 = 0.0;
				foreach (Arc item2 in Graph.Arcs(Target, ArcFilter.Backward))
				{
					if (Graph.Other(item2, Target) != Target)
					{
						num3 += Capacity(item2);
						if (num3 > U)
						{
							break;
						}
					}
				}
				U = Math.Min(U, num3);
				Supergraph supergraph = new Supergraph(Graph);
				Node node2 = supergraph.AddNode();
				artificialArc = supergraph.AddArc(node2, Source, Directedness.Directed);
				CapacityMultiplier = Utils.LargestPowerOfTwo(9.2233720368547758E+18 / U);
				if (CapacityMultiplier == 0.0)
				{
					CapacityMultiplier = 1.0;
				}
				IntegerPreflow integerPreflow = new IntegerPreflow(supergraph, IntegralCapacity, node2, Target);
				FlowSize = (double)integerPreflow.FlowSize / CapacityMultiplier;
				Error = (double)Graph.ArcCount(ArcFilter.All) / CapacityMultiplier;
				foreach (KeyValuePair<Arc, long> nonzeroArc in integerPreflow.NonzeroArcs)
				{
					flow[nonzeroArc.Key] = (double)nonzeroArc.Value / CapacityMultiplier;
				}
			}
		}

		private long IntegralCapacity(Arc arc)
		{
			return (long)(CapacityMultiplier * ((!(arc == artificialArc)) ? Math.Min(U, Capacity(arc)) : U));
		}

		public double Flow(Arc arc)
		{
			flow.TryGetValue(arc, out double value);
			return value;
		}
	}
}
