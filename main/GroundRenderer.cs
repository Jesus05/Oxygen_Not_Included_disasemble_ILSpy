using ProcGen;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GroundRenderer : KMonoBehaviour
{
	[Serializable]
	private struct Materials
	{
		public Material opaque;

		public Material alpha;

		public Materials(Material opaque, Material alpha)
		{
			this.opaque = opaque;
			this.alpha = alpha;
		}
	}

	private class ElementChunk
	{
		private class RenderData
		{
			public Material material;

			public Mesh mesh;

			public List<Vector3> pos;

			public List<Vector2> uv;

			public List<int> indices;

			public RenderData(Material material)
			{
				this.material = material;
				mesh = new Mesh();
				mesh.MarkDynamic();
				mesh.name = "ElementChunk";
				pos = new List<Vector3>();
				uv = new List<Vector2>();
				indices = new List<int>();
			}

			public void ClearMesh()
			{
				if ((UnityEngine.Object)mesh != (UnityEngine.Object)null)
				{
					mesh.Clear();
					UnityEngine.Object.DestroyImmediate(mesh);
					mesh = null;
				}
			}

			public void Clear()
			{
				if ((UnityEngine.Object)mesh != (UnityEngine.Object)null)
				{
					mesh.Clear();
				}
				if (pos != null)
				{
					pos.Clear();
				}
				if (uv != null)
				{
					uv.Clear();
				}
				if (indices != null)
				{
					indices.Clear();
				}
			}

			public void FreeResources()
			{
				ClearMesh();
				Clear();
				pos = null;
				uv = null;
				indices = null;
				material = null;
			}

			public void Build()
			{
				mesh.SetVertices(pos);
				mesh.SetUVs(0, uv);
				mesh.SetTriangles(indices, 0);
			}

			public void AddQuad(int x, int y, GroundMasks.UVData uvs)
			{
				int count = pos.Count;
				indices.Add(count);
				indices.Add(count + 1);
				indices.Add(count + 3);
				indices.Add(count);
				indices.Add(count + 3);
				indices.Add(count + 2);
				pos.Add(new Vector3((float)x + -0.5f, (float)y + -0.5f, 0f));
				pos.Add(new Vector3((float)x + 1f + -0.5f, (float)y + -0.5f, 0f));
				pos.Add(new Vector3((float)x + -0.5f, (float)y + 1f + -0.5f, 0f));
				pos.Add(new Vector3((float)x + 1f + -0.5f, (float)y + 1f + -0.5f, 0f));
				uv.Add(uvs.bl);
				uv.Add(uvs.br);
				uv.Add(uvs.tl);
				uv.Add(uvs.tr);
			}

			public void Render(Vector3 position, int layer)
			{
				if (pos.Count != 0)
				{
					Graphics.DrawMesh(mesh, position, Quaternion.identity, material, layer, null, 0, null, ShadowCastingMode.Off, false, null, false);
				}
			}
		}

		private class Tuning : TuningData<Tuning>
		{
			public bool _DrawAlpha;

			public bool _DrawOpaque;
		}

		public SimHashes element;

		private RenderData alpha;

		private RenderData opaque;

		public int tileCount;

		public ElementChunk(SimHashes element, Dictionary<SimHashes, Materials> materials)
		{
			this.element = element;
			Materials materials2 = materials[element];
			alpha = new RenderData(materials2.alpha);
			opaque = new RenderData(materials2.opaque);
			Clear();
		}

		public void Clear()
		{
			opaque.Clear();
			alpha.Clear();
			tileCount = 0;
		}

		public void AddOpaqueQuad(int x, int y, GroundMasks.UVData uvs)
		{
			opaque.AddQuad(x, y, uvs);
			tileCount++;
		}

		public void AddAlphaQuad(int x, int y, GroundMasks.UVData uvs)
		{
			alpha.AddQuad(x, y, uvs);
			tileCount++;
		}

		public void Build()
		{
			opaque.Build();
			alpha.Build();
		}

		public void Render(int layer, int element_idx)
		{
			float layerZ = Grid.GetLayerZ(Grid.SceneLayer.Ground);
			layerZ -= 0.0001f * (float)element_idx;
			opaque.Render(new Vector3(0f, 0f, layerZ), layer);
			alpha.Render(new Vector3(0f, 0f, layerZ), layer);
		}

		public void FreeResources()
		{
			alpha.FreeResources();
			opaque.FreeResources();
			alpha = null;
			opaque = null;
		}
	}

	private struct WorldChunk
	{
		public readonly int chunkX;

		public readonly int chunkY;

		private List<ElementChunk> elementChunks;

		private static Element[] elements = new Element[4];

		private static Element[] uniqueElements = new Element[4];

		private static int[] substances = new int[4];

		private static Vector2 NoiseScale = new Vector3(1f, 1f);

		public WorldChunk(int x, int y)
		{
			chunkX = x;
			chunkY = y;
			elementChunks = new List<ElementChunk>();
		}

		public void Clear()
		{
			elementChunks.Clear();
		}

		private static void InsertSorted(Element element, Element[] array, int size)
		{
			int id = (int)element.id;
			for (int i = 0; i < size; i++)
			{
				Element element2 = array[i];
				if ((int)element2.id > id)
				{
					array[i] = element;
					element = element2;
					id = (int)element2.id;
				}
			}
			array[size] = element;
		}

		public void Rebuild(GroundMasks.BiomeMaskData[] biomeMasks, Dictionary<SimHashes, Materials> materials)
		{
			foreach (ElementChunk elementChunk3 in elementChunks)
			{
				elementChunk3.Clear();
			}
			Vector2I vector2I = new Vector2I(chunkX * 16, chunkY * 16);
			Vector2I vector2I2 = new Vector2I(Math.Min(Grid.WidthInCells, (chunkX + 1) * 16), Math.Min(Grid.HeightInCells, (chunkY + 1) * 16));
			for (int i = vector2I.y; i < vector2I2.y; i++)
			{
				int num = Math.Max(0, i - 1);
				int num2 = i;
				for (int j = vector2I.x; j < vector2I2.x; j++)
				{
					int num3 = Math.Max(0, j - 1);
					int num4 = j;
					int num5 = num * Grid.WidthInCells + num3;
					int num6 = num * Grid.WidthInCells + num4;
					int num7 = num2 * Grid.WidthInCells + num3;
					int num8 = num2 * Grid.WidthInCells + num4;
					elements[0] = Grid.Element[num5];
					elements[1] = Grid.Element[num6];
					elements[2] = Grid.Element[num7];
					elements[3] = Grid.Element[num8];
					substances[0] = ((!Grid.RenderedByWorld[num5] || !elements[0].IsSolid) ? (-1) : elements[0].substance.idx);
					substances[1] = ((!Grid.RenderedByWorld[num6] || !elements[1].IsSolid) ? (-1) : elements[1].substance.idx);
					substances[2] = ((!Grid.RenderedByWorld[num7] || !elements[2].IsSolid) ? (-1) : elements[2].substance.idx);
					substances[3] = ((!Grid.RenderedByWorld[num8] || !elements[3].IsSolid) ? (-1) : elements[3].substance.idx);
					uniqueElements[0] = elements[0];
					InsertSorted(elements[1], uniqueElements, 1);
					InsertSorted(elements[2], uniqueElements, 2);
					InsertSorted(elements[3], uniqueElements, 3);
					int num9 = -1;
					int cell = i * Grid.WidthInCells + j;
					int biomeIdx = GetBiomeIdx(cell);
					GroundMasks.BiomeMaskData biomeMaskData = biomeMasks[biomeIdx];
					for (int k = 0; k < uniqueElements.Length; k++)
					{
						Element element = uniqueElements[k];
						if (element.IsSolid)
						{
							int idx = element.substance.idx;
							if (idx != num9)
							{
								num9 = idx;
								int num10 = (((substances[2] >= idx) ? 1 : 0) << 3) | (((substances[3] >= idx) ? 1 : 0) << 2) | (((substances[0] >= idx) ? 1 : 0) << 1) | (((substances[1] >= idx) ? 1 : 0) << 0);
								if (num10 > 0)
								{
									GroundMasks.UVData[] variationUVs = biomeMaskData.tiles[num10].variationUVs;
									float staticRandom = GetStaticRandom(j, i);
									int num11 = Mathf.Min(variationUVs.Length - 1, (int)((float)variationUVs.Length * staticRandom));
									GroundMasks.UVData uvs = variationUVs[num11 % variationUVs.Length];
									ElementChunk elementChunk = GetElementChunk(element.id, materials);
									if (num10 == 15)
									{
										elementChunk.AddOpaqueQuad(j, i, uvs);
									}
									else
									{
										elementChunk.AddAlphaQuad(j, i, uvs);
									}
								}
							}
						}
					}
				}
			}
			foreach (ElementChunk elementChunk4 in elementChunks)
			{
				elementChunk4.Build();
			}
			for (int num12 = elementChunks.Count - 1; num12 >= 0; num12--)
			{
				ElementChunk elementChunk2 = elementChunks[num12];
				if (elementChunk2.tileCount == 0)
				{
					int index = elementChunks.Count - 1;
					elementChunks[num12] = elementChunks[index];
					elementChunks.RemoveAt(index);
				}
			}
		}

		private ElementChunk GetElementChunk(SimHashes elementID, Dictionary<SimHashes, Materials> materials)
		{
			ElementChunk elementChunk = null;
			for (int i = 0; i < elementChunks.Count; i++)
			{
				if (elementChunks[i].element == elementID)
				{
					elementChunk = elementChunks[i];
					break;
				}
			}
			if (elementChunk == null)
			{
				elementChunk = new ElementChunk(elementID, materials);
				elementChunks.Add(elementChunk);
			}
			return elementChunk;
		}

		private static int GetBiomeIdx(int cell)
		{
			if (!Grid.IsValidCell(cell))
			{
				return 0;
			}
			return (int)World.Instance.zoneRenderData.GetSubWorldZoneType(cell);
		}

		private static float GetStaticRandom(int x, int y)
		{
			return PerlinSimplexNoise.noise((float)x * NoiseScale.x, (float)y * NoiseScale.y);
		}

		public void Render(int layer)
		{
			for (int i = 0; i < elementChunks.Count; i++)
			{
				ElementChunk elementChunk = elementChunks[i];
				elementChunk.Render(layer, ElementLoader.FindElementByHash(elementChunk.element).substance.idx);
			}
		}

		public void FreeResources()
		{
			foreach (ElementChunk elementChunk in elementChunks)
			{
				elementChunk.FreeResources();
			}
			elementChunks.Clear();
			elementChunks = null;
		}
	}

	[SerializeField]
	private GroundMasks masks;

	private GroundMasks.BiomeMaskData[] biomeMasks;

	private Dictionary<SimHashes, Materials> elementMaterials = new Dictionary<SimHashes, Materials>();

	private bool[,] dirtyChunks;

	private WorldChunk[,] worldChunks;

	private const int ChunkEdgeSize = 16;

	private Vector2I size;

	private static bool forceVisibleRebuild;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ShaderReloader.Register(OnShadersReloaded);
		OnShadersReloaded();
		masks.Initialize();
		SubWorld.ZoneType[] array = (SubWorld.ZoneType[])Enum.GetValues(typeof(SubWorld.ZoneType));
		biomeMasks = new GroundMasks.BiomeMaskData[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			SubWorld.ZoneType zone_type = array[i];
			biomeMasks[i] = GetBiomeMask(zone_type);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		size = new Vector2I((Grid.WidthInCells + 16 - 1) / 16, (Grid.HeightInCells + 16 - 1) / 16);
		dirtyChunks = new bool[size.x, size.y];
		worldChunks = new WorldChunk[size.x, size.y];
		for (int i = 0; i < size.y; i++)
		{
			for (int j = 0; j < size.x; j++)
			{
				worldChunks[j, i] = new WorldChunk(j, i);
				dirtyChunks[j, i] = true;
			}
		}
	}

	public void Render(Vector2I vis_min, Vector2I vis_max)
	{
		if (base.enabled)
		{
			int layer = LayerMask.NameToLayer("World");
			Vector2I vector2I = new Vector2I(vis_min.x / 16, vis_min.y / 16);
			Vector2I vector2I2 = new Vector2I((vis_max.x + 16 - 1) / 16, (vis_max.y + 16 - 1) / 16);
			for (int i = vector2I.y; i < vector2I2.y; i++)
			{
				for (int j = vector2I.x; j < vector2I2.x; j++)
				{
					WorldChunk worldChunk = worldChunks[j, i];
					if (dirtyChunks[j, i] || forceVisibleRebuild)
					{
						dirtyChunks[j, i] = false;
						worldChunk.Rebuild(biomeMasks, elementMaterials);
					}
					worldChunk.Render(layer);
				}
			}
			RebuildDirtyChunks();
		}
	}

	private void RebuildDirtyChunks()
	{
		for (int i = 0; i < dirtyChunks.GetLength(1); i++)
		{
			for (int j = 0; j < dirtyChunks.GetLength(0); j++)
			{
				if (dirtyChunks[j, i])
				{
					dirtyChunks[j, i] = false;
					worldChunks[j, i].Rebuild(biomeMasks, elementMaterials);
				}
			}
		}
	}

	public void MarkDirty(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2I vector2I2 = new Vector2I(vector2I.x / 16, vector2I.y / 16);
		dirtyChunks[vector2I2.x, vector2I2.y] = true;
		bool flag = vector2I.x % 16 == 0 && vector2I2.x > 0;
		bool flag2 = vector2I.x % 16 == 15 && vector2I2.x < size.x - 1;
		bool flag3 = vector2I.y % 16 == 0 && vector2I2.y > 0;
		bool flag4 = vector2I.y % 16 == 15 && vector2I2.y < size.y - 1;
		if (flag)
		{
			dirtyChunks[vector2I2.x - 1, vector2I2.y] = true;
			if (flag3)
			{
				dirtyChunks[vector2I2.x - 1, vector2I2.y - 1] = true;
			}
			if (flag4)
			{
				dirtyChunks[vector2I2.x - 1, vector2I2.y + 1] = true;
			}
		}
		if (flag3)
		{
			dirtyChunks[vector2I2.x, vector2I2.y - 1] = true;
		}
		if (flag4)
		{
			dirtyChunks[vector2I2.x, vector2I2.y + 1] = true;
		}
		if (flag2)
		{
			dirtyChunks[vector2I2.x + 1, vector2I2.y] = true;
			if (flag3)
			{
				dirtyChunks[vector2I2.x + 1, vector2I2.y - 1] = true;
			}
			if (flag4)
			{
				dirtyChunks[vector2I2.x + 1, vector2I2.y + 1] = true;
			}
		}
	}

	private Vector2I GetChunkIdx(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		return new Vector2I(vector2I.x / 16, vector2I.y / 16);
	}

	private GroundMasks.BiomeMaskData GetBiomeMask(SubWorld.ZoneType zone_type)
	{
		GroundMasks.BiomeMaskData result = null;
		string a = zone_type.ToString().ToLower();
		foreach (KeyValuePair<string, GroundMasks.BiomeMaskData> biomeMask in masks.biomeMasks)
		{
			string key = biomeMask.Key;
			if (a == key)
			{
				return biomeMask.Value;
			}
		}
		return result;
	}

	private void InitOpaqueMaterial(Material material, Element element)
	{
		material.name = element.id.ToString() + "_opaque";
		material.renderQueue = RenderQueues.WorldOpaque;
		material.EnableKeyword("OPAQUE");
		material.DisableKeyword("ALPHA");
		ConfigureMaterialShine(material);
		material.SetInt("_SrcAlpha", 1);
		material.SetInt("_DstAlpha", 0);
		material.SetInt("_ZWrite", 1);
		material.SetTexture("_AlphaTestMap", Texture2D.whiteTexture);
	}

	private void InitAlphaMaterial(Material material, Element element)
	{
		material.name = element.id.ToString() + "_alpha";
		material.renderQueue = RenderQueues.WorldTransparent;
		material.EnableKeyword("ALPHA");
		material.DisableKeyword("OPAQUE");
		ConfigureMaterialShine(material);
		material.SetTexture("_AlphaTestMap", masks.maskAtlas.texture);
		material.SetInt("_SrcAlpha", 5);
		material.SetInt("_DstAlpha", 10);
		material.SetInt("_ZWrite", 0);
	}

	private void ConfigureMaterialShine(Material material)
	{
		Texture texture = material.GetTexture("_ShineMask");
		if ((UnityEngine.Object)texture != (UnityEngine.Object)null)
		{
			material.DisableKeyword("MATTE");
			material.EnableKeyword("SHINY");
		}
		else
		{
			material.EnableKeyword("MATTE");
			material.DisableKeyword("SHINY");
		}
	}

	[ContextMenu("Reload Shaders")]
	public void OnShadersReloaded()
	{
		FreeMaterials();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsSolid)
			{
				if ((UnityEngine.Object)element.substance.material == (UnityEngine.Object)null)
				{
					Output.LogError(element.name, "must have material associated with it in the substance table");
				}
				Material material = new Material(element.substance.material);
				InitOpaqueMaterial(material, element);
				Material material2 = new Material(material);
				InitAlphaMaterial(material2, element);
				Materials value = new Materials(material, material2);
				elementMaterials[element.id] = value;
			}
		}
		if (worldChunks != null)
		{
			for (int i = 0; i < dirtyChunks.GetLength(1); i++)
			{
				for (int j = 0; j < dirtyChunks.GetLength(0); j++)
				{
					dirtyChunks[j, i] = true;
				}
			}
			WorldChunk[,] array = worldChunks;
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int k = 0; k < length; k++)
			{
				for (int l = 0; l < length2; l++)
				{
					WorldChunk worldChunk = array[k, l];
					worldChunk.Clear();
					worldChunk.Rebuild(biomeMasks, elementMaterials);
				}
			}
		}
	}

	public void FreeResources()
	{
		FreeMaterials();
		elementMaterials.Clear();
		elementMaterials = null;
		if (worldChunks != null)
		{
			WorldChunk[,] array = worldChunks;
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					WorldChunk worldChunk = array[i, j];
					worldChunk.FreeResources();
				}
			}
			worldChunks = null;
		}
	}

	private void FreeMaterials()
	{
		foreach (Materials value in elementMaterials.Values)
		{
			Materials current = value;
			UnityEngine.Object.Destroy(current.opaque);
			UnityEngine.Object.Destroy(current.alpha);
		}
		elementMaterials.Clear();
	}
}
