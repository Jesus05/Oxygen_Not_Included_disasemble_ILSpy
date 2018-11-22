using System;

namespace MIConvexHull
{
	internal class ObjectManager
	{
		private readonly int Dimension;

		private FaceConnector ConnectorStack;

		private readonly SimpleList<DeferredFace> DeferredFaceStack;

		private readonly SimpleList<IndexBuffer> EmptyBufferStack;

		private ConvexFaceInternal[] FacePool;

		private int FacePoolSize;

		private int FacePoolCapacity;

		private readonly IndexBuffer FreeFaceIndices;

		private readonly ConvexHullAlgorithm Hull;

		public ObjectManager(ConvexHullAlgorithm hull)
		{
			Dimension = hull.NumOfDimensions;
			Hull = hull;
			FacePool = hull.FacePool;
			FacePoolSize = 0;
			FacePoolCapacity = hull.FacePool.Length;
			FreeFaceIndices = new IndexBuffer();
			EmptyBufferStack = new SimpleList<IndexBuffer>();
			DeferredFaceStack = new SimpleList<DeferredFace>();
		}

		public void DepositFace(int faceIndex)
		{
			ConvexFaceInternal convexFaceInternal = FacePool[faceIndex];
			int[] adjacentFaces = convexFaceInternal.AdjacentFaces;
			for (int i = 0; i < adjacentFaces.Length; i++)
			{
				adjacentFaces[i] = -1;
			}
			FreeFaceIndices.Push(faceIndex);
		}

		private void ReallocateFacePool()
		{
			ConvexFaceInternal[] array = new ConvexFaceInternal[2 * FacePoolCapacity];
			bool[] array2 = new bool[2 * FacePoolCapacity];
			Array.Copy(FacePool, array, FacePoolCapacity);
			Buffer.BlockCopy(Hull.AffectedFaceFlags, 0, array2, 0, FacePoolCapacity);
			FacePoolCapacity = 2 * FacePoolCapacity;
			Hull.FacePool = array;
			FacePool = array;
			Hull.AffectedFaceFlags = array2;
		}

		private int CreateFace()
		{
			int facePoolSize = FacePoolSize;
			ConvexFaceInternal convexFaceInternal = new ConvexFaceInternal(Dimension, facePoolSize, GetVertexBuffer());
			FacePoolSize++;
			if (FacePoolSize > FacePoolCapacity)
			{
				ReallocateFacePool();
			}
			FacePool[facePoolSize] = convexFaceInternal;
			return facePoolSize;
		}

		public int GetFace()
		{
			if (FreeFaceIndices.Count <= 0)
			{
				return CreateFace();
			}
			return FreeFaceIndices.Pop();
		}

		public void DepositConnector(FaceConnector connector)
		{
			if (ConnectorStack == null)
			{
				connector.Next = null;
				ConnectorStack = connector;
			}
			else
			{
				connector.Next = ConnectorStack;
				ConnectorStack = connector;
			}
		}

		public FaceConnector GetConnector()
		{
			if (ConnectorStack != null)
			{
				FaceConnector connectorStack = ConnectorStack;
				ConnectorStack = ConnectorStack.Next;
				connectorStack.Next = null;
				return connectorStack;
			}
			return new FaceConnector(Dimension);
		}

		public void DepositVertexBuffer(IndexBuffer buffer)
		{
			buffer.Clear();
			EmptyBufferStack.Push(buffer);
		}

		public IndexBuffer GetVertexBuffer()
		{
			return (EmptyBufferStack.Count == 0) ? new IndexBuffer() : EmptyBufferStack.Pop();
		}

		public void DepositDeferredFace(DeferredFace face)
		{
			DeferredFaceStack.Push(face);
		}

		public DeferredFace GetDeferredFace()
		{
			return (DeferredFaceStack.Count == 0) ? new DeferredFace() : DeferredFaceStack.Pop();
		}
	}
}
