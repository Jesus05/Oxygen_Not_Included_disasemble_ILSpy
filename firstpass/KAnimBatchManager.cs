using System.Collections.Generic;
using UnityEngine;

public class KAnimBatchManager
{
	private struct BatchSetInfo
	{
		public BatchSet batchSet;

		public Vector2I spatialIdx;

		public bool isActive;
	}

	private const int DEFAULT_BATCH_SIZE = 30;

	public const int CHUNK_SIZE = 32;

	public static HashedString NO_BATCH = new HashedString("NO_BATCH");

	public static HashedString IGNORE = new HashedString("IGNORE");

	public static Vector2 GROUP_SIZE = new Vector2(32f, 32f);

	private bool ready = false;

	private Dictionary<HashedString, KBatchGroupData> batchGroupData = new Dictionary<HashedString, KBatchGroupData>();

	private Dictionary<BatchGroupKey, KAnimBatchGroup> batchGroups = new Dictionary<BatchGroupKey, KAnimBatchGroup>();

	private Dictionary<BatchKey, BatchSet> batchSets = new Dictionary<BatchKey, BatchSet>();

	private List<BatchSetInfo> culledBatchSetInfos = new List<BatchSetInfo>();

	private List<BatchSetInfo> uiBatchSets = new List<BatchSetInfo>();

	private List<BatchSet> activeBatchSets = new List<BatchSet>();

	public int[] atlasNames = new int[12]
	{
		Shader.PropertyToID("atlas0"),
		Shader.PropertyToID("atlas1"),
		Shader.PropertyToID("atlas2"),
		Shader.PropertyToID("atlas3"),
		Shader.PropertyToID("atlas4"),
		Shader.PropertyToID("atlas5"),
		Shader.PropertyToID("atlas6"),
		Shader.PropertyToID("atlas7"),
		Shader.PropertyToID("atlas8"),
		Shader.PropertyToID("atlas9"),
		Shader.PropertyToID("atlas10"),
		Shader.PropertyToID("atlas11")
	};

	public int dirtyBatchLastFrame
	{
		get;
		private set;
	}

	public static KAnimBatchManager instance => Singleton<KAnimBatchManager>.Instance;

	public bool isReady => ready;

	public static void CreateInstance()
	{
		Singleton<KAnimBatchManager>.CreateInstance();
	}

	public static KAnimBatchManager Instance()
	{
		return instance;
	}

	public static void DestroyInstance()
	{
		if (instance != null)
		{
			instance.ready = false;
			foreach (KeyValuePair<BatchGroupKey, KAnimBatchGroup> batchGroup in instance.batchGroups)
			{
				batchGroup.Value.FreeResources();
			}
			instance.batchGroups.Clear();
			foreach (KeyValuePair<HashedString, KBatchGroupData> batchGroupDatum in instance.batchGroupData)
			{
				if (batchGroupDatum.Value != null)
				{
					batchGroupDatum.Value.FreeResources();
				}
			}
			instance.batchGroupData.Clear();
			foreach (KeyValuePair<BatchKey, BatchSet> batchSet in instance.batchSets)
			{
				if (batchSet.Value != null)
				{
					batchSet.Value.Clear();
				}
			}
			instance.batchSets.Clear();
			instance.culledBatchSetInfos.Clear();
			instance.uiBatchSets.Clear();
			instance.activeBatchSets.Clear();
			instance.dirtyBatchLastFrame = 0;
			KAnimBatchGroup.FinalizeTextureCache();
		}
		Singleton<KAnimBatchManager>.DestroyInstance();
	}

	public KBatchGroupData GetBatchGroupData(HashedString groupID)
	{
		if (groupID.IsValid && !(groupID == NO_BATCH) && !(groupID == IGNORE))
		{
			KBatchGroupData value = null;
			if (!batchGroupData.TryGetValue(groupID, out value))
			{
				value = new KBatchGroupData(groupID);
				batchGroupData[groupID] = value;
			}
			return value;
		}
		return null;
	}

	public KAnimBatchGroup GetBatchGroup(BatchGroupKey group_key)
	{
		KAnimBatchGroup value = null;
		if (!batchGroups.TryGetValue(group_key, out value))
		{
			value = new KAnimBatchGroup(group_key.groupID);
			batchGroups.Add(group_key, value);
		}
		return value;
	}

