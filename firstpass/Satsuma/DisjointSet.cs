using System.Collections.Generic;

namespace Satsuma
{
	public sealed class DisjointSet<T> : IDisjointSet<T>, IReadOnlyDisjointSet<T>, IClearable
	{
		private readonly Dictionary<T, T> parent;

		private readonly Dictionary<T, T> next;

		private readonly Dictionary<T, T> last;

		private readonly List<T> tmpList;

		public DisjointSet()
		{
			parent = new Dictionary<T, T>();
			next = new Dictionary<T, T>();
			last = new Dictionary<T, T>();
			tmpList = new List<T>();
		}

		public void Clear()
		{
			parent.Clear();
			next.Clear();
			last.Clear();
		}

		public DisjointSetSet<T> WhereIs(T element)
		{
			T value;
			while (parent.TryGetValue(element, out value))
			{
				tmpList.Add(element);
				element = value;
			}
			foreach (T tmp in tmpList)
			{
				parent[tmp] = element;
			}
			tmpList.Clear();
			return new DisjointSetSet<T>(element);
		}

		private T GetLast(T x)
		{
			if (!last.TryGetValue(x, out T value))
			{
				return x;
			}
			return value;
		}

		public DisjointSetSet<T> Union(DisjointSetSet<T> a, DisjointSetSet<T> b)
		{
			T representative = a.Representative;
			T representative2 = b.Representative;
			if (!representative.Equals(representative2))
			{
				parent[representative] = representative2;
				next[GetLast(representative2)] = representative;
				last[representative2] = GetLast(representative);
			}
			return b;
		}

		public IEnumerable<T> Elements(DisjointSetSet<T> aSet)
		{
			T element = aSet.Representative;
			yield return element;
			/*Error: Unable to find new state assignment for yield return*/;
		}
	}
}
