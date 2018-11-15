using Delaunay.Geo;
using Delaunay.LR;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
	public static class DelaunayHelpers
	{
		public class LineSegmentWithSites : LineSegment
		{
			public uint id0
			{
				get;
				private set;
			}

			public uint id1
			{
				get;
				private set;
			}

			public LineSegmentWithSites(Vector2? p0, Vector2? p1, uint id0, uint id1)
				: base(p0, p1)
			{
				this.id0 = id0;
				this.id1 = id1;
			}
		}

		public static List<LineSegment> VisibleLineSegments(List<Edge> edges)
		{
			List<LineSegment> list = new List<LineSegment>();
			for (int i = 0; i < edges.Count; i++)
			{
				Edge edge = edges[i];
				if (edge.visible)
				{
					Vector2? p = edge.clippedEnds[Side.LEFT];
					Vector2? p2 = edge.clippedEnds[Side.RIGHT];
					list.Add(new LineSegment(p, p2));
				}
			}
			return list;
		}

		public static List<LineSegmentWithSites> VisibleLineSegmentsWithSite(List<Edge> edges)
		{
			List<LineSegmentWithSites> list = new List<LineSegmentWithSites>();
			for (int i = 0; i < edges.Count; i++)
			{
				Edge edge = edges[i];
				if (edge.visible)
				{
					Vector2? p = edge.clippedEnds[Side.LEFT];
					Vector2? p2 = edge.clippedEnds[Side.RIGHT];
					list.Add(new LineSegmentWithSites(p, p2, edge.leftSite.color, edge.rightSite.color));
				}
			}
			return list;
		}

		public static List<Edge> SelectEdgesForSitePoint(Vector2 coord, List<Edge> edgesToTest)
		{
			return edgesToTest.FindAll((Edge edge) => (edge.leftSite != null && edge.leftSite.Coord == coord) || (edge.rightSite != null && edge.rightSite.Coord == coord));
		}

		public static List<Edge> SelectNonIntersectingEdges(List<Edge> edgesToTest)
		{
			return edgesToTest;
		}

		public static List<LineSegment> DelaunayLinesForEdges(List<Edge> edges)
		{
			List<LineSegment> list = new List<LineSegment>();
			for (int i = 0; i < edges.Count; i++)
			{
				Edge edge = edges[i];
				list.Add(edge.DelaunayLine());
			}
			return list;
		}

		public static List<LineSegment> Kruskal(List<LineSegment> lineSegments, KruskalType type = KruskalType.MINIMUM)
		{
			Dictionary<Vector2?, Node> dictionary = new Dictionary<Vector2?, Node>();
			List<LineSegment> list = new List<LineSegment>();
			Stack<Node> pool = Node.pool;
			if (type == KruskalType.MAXIMUM)
			{
				lineSegments.Sort((LineSegment l1, LineSegment l2) => LineSegment.CompareLengths(l1, l2));
			}
			else
			{
				lineSegments.Sort((LineSegment l1, LineSegment l2) => LineSegment.CompareLengths_MAX(l1, l2));
			}
			int num = lineSegments.Count;
			while (--num > -1)
			{
				LineSegment lineSegment = lineSegments[num];
				Node node = null;
				Node node2;
				if (!dictionary.ContainsKey(lineSegment.p0))
				{
					node = ((pool.Count <= 0) ? new Node() : pool.Pop());
					node2 = (node.parent = node);
					node.treeSize = 1;
					dictionary[lineSegment.p0] = node;
				}
				else
				{
					node = dictionary[lineSegment.p0];
					node2 = Find(node);
				}
				Node node3 = null;
				Node node4;
				if (!dictionary.ContainsKey(lineSegment.p1))
				{
					node3 = ((pool.Count <= 0) ? new Node() : pool.Pop());
					node4 = (node3.parent = node3);
					node3.treeSize = 1;
					dictionary[lineSegment.p1] = node3;
				}
				else
				{
					node3 = dictionary[lineSegment.p1];
					node4 = Find(node3);
				}
				if (node2 != node4)
				{
					list.Add(lineSegment);
					int treeSize = node2.treeSize;
					int treeSize2 = node4.treeSize;
					if (treeSize >= treeSize2)
					{
						node4.parent = node2;
						node2.treeSize += treeSize2;
					}
					else
					{
						node2.parent = node4;
						node4.treeSize += treeSize;
					}
				}
			}
			foreach (Node value in dictionary.Values)
			{
				pool.Push(value);
			}
			return list;
		}

		private static Node Find(Node node)
		{
			if (node.parent == node)
			{
				return node;
			}
			return node.parent = Find(node.parent);
		}
	}
}
