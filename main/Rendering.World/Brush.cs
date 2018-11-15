using System.Collections.Generic;
using UnityEngine;

namespace Rendering.World
{
	public class Brush
	{
		private bool dirty;

		private Material material;

		private int layer;

		private HashSet<int> tiles = new HashSet<int>();

		private List<Brush> activeBrushes;

		private List<Brush> dirtyBrushes;

		private int widthInTiles;

		private Mask mask;

		private DynamicMesh mesh;

		private MaterialPropertyBlock propertyBlock;

		public int Id
		{
			get;
			private set;
		}

		public Brush(int id, string name, Material material, Mask mask, List<Brush> active_brushes, List<Brush> dirty_brushes, int width_in_tiles, MaterialPropertyBlock property_block)
		{
			Id = id;
			this.material = material;
			this.mask = mask;
			mesh = new DynamicMesh(name, new Bounds(Vector3.zero, new Vector3(3.40282347E+38f, 3.40282347E+38f, 0f)));
			activeBrushes = active_brushes;
			dirtyBrushes = dirty_brushes;
			layer = LayerMask.NameToLayer("World");
			widthInTiles = width_in_tiles;
			propertyBlock = property_block;
		}

		public void Add(int tile_idx)
		{
			tiles.Add(tile_idx);
			if (!dirty)
			{
				dirtyBrushes.Add(this);
				dirty = true;
			}
		}

		public void Remove(int tile_idx)
		{
			tiles.Remove(tile_idx);
			if (!dirty)
			{
				dirtyBrushes.Add(this);
				dirty = true;
			}
		}

		public void SetMaskOffset(int offset)
		{
			mask.SetOffset(offset);
		}

		public void Refresh()
		{
			bool flag = mesh.Meshes.Length > 0;
			int count = tiles.Count;
			int vertex_count = count * 4;
			int triangle_count = count * 6;
			mesh.Reserve(vertex_count, triangle_count);
			if (mesh.SetTriangles)
			{
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					mesh.AddTriangle(num);
					mesh.AddTriangle(2 + num);
					mesh.AddTriangle(1 + num);
					mesh.AddTriangle(1 + num);
					mesh.AddTriangle(2 + num);
					mesh.AddTriangle(3 + num);
					num += 4;
				}
			}
			foreach (int tile in tiles)
			{
				float num2 = (float)(tile % widthInTiles);
				float num3 = (float)(tile / widthInTiles);
				float z = 0f;
				mesh.AddVertex(new Vector3(num2 - 0.5f, num3 - 0.5f, z));
				mesh.AddVertex(new Vector3(num2 + 0.5f, num3 - 0.5f, z));
				mesh.AddVertex(new Vector3(num2 - 0.5f, num3 + 0.5f, z));
				mesh.AddVertex(new Vector3(num2 + 0.5f, num3 + 0.5f, z));
			}
			if (mesh.SetUVs)
			{
				for (int j = 0; j < count; j++)
				{
					mesh.AddUV(mask.UV0);
					mesh.AddUV(mask.UV1);
					mesh.AddUV(mask.UV2);
					mesh.AddUV(mask.UV3);
				}
			}
			dirty = false;
			mesh.Commit();
			if (mesh.Meshes.Length > 0)
			{
				if (!flag)
				{
					activeBrushes.Add(this);
				}
			}
			else if (flag)
			{
				activeBrushes.Remove(this);
			}
		}

		public void Render()
		{
			Vector3 position = new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.Ground));
			mesh.Render(position, Quaternion.identity, material, layer, propertyBlock);
		}

		public void SetMaterial(Material material, MaterialPropertyBlock property_block)
		{
			this.material = material;
			propertyBlock = property_block;
		}
	}
}
