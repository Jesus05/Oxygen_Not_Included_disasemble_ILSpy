using System.Collections.Generic;
using UnityEngine;

public class KAnimBatch
{
	public struct SymbolInstanceSlot
	{
		public SymbolInstanceGpuData symbolInstanceData;

		public int dataVersion;
	}

	public struct SymbolOverrideInfoSlot
	{
		public SymbolOverrideInfoGpuData symbolOverrideInfo;

		public int dataVersion;
	}

	public class AtlasList
	{
		private List<Texture2D> atlases = new List<Texture2D>();

		private int startIdx;

		public int Count => atlases.Count;

		public AtlasList(int start_idx)
		{
			startIdx = start_idx;
		}

		public int Add(Texture2D atlas)
		{
			DebugUtil.Assert((Object)atlas != (Object)null);
			DebugUtil.Assert(atlases.Count < KAnimBatchManager.instance.atlasNames.Length);
			int num = atlases.IndexOf(atlas);
			if (num == -1)
			{
				num = atlases.Count;
				atlases.Add(atlas);
			}
			return num + startIdx;
		}

		public void Apply(MaterialPropertyBlock material_property_block)
		{
			bool flag = false;
			for (int i = 0; i < atlases.Count; i++)
			{
				int num = startIdx + i;
				if (num >= KAnimBatchManager.instance.atlasNames.Length)
				{
					flag = true;
				}
				else
				{
					material_property_block.SetTexture(KAnimBatchManager.instance.atlasNames[num], atlases[i]);
				}
			}
			if (flag)
			{
				string text = "Atlas overflow: + \n";
				foreach (Texture2D atlase in atlases)
				{
					text = text + atlase.name + "\n";
				}
				Debug.LogError(text, null);
			}
		}

		public void Clear(int start_idx)
		{
			atlases.Clear();
			startIdx = start_idx;
		}

		public int GetAtlasIdx(Texture2D atlas)
		{
			for (int i = 0; i < atlases.Count; i++)
			{
				if ((Object)atlases[i] == (Object)atlas)
				{
					return i + startIdx;
				}
			}
			return -1;
		}
	}

	private List<KAnimConverter.IAnimConverter> controllers = new List<KAnimConverter.IAnimConverter>();

	private List<int> dirtySet = new List<int>();

	private static int nextBatchId;

	private int currentOffset = 0;

	private static int ShaderProperty_SYMBOL_INSTANCE_TEXTURE_SIZE = Shader.PropertyToID("SYMBOL_INSTANCE_TEXTURE_SIZE");

	private static int ShaderProperty_symbolInstanceTex = Shader.PropertyToID("symbolInstanceTex");

	private static int ShaderProperty_SYMBOL_OVERRIDE_INFO_TEXTURE_SIZE = Shader.PropertyToID("SYMBOL_OVERRIDE_INFO_TEXTURE_SIZE");

	private static int ShaderProperty_symbolOverrideInfoTex = Shader.PropertyToID("symbolOverrideInfoTex");

	public static int ShaderProperty_SUPPORTS_SYMBOL_OVERRIDING = Shader.PropertyToID("SUPPORTS_SYMBOL_OVERRIDING");

	public static int ShaderProperty_ANIM_TEXTURE_START_OFFSET = Shader.PropertyToID("ANIM_TEXTURE_START_OFFSET");

	private SymbolInstanceSlot[] symbolInstanceSlots;

	private SymbolOverrideInfoSlot[] symbolOverrideInfoSlots;

	public AtlasList atlases = new AtlasList(0);

	private bool needsWrite = false;

	public int id
	{
		get;
		private set;
	}

	public bool dirty => dirtySet.Count > 0;

	public int dirtyCount => dirtySet.Count;

	public bool active
	{
		get;
		private set;
	}

	public int size => controllers.Count;

	public Vector3 position
	{
		get;
		private set;
	}

	public int layer
	{
		get;
		private set;
	}

	public List<KAnimConverter.IAnimConverter> Controllers => controllers;

	public KAnimBatchGroup.MaterialType materialType
	{
		get;
		private set;
	}

	public HashedString batchGroup
	{
		get;
		private set;
	}

	public BatchSet batchset
	{
		get;
		private set;
	}

	public KAnimBatchGroup group
	{
		get;
		private set;
	}

	public int writtenLastFrame
	{
		get;
		private set;
	}

	public MaterialPropertyBlock matProperties
	{
		get;
		private set;
	}

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry dataTex
	{
		get;
		private set;
	}

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry symbolInstanceTex
	{
		get;
		private set;
	}

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry symbolOverrideInfoTex
	{
		get;
		private set;
	}

