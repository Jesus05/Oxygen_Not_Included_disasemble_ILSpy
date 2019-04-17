using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class KAnimBatchGroup
{
	public class KAnimBatchTextureCache
	{
		public class Entry
		{
			[StructLayout(LayoutKind.Explicit)]
			public struct ByteToFloatConverter
			{
				[FieldOffset(0)]
				public byte[] bytes;

				[FieldOffset(0)]
				public float[] floats;
			}

			private ByteToFloatConverter floatConverter;

			public int texturePropertyId;

			public int textureSizePropertyId;

			public Texture2D texture
			{
				get;
				private set;
			}

			public byte[] bytes => floatConverter.bytes;

			public float[] floats => floatConverter.floats;

			public int width => texture.width;

			public int height => texture.height;

			public Vector2 texelSize => texture.texelSize;

			public string name
			{
				get
				{
					return texture.name;
				}
				set
				{
					texture.name = value;
				}
			}

			public Entry(int float4s_per_side)
			{
				texture = new Texture2D(float4s_per_side, float4s_per_side, TextureFormat.RGBAFloat, false);
				texture.wrapMode = TextureWrapMode.Clamp;
				texture.filterMode = FilterMode.Point;
				texture.anisoLevel = 0;
				floatConverter = new ByteToFloatConverter
				{
					bytes = new byte[float4s_per_side * float4s_per_side * 4 * 4]
				};
			}

			public void SetTextureAndSize(MaterialPropertyBlock property_block)
			{
				property_block.SetTexture(texturePropertyId, texture);
				int nameID = textureSizePropertyId;
				Vector2 texelSize = this.texelSize;
				float x = texelSize.x;
				Vector2 texelSize2 = this.texelSize;
				property_block.SetVector(nameID, new Vector4(x, texelSize2.y, (float)width, (float)height));
			}

			public void Apply()
			{
				texture.Apply();
			}

			public void LoadRawTextureData()
			{
				texture.LoadRawTextureData(bytes);
			}
		}

		private Dictionary<int, List<Entry>> unused = new Dictionary<int, List<Entry>>();

		private Dictionary<int, List<Entry>> inuse = new Dictionary<int, List<Entry>>();

		public Entry Get(int float4s_per_side, int texture_property_id, int texture_size_property_id)
		{
			if (!unused.ContainsKey(float4s_per_side))
			{
				unused.Add(float4s_per_side, new List<Entry>());
			}
			Entry entry = null;
			if (unused[float4s_per_side].Count > 0)
			{
				entry = unused[float4s_per_side][0];
				unused[float4s_per_side].RemoveAt(0);
			}
			else
			{
				entry = new Entry(float4s_per_side);
			}
			entry.texturePropertyId = texture_property_id;
			entry.textureSizePropertyId = texture_size_property_id;
			if (!inuse.ContainsKey(float4s_per_side))
			{
				inuse.Add(float4s_per_side, new List<Entry>());
			}
			inuse[float4s_per_side].Add(entry);
			return entry;
		}

		public void Free(Entry entry)
		{
			int width = entry.texture.width;
			if (inuse.ContainsKey(width))
			{
				inuse[width].Remove(entry);
			}
			if (unused.ContainsKey(width))
			{
				unused[width].Add(entry);
			}
		}

		public void Finalise()
		{
			foreach (KeyValuePair<int, List<Entry>> item in inuse)
			{
				for (int i = 0; i < item.Value.Count; i++)
				{
					Object.Destroy(item.Value[i].texture);
				}
			}
			inuse.Clear();
			foreach (KeyValuePair<int, List<Entry>> item2 in unused)
			{
				for (int j = 0; j < item2.Value.Count; j++)
				{
					Object.Destroy(item2.Value[j].texture);
				}
			}
			unused.Clear();
		}
	}

	public enum RendererType
	{
		Default,
		UI,
		StaticBatch,
		DontRender,
		AnimOnly
	}

	public enum MaterialType
	{
		Default,
		Simple,
		Placer,
		UI,
		Overlay,
		NumMaterials
	}

	public static int ShaderProperty_SYMBOLS_PER_BUILD = Shader.PropertyToID("SYMBOLS_PER_BUILD");

	public static int ShaderProperty_ANIM_TEXTURE_START_OFFSET = Shader.PropertyToID("ANIM_TEXTURE_START_OFFSET");

	public static int ShaderProperty_SYMBOL_OVERRIDES_PER_BUILD = Shader.PropertyToID("SYMBOL_OVERRIDES_PER_BUILD");

	private static KAnimBatchTextureCache cache = new KAnimBatchTextureCache();

	public int batchCount;

	private int float4sPerSide;

	private static int ShaderProperty_BUILD_AND_ANIM_TEXTURE_SIZE = Shader.PropertyToID("BUILD_AND_ANIM_TEXTURE_SIZE");

	private static int ShaderProperty_buildAndAnimTex = Shader.PropertyToID("buildAndAnimTex");

	private static int ShaderProperty_INSTANCE_TEXTURE_SIZE = Shader.PropertyToID("INSTANCE_TEXTURE_SIZE");

	private static int ShaderProperty_instanceTex = Shader.PropertyToID("instanceTex");

	private Material[] materials;

	public int maxGroupSize
	{
		get;
		private set;
	}

	public Mesh mesh
	{
		get;
		private set;
	}

	public HashedString batchID
	{
		get;
		private set;
	}

	public KBatchGroupData data
	{
		get;
		private set;
	}

	public KAnimBatchTextureCache.Entry buildAndAnimTex
	{
		get;
		private set;
	}

	public bool InitOK => float4sPerSide > 0;

	public KAnimBatchGroup(HashedString id)
	{
		data = KAnimBatchManager.Instance().GetBatchGroupData(id);
		materials = new Material[5];
		batchID = id;
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(id);
		if (group != null)
		{
			maxGroupSize = group.maxGroupSize;
			if (maxGroupSize <= 0)
			{
				maxGroupSize = 30;
			}
			SetupMeshData();
			InitBuildAndAnimTex();
		}
	}

	public static void FinalizeTextureCache()
	{
		cache.Finalise();
	}

	private Material CreateMaterial(MaterialType material_type)
	{
		Material material = null;
		switch (material_type)
		{
		case MaterialType.Simple:
			material = new Material(Shader.Find("Klei/AnimationSimple"));
			break;
		case MaterialType.UI:
			material = new Material(Shader.Find("Klei/BatchedAnimationUI"));
			break;
		case MaterialType.Overlay:
			material = new Material(Shader.Find("Klei/AnimationOverlay"));
			break;
		default:
			material = new Material(Shader.Find("Klei/BatchedAnimation"));
			break;
		}
		material.name = "Material:" + batchID.ToString();
		material.SetFloat(ShaderProperty_SYMBOLS_PER_BUILD, (float)data.maxSymbolsPerBuild);
		material.SetFloat(ShaderProperty_ANIM_TEXTURE_START_OFFSET, (float)data.animDataStartOffset);
		material.SetFloat(ShaderProperty_SYMBOL_OVERRIDES_PER_BUILD, (float)data.symbolFrameInstances.Count);
		return material;
	}

	public Material GetMaterial(MaterialType material_type)
	{
		if ((Object)materials[(int)material_type] == (Object)null)
		{
			materials[(int)material_type] = CreateMaterial(material_type);
		}
		return materials[(int)material_type];
	}

	public void FreeResources()
	{
		if (buildAndAnimTex != null)
		{
			cache.Free(buildAndAnimTex);
			buildAndAnimTex = null;
		}
		for (int i = 0; i < 5; i++)
		{
			if ((Object)materials[i] != (Object)null)
			{
				Object.Destroy(materials[i]);
				materials[i] = null;
			}
		}
		if ((Object)mesh != (Object)null)
		{
			Object.Destroy(mesh);
			mesh = null;
		}
		if (data != null)
		{
			data.FreeResources();
		}
		data = null;
	}

	public static int GetBestTextureSize(float cost)
	{
		float f = Mathf.Sqrt(cost);
		int num = Mathf.CeilToInt(f);
		int num2 = 32;
		float f2 = (float)num / (float)num2;
		int num3 = Mathf.CeilToInt(f2);
		return num3 * num2;
	}

	private void SetupMeshData()
	{
		Debug.Assert(maxGroupSize > 0, "Group size must be >0");
		maxGroupSize = Mathf.Min(maxGroupSize, 30);
		mesh = BuildMesh(maxGroupSize * data.maxVisibleSymbols);
		float cost = (float)(maxGroupSize * 28) / 4f;
		float4sPerSide = GetBestTextureSize(cost);
	}

	private float GetBuildDataSize()
	{
		int num = data.GetBuildSymbolFrameCount() * 16;
		return (float)num / 4f;
	}

	private float GetAnimDataSize()
	{
		int num = 4;
		List<KAnim.Anim.Frame> animFrames = data.GetAnimFrames();
		if (animFrames.Count == 0)
		{
			num += data.symbolFrameInstances.Count * 4;
			num += data.symbolFrameInstances.Count * 16;
		}
		else
		{
			num += animFrames.Count * 4;
			List<KAnim.Anim.FrameElement> animFrameElements = data.GetAnimFrameElements();
			num += animFrameElements.Count * 16;
		}
		return (float)num / 4f;
	}

	public void InitBuildAndAnimTex()
	{
		float num = GetBuildDataSize() + GetAnimDataSize();
		int bestTextureSize = GetBestTextureSize(num);
		buildAndAnimTex = cache.Get(bestTextureSize, ShaderProperty_buildAndAnimTex, ShaderProperty_BUILD_AND_ANIM_TEXTURE_SIZE);
		buildAndAnimTex.name = "BuildAndAnimData:" + batchID.ToString();
		if (num > (float)(buildAndAnimTex.width * buildAndAnimTex.height))
		{
			Debug.LogErrorFormat("Texture is the wrong size! {0} <= {1}", num, buildAndAnimTex.width * buildAndAnimTex.height);
		}
		int start_index = data.WriteBuildData(data.symbolFrameInstances, buildAndAnimTex.floats);
		data.WriteAnimData(start_index, buildAndAnimTex.floats);
		buildAndAnimTex.LoadRawTextureData();
		buildAndAnimTex.Apply();
	}

	private Mesh BuildMesh(int numQuads)
	{
		Mesh mesh = new Mesh();
		int[] array = new int[numQuads * 6];
		for (int i = 0; i < numQuads; i++)
		{
			int num = i * 6;
			int num2 = array[num] = i * 4;
			array[num + 1] = num2 + 1;
			array[num + 2] = num2 + 2;
			array[num + 3] = num2 + 1;
			array[num + 4] = num2 + 2;
			array[num + 5] = num2 + 3;
		}
		Vector3[] array2 = new Vector3[numQuads * 4];
		Vector2[] array3 = new Vector2[numQuads * 4];
		Vector4[] array4 = new Vector4[numQuads * 4];
		for (int j = 0; j < numQuads; j++)
		{
			int num3 = j * 4;
			array2[num3] = Vector3.zero;
			array2[num3 + 1] = Vector3.zero;
			array2[num3 + 2] = Vector3.zero;
			array2[num3 + 3] = Vector3.zero;
			Vector2 vector = new Vector2((float)(j / data.maxVisibleSymbols), (float)(data.maxVisibleSymbols - j % data.maxVisibleSymbols - 1));
			array3[num3] = vector;
			array3[num3 + 1] = vector;
			array3[num3 + 2] = vector;
			array3[num3 + 3] = vector;
			array4[num3] = new Vector4(0f, (float)num3, (float)j);
			array4[num3 + 1] = new Vector4(1f, (float)num3, (float)j);
			array4[num3 + 2] = new Vector4(2f, (float)num3, (float)j);
			array4[num3 + 3] = new Vector4(3f, (float)num3, (float)j);
		}
		mesh.name = "BatchGroup:" + batchID.ToString();
		mesh.vertices = array2;
		mesh.SetUVs(0, new List<Vector2>(array3));
		mesh.SetUVs(1, new List<Vector4>(array4));
		mesh.SetIndices(array, MeshTopology.Triangles, 0);
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(3.40282347E+38f, 3.40282347E+38f, 3.40282347E+38f));
		return mesh;
	}

	public KAnimBatchTextureCache.Entry CreateTexture(string name, int size_in_floats, int texture_property_id, int texture_size_property_id)
	{
		DebugUtil.Assert(size_in_floats > 0);
		KAnimBatchTextureCache.Entry entry = cache.Get(size_in_floats, texture_property_id, texture_size_property_id);
		entry.name = name;
		return entry;
	}

	public KAnimBatchTextureCache.Entry CreateTexture()
	{
		if (float4sPerSide <= 0)
		{
			Debug.LogErrorFormat("Need to init AnimBatchGroup [{0}] first!", batchID);
		}
		return CreateTexture("InstanceData:" + batchID.ToString(), float4sPerSide, ShaderProperty_instanceTex, ShaderProperty_INSTANCE_TEXTURE_SIZE);
	}

	public void FreeTexture(KAnimBatchTextureCache.Entry entry)
	{
		cache.Free(entry);
	}

	public void GetDataTextures(MaterialPropertyBlock matProperties, KAnimBatch.AtlasList atlases)
	{
		if (buildAndAnimTex != null)
		{
			buildAndAnimTex.SetTextureAndSize(matProperties);
		}
		matProperties.SetFloat(ShaderProperty_ANIM_TEXTURE_START_OFFSET, (float)data.animDataStartOffset);
		for (int i = 0; i < data.textures.Count; i++)
		{
			atlases.Add(data.textures[i]);
		}
	}
}
