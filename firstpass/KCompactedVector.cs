using System;
using System.Collections;
using System.Collections.Generic;

public class KCompactedVector<T> : KCompactedVectorBase, ICollection, IEnumerable
{
	protected List<T> data;

	public int Count => data.Count;

	public bool IsSynchronized
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public object SyncRoot
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public KCompactedVector(int initial_count = 0)
		: base(initial_count)
	{
		data = new List<T>(initial_count);
	}

	public HandleVector<int>.Handle Allocate(T initial_data)
	{
		data.Add(initial_data);
		return Allocate(data.Count - 1);
	}

	public HandleVector<int>.Handle Free(HandleVector<int>.Handle handle)
	{
		int num = data.Count - 1;
		int free_component_idx;
		bool flag = Free(handle, num, out free_component_idx);
		if (flag)
		{
			if (free_component_idx < num)
			{
				data[free_component_idx] = data[num];
			}
			data.RemoveAt(num);
		}
		return (!flag) ? handle : HandleVector<int>.InvalidHandle;
	}

	public T GetData(HandleVector<int>.Handle handle)
	{
		return data[ComputeIndex(handle)];
	}

	public void SetData(HandleVector<int>.Handle handle, T new_data)
	{
		data[ComputeIndex(handle)] = new_data;
	}

	public new virtual void Clear()
	{
		base.Clear();
		data.Clear();
	}

	public List<T> GetDataList()
	{
		return data;
	}

	public void CopyTo(Array array, int index)
	{
		throw new NotImplementedException();
	}

	public IEnumerator GetEnumerator()
	{
		return data.GetEnumerator();
	}
}
