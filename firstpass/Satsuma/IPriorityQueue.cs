namespace Satsuma
{
	public interface IPriorityQueue<TElement, TPriority> : IReadOnlyPriorityQueue<TElement, TPriority>, IClearable
	{
		TPriority this[TElement element]
		{
			get;
			set;
		}

		bool Remove(TElement element);

		bool Pop();
	}
}
