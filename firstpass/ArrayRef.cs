using KSerialization;
using System;

[SerializationConfig(MemberSerialization.OptIn)]
public struct ArrayRef<T>
{
	[Serialize]
	private T[] elements;

	[Serialize]
	private int sizeImpl;

	[Serialize]
	private int capacityImpl;

	public T this[int i]
	{
		get
		{
			ValidateIndex(i);
			return elements[i];
		}
		set
		{
			ValidateIndex(i);
			elements[i] = value;
		}
	}

	public int size => sizeImpl;

	public int Count => size;

	public int capacity => capacityImpl;

	public ArrayRef(int initialCapacity)
	{
		capacityImpl = initialCapacity;
		elements = new T[initialCapacity];
		sizeImpl = 0;
	}

	public ArrayRef(T[] elements, int size)
	{
		this.elements = elements;
		sizeImpl = size;
		capacityImpl = elements.Length;
	}

	public int Add(T item)
	{
		MaybeGrow(size);
		elements[size] = item;
		sizeImpl++;
		return size;
	}

	public bool RemoveFirst(Predicate<T> match)
	{
		int num = FindIndex(match);
		if (num == -1)
		{
			return false;
		}
		RemoveAt(num);
		return true;
	}

	public bool RemoveFirstSwap(Predicate<T> match)
	{
		int num = FindIndex(match);
		if (num == -1)
		{
			return false;
		}
		RemoveAtSwap(num);
		return true;
	}

	public void RemoveAt(int index)
	{
		ValidateIndex(index);
		for (int i = index; i != size - 1; i++)
		{
			elements[i] = elements[i + 1];
		}
		sizeImpl--;
		DebugUtil.Assert(sizeImpl >= 0);
	}

	public void RemoveAtSwap(int index)
	{
		ValidateIndex(index);
		elements[index] = elements[size - 1];
		sizeImpl--;
		DebugUtil.Assert(sizeImpl >= 0);
	}

	public void RemoveAll(Predicate<T> match)
	{
		for (int num = size - 1; num != -1; num--)
		{
			if (match(elements[num]))
			{
				RemoveAt(num);
			}
		}
	}

	public void RemoveAllSwap(Predicate<T> match)
	{
		int num = 0;
		while (num != size)
		{
			if (match(elements[num]))
			{
				elements[num] = elements[size - 1];
				sizeImpl--;
				DebugUtil.Assert(sizeImpl >= 0);
			}
			else
			{
				num++;
			}
		}
	}

	public void Clear()
	{
		sizeImpl = 0;
	}

	public int FindIndex(Predicate<T> match)
	{
		for (int i = 0; i != size; i++)
		{
			if (match(elements[i]))
			{
				return i;
			}
		}
		return -1;
	}

	public void ShrinkToFit()
	{
		if (size != capacity)
		{
			Reallocate(size);
		}
	}

	private void ValidateIndex(int index)
	{
	}

	private void MaybeGrow(int index)
	{
		DebugUtil.Assert(capacity == 0 || capacity == elements.Length);
		DebugUtil.Assert(index >= 0);
		if (index >= capacity)
		{
			Reallocate((capacity == 0) ? 1 : (capacity * 2));
			DebugUtil.Assert(capacity == 0 || capacity == elements.Length);
		}
	}

	private void Reallocate(int newCapacity)
	{
		capacityImpl = newCapacity;
		T[] array = new T[capacity];
		for (int i = 0; i != size; i++)
		{
			array[i] = elements[i];
		}
		elements = array;
	}
}
