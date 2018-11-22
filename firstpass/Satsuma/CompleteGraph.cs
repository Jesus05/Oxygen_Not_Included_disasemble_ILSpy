using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class CompleteGraph : IGraph, IArcLookup
	{
		private readonly int nodeCount;

		public bool Directed
		{
			get;
			private set;
		}

		public CompleteGraph(int nodeCount, Directedness directedness)
		{
			this.nodeCount = nodeCount;
			Directed = (directedness == Directedness.Directed);
			if (nodeCount < 0)
			{
				throw new ArgumentException("Invalid node count: " + nodeCount);
			}
			long num = (long)nodeCount * (long)(nodeCount - 1);
			if (!Directed)
			{
				num /= 2;
			}
			if (num > 2147483647)
			{
				throw new ArgumentException("Too many nodes: " + nodeCount);
			}
		}

		public Node GetNode(int index)
		{
			return new Node(1L + (long)index);
		}

		public int GetNodeIndex(Node node)
		{
			return (int)(node.Id - 1);
		}

		public Arc GetArc(Node u, Node v)
		{
			int num = GetNodeIndex(u);
			int num2 = GetNodeIndex(v);
			if (num != num2)
			{
				if (!Directed && num > num2)
				{
					int num3 = num;
					num = num2;
					num2 = num3;
				}
				return GetArcInternal(num, num2);
			}
			return Arc.Invalid;
		}

		private Arc GetArcInternal(int x, int y)
		{
			return new Arc(1 + (long)y * (long)nodeCount + x);
		}

		public Node U(Arc arc)
		{
			return new Node(1 + (arc.Id - 1) % nodeCount);
		}

		public Node V(Arc arc)
		{
			return new Node(1 + (arc.Id - 1) / nodeCount);
		}

		public bool IsEdge(Arc arc)
		{
			return !Directed;
		}

		public IEnumerable<Node> Nodes()
		{
			int i = 0;
			if (i < nodeCount)
			{
				yield return GetNode(i);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (!Directed)
			{
				int j = 0;
				int i;
				while (true)
				{
					if (j >= nodeCount)
					{
						yield break;
					}
					i = j + 1;
					if (i < nodeCount)
					{
						break;
					}
					j++;
				}
				yield return GetArcInternal(j, i);
				/*Error: Unable to find new state assignment for yield return*/;
			}
			for (int l = 0; l < nodeCount; l++)
			{
				for (int k = 0; k < nodeCount; k++)
				{
					if (l != k)
					{
						yield return GetArcInternal(l, k);
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
		}

		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (Directed)
			{
				switch (filter)
				{
				case ArcFilter.Edge:
					yield break;
				default:
					foreach (Node item in Nodes())
					{
						if (item != u)
						{
							yield return GetArc(item, u);
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
					break;
				case ArcFilter.Forward:
					break;
				}
			}
			if (!Directed || filter != ArcFilter.Backward)
			{
				foreach (Node item2 in Nodes())
				{
					if (item2 != u)
					{
						yield return GetArc(u, item2);
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_01e6:
			/*Error near IL_01e7: Unexpected return in MoveNext()*/;
		}

		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (Directed)
			{
				switch (filter)
				{
				case ArcFilter.Edge:
					yield break;
				default:
					yield return GetArc(v, u);
					/*Error: Unable to find new state assignment for yield return*/;
				case ArcFilter.Forward:
					break;
				}
			}
			if (!Directed || filter != ArcFilter.Backward)
			{
				yield return GetArc(u, v);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public int NodeCount()
		{
			return nodeCount;
		}

		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			int num = nodeCount * (nodeCount - 1);
			if (!Directed)
			{
				num /= 2;
			}
			return num;
		}

		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			if (Directed)
			{
				switch (filter)
				{
				case ArcFilter.All:
					return 2 * (nodeCount - 1);
				case ArcFilter.Edge:
					return 0;
				default:
					return nodeCount - 1;
				}
			}
			return nodeCount - 1;
		}

		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (Directed)
			{
				switch (filter)
				{
				case ArcFilter.All:
					return 2;
				case ArcFilter.Edge:
					return 0;
				default:
					return 1;
				}
			}
			return 1;
		}

		public bool HasNode(Node node)
		{
			return node.Id >= 1 && node.Id <= nodeCount;
		}

		public bool HasArc(Arc arc)
		{
			Node node = V(arc);
			if (HasNode(node))
			{
				Node node2 = U(arc);
				return Directed || node2.Id < node.Id;
			}
			return false;
		}
	}
}
