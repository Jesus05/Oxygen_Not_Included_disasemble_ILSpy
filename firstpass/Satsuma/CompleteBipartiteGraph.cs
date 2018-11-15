using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class CompleteBipartiteGraph : IGraph, IArcLookup
	{
		public enum Color
		{
			Red,
			Blue
		}

		public int RedNodeCount
		{
			get;
			private set;
		}

		public int BlueNodeCount
		{
			get;
			private set;
		}

		public bool Directed
		{
			get;
			private set;
		}

		public CompleteBipartiteGraph(int redNodeCount, int blueNodeCount, Directedness directedness)
		{
			if (redNodeCount < 0 || blueNodeCount < 0)
			{
				throw new ArgumentException("Invalid node count: " + redNodeCount + ";" + blueNodeCount);
			}
			if ((long)redNodeCount + (long)blueNodeCount > 2147483647 || (long)redNodeCount * (long)blueNodeCount > 2147483647)
			{
				throw new ArgumentException("Too many nodes: " + redNodeCount + ";" + blueNodeCount);
			}
			RedNodeCount = redNodeCount;
			BlueNodeCount = blueNodeCount;
			Directed = (directedness == Directedness.Directed);
		}

		public Node GetRedNode(int index)
		{
			return new Node(1L + (long)index);
		}

		public Node GetBlueNode(int index)
		{
			return new Node(1L + (long)RedNodeCount + index);
		}

		public bool IsRed(Node node)
		{
			return node.Id <= RedNodeCount;
		}

		public Arc GetArc(Node u, Node v)
		{
			bool flag = IsRed(u);
			bool flag2 = IsRed(v);
			if (flag == flag2)
			{
				return Arc.Invalid;
			}
			if (flag2)
			{
				Node node = u;
				u = v;
				v = node;
			}
			int num = (int)(u.Id - 1);
			int num2 = (int)(v.Id - RedNodeCount - 1);
			return new Arc(1 + (long)num2 * (long)RedNodeCount + num);
		}

		public Node U(Arc arc)
		{
			return new Node(1 + (arc.Id - 1) % RedNodeCount);
		}

		public Node V(Arc arc)
		{
			return new Node(1L + (long)RedNodeCount + (arc.Id - 1) / RedNodeCount);
		}

		public bool IsEdge(Arc arc)
		{
			return !Directed;
		}

		public IEnumerable<Node> Nodes(Color color)
		{
			switch (color)
			{
			case Color.Red:
			{
				int i = 0;
				if (i < RedNodeCount)
				{
					yield return GetRedNode(i);
					/*Error: Unable to find new state assignment for yield return*/;
				}
				break;
			}
			case Color.Blue:
			{
				int j = 0;
				if (j < BlueNodeCount)
				{
					yield return GetBlueNode(j);
					/*Error: Unable to find new state assignment for yield return*/;
				}
				break;
			}
			}
		}

		public IEnumerable<Node> Nodes()
		{
			int j = 0;
			if (j < RedNodeCount)
			{
				yield return GetRedNode(j);
				/*Error: Unable to find new state assignment for yield return*/;
			}
			int i = 0;
			if (i < BlueNodeCount)
			{
				yield return GetBlueNode(i);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (!Directed || filter != ArcFilter.Edge)
			{
				int j = 0;
				int i;
				while (true)
				{
					if (j >= RedNodeCount)
					{
						yield break;
					}
					i = 0;
					if (i < BlueNodeCount)
					{
						break;
					}
					j++;
				}
				yield return GetArc(GetRedNode(j), GetBlueNode(i));
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			bool isRed = IsRed(u);
			if (Directed)
			{
				switch (filter)
				{
				case ArcFilter.Edge:
					yield break;
				case ArcFilter.Forward:
					if (!isRed)
					{
						yield break;
					}
					break;
				}
				if (filter == ArcFilter.Backward && isRed)
				{
					yield break;
				}
			}
			if (isRed)
			{
				int j = 0;
				if (j < BlueNodeCount)
				{
					yield return GetArc(u, GetBlueNode(j));
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			else
			{
				int i = 0;
				if (i < RedNodeCount)
				{
					yield return GetArc(GetRedNode(i), u);
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			Arc arc = GetArc(u, v);
			if (arc != Arc.Invalid && ArcCount(u, filter) > 0)
			{
				yield return arc;
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public int NodeCount()
		{
			return RedNodeCount + BlueNodeCount;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (Directed && filter == ArcFilter.Edge)
			{
				return 0;
			}
			return RedNodeCount * BlueNodeCount;
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			bool flag = IsRed(u);
			if (Directed)
			{
				switch (filter)
				{
				case ArcFilter.Forward:
					if (flag)
					{
						goto default;
					}
					goto case ArcFilter.Edge;
				default:
					if (filter != ArcFilter.Backward || !flag)
					{
						break;
					}
					goto case ArcFilter.Edge;
				case ArcFilter.Edge:
					return 0;
				}
			}
			return (!flag) ? RedNodeCount : BlueNodeCount;
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (IsRed(u) == IsRed(v))
			{
				return 0;
			}
			return (ArcCount(u, filter) > 0) ? 1 : 0;
		}

		public bool HasNode(Node node)
		{
			return node.Id >= 1 && node.Id <= RedNodeCount + BlueNodeCount;
		}

		public bool HasArc(Arc arc)
		{
			return arc.Id >= 1 && arc.Id <= RedNodeCount * BlueNodeCount;
		}
	}
}
