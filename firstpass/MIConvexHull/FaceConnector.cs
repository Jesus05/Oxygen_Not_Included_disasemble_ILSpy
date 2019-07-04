namespace MIConvexHull
{
	internal sealed class FaceConnector
	{
		public int EdgeIndex;

		public ConvexFaceInternal Face;

		public uint HashCode;

		public FaceConnector Next;

		public FaceConnector Previous;

		public int[] Vertices;

		public FaceConnector(int dimension)
		{
			Vertices = new int[dimension - 1];
		}

		public void Update(ConvexFaceInternal face, int edgeIndex, int dim)
		{
			Face = face;
			EdgeIndex = edgeIndex;
			uint num = 23u;
			int[] vertices = face.Vertices;
			int num2 = 0;
			for (int i = 0; i < edgeIndex; i++)
			{
				Vertices[num2++] = vertices[i];
				num = (uint)((int)num + ((int)(31 * num) + vertices[i]));
			}
			for (int i = edgeIndex + 1; i < vertices.Length; i++)
			{
				Vertices[num2++] = vertices[i];
				num = (uint)((int)num + ((int)(31 * num) + vertices[i]));
			}
			HashCode = num;
		}

		public static bool AreConnectable(FaceConnector a, FaceConnector b, int dim)
		{
			if (a.HashCode == b.HashCode)
			{
				int[] vertices = a.Vertices;
				int[] vertices2 = b.Vertices;
				for (int i = 0; i < vertices.Length; i++)
				{
					if (vertices[i] != vertices2[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public static void Connect(FaceConnector a, FaceConnector b)
		{
			a.Face.AdjacentFaces[a.EdgeIndex] = b.Face.Index;
			b.Face.AdjacentFaces[b.EdgeIndex] = a.Face.Index;
		}
	}
}
