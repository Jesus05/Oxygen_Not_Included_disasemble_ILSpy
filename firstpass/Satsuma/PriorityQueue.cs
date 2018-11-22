using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class PriorityQueue<TElement, TPriority> : IPriorityQueue<TElement, TPriority>, IReadOnlyPriorityQueue<TElement, TPriority>, IClearable where TPriority : IComparable<TPriority>
	{
		private List<TElement> payloads = new List<TElement>();

		private List<TPriority> priorities = new List<TPriority>();

		private Dictionary<TElement, int> positions = new Dictionary<TElement, int>();

		public int Count => payloads.Count;

		public IEnumerable<KeyValuePair<TElement, TPriority>> Items
		{
			get
			{
				int j = 0;
				int i = this.Count;
				if (j < i)
				{
					yield return new KeyValuePair<TElement, TPriority>(this.payloads[j], this.priorities[j]);
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}

		public TPriority this[TElement element]
		{
			get
			{
				return priorities[positions[element]];
			}
			set
			{
				if (positions.TryGetValue(element, out int value2))
				{
					TPriority other = priorities[value2];
					priorities[value2] = value;
					int num = value.CompareTo(other);
					if (num < 0)
					{
						MoveUp(value2);
					}
					else if (num > 0)
					{
						MoveDown(value2);
					}
				}
				else
				{
					payloads.Add(element);
					priorities.Add(value);
					value2 = Count - 1;
					positions[element] = value2;
					MoveUp(value2);
				}
			}
		}

		public void Clear()
		{
			payloads.Clear();
			priorities.Clear();
			positions.Clear();
		}

		public bool Contains(TElement element)
		{
			return positions.ContainsKey(element);
		}

		public bool TryGetPriority(TElement element, out TPriority priority)
		{
			if (positions.TryGetValue(element, out int value))
			{
				priority = priorities[value];
				return true;
			}
			priority = default(TPriority);
			return false;
		}

		private void RemoveAt(int pos)
		{
			int count = Count;
			TElement key = payloads[pos];
			TPriority other = priorities[pos];
			positions.Remove(key);
			bool flag = count <= 1;
			if (!flag && pos != count - 1)
			{
				payloads[pos] = payloads[count - 1];
				priorities[pos] = priorities[count - 1];
				positions[payloads[pos]] = pos;
			}
			payloads.RemoveAt(count - 1);
			priorities.RemoveAt(count - 1);
			if (!flag && pos != count - 1)
			{
				int num = priorities[pos].CompareTo(other);
				if (num > 0)
				{
					MoveDown(pos);
				}
				else if (num < 0)
				{
					MoveUp(pos);
				}
			}
		}

		public bool Remove(TElement element)
		{
			int value;
			bool flag = positions.TryGetValue(element, out value);
			if (flag)
			{
				RemoveAt(value);
			}
			return flag;
		}

		public TElement Peek()
		{
			return payloads[0];
		}

		public TElement Peek(out TPriority priority)
		{
			priority = priorities[0];
			return payloads[0];
		}

		public bool Pop()
		{
			if (Count != 0)
			{
				RemoveAt(0);
				return true;
			}
			return false;
		}

		private void MoveUp(int index)
		{
			TElement val = payloads[index];
			TPriority value = priorities[index];
			int num;
			int num2;
			for (num = index; num > 0; num = num2)
			{
				num2 = num / 2;
				if (value.CompareTo(priorities[num2]) >= 0)
				{
					break;
				}
				payloads[num] = payloads[num2];
				priorities[num] = priorities[num2];
				positions[payloads[num]] = num;
			}
			if (num != index)
			{
				payloads[num] = val;
				priorities[num] = value;
				positions[val] = num;
			}
		}

		private void MoveDown(int index)
		{
			TElement val = payloads[index];
			TPriority value = priorities[index];
			int num = index;
			while (2 * num < Count)
			{
				int num2 = num;
				int num3 = 2 * num;
				if (value.CompareTo(priorities[num3]) >= 0)
				{
					num2 = num3;
				}
				num3++;
				if (num3 < Count && priorities[num2].CompareTo(priorities[num3]) >= 0)
				{
					num2 = num3;
				}
				if (num2 == num)
				{
					break;
				}
				payloads[num] = payloads[num2];
				priorities[num] = priorities[num2];
				positions[payloads[num]] = num;
				num = num2;
			}
			if (num != index)
			{
				payloads[num] = val;
				priorities[num] = value;
				positions[val] = num;
			}
		}
	}
}
