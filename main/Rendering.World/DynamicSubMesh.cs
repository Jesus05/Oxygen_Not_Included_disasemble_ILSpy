using UnityEngine;

namespace Rendering.World
{
	public class DynamicSubMesh
	{
		public Vector3[] Vertices = new Vector3[0];

		public Vector2[] UVs = new Vector2[0];

		public int[] Triangles = new int[0];

		public Mesh Mesh;

		public bool SetUVs;

		public bool SetTriangles;

		private int VertexIdx;

		private int UVIdx;

		private int TriangleIdx;

		private int IdxOffset;

		public DynamicSubMesh(string name, Bounds bounds, int idx_offset)
		{
			IdxOffset = idx_offset;
			Mesh = new Mesh();
			Mesh.name = name;
			Mesh.bounds = bounds;
			Mesh.MarkDynamic();
		}

		public void Reserve(int vertex_count, int triangle_count)
		{
			if (vertex_count > Vertices.Length)
			{
				Vertices = new Vector3[vertex_count];
				UVs = new Vector2[vertex_count];
				SetUVs = true;
			}
			else
			{
				SetUVs = false;
			}
			if (Triangles.Length != triangle_count)
			{
				Triangles = new int[triangle_count];
				SetTriangles = true;
			}
			else
			{
				SetTriangles = false;
			}
		}

		public bool AreTrianglesFull()
		{
			return Triangles.Length == TriangleIdx;
		}

		public bool AreVerticesFull()
		{
			return Vertices.Length == VertexIdx;
		}

		public bool AreUVsFull()
		{
			return UVs.Length == UVIdx;
		}

		public void Commit()
		{
			if (SetTriangles)
			{
				Mesh.Clear();
			}
			Mesh.vertices = Vertices;
			if (SetUVs || SetTriangles)
			{
				Mesh.uv = UVs;
			}
			if (SetTriangles)
			{
				Mesh.triangles = Triangles;
			}
			VertexIdx = 0;
			UVIdx = 0;
			TriangleIdx = 0;
		}

		public void AddTriangle(int triangle)
		{
			Triangles[TriangleIdx++] = triangle + IdxOffset;
		}

		public void AddUV(Vector2 uv)
		{
			UVs[UVIdx++] = uv;
		}

		public void AddVertex(Vector3 vertex)
		{
			Vertices[VertexIdx++] = vertex;
		}

		public void Render(Vector3 position, Quaternion rotation, Material material, int layer, MaterialPropertyBlock property_block)
		{
			Graphics.DrawMesh(Mesh, position, rotation, material, layer, null, 0, property_block, false, false);
		}
	}
}
