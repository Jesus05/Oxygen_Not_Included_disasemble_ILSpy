using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace YamlDotNet.Helpers
{
	internal sealed class GenericDictionaryToNonGenericAdapter : IDictionary, ICollection, IEnumerable
	{
		private class DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
		{
			private readonly IEnumerator enumerator;

			private readonly MethodInfo getKeyMethod;

			private readonly MethodInfo getValueMethod;

			public DictionaryEntry Entry => new DictionaryEntry(Key, Value);

			public object Key => getKeyMethod.Invoke(enumerator.Current, null);

			public object Value => getValueMethod.Invoke(enumerator.Current, null);

			public object Current => Entry;

			public DictionaryEnumerator(object genericDictionary, Type genericDictionaryType)
			{
				Type[] genericArguments = genericDictionaryType.GetGenericArguments();
				Type type = typeof(KeyValuePair<, >).MakeGenericType(genericArguments);
				getKeyMethod = type.GetPublicProperty("Key").GetGetMethod();
				getValueMethod = type.GetPublicProperty("Value").GetGetMethod();
				enumerator = ((IEnumerable)genericDictionary).GetEnumerator();
			}

			public bool MoveNext()
			{
				return enumerator.MoveNext();
			}

			public void Reset()
			{
				enumerator.Reset();
			}
		}

		private readonly object genericDictionary;

		private readonly Type genericDictionaryType;

		private readonly MethodInfo indexerSetter;

		public bool IsFixedSize
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public ICollection Keys
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public ICollection Values
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public object this[object key]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				indexerSetter.Invoke(genericDictionary, new object[2]
				{
					key,
					value
				});
			}
		}

		public int Count
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public GenericDictionaryToNonGenericAdapter(object genericDictionary, Type genericDictionaryType)
		{
			this.genericDictionary = genericDictionary;
			this.genericDictionaryType = genericDictionaryType;
			indexerSetter = genericDictionaryType.GetPublicProperty("Item").GetSetMethod();
		}

		public void Add(object key, object value)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(object key)
		{
			throw new NotSupportedException();
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new DictionaryEnumerator(genericDictionary, genericDictionaryType);
		}

		public void Remove(object key)
		{
			throw new NotSupportedException();
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)genericDictionary).GetEnumerator();
		}
	}
}
