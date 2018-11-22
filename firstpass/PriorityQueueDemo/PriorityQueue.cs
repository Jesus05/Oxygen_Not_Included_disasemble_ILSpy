using System;
using System.Collections;
using System.Collections.Generic;

namespace PriorityQueueDemo
{
	public class PriorityQueue<TPriority, TValue> : ICollection<KeyValuePair<TPriority, TValue>>, IEnumerable<KeyValuePair<TPriority, TValue>>, IEnumerable
	{
		private List<KeyValuePair<TPriority, TValue>> _baseHeap;

		private IComparer<TPriority> _comparer;

		public bool IsEmpty => _baseHeap.Count == 0;

		public int Count => _baseHeap.Count;

		public bool IsReadOnly => false;

		public PriorityQueue()
			: this((IComparer<TPriority>)Comparer<TPriority>.Default)
		{
		}

		public PriorityQueue(int capacity)
			: this(capacity, (IComparer<TPriority>)Comparer<TPriority>.Default)
		{
		}

		public PriorityQueue(int capacity, IComparer<TPriority> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException();
			}
			_baseHeap = new List<KeyValuePair<TPriority, TValue>>(capacity);
			_comparer = comparer;
		}

		public PriorityQueue(IComparer<TPriority> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException();
			}
			_baseHeap = new List<KeyValuePair<TPriority, TValue>>();
			_comparer = comparer;
		}

		public PriorityQueue(IEnumerable<KeyValuePair<TPriority, TValue>> data)
			: this(data, (IComparer<TPriority>)Comparer<TPriority>.Default)
		{
		}

		public PriorityQueue(IEnumerable<KeyValuePair<TPriority, TValue>> data, IComparer<TPriority> comparer)
		{
			if (data == null || comparer == null)
			{
				throw new ArgumentNullException();
			}
			_comparer = comparer;
			_baseHeap = new List<KeyValuePair<TPriority, TValue>>(data);
			for (int num = _baseHeap.Count / 2 - 1; num >= 0; num--)
			{
				HeapifyFromBeginningToEnd(num);
			}
		}

		public static PriorityQueue<TPriority, TValue> MergeQueues(PriorityQueue<TPriority, TValue> pq1, PriorityQueue<TPriority, TValue> pq2)
		{
			if (pq1 == null || pq2 == null)
			{
				throw new ArgumentNullException();
			}
			if (pq1._comparer != pq2._comparer)
			{
				throw new InvalidOperationException("Priority queues to be merged must have equal comparers");
			}
			return MergeQueues(pq1, pq2, pq1._comparer);
		}

		public static PriorityQueue<TPriority, TValue> MergeQueues(PriorityQueue<TPriority, TValue> pq1, PriorityQueue<TPriority, TValue> pq2, IComparer<TPriority> comparer)
		{
			if (pq1 == null || pq2 == null || comparer == null)
			{
				throw new ArgumentNullException();
			}
			PriorityQueue<TPriority, TValue> priorityQueue = new PriorityQueue<TPriority, TValue>(pq1.Count + pq2.Count, pq1._comparer);
			priorityQueue._baseHeap.AddRange(pq1._baseHeap);
			priorityQueue._baseHeap.AddRange(pq2._baseHeap);
			for (int num = priorityQueue._baseHeap.Count / 2 - 1; num >= 0; num--)
			{
				priorityQueue.HeapifyFromBeginningToEnd(num);
			}
			return priorityQueue;
		}

		public void Enqueue(TPriority priority, TValue value)
		{
			Insert(priority, value);
		}

		public KeyValuePair<TPriority, TValue> Dequeue()
		{
			if (IsEmpty)
			{
				throw new InvalidOperationException("Priority queue is empty");
			}
			KeyValuePair<TPriority, TValue> result = _baseHeap[0];
			DeleteRoot();
			return result;
		}

		public TValue DequeueValue()
		{
			return Dequeue().Value;
		}

		public KeyValuePair<TPriority, TValue> Peek()
		{
			if (IsEmpty)
			{
				throw new InvalidOperationException("Priority queue is empty");
			}
			return _baseHeap[0];
		}

		public TValue PeekValue()
		{
			return Peek().Value;
		}

		private void ExchangeElements(int pos1, int pos2)
		{
			KeyValuePair<TPriority, TValue> value = _baseHeap[pos1];
			_baseHeap[pos1] = _baseHeap[pos2];
			_baseHeap[pos2] = value;
		}

		private void Insert(TPriority priority, TValue value)
		{
			KeyValuePair<TPriority, TValue> item = new KeyValuePair<TPriority, TValue>(priority, value);
			_baseHeap.Add(item);
			HeapifyFromEndToBeginning(_baseHeap.Count - 1);
		}

		private int HeapifyFromEndToBeginning(int pos)
		{
			if (pos < _baseHeap.Count)
			{
				while (pos > 0)
				{
					int num = (pos - 1) / 2;
					if (_comparer.Compare(_baseHeap[num].Key, _baseHeap[pos].Key) <= 0)
					{
						break;
					}
					ExchangeElements(num, pos);
					pos = num;
				}
				return pos;
			}
			return -1;
		}

		private void DeleteRoot()
		{
			if (_baseHeap.Count <= 1)
			{
				_baseHeap.Clear();
			}
			else
			{
				_baseHeap[0] = _baseHeap[_baseHeap.Count - 1];
				_baseHeap.RemoveAt(_baseHeap.Count - 1);
				HeapifyFromBeginningToEnd(0);
			}
		}

		private void HeapifyFromBeginningToEnd(int pos)
		{
			if (pos < _baseHeap.Count)
			{
				while (true)
				{
					int num = pos;
					int num2 = 2 * pos + 1;
					int num3 = 2 * pos + 2;
					if (num2 < _baseHeap.Count && _comparer.Compare(_baseHeap[num].Key, _baseHeap[num2].Key) > 0)
					{
						num = num2;
					}
					if (num3 < _baseHeap.Count && _comparer.Compare(_baseHeap[num].Key, _baseHeap[num3].Key) > 0)
					{
						num = num3;
					}
					if (num == pos)
					{
						break;
					}
					ExchangeElements(num, pos);
					pos = num;
				}
			}
		}

		public void Add(KeyValuePair<TPriority, TValue> item)
		{
			Enqueue(item.Key, item.Value);
		}

		public void Clear()
		{
			_baseHeap.Clear();
		}

		public bool Contains(KeyValuePair<TPriority, TValue> item)
		{
			return _baseHeap.Contains(item);
		}

		public void CopyTo(KeyValuePair<TPriority, TValue>[] array, int arrayIndex)
		{
			_baseHeap.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TPriority, TValue> item)
		{
			int num = _baseHeap.IndexOf(item);
			if (num >= 0)
			{
				_baseHeap[num] = _baseHeap[_baseHeap.Count - 1];
				_baseHeap.RemoveAt(_baseHeap.Count - 1);
				int num2 = HeapifyFromEndToBeginning(num);
				if (num2 == num)
				{
					HeapifyFromBeginningToEnd(num);
				}
				return true;
			}
			return false;
		}

		public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
		{
			return _baseHeap.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
