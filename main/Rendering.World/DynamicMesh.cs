using UnityEngine;

namespace Rendering.World
{
	public class DynamicMesh
	{
		private static int TrianglesPerMesh = 65004;

		private static int VerticesPerMesh = 4 * TrianglesPerMesh / 6;

		public bool SetUVs;

		public bool SetTriangles;

		public string Name;

		public Bounds Bounds;

		public DynamicSubMesh[] Meshes = new DynamicSubMesh[0];

		private int VertexCount;

		private int TriangleCount;

		private int VertexIdx;

		private int UVIdx;

		private int TriangleIdx;

		private int TriangleMeshIdx;

		private int VertexMeshIdx;

		private int UVMeshIdx;

		public DynamicMesh(string name, Bounds bounds)
		{
			Name = name;
			Bounds = bounds;
		}

		public void Reserve(int vertex_count, int triangle_count)
		{
			if (vertex_count > VertexCount)
			{
				SetUVs = true;
			}
			else
			{
				SetUVs = false;
			}
			if (TriangleCount != triangle_count)
			{
				SetTriangles = true;
			}
			else
			{
				SetTriangles = false;
			}
			int num = (int)Mathf.Ceil((float)triangle_count / (float)TrianglesPerMesh);
			if (num != Meshes.Length)
			{
				Meshes = new DynamicSubMesh[num];
				for (int i = 0; i < Meshes.Length; i++)
				{
					int idx_offset = -i * VerticesPerMesh;
					Meshes[i] = new DynamicSubMesh(Name, Bounds, idx_offset);
				}
				SetUVs = true;
				SetTriangles = true;
			}
			for (int j = 0; j < Meshes.Length; j++)
			{
				if (j == Meshes.Length - 1)
				{
					Meshes[j].Reserve(vertex_count % VerticesPerMesh, triangle_count % TrianglesPerMesh);
				}
				else
				{
					Meshes[j].Reserve(VerticesPerMesh, TrianglesPerMesh);
				}
			}
			VertexCount = vertex_count;
			TriangleCount = triangle_count;
		}

		public void Commit()
		{
			DynamicSubMesh[] meshes = Meshes;
			foreach (DynamicSubMesh dynamicSubMesh in meshes)
			{
				dynamicSubMesh.Commit();
			}
			TriangleMeshIdx = 0;
			UVMeshIdx = 0;
			VertexMeshIdx = 0;
		}

		public void AddTriangle(int triangle)
		{
			DynamicSubMesh dynamicSubMesh = Meshes[TriangleMeshIdx];
			if (dynamicSubMesh.AreTrianglesFull())
			{
				dynamicSubMesh = Meshes[++TriangleMeshIdx];
			}
			Meshes[TriangleMeshIdx].AddTriangle(triangle);
		}

		public void AddUV(Vector2 uv)
		{
			DynamicSubMesh dynamicSubMesh = Meshes[UVMeshIdx];
			if (dynamicSubMesh.AreUVsFull())
			{
				dynamicSubMesh = Meshes[++UVMeshIdx];
			}
			dynamicSubMesh.AddUV(uv);
		}

		public void AddVertex(Vector3 vertex)
		{
			DynamicSubMesh dynamicSubMesh = Meshes[VertexMeshIdx];
			if (dynamicSubMesh.AreVerticesFull())
			{
				dynamicSubMesh = Meshes[++VertexMeshIdx];
			}
			dynamicSubMesh.AddVertex(vertex);
		}

		public void Render(Vector3 position, Quaternion rotation, Material material, int layer, MaterialPropertyBlock property_block)
		{
			DynamicSubMesh[] meshes = Meshes;
			foreach (DynamicSubMesh dynamicSubMesh in meshes)
			{
				dynamicSubMesh.Render(position, rotation, material, layer, property_block);
			}
		}
	}
}
