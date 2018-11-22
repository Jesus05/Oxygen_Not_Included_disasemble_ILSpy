using System;
using System.Collections;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class ArrayNodeDeserializer : INodeDeserializer
	{
		private sealed class ArrayList : IList, ICollection, IEnumerable
		{
			private object[] data;

			private int count;

			public bool IsFixedSize => false;

			public bool IsReadOnly => false;

			public object this[int index]
			{
				get
				{
					return data[index];
				}
				set
				{
					data[index] = value;
				}
			}

			public int Count => count;

			public bool IsSynchronized => false;

			public object SyncRoot => data;

			public ArrayList()
			{
				Clear();
			}

			public int Add(object value)
			{
				if (count == data.Length)
				{
					Array.Resize(ref data, data.Length * 2);
				}
				data[count] = value;
				return count++;
			}

			public void Clear()
			{
				data = new object[10];
				count = 0;
			}

			public bool Contains(object value)
			{
				throw new NotSupportedException();
			}

			public int IndexOf(object value)
			{
				throw new NotSupportedException();
			}

			public void Insert(int index, object value)
			{
				throw new NotSupportedException();
			}

			public void Remove(object value)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public void CopyTo(Array array, int index)
			{
				Array.Copy(data, 0, array, index, count);
			}

			public IEnumerator GetEnumerator()
			{
				int i = 0;
				if (i < count)
				{
					yield return data[i];
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}

		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (expectedType.IsArray)
			{
				Type elementType = expectedType.GetElementType();
				ArrayList arrayList = new ArrayList();
				CollectionNodeDeserializer.DeserializeHelper(elementType, parser, nestedObjectDeserializer, arrayList, true);
				Array array = Array.CreateInstance(elementType, arrayList.Count);
				arrayList.CopyTo(array, 0);
				value = array;
				return true;
			}
			value = false;
			return false;
		}
	}
}
