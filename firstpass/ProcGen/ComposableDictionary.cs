using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class ComposableDictionary<Key, Value> : IMerge<ComposableDictionary<Key, Value>>
	{
		public Dictionary<Key, Value> add
		{
			get;
			private set;
		}

		public List<Key> remove
		{
			get;
			private set;
		}

		public Value this[Key key]
		{
			get
			{
				VerifyConsolidated();
				return add[key];
			}
			set
			{
				add[key] = value;
			}
		}

		public ICollection<Key> Keys
		{
			get
			{
				VerifyConsolidated();
				return add.Keys;
			}
		}

		public ICollection<Value> Values
		{
			get
			{
				VerifyConsolidated();
				return add.Values;
			}
		}

		public int Count
		{
			get
			{
				VerifyConsolidated();
				return add.Count;
			}
		}

		public bool IsReadOnly => false;

		public ComposableDictionary()
		{
			add = new Dictionary<Key, Value>();
			remove = new List<Key>();
		}

		private void VerifyConsolidated()
		{
			DebugUtil.Assert(remove.Count == 0, "needs to be Consolidate()d before being used");
		}

		public void Add(Key key, Value value)
		{
			add.Add(key, value);
		}

		public void Add(KeyValuePair<Key, Value> pair)
		{
			Add(pair.Key, pair.Value);
		}

		public bool Remove(Key key)
		{
			add.Remove(key);
			return true;
		}

		public void Clear()
		{
			add.Clear();
		}

		public bool ContainsKey(Key key)
		{
			VerifyConsolidated();
			return add.ContainsKey(key);
		}

		public bool TryGetValue(Key key, out Value value)
		{
			VerifyConsolidated();
			return add.TryGetValue(key, out value);
		}

		public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
		{
			VerifyConsolidated();
			return add.GetEnumerator();
		}

		public void Merge(ComposableDictionary<Key, Value> other)
		{
			VerifyConsolidated();
			foreach (Key item in other.remove)
			{
				add.Remove(item);
			}
			foreach (KeyValuePair<Key, Value> item2 in other.add)
			{
				if (add.ContainsKey(item2.Key))
				{
					DebugUtil.LogArgs("Overwriting entry {0}", item2.Key.ToString());
				}
				add.Add(item2.Key, item2.Value);
			}
		}
	}
}
