public class HandleValueArray<T> : HandleValueArrayBase
{
	public struct Entry
	{
		public T Value;

		public int Handle;
	}

	public int Count;

	public Entry[] Entries;

	private int[] Indices;

	public HandleValueArray(int reserve_size)
	{
		Entries = new Entry[reserve_size];
		Indices = new int[reserve_size];
		for (int i = 0; i < Entries.Length; i++)
		{
			Entries[i].Handle = i;
		}
	}

	public ValueArrayHandle Add(ref T value)
	{
		if (Count == Entries.Length)
		{
			Entry[] array = new Entry[Entries.Length * 2];
			int[] array2 = new int[Entries.Length * 2];
			for (int i = 0; i < Entries.Length; i++)
			{
				array[i] = Entries[i];
				array2[i] = Indices[i];
			}
			for (int j = Entries.Length; j < array.Length; j++)
			{
				array[j].Handle = j;
			}
			Entries = array;
			Indices = array2;
		}
		int handle = Entries[Count].Handle;
		int count = Count;
		Entries[count].Value = value;
		Indices[handle] = count;
		Count++;
		return new ValueArrayHandle(handle);
	}

	public int GetIndex(ref ValueArrayHandle handle)
	{
		return Indices[handle.handle];
	}

	public void Remove(ref ValueArrayHandle handle)
	{
		int num = Indices[handle.handle];
		Count--;
		int handle2 = Entries[Count].Handle;
		Entries[num] = Entries[Count];
		Entries[Count].Handle = handle.handle;
		Indices[handle2] = num;
	}
}
