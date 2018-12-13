using System.Collections.Generic;

public abstract class KCompactedVectorBase
{
	protected List<int> dataHandleIndices = new List<int>();

	protected HandleVector<int> handles;

	protected KCompactedVectorBase(int initial_count)
	{
		handles = new HandleVector<int>(initial_count);
	}

	protected HandleVector<int>.Handle Allocate(int item)
	{
		HandleVector<int>.Handle handle = handles.Add(item);
		handles.UnpackHandle(handle, out byte _, out int index);
		dataHandleIndices.Add(index);
		return handle;
	}

	protected bool Free(HandleVector<int>.Handle handle, int last_idx, out int free_component_idx)
	{
		free_component_idx = -1;
		if (!handle.IsValid())
		{
			return false;
		}
		free_component_idx = handles.Release(handle);
		if (free_component_idx < last_idx)
		{
			int num = dataHandleIndices[last_idx];
			if (handles.Items[num] != last_idx)
			{
				Output.LogError("KCompactedVector: Bad state after attempting to free handle", handle.index);
			}
			handles.Items[num] = free_component_idx;
			dataHandleIndices[free_component_idx] = num;
		}
		dataHandleIndices.RemoveAt(last_idx);
		return true;
	}

	public bool IsValid(HandleVector<int>.Handle handle)
	{
		return handles.IsValid(handle);
	}

	public bool IsVersionValid(HandleVector<int>.Handle handle)
	{
		return handles.IsVersionValid(handle);
	}

	protected int ComputeIndex(HandleVector<int>.Handle handle)
	{
		handles.UnpackHandle(handle, out byte _, out int index);
		return handles.Items[index];
	}

	protected void Clear()
	{
		dataHandleIndices.Clear();
		handles.Clear();
	}
}
