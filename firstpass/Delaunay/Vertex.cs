using Delaunay.LR;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
	public sealed class Vertex : ICoord
	{
		public static readonly Vertex VERTEX_AT_INFINITY = new Vertex(float.NaN, float.NaN);

		private static Stack<Vertex> _pool = new Stack<Vertex>();

		private static int _nvertices = 0;

		private Vector2 _coord;

		private int _vertexIndex;

		public Vector2 Coord => _coord;

		public int vertexIndex => _vertexIndex;

		public float x => _coord.x;

		public float y => _coord.y;

		public Vertex(float x, float y)
		{
			Init(x, y);
		}

		private static Vertex Create(float x, float y)
		{
			if (float.IsNaN(x) || float.IsNaN(y))
			{
				return VERTEX_AT_INFINITY;
			}
			if (_pool.Count > 0)
			{
				return _pool.Pop().Init(x, y);
			}
			return new Vertex(x, y);
		}

		private Vertex Init(float x, float y)
		{
			_coord = new Vector2(x, y);
			return this;
		}

		public void Dispose()
		{
			_pool.Push(this);
		}

		public void SetIndex()
		{
			_vertexIndex = _nvertices++;
		}

		public override string ToString()
		{
			return "Vertex (" + _vertexIndex + ")";
		}

		public static Vertex Intersect(Halfedge halfedge0, Halfedge halfedge1)
		{
			Edge edge = halfedge0.edge;
			Edge edge2 = halfedge1.edge;
			if (edge == null || edge2 == null)
			{
				return null;
			}
			if (edge.rightSite == edge2.rightSite)
			{
				return null;
			}
			float num = edge.a * edge2.b - edge.b * edge2.a;
			if (-1E-10 < (double)num && (double)num < 1E-10)
			{
				return null;
			}
			float num2 = (edge.c * edge2.b - edge2.c * edge.b) / num;
			float y = (edge2.c * edge.a - edge.c * edge2.a) / num;
			Halfedge halfedge2;
			Edge edge3;
			if (Voronoi.CompareByYThenX(edge.rightSite, edge2.rightSite) < 0)
			{
				halfedge2 = halfedge0;
				edge3 = edge;
			}
			else
			{
				halfedge2 = halfedge1;
				edge3 = edge2;
			}
			bool flag = num2 >= edge3.rightSite.x;
			if (flag)
			{
				Side? leftRight = halfedge2.leftRight;
				if (leftRight.GetValueOrDefault() == Side.LEFT && leftRight.HasValue)
				{
					goto IL_0138;
				}
			}
			if (flag || halfedge2.leftRight != Side.RIGHT)
			{
				return Create(num2, y);
			}
			goto IL_0138;
			IL_0138:
			return null;
		}
	}
}
