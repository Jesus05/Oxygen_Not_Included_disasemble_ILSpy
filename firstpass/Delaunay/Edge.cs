using Delaunay.Geo;
using Delaunay.LR;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
	public sealed class Edge
	{
		private static Stack<Edge> _pool = new Stack<Edge>();

		private static int _nedges = 0;

		public static readonly Edge DELETED = new Edge();

		public float a;

		public float b;

		public float c;

		private Vertex _leftVertex;

		private Vertex _rightVertex;

		private Dictionary<Side, Vector2?> _clippedVertices;

		private Dictionary<Side, Site> _sites;

		private int _edgeIndex;

		public Vertex leftVertex => _leftVertex;

		public Vertex rightVertex => _rightVertex;

		public Dictionary<Side, Vector2?> clippedEnds => _clippedVertices;

		public bool visible => _clippedVertices != null;

		public Site leftSite
		{
			get
			{
				return _sites[Side.LEFT];
			}
			set
			{
				_sites[Side.LEFT] = value;
			}
		}

		public Site rightSite
		{
			get
			{
				return _sites[Side.RIGHT];
			}
			set
			{
				_sites[Side.RIGHT] = value;
			}
		}

		private Edge()
		{
			_edgeIndex = _nedges++;
			Init();
		}

		public static Edge CreateBisectingEdge(Site site0, Site site1)
		{
			Vector2 coord = site1.Coord;
			Vector2 coord2 = site0.Coord;
			float num = coord2.x - coord.x;
			float num2 = coord2.y - coord.y;
			float num3 = (!(num > 0f)) ? (0f - num) : num;
			float num4 = (!(num2 > 0f)) ? (0f - num2) : num2;
			float num5 = coord.x * num + coord.y * num2 + (num * num + num2 * num2) * 0.5f;
			float num6;
			float num7;
			if (num3 > num4)
			{
				num6 = 1f;
				num7 = num2 / num;
				num5 /= num;
			}
			else
			{
				num7 = 1f;
				num6 = num / num2;
				num5 /= num2;
			}
			Edge edge = Create();
			edge.leftSite = site0;
			edge.rightSite = site1;
			site0.AddEdge(edge);
			site1.AddEdge(edge);
			edge._leftVertex = null;
			edge._rightVertex = null;
			edge.a = num6;
			edge.b = num7;
			edge.c = num5;
			return edge;
		}

		private static Edge Create()
		{
			Edge edge;
			if (_pool.Count > 0)
			{
				edge = _pool.Pop();
				edge.Init();
			}
			else
			{
				edge = new Edge();
			}
			return edge;
		}

		public LineSegment DelaunayLine()
		{
			return new LineSegment(leftSite.Coord, rightSite.Coord);
		}

		public LineSegment VoronoiEdge()
		{
			if (visible)
			{
				return new LineSegment(_clippedVertices[Side.LEFT], _clippedVertices[Side.RIGHT]);
			}
			return new LineSegment(null, null);
		}

		public Vertex Vertex(Side leftRight)
		{
			return (leftRight != 0) ? _rightVertex : _leftVertex;
		}

		public void SetVertex(Side leftRight, Vertex v)
		{
			if (leftRight == Side.LEFT)
			{
				_leftVertex = v;
			}
			else
			{
				_rightVertex = v;
			}
		}

		public bool IsPartOfConvexHull()
		{
			return _leftVertex == null || _rightVertex == null;
		}

		public float SitesDistance()
		{
			return Vector2.Distance(leftSite.Coord, rightSite.Coord) + (leftSite.weight + rightSite.weight) * (leftSite.weight + rightSite.weight);
		}

		public static int CompareSitesDistances_MAX(Edge edge0, Edge edge1)
		{
			float num = edge0.SitesDistance();
			float num2 = edge1.SitesDistance();
			if (!(num < num2))
			{
				if (!(num > num2))
				{
					return 0;
				}
				return -1;
			}
			return 1;
		}

		public static int CompareSitesDistances(Edge edge0, Edge edge1)
		{
			return -CompareSitesDistances_MAX(edge0, edge1);
		}

		public Site Site(Side leftRight)
		{
			return _sites[leftRight];
		}

		public void Dispose()
		{
			_leftVertex = null;
			_rightVertex = null;
			if (_clippedVertices != null)
			{
				_clippedVertices[Side.LEFT] = null;
				_clippedVertices[Side.RIGHT] = null;
				_clippedVertices = null;
			}
			_sites[Side.LEFT] = null;
			_sites[Side.RIGHT] = null;
			_sites = null;
			_pool.Push(this);
		}

		private void Init()
		{
			_sites = new Dictionary<Side, Site>();
		}

		public override string ToString()
		{
			return "Edge " + _edgeIndex.ToString() + "; sites " + _sites[Side.LEFT].ToString() + ", " + _sites[Side.RIGHT].ToString() + "; endVertices " + ((_leftVertex == null) ? "null" : _leftVertex.vertexIndex.ToString()) + ", " + ((_rightVertex == null) ? "null" : _rightVertex.vertexIndex.ToString()) + "::";
		}

		public void ClipVertices(Rect bounds)
		{
			float xMin = bounds.xMin;
			float yMin = bounds.yMin;
			float xMax = bounds.xMax;
			float yMax = bounds.yMax;
			Vertex vertex;
			Vertex vertex2;
			if ((double)a == 1.0 && (double)b >= 0.0)
			{
				vertex = _rightVertex;
				vertex2 = _leftVertex;
			}
			else
			{
				vertex = _leftVertex;
				vertex2 = _rightVertex;
			}
			float num;
			float num2;
			float num3;
			float num4;
			if ((double)a == 1.0)
			{
				num = yMin;
				if (vertex != null && vertex.y > yMin)
				{
					num = vertex.y;
				}
				if (num > yMax)
				{
					return;
				}
				num2 = c - b * num;
				num3 = yMax;
				if (vertex2 != null && vertex2.y < yMax)
				{
					num3 = vertex2.y;
				}
				if (num3 < yMin)
				{
					return;
				}
				num4 = c - b * num3;
				if ((num2 > xMax && num4 > xMax) || (num2 < xMin && num4 < xMin))
				{
					return;
				}
				if (num2 > xMax)
				{
					num2 = xMax;
					num = (c - num2) / b;
				}
				else if (num2 < xMin)
				{
					num2 = xMin;
					num = (c - num2) / b;
				}
				if (num4 > xMax)
				{
					num4 = xMax;
					num3 = (c - num4) / b;
				}
				else if (num4 < xMin)
				{
					num4 = xMin;
					num3 = (c - num4) / b;
				}
			}
			else
			{
				num2 = xMin;
				if (vertex != null && vertex.x > xMin)
				{
					num2 = vertex.x;
				}
				if (num2 > xMax)
				{
					return;
				}
				num = c - a * num2;
				num4 = xMax;
				if (vertex2 != null && vertex2.x < xMax)
				{
					num4 = vertex2.x;
				}
				if (num4 < xMin)
				{
					return;
				}
				num3 = c - a * num4;
				if ((num > yMax && num3 > yMax) || (num < yMin && num3 < yMin))
				{
					return;
				}
				if (num > yMax)
				{
					num = yMax;
					num2 = (c - num) / a;
				}
				else if (num < yMin)
				{
					num = yMin;
					num2 = (c - num) / a;
				}
				if (num3 > yMax)
				{
					num3 = yMax;
					num4 = (c - num3) / a;
				}
				else if (num3 < yMin)
				{
					num3 = yMin;
					num4 = (c - num3) / a;
				}
			}
			_clippedVertices = new Dictionary<Side, Vector2?>();
			if (vertex == _leftVertex)
			{
				_clippedVertices[Side.LEFT] = new Vector2(num2, num);
				_clippedVertices[Side.RIGHT] = new Vector2(num4, num3);
			}
			else
			{
				_clippedVertices[Side.RIGHT] = new Vector2(num2, num);
				_clippedVertices[Side.LEFT] = new Vector2(num4, num3);
			}
		}

		public void ClipVertices(Polygon bounds)
		{
			LineSegment lineSegment = new LineSegment(null, null);
			bool flag = (double)a == 1.0 && (double)b >= 0.0;
			if (flag)
			{
				lineSegment.p0 = _rightVertex.Coord;
				lineSegment.p1 = _leftVertex.Coord;
			}
			else
			{
				lineSegment.p0 = _leftVertex.Coord;
				lineSegment.p1 = _rightVertex.Coord;
			}
			LineSegment intersectingSegment = new LineSegment(null, null);
			bounds.ClipSegment(lineSegment, ref intersectingSegment);
			_clippedVertices = new Dictionary<Side, Vector2?>();
			if (!flag)
			{
				_clippedVertices[Side.LEFT] = intersectingSegment.p0;
				_clippedVertices[Side.RIGHT] = intersectingSegment.p1;
			}
			else
			{
				_clippedVertices[Side.RIGHT] = intersectingSegment.p0;
				_clippedVertices[Side.LEFT] = intersectingSegment.p1;
			}
		}
	}
}
