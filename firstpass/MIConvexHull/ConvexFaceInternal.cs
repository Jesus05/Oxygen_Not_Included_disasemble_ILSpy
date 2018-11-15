namespace MIConvexHull
{
	internal sealed class ConvexFaceInternal
	{
		public int[] AdjacentFaces;

		public int FurthestVertex;

		public int Index;

		public bool InList;

		public bool IsNormalFlipped;

		public ConvexFaceInternal Next;

		public double[] Normal;

		public double Offset;

		public ConvexFaceInternal Previous;

		public int Tag;

		public int[] Vertices;

		public IndexBuffer VerticesBeyond;

		public ConvexFaceInternal(int dimension, int index, IndexBuffer beyondList)
		{
			Index = index;
			AdjacentFaces = new int[dimension];
			VerticesBeyond = beyondList;
			Normal = new double[dimension];
			Vertices = new int[dimension];
		}
	}
}
