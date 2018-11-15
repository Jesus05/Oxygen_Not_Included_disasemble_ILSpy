public class ValueArray<T>
{
	public int Count;

	public T[] Values;

	public T this[int idx]
	{
		get
		{
			return Values[idx];
		}
	}

	public ValueArray(int reserve_size)
	{
		Values = new T[reserve_size];
	}

	public void Add(ref T value)
	{
		if (Count == Values.Length)
		{
			Resize(Values.Length * 2);
		}
		Values[Count] = value;
		Count++;
	}

	public void Resize(int new_size)
	{
		T[] array = new T[new_size];
		for (int i = 0; i < Values.Length; i++)
		{
			array[i] = Values[i];
		}
		Values = array;
	}

	public void Remove(int index)
	{
		if (Count > 0)
		{
			Values[index] = Values[Count - 1];
		}
		Count--;
	}

	public void Clear()
	{
		Count = 0;
	}

	public bool IsEqual(ValueArray<T> array)
	{
		if (Count != array.Count)
		{
			return false;
		}
		for (int i = 0; i < Count; i++)
		{
			if (!Values[i].Equals(array.Values[i]))
			{
				return false;
			}
		}
		return true;
	}

	public void CopyFrom(ValueArray<T> array)
	{
		Clear();
		for (int i = 0; i < array.Count; i++)
		{
			T value = array[i];
			Add(ref value);
		}
	}
}
