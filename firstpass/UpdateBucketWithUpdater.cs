using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{name}")]
public class UpdateBucketWithUpdater<DataType> : StateMachineUpdater.BaseUpdateBucket
{
	public struct Entry
	{
		public DataType data;

		public float lastUpdateTime;

		public IUpdater updater;
	}

	public interface IUpdater
	{
		void Update(DataType smi, float dt);
	}

	public delegate void BatchUpdateDelegate(List<Entry> items, float time_delta);

	private KCompactedVector<Entry> entries = new KCompactedVector<Entry>(0);

	private List<HandleVector<int>.Handle> pendingRemovals = new List<HandleVector<int>.Handle>();

	public BatchUpdateDelegate batch_update_delegate;

	public override int count => entries.Count;

	public UpdateBucketWithUpdater(string name)
		: base(name)
	{
	}

	public HandleVector<int>.Handle Add(DataType data, float last_update_time, IUpdater updater)
	{
		Entry entry = default(Entry);
		entry.data = data;
		entry.lastUpdateTime = last_update_time;
		entry.updater = updater;
		HandleVector<int>.Handle handle = entries.Allocate(entry);
		entries.SetData(handle, entry);
		return handle;
	}

	public override void Remove(HandleVector<int>.Handle handle)
	{
		pendingRemovals.Add(handle);
		Entry data = entries.GetData(handle);
		data.updater = null;
		entries.SetData(handle, data);
	}

	public override void Update(float dt)
	{
		List<Entry> dataList = entries.GetDataList();
		foreach (HandleVector<int>.Handle pendingRemoval in pendingRemovals)
		{
			entries.Free(pendingRemoval);
		}
		pendingRemovals.Clear();
		if (batch_update_delegate != null)
		{
			batch_update_delegate(dataList, dt);
		}
		else
		{
			int count = dataList.Count;
			for (int i = 0; i < count; i++)
			{
				Entry value = dataList[i];
				if (value.updater != null)
				{
					value.updater.Update(value.data, dt - value.lastUpdateTime);
					value.lastUpdateTime = 0f;
					dataList[i] = value;
				}
			}
		}
	}
}
