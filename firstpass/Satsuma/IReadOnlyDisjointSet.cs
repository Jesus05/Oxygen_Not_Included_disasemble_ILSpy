using System.Collections.Generic;

namespace Satsuma
{
	public interface IReadOnlyDisjointSet<T>
	{
		DisjointSetSet<T> WhereIs(T element);

		IEnumerable<T> Elements(DisjointSetSet<T> aSet);
	}
}
