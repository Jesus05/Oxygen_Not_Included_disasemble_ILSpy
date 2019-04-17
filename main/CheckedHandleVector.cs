using System.Collections.Generic;

public class CheckedHandleVector<T> where T : new()
{
	private HandleVector<T> handleVector;

	private List<string> debugInfo = new List<string>();

	private List<bool> isFree;

	public CheckedHandleVector(int initial_size)
	{
		handleVector = new HandleVector<T>(initial_size);
		isFree = new List<bool>(initial_size);
		for (int i = 0; i < initial_size; i++)
		{
			isFree.Add(true);
		}
	}

	public HandleVector<T>.Handle Add(T item, string debug_info)
	{
		HandleVector<T>.Handle result = handleVector.Add(item);
		if (result.index >= isFree.Count)
		{
			isFree.Add(false);
		}
		else
		{
			isFree[result.index] = false;
		}
		int count = handleVector.Items.Count;
		while (count > debugInfo.Count)
		{
			debugInfo.Add(null);
		}
		debugInfo[result.index] = debug_info;
		return result;
	}

	public T Release(HandleVector<T>.Handle handle)
	{
		if (isFree[handle.index])
		{
			DebugUtil.LogErrorArgs("Tried to double free checked handle ", handle.index, "- Debug info:", debugInfo[handle.index]);
		}
		isFree[handle.index] = true;
		return handleVector.Release(handle);
	}

	public T Get(HandleVector<T>.Handle handle)
	{
		return handleVector.GetItem(handle);
	}
}
