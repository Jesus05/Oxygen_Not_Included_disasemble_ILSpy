using System.Collections.Generic;

namespace Klei.AI
{
	public class ModifierGroup<T> : Resource
	{
		public List<T> modifiers = new List<T>();

		public T this[int idx]
		{
			get
			{
				return modifiers[idx];
			}
		}

		public int Count => modifiers.Count;

		public ModifierGroup(string id, string name)
			: base(id, name)
		{
		}

		public IEnumerator<T> GetEnumerator()
		{
			return modifiers.GetEnumerator();
		}

		public void Add(T modifier)
		{
			modifiers.Add(modifier);
		}
	}
}
