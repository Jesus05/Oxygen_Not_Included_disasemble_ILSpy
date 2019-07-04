using Delaunay.Utils;
using UnityEngine;

namespace Delaunay
{
	internal sealed class EdgeList : IDisposable
	{
		private float _deltax;

		private float _xmin;

		private int _hashsize;

		private Halfedge[] _hash;

		private Halfedge _leftEnd;

		private Halfedge _rightEnd;

		public Halfedge leftEnd => _leftEnd;

		public Halfedge rightEnd => _rightEnd;

		public EdgeList(float xmin, float deltax, int sqrt_nsites)
		{
			_xmin = xmin;
			_deltax = deltax;
			_hashsize = 2 * sqrt_nsites;
			_hash = new Halfedge[_hashsize];
			_leftEnd = Halfedge.CreateDummy();
			_rightEnd = Halfedge.CreateDummy();
			_leftEnd.edgeListLeftNeighbor = null;
			_leftEnd.edgeListRightNeighbor = _rightEnd;
			_rightEnd.edgeListLeftNeighbor = _leftEnd;
			_rightEnd.edgeListRightNeighbor = null;
			_hash[0] = _leftEnd;
			_hash[_hashsize - 1] = _rightEnd;
		}

		public void Dispose()
		{
			Halfedge halfedge = _leftEnd;
			while (halfedge != _rightEnd)
			{
				Halfedge halfedge2 = halfedge;
				halfedge = halfedge.edgeListRightNeighbor;
				halfedge2.Dispose();
			}
			_leftEnd = null;
			_rightEnd.Dispose();
			_rightEnd = null;
			for (int i = 0; i < _hashsize; i++)
			{
				_hash[i] = null;
			}
			_hash = null;
		}

		public void Insert(Halfedge lb, Halfedge newHalfedge)
		{
			newHalfedge.edgeListLeftNeighbor = lb;
			newHalfedge.edgeListRightNeighbor = lb.edgeListRightNeighbor;
			lb.edgeListRightNeighbor.edgeListLeftNeighbor = newHalfedge;
			lb.edgeListRightNeighbor = newHalfedge;
		}

		public void Remove(Halfedge halfEdge)
		{
			halfEdge.edgeListLeftNeighbor.edgeListRightNeighbor = halfEdge.edgeListRightNeighbor;
			halfEdge.edgeListRightNeighbor.edgeListLeftNeighbor = halfEdge.edgeListLeftNeighbor;
			halfEdge.edge = Edge.DELETED;
			halfEdge.edgeListLeftNeighbor = (halfEdge.edgeListRightNeighbor = null);
		}

		public Halfedge EdgeListLeftNeighbor(Vector2 p)
		{
			int num = (int)((p.x - _xmin) / _deltax * (float)_hashsize);
			if (num < 0)
			{
				num = 0;
			}
			if (num >= _hashsize)
			{
				num = _hashsize - 1;
			}
			Halfedge halfedge = GetHash(num);
			if (halfedge == null)
			{
				int num2 = 1;
				while ((halfedge = GetHash(num - num2)) == null && (halfedge = GetHash(num + num2)) == null)
				{
					num2++;
				}
			}
			if (halfedge != leftEnd && (halfedge == rightEnd || !halfedge.IsLeftOf(p)))
			{
				do
				{
					halfedge = halfedge.edgeListLeftNeighbor;
				}
				while (halfedge != leftEnd && !halfedge.IsLeftOf(p));
			}
			else
			{
				do
				{
					halfedge = halfedge.edgeListRightNeighbor;
				}
				while (halfedge != rightEnd && halfedge.IsLeftOf(p));
				halfedge = halfedge.edgeListLeftNeighbor;
			}
			if (num > 0 && num < _hashsize - 1)
			{
				_hash[num] = halfedge;
			}
			return halfedge;
		}

		private Halfedge GetHash(int b)
		{
			if (b >= 0 && b < _hashsize)
			{
				Halfedge halfedge = _hash[b];
				if (halfedge != null && halfedge.edge == Edge.DELETED)
				{
					_hash[b] = null;
					return null;
				}
				return halfedge;
			}
			return null;
		}
	}
}
