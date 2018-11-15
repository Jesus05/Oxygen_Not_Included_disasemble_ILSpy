using System.Collections.Generic;

namespace Satsuma
{
	public interface IReadOnlyPriorityQueue<TElement, TPriority>
	{
		int Count
		{
			get;
		}

		IEnumerable<KeyValuePair<TElement, TPriority>> Items
		{
			get;
		}

		bool Contains(TElement element);

		bool TryGetPriority(TElement element, out TPriority priority);

		TElement Peek();

		TElement Peek(out TPriority priority);
	}
}