	public KAnimBatch(KAnimBatchGroup group, int layer, float z, KAnimBatchGroup.MaterialType material_type)
	{
		id = nextBatchId++;
		active = true;
		this.group = group;
		this.layer = layer;
		batchGroup = group.batchID;
		materialType = material_type;
		position = new Vector3(0f, 0f, z);
		symbolInstanceSlots = new SymbolInstanceSlot[group.maxGroupSize];
		symbolOverrideInfoSlots = new SymbolOverrideInfoSlot[group.maxGroupSize];
	}

	public void DestroyTex()
	{
		if (dataTex != null)
		{
			group.FreeTexture(dataTex);
			dataTex = null;
		}
		if (symbolInstanceTex != null)
		{
			group.FreeTexture(symbolInstanceTex);
			symbolInstanceTex = null;
		}
		if (symbolOverrideInfoTex != null)
		{
			group.FreeTexture(symbolOverrideInfoTex);
			symbolOverrideInfoTex = null;
		}
	}

	public void Init()
	{
		dataTex = group.CreateTexture();
		if (dataTex == null)
		{
			Debug.LogErrorFormat("Got null data texture from AnimBatchGroup [{0}]", batchGroup);
		}
		int bestTextureSize = KAnimBatchGroup.GetBestTextureSize((float)(group.data.maxSymbolsPerBuild * group.maxGroupSize * 8));
		symbolInstanceTex = group.CreateTexture("SymbolInstanceTex", bestTextureSize, ShaderProperty_symbolInstanceTex, ShaderProperty_SYMBOL_INSTANCE_TEXTURE_SIZE);
		int width = dataTex.width;
		if (width == 0)
		{
			Debug.LogWarning("Empty group [" + group.batchID + "] " + batchset.idx + " (probably just anims)", null);
		}
		else
		{
			for (int i = 0; i < width * width; i++)
			{
				dataTex.floats[i * 4] = -1f;
				dataTex.floats[i * 4 + 1] = 0f;
				dataTex.floats[i * 4 + 2] = 0f;
				dataTex.floats[i * 4 + 3] = 0f;
			}
			matProperties = new MaterialPropertyBlock();
			dataTex.SetTextureAndSize(matProperties);
			symbolInstanceTex.SetTextureAndSize(matProperties);
			group.GetDataTextures(matProperties, atlases);
			atlases.Apply(matProperties);
		}
	}

	public void Clear()
	{
		DestroyTex();
		controllers.Clear();
		dirtySet.Clear();
		batchset = null;
		group = null;
		matProperties = null;
		dataTex = null;
	}

	public void SetBatchSet(BatchSet newBatchSet)
	{
		if (batchset != null && batchset != newBatchSet)
		{
			batchset.RemoveBatch(this);
		}
		batchset = newBatchSet;
		if (batchset != null)
		{
			Vector2I idx = batchset.idx;
			float x = (float)(idx.x * 32);
			Vector2I idx2 = batchset.idx;
			float y = (float)(idx2.y * 32);
			Vector3 position = this.position;
			this.position = new Vector3(x, y, position.z);
			active = batchset.active;
		}
	}

	public bool Register(KAnimConverter.IAnimConverter controller)
	{
		if (dataTex == null || dataTex.floats.Length == 0)
		{
			Init();
		}
		if (!controllers.Contains(controller))
		{
			controllers.Add(controller);
			currentOffset += 28;
		}
		AddToDirty(controllers.IndexOf(controller));
		controller.GetBatch()?.Deregister(controller);
		controller.SetBatch(this);
		return true;
	}

	public void OverrideZ(float z)
	{
		Vector3 position = this.position;
		float x = position.x;
		Vector3 position2 = this.position;
		this.position = new Vector3(x, position2.y, z);
	}

	public void SetLayer(int layer)
	{
		this.layer = layer;
	}

	public void Deregister(KAnimConverter.IAnimConverter controller)
	{
		if (!App.IsExiting)
		{
			int num = controllers.IndexOf(controller);
			if (num >= 0)
			{
				if (!controllers.Remove(controller))
				{
					Debug.LogError("Failed to remove controller [" + controller.GetName() + "]", null);
				}
				controller.SetBatch(null);
				currentOffset -= 28;
				currentOffset = Mathf.Max(0, currentOffset);
				for (int i = 0; i < 28; i++)
				{
					dataTex.floats[currentOffset + i] = -1f;
				}
				currentOffset = 28 * controllers.Count;
				ClearDirty();
				for (int j = 0; j < controllers.Count; j++)
				{
					AddToDirty(j);
				}
			}
			else
			{
				Debug.LogError("Deregister called for [" + controller.GetName() + "] but its not in this batch ", null);
			}
			if (controllers.Count == 0)
			{
				batchset.RemoveBatch(this);
				DestroyTex();
			}
		}
	}

	private void ClearDirty()
	{
		dirtySet.Clear();
	}

	private void AddToDirty(int dirtyIdx)
	{
		if (!dirtySet.Contains(dirtyIdx))
		{
			dirtySet.Add(dirtyIdx);
		}
		batchset.SetDirty();
		needsWrite = true;
	}

