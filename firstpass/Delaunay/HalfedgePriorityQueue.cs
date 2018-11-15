using Delaunay.Utils;
using UnityEngine;

namespace Delaunay
{
	internal sealed class HalfedgePriorityQueue : IDisposable
	{
		private Halfedge[] _hash;

		private int _count;

		private int _minBucket;

		private int _hashsize;

		private float _ymin;

		private float _deltay;

		public HalfedgePriorityQueue(float ymin, float deltay, int sqrt_nsites)
		{
			_ymin = ymin;
			_deltay = deltay;
			_hashsize = 4 * sqrt_nsites;
			Initialize();
		}

		public void Dispose()
		{
			for (int i = 0; i < _hashsize; i++)
			{
				_hash[i].Dispose();
				_hash[i] = null;
			}
			_hash = null;
		}

		private void Initialize()
		{
			_count = 0;
			_minBucket = 0;
			_hash = new Halfedge[_hashsize];
			for (int i = 0; i < _hashsize; i++)
			{
				_hash[i] = Halfedge.CreateDummy();
				_hash[i].nextInPriorityQueue = null;
			}
		}

		public void Insert(Halfedge halfEdge)
		{
			int num = Bucket(halfEdge);
			if (num < _minBucket)
			{
				_minBucket = num;
			}
			Halfedge halfedge = _hash[num];
			Halfedge nextInPriorityQueue;
			while ((nextInPriorityQueue = halfedge.nextInPriorityQueue) != null && (halfEdge.ystar > nextInPriorityQueue.ystar || (halfEdge.ystar == nextInPriorityQueue.ystar && halfEdge.vertex.x > nextInPriorityQueue.vertex.x)))
			{
				halfedge = nextInPriorityQueue;
			}
			halfEdge.nextInPriorityQueue = halfedge.nextInPriorityQueue;
			halfedge.nextInPriorityQueue = halfEdge;
			_count++;
		}

		public void Remove(Halfedge halfEdge)
		{
			int num = Bucket(halfEdge);
			if (halfEdge.vertex != null)
			{
				Halfedge halfedge = _hash[num];
				while (halfedge.nextInPriorityQueue != halfEdge)
				{
					halfedge = halfedge.nextInPriorityQueue;
				}
				halfedge.nextInPriorityQueue = halfEdge.nextInPriorityQueue;
				_count--;
				halfEdge.vertex = null;
				halfEdge.nextInPriorityQueue = null;
				halfEdge.Dispose();
			}
		}

		private int Bucket(Halfedge halfEdge)
		{
			int num = (int)((halfEdge.ystar - _ymin) / _deltay * (float)_hashsize);
			if (num < 0)
			{
				num = 0;
			}
			if (num >= _hashsize)
			{
				num = _hashsize - 1;
			}
			return num;
		}

		private bool IsEmpty(int bucket)
		{
			return _hash[bucket].nextInPriorityQueue == null;
		}

		private void AdjustMinBucket()
		{
			while (_minBucket < _hashsize - 1 && IsEmpty(_minBucket))
			{
				_minBucket++;
			}
		}

		public bool Empty()
		{
			return _count == 0;
		}

		public Vector2 Min()
		{
			AdjustMinBucket();
			Halfedge nextInPriorityQueue = _hash[_minBucket].nextInPriorityQueue;
			return new Vector2(nextInPriorityQueue.vertex.x, nextInPriorityQueue.ystar);
		}

		public Halfedge ExtractMin()
		{
			Halfedge nextInPriorityQueue = _hash[_minBucket].nextInPriorityQueue;
			_hash[_minBucket].nextInPriorityQueue = nextInPriorityQueue.nextInPriorityQueue;
			_count--;
			nextInPriorityQueue.nextInPriorityQueue = null;
			return nextInPriorityQueue;
		}
	}
}
