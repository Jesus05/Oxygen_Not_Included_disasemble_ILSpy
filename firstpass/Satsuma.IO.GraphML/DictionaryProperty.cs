using System.Collections.Generic;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public abstract class DictionaryProperty<T> : GraphMLProperty, IClearable
	{
		public bool HasDefaultValue
		{
			get;
			set;
		}

		public T DefaultValue
		{
			get;
			set;
		}

		public Dictionary<object, T> Values
		{
			get;
			private set;
		}

		protected DictionaryProperty()
		{
			HasDefaultValue = false;
			Values = new Dictionary<object, T>();
		}

		public void Clear()
		{
			HasDefaultValue = false;
			Values.Clear();
		}

		public bool TryGetValue(object key, out T result)
		{
			if (!Values.TryGetValue(key, out result))
			{
				if (!HasDefaultValue)
				{
					result = default(T);
					return false;
				}
				result = DefaultValue;
				return true;
			}
			return true;
		}

		public override void ReadData(XElement x, object key)
		{
			if (x == null)
			{
				if (key == null)
				{
					HasDefaultValue = false;
				}
				else
				{
					Values.Remove(key);
				}
			}
			else
			{
				T val = ReadValue(x);
				if (key == null)
				{
					HasDefaultValue = true;
					DefaultValue = val;
				}
				else
				{
					Values[key] = val;
				}
			}
		}

		public override XElement WriteData(object key)
		{
			if (key != null)
			{
				if (Values.TryGetValue(key, out T value))
				{
					return WriteValue(value);
				}
				return null;
			}
			return (!HasDefaultValue) ? null : WriteValue(DefaultValue);
		}

		protected abstract T ReadValue(XElement x);

		protected abstract XElement WriteValue(T value);
	}
}
