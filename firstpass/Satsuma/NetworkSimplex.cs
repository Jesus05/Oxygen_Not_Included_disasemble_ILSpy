using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class NetworkSimplex : IClearable
	{
		private class RecalculatePotentialDfs : Dfs
		{
			public NetworkSimplex Parent;

			protected override void Start(out Direction direction)
			{
				direction = Direction.Undirected;
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					Parent.Potential[node] = 0.0;
				}
				else
				{
					Node key = Parent.MyGraph.Other(arc, node);
					Parent.Potential[node] = Parent.Potential[key] + ((!(node == Parent.MyGraph.V(arc))) ? (0.0 - Parent.ActualCost(arc)) : Parent.ActualCost(arc));
				}
				return true;
			}
		}

		private class UpdatePotentialDfs : Dfs
		{
			public NetworkSimplex Parent;

			public double Diff;

			protected override void Start(out Direction direction)
			{
				direction = Direction.Undirected;
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				Dictionary<Node, double> potential;
				Node key;
				(potential = Parent.Potential)[key = node] = potential[key] + Diff;
				return true;
			}
		}

		private double Epsilon;

		private Supergraph MyGraph;

		private Node ArtificialNode;

		private HashSet<Arc> ArtificialArcs;

		private Dictionary<Arc, long> Tree;

		private Subgraph TreeSubgraph;

		private HashSet<Arc> Saturated;

		private Dictionary<Node, double> Potential;

		private IEnumerator<Arc> EnteringArcEnumerator;

		public IGraph Graph
		{
			get;
			private set;
		}

		public Func<Arc, long> LowerBound
		{
			get;
			private set;
		}

		public Func<Arc, long> UpperBound
		{
			get;
			private set;
		}

		public Func<Node, long> Supply
		{
			get;
			private set;
		}

		public Func<Arc, double> Cost
		{
			get;
			private set;
		}

		public SimplexState State
		{
			get;
			private set;
		}

		public IEnumerable<KeyValuePair<Arc, long>> Forest => from kv in Tree
		where Graph.HasArc(kv.Key)
		select kv;

		public IEnumerable<Arc> UpperBoundArcs => Saturated;

		public NetworkSimplex(IGraph graph, Func<Arc, long> lowerBound = null, Func<Arc, long> upperBound = null, Func<Node, long> supply = null, Func<Arc, double> cost = null)
		{
			Graph = graph;
			LowerBound = (lowerBound ?? ((Func<Arc, long>)((Arc x) => 0L)));
			UpperBound = (upperBound ?? ((Func<Arc, long>)((Arc x) => 9223372036854775807L)));
			Supply = (supply ?? ((Func<Node, long>)((Node x) => 0L)));
			Cost = (cost ?? ((Func<Arc, double>)((Arc x) => 1.0)));
			Epsilon = 1.0;
			foreach (Arc item in graph.Arcs(ArcFilter.All))
			{
				double num = Math.Abs(Cost(item));
				if (num > 0.0 && num < Epsilon)
				{
					Epsilon = num;
				}
			}
			Epsilon *= 1E-12;
			Clear();
		}

		public long Flow(Arc arc)
		{
			if (!Saturated.Contains(arc))
			{
				if (!Tree.TryGetValue(arc, out long value))
				{
					value = LowerBound(arc);
					return (value != -9223372036854775808L) ? value : 0;
				}
				return value;
			}
			return UpperBound(arc);
		}

		public void Clear()
		{
			Dictionary<Node, long> dictionary = new Dictionary<Node, long>();
			foreach (Node item in Graph.Nodes())
			{
				dictionary[item] = Supply(item);
			}
			Saturated = new HashSet<Arc>();
			foreach (Arc item2 in Graph.Arcs(ArcFilter.All))
			{
				LowerBound(item2);
				long num = UpperBound(item2);
				if (num < 9223372036854775807L)
				{
					Saturated.Add(item2);
				}
				long num2 = Flow(item2);
				Node key;
				Dictionary<Node, long> dictionary2;
				(dictionary2 = dictionary)[key = Graph.U(item2)] = dictionary2[key] - num2;
				Node key2;
				(dictionary2 = dictionary)[key2 = Graph.V(item2)] = dictionary2[key2] + num2;
			}
			Potential = new Dictionary<Node, double>();
			MyGraph = new Supergraph(Graph);
			ArtificialNode = MyGraph.AddNode();
			Potential[ArtificialNode] = 0.0;
			ArtificialArcs = new HashSet<Arc>();
			Dictionary<Node, Arc> dictionary3 = new Dictionary<Node, Arc>();
			foreach (Node item3 in Graph.Nodes())
			{
				long num3 = dictionary[item3];
				Arc arc = (num3 <= 0) ? MyGraph.AddArc(ArtificialNode, item3, Directedness.Directed) : MyGraph.AddArc(item3, ArtificialNode, Directedness.Directed);
				Potential[item3] = (double)((num3 <= 0) ? 1 : (-1));
				ArtificialArcs.Add(arc);
				dictionary3[item3] = arc;
			}
			Tree = new Dictionary<Arc, long>();
			TreeSubgraph = new Subgraph(MyGraph);
			TreeSubgraph.EnableAllArcs(false);
			foreach (KeyValuePair<Node, Arc> item4 in dictionary3)
			{
				Tree[item4.Value] = Math.Abs(dictionary[item4.Key]);
				TreeSubgraph.Enable(item4.Value, true);
			}
			State = SimplexState.FirstPhase;
			EnteringArcEnumerator = MyGraph.Arcs(ArcFilter.All).GetEnumerator();
			EnteringArcEnumerator.MoveNext();
		}

		private long ActualLowerBound(Arc arc)
		{
			return (!ArtificialArcs.Contains(arc)) ? LowerBound(arc) : 0;
		}

		private long ActualUpperBound(Arc arc)
		{
			return (!ArtificialArcs.Contains(arc)) ? UpperBound(arc) : ((State != 0) ? 0 : 9223372036854775807L);
		}

		private double ActualCost(Arc arc)
		{
			return ArtificialArcs.Contains(arc) ? 1.0 : ((State != 0) ? Cost(arc) : 0.0);
		}

		private static ulong MySubtract(long a, long b)
		{
			if (a != 9223372036854775807L && b != -9223372036854775808L)
			{
				return (ulong)(a - b);
			}
			return ulong.MaxValue;
		}

		public void Step()
		{
			if (State == SimplexState.FirstPhase || State == SimplexState.SecondPhase)
			{
				Arc current = EnteringArcEnumerator.Current;
				Arc arc = Arc.Invalid;
				double num = double.NaN;
				bool flag = false;
				do
				{
					Arc current2 = EnteringArcEnumerator.Current;
					if (!Tree.ContainsKey(current2))
					{
						bool flag2 = Saturated.Contains(current2);
						double num2 = ActualCost(current2) - (Potential[MyGraph.V(current2)] - Potential[MyGraph.U(current2)]);
						if ((num2 < 0.0 - Epsilon && !flag2) || (num2 > Epsilon && (flag2 || ActualLowerBound(current2) == -9223372036854775808L)))
						{
							arc = current2;
							num = num2;
							flag = flag2;
							break;
						}
					}
					if (!EnteringArcEnumerator.MoveNext())
					{
						EnteringArcEnumerator = MyGraph.Arcs(ArcFilter.All).GetEnumerator();
						EnteringArcEnumerator.MoveNext();
					}
				}
				while (!(EnteringArcEnumerator.Current == current));
				if (arc == Arc.Invalid)
				{
					if (State == SimplexState.FirstPhase)
					{
						State = SimplexState.SecondPhase;
						foreach (Arc artificialArc in ArtificialArcs)
						{
							if (Flow(artificialArc) > 0)
							{
								State = SimplexState.Infeasible;
								break;
							}
						}
						if (State == SimplexState.SecondPhase)
						{
							RecalculatePotentialDfs recalculatePotentialDfs = new RecalculatePotentialDfs();
							recalculatePotentialDfs.Parent = this;
							recalculatePotentialDfs.Run(TreeSubgraph, null);
						}
					}
					else
					{
						State = SimplexState.Optimal;
					}
				}
				else
				{
					Node node = MyGraph.U(arc);
					Node node2 = MyGraph.V(arc);
					List<Arc> list = new List<Arc>();
					List<Arc> list2 = new List<Arc>();
					IPath path = TreeSubgraph.FindPath(node2, node, Dfs.Direction.Undirected);
					foreach (Node item in path.Nodes())
					{
						Arc arc2 = path.NextArc(item);
						((!(MyGraph.U(arc2) == item)) ? list2 : list).Add(arc2);
					}
					ulong num3 = (!(num < 0.0)) ? MySubtract(Flow(arc), ActualLowerBound(arc)) : MySubtract(ActualUpperBound(arc), Flow(arc));
					Arc arc3 = arc;
					bool flag3 = !flag;
					foreach (Arc item2 in list)
					{
						ulong num4 = (!(num < 0.0)) ? MySubtract(Tree[item2], ActualLowerBound(item2)) : MySubtract(ActualUpperBound(item2), Tree[item2]);
						if (num4 < num3)
						{
							num3 = num4;
							arc3 = item2;
							flag3 = (num < 0.0);
						}
					}
					foreach (Arc item3 in list2)
					{
						ulong num5 = (!(num > 0.0)) ? MySubtract(Tree[item3], ActualLowerBound(item3)) : MySubtract(ActualUpperBound(item3), Tree[item3]);
						if (num5 < num3)
						{
							num3 = num5;
							arc3 = item3;
							flag3 = (num > 0.0);
						}
					}
					long num6 = 0L;
					switch (num3)
					{
					case ulong.MaxValue:
						State = SimplexState.Unbounded;
						return;
					default:
						num6 = (long)((!(num < 0.0)) ? (0L - num3) : num3);
						foreach (Arc item4 in list)
						{
							Dictionary<Arc, long> tree;
							Arc key;
							(tree = Tree)[key = item4] = tree[key] + num6;
						}
						foreach (Arc item5 in list2)
						{
							Dictionary<Arc, long> tree;
							Arc key2;
							(tree = Tree)[key2 = item5] = tree[key2] - num6;
						}
						break;
					case 0uL:
						break;
					}
					if (arc3 == arc)
					{
						if (flag)
						{
							Saturated.Remove(arc);
						}
						else
						{
							Saturated.Add(arc);
						}
					}
					else
					{
						Tree.Remove(arc3);
						TreeSubgraph.Enable(arc3, false);
						if (flag3)
						{
							Saturated.Add(arc3);
						}
						double num7 = ActualCost(arc) - (Potential[node2] - Potential[node]);
						if (num7 != 0.0)
						{
							UpdatePotentialDfs updatePotentialDfs = new UpdatePotentialDfs();
							updatePotentialDfs.Parent = this;
							updatePotentialDfs.Diff = num7;
							updatePotentialDfs.Run(TreeSubgraph, new Node[1]
							{
								node2
							});
						}
						Tree[arc] = Flow(arc) + num6;
						if (flag)
						{
							Saturated.Remove(arc);
						}
						TreeSubgraph.Enable(arc, true);
					}
				}
			}
		}

		public void Run()
		{
			while (State == SimplexState.FirstPhase || State == SimplexState.SecondPhase)
			{
				Step();
			}
		}
	}
}