	public void Activate()
	{
		active = true;
	}

	public void Deactivate()
	{
		active = false;
	}

	public void SetDirty(KAnimConverter.IAnimConverter controller)
	{
		int num = controllers.IndexOf(controller);
		if (num < 0)
		{
			Debug.LogError("Setting controller [" + controller.GetName() + "] to dirty but its not in this batch", null);
		}
		else
		{
			AddToDirty(num);
		}
	}

	private void WriteBatchedAnimInstanceData(int index, KAnimConverter.IAnimConverter controller)
	{
		controller.GetBatchInstanceData().WriteToTexture(dataTex.bytes, index * 112, index);
	}

	private bool WriteSymbolInstanceData(int index, KAnimConverter.IAnimConverter controller)
	{
		bool result = false;
		SymbolInstanceSlot symbolInstanceSlot = symbolInstanceSlots[index];
		if (symbolInstanceSlot.symbolInstanceData != controller.symbolInstanceGpuData || symbolInstanceSlot.dataVersion != controller.symbolInstanceGpuData.version)
		{
			controller.symbolInstanceGpuData.WriteToTexture(symbolInstanceTex.bytes, index * 8 * group.data.maxSymbolsPerBuild * 4, index);
			symbolInstanceSlot.symbolInstanceData = controller.symbolInstanceGpuData;
			symbolInstanceSlot.dataVersion = controller.symbolInstanceGpuData.version;
			symbolInstanceSlots[index] = symbolInstanceSlot;
			result = true;
		}
		return result;
	}

	private bool WriteSymbolOverrideInfoTex(int index, KAnimConverter.IAnimConverter controller)
	{
		bool result = false;
		SymbolOverrideInfoSlot symbolOverrideInfoSlot = symbolOverrideInfoSlots[index];
		if (symbolOverrideInfoSlot.symbolOverrideInfo != controller.symbolOverrideInfoGpuData || symbolOverrideInfoSlot.dataVersion != controller.symbolOverrideInfoGpuData.version)
		{
			controller.symbolOverrideInfoGpuData.WriteToTexture(symbolOverrideInfoTex.bytes, index * 12 * group.data.maxSymbolFrameInstancesPerbuild * 4, index);
			symbolOverrideInfoSlot.symbolOverrideInfo = controller.symbolOverrideInfoGpuData;
			symbolOverrideInfoSlot.dataVersion = controller.symbolOverrideInfoGpuData.version;
			symbolOverrideInfoSlots[index] = symbolOverrideInfoSlot;
			result = true;
		}
		return result;
	}

	public int UpdateDirty(int frame)
	{
		if (needsWrite)
		{
			if (dataTex == null || dataTex.floats.Length == 0)
			{
				Init();
			}
			writtenLastFrame = 0;
			bool flag = false;
			bool flag2 = false;
			if (dirtySet.Count > 0)
			{
				foreach (int item in dirtySet)
				{
					KAnimConverter.IAnimConverter animConverter = controllers[item];
					if (animConverter != null && animConverter as Object != (Object)null)
					{
						WriteBatchedAnimInstanceData(item, animConverter);
						bool flag3 = WriteSymbolInstanceData(item, animConverter);
						flag = (flag || flag3);
						if (animConverter.ApplySymbolOverrides())
						{
							if (symbolOverrideInfoTex == null)
							{
								int bestTextureSize = KAnimBatchGroup.GetBestTextureSize((float)(group.data.maxSymbolFrameInstancesPerbuild * group.maxGroupSize * 12));
								symbolOverrideInfoTex = group.CreateTexture("SymbolOverrideInfoTex", bestTextureSize, ShaderProperty_symbolOverrideInfoTex, ShaderProperty_SYMBOL_OVERRIDE_INFO_TEXTURE_SIZE);
								symbolOverrideInfoTex.SetTextureAndSize(matProperties);
								matProperties.SetFloat(ShaderProperty_SUPPORTS_SYMBOL_OVERRIDING, 1f);
							}
							bool flag4 = WriteSymbolOverrideInfoTex(item, animConverter);
							flag2 = (flag2 || flag4);
						}
						writtenLastFrame++;
					}
				}
				if (writtenLastFrame != 0)
				{
					ClearDirty();
				}
				else
				{
					Debug.LogError("dirtySet not written", null);
				}
			}
			dataTex.LoadRawTextureData();
			dataTex.Apply();
			if (flag)
			{
				symbolInstanceTex.LoadRawTextureData();
				symbolInstanceTex.Apply();
			}
			if (flag2)
			{
				symbolOverrideInfoTex.LoadRawTextureData();
				symbolOverrideInfoTex.Apply();
			}
			return writtenLastFrame;
		}
		return 0;
	}
}