	public static Vector2I CellXYToChunkXY(Vector2I cell_xy)
	{
		return new Vector2I(cell_xy.x / 32, cell_xy.y / 32);
	}

	public static Vector2I ControllerToChunkXY(KAnimConverter.IAnimConverter controller)
	{
		Vector2I cellXY = controller.GetCellXY();
		return CellXYToChunkXY(cellXY);
	}

	public void Register(KAnimConverter.IAnimConverter controller)
	{
		if (!isReady)
		{
			Debug.LogError($"Batcher isnt finished setting up, controller [{controller.GetName()}] is registering too early.", null);
		}
		BatchKey batchKey = BatchKey.Create(controller);
		Vector2I vector2I = ControllerToChunkXY(controller);
		if (!batchSets.TryGetValue(batchKey, out BatchSet value))
		{
			value = new BatchSet(GetBatchGroup(new BatchGroupKey(batchKey.groupID)), batchKey, vector2I);
			batchSets[batchKey] = value;
			if (value.key.materialType == KAnimBatchGroup.MaterialType.UI)
			{
				uiBatchSets.Add(new BatchSetInfo
				{
					batchSet = value,
					isActive = false,
					spatialIdx = vector2I
				});
			}
			else
			{
				culledBatchSetInfos.Add(new BatchSetInfo
				{
					batchSet = value,
					isActive = false,
					spatialIdx = vector2I
				});
			}
		}
		value.Add(controller);
	}

	public void UpdateActiveArea(Vector2I vis_chunk_min, Vector2I vis_chunk_max)
	{
		activeBatchSets.Clear();
		for (int i = 0; i < uiBatchSets.Count; i++)
		{
			BatchSetInfo value = uiBatchSets[i];
			activeBatchSets.Add(value.batchSet);
			if (!value.isActive)
			{
				value.isActive = true;
				value.batchSet.SetActive(true);
				uiBatchSets[i] = value;
			}
		}
		for (int j = 0; j < culledBatchSetInfos.Count; j++)
		{
			BatchSetInfo value2 = culledBatchSetInfos[j];
			if (value2.spatialIdx.x >= vis_chunk_min.x && value2.spatialIdx.x <= vis_chunk_max.x && value2.spatialIdx.y >= vis_chunk_min.y && value2.spatialIdx.y <= vis_chunk_max.y)
			{
				activeBatchSets.Add(value2.batchSet);
				if (!value2.isActive)
				{
					value2.isActive = true;
					culledBatchSetInfos[j] = value2;
					value2.batchSet.SetActive(true);
				}
			}
			else if (value2.isActive)
			{
				value2.isActive = false;
				culledBatchSetInfos[j] = value2;
				value2.batchSet.SetActive(false);
			}
		}
	}

	public int UpdateDirty(int frame)
	{
		if (ready)
		{
			dirtyBatchLastFrame = 0;
			foreach (BatchSet activeBatchSet in activeBatchSets)
			{
				dirtyBatchLastFrame += activeBatchSet.UpdateDirty(frame);
			}
			return dirtyBatchLastFrame;
		}
		return 0;
	}

	public void Render()
	{
		if (ready)
		{
			foreach (BatchSet activeBatchSet in activeBatchSets)
			{
				DebugUtil.Assert(activeBatchSet != null);
				DebugUtil.Assert(activeBatchSet.group != null);
				Mesh mesh = activeBatchSet.group.mesh;
				for (int i = 0; i < activeBatchSet.batchCount; i++)
				{
					KAnimBatch batch = activeBatchSet.GetBatch(i);
					float num = 0.01f / (float)(1 + batch.id % 256);
					if (batch.size != 0 && batch.active && batch.materialType != KAnimBatchGroup.MaterialType.UI)
					{
						Vector3 zero = Vector3.zero;
						Vector3 position = batch.position;
						zero.z = position.z + num;
						int layer = batch.layer;
						Graphics.DrawMesh(mesh, zero, Quaternion.identity, activeBatchSet.group.GetMaterial(batch.materialType), layer, null, 0, batch.matProperties);
					}
				}
			}
		}
	}

	public void CompleteInit()
	{
		ready = true;
	}
}
