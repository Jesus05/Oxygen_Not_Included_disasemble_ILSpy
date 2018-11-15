using System;
using System.Collections;
using System.Collections.Generic;

public class KCompactedVector<T> : ICollection, IEnumerable
{
	protected List<T> data;

	protected List<int> dataHandleIndices = new List<int>();

	protected HandleVector<int> handles;

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
	{
		handles = new HandleVector<int>(initial_count);
		data = new List<T>(initial_count);
	}

	public HandleVector<int>.Handle Allocate(T initial_data)
	{
		HandleVector<int>.Handle handle = handles.Add(data.Count);
		handles.UnpackHandle(handle, out byte _, out int index);
		dataHandleIndices.Add(index);
		data.Add(initial_data);
		return handle;
	}

	public HandleVector<int>.Handle Free(HandleVector<int>.Handle handle)
	{
		if (!handle.IsValid())
		{
			return handle;
		}
		int num = handles.Release(handle);
		int num2 = data.Count - 1;
		if (num < num2)
		{
			data[num] = data[num2];
			int num3 = dataHandleIndices[num2];
			if (handles.Items[num3] != num2)
			{
				Output.LogError("KCompactedVector: Bad state after attempting to free handle", handle.index);
			}
			handles.Items[num3] = num;
			dataHandleIndices[num] = num3;
		}
		data.RemoveAt(num2);
		dataHandleIndices.RemoveAt(num2);
		return HandleVector<int>.InvalidHandle;
	}

	public bool IsValid(HandleVector<int>.Handle handle)
	{
		return handles.IsValid(handle);
	}

	public bool IsVersionValid(HandleVector<int>.Handle handle)
	{
		return handles.IsVersionValid(handle);
	}

	public T GetData(HandleVector<int>.Handle handle)
	{
		handles.UnpackHandle(handle, out byte _, out int index);
		int index2 = handles.Items[index];
		return data[index2];
	}

	public void SetData(HandleVector<int>.Handle handle, T new_data)
	{
		handles.UnpackHandle(handle, out byte _, out int index);
		int index2 = handles.Items[index];
		data[index2] = new_data;
	}

	public virtual void Clear()
	{
		dataHandleIndices.Clear();
		handles.Clear();
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
