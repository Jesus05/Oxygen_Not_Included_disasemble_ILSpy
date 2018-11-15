using System;

namespace MIConvexHull
{
	internal class SimpleList<T>
	{
		private int capacity;

		public int Count;

		private T[] items;

		public T this[int i]
		{
			get
			{
				return items[i];
			}
			set
			{
				items[i] = value;
			}
		}

		private void EnsureCapacity()
		{
			if (capacity == 0)
			{
				capacity = 32;
				items = new T[32];
			}
			else
			{
				T[] destinationArray = new T[capacity * 2];
				Array.Copy(items, destinationArray, capacity);
				capacity = 2 * capacity;
				items = destinationArray;
			}
		}

		public void Add(T item)
		{
			if (Count + 1 > capacity)
			{
				EnsureCapacity();
			}
			items[Count++] = item;
		}

		public void Push(T item)
		{
			if (Count + 1 > capacity)
			{
				EnsureCapacity();
			}
			items[Count++] = item;
		}

		public T Pop()
		{
			return items[--Count];
		}

		public void Clear()
		{
			Count = 0;
		}
	}
}
