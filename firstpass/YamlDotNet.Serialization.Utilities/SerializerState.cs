using System;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization.Utilities
{
	public sealed class SerializerState : IDisposable
	{
		private readonly IDictionary<Type, object> items = new Dictionary<Type, object>();

		public T Get<T>() where T : class, new()
		{
			if (!items.TryGetValue(typeof(T), out object value))
			{
				value = new T();
				items.Add(typeof(T), value);
			}
			return (T)value;
		}

		public void OnDeserialization()
		{
			foreach (IPostDeserializationCallback item in items.Values.OfType<IPostDeserializationCallback>())
			{
				item.OnDeserialization();
			}
		}

		public void Dispose()
		{
			foreach (IDisposable item in items.Values.OfType<IDisposable>())
			{
				item.Dispose();
			}
		}
	}
}
