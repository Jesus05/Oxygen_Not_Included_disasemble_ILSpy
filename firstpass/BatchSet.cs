using System.Collections.Generic;
using UnityEngine;

public class BatchSet
{
	public KAnimBatchGroup group
	{
		get;
		private set;
	}

	protected List<KAnimBatch> batches
	{
		get;
		private set;
	}

	public Vector2I idx
	{
		get;
		private set;
	}

	public BatchKey key
	{
		get;
		private set;
	}

	public bool dirty
	{
		get;
		private set;
	}

	public bool active
	{
		get;
		private set;
	}

	public int batchCount => batches.Count;

	public int dirtyBatchLastFrame
	{
		get;
		private set;
	}

	public int lastDirtyFrame
	{
		get;
		private set;
	}

	public BatchSet(KAnimBatchGroup batchGroup, BatchKey batchKey, Vector2I spacialIdx)
	{
		idx = spacialIdx;
		key = batchKey;
		dirty = true;
		group = batchGroup;
		batches = new List<KAnimBatch>();
	}

	public void Clear()
	{
		group = null;
		for (int i = 0; i < batches.Count; i++)
		{
			if (batches[i] != null)
			{
				batches[i].Clear();
			}
		}
		batches.Clear();
	}

	public KAnimBatch GetBatch(int idx)
	{
		return batches[idx];
	}

	public void Add(KAnimConverter.IAnimConverter controller)
	{
		int layer = controller.GetLayer();
		if (layer != key.layer)
		{
			Debug.LogError("Registering with wrong batch set (layer) " + controller.GetName(), null);
		}
		HashedString batchGroupID = controller.GetBatchGroupID(false);
		if (!(batchGroupID == key.groupID))
		{
			Debug.LogError("Registering with wrong batch set (groupID) " + controller.GetName(), null);
		}
		KAnimBatchGroup.MaterialType materialType = controller.GetMaterialType();
		for (int i = 0; i < batches.Count; i++)
		{
			if (batches[i].size < group.maxGroupSize && batches[i].materialType == materialType)
			{
				if (batches[i].Register(controller))
				{
					SetDirty();
				}
				return;
			}
		}
		KAnimBatch kAnimBatch = new KAnimBatch(group, layer, controller.GetZ(), materialType);
		kAnimBatch.Init();
		AddBatch(kAnimBatch);
		kAnimBatch.Register(controller);
	}

	public void RemoveBatch(KAnimBatch batch)
	{
		if (batches.Contains(batch))
		{
			group.batchCount--;
			batches.Remove(batch);
			batch.SetBatchSet(null);
		}
	}

	public void AddBatch(KAnimBatch batch)
	{
		if (batch.batchset != this)
		{
			if (batch.batchset != null)
			{
				batch.batchset.RemoveBatch(batch);
			}
			batch.SetBatchSet(this);
			if (!batches.Contains(batch))
			{
				group.batchCount++;
				batches.Add(batch);
				batches.Sort(delegate(KAnimBatch b0, KAnimBatch b1)
				{
					Vector3 position = b0.position;
					ref float z = ref position.z;
					Vector3 position2 = b1.position;
					return z.CompareTo(position2.z);
				});
			}
		}
		SetDirty();
	}

	public void SetDirty()
	{
		dirty = true;
	}

	public void SetActive(bool isActive)
	{
		if (isActive != active)
		{
			if (!isActive)
			{
				for (int i = 0; i < batches.Count; i++)
				{
					if (batches[i] != null)
					{
						batches[i].Deactivate();
					}
				}
			}
			else
			{
				for (int j = 0; j < batches.Count; j++)
				{
					if (batches[j] != null)
					{
						batches[j].Activate();
					}
				}
				SetDirty();
			}
		}
		active = isActive;
	}

	public int UpdateDirty(int frame)
	{
		dirtyBatchLastFrame = 0;
		if (dirty)
		{
			for (int i = 0; i < batches.Count; i++)
			{
				dirtyBatchLastFrame += batches[i].UpdateDirty(frame);
			}
			lastDirtyFrame = frame;
			dirty = false;
		}
		return dirtyBatchLastFrame;
	}
}
