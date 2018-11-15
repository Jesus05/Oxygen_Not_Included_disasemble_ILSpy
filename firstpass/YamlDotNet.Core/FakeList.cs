using System;
using System.Collections.Generic;

namespace YamlDotNet.Core
{
	public class FakeList<T>
	{
		private readonly IEnumerator<T> collection;

		private int currentIndex = -1;

		public T this[int index]
		{
			get
			{
				if (index < currentIndex)
				{
					collection.Reset();
					currentIndex = -1;
				}
				while (currentIndex < index)
				{
					if (!collection.MoveNext())
					{
						throw new ArgumentOutOfRangeException("index");
					}
					currentIndex++;
				}
				return collection.Current;
			}
		}

		public FakeList(IEnumerator<T> collection)
		{
			this.collection = collection;
		}

		public FakeList(IEnumerable<T> collection)
			: this(collection.GetEnumerator())
		{
		}
	}
}
