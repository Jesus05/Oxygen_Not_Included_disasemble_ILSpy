using System;
using System.Collections;
using System.Reflection;

namespace YamlDotNet.Helpers
{
	internal sealed class GenericCollectionToNonGenericAdapter : IList, ICollection, IEnumerable
	{
		private readonly object genericCollection;

		private readonly MethodInfo addMethod;

		private readonly MethodInfo indexerSetter;

		private readonly MethodInfo countGetter;

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

		public object this[int index]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				indexerSetter.Invoke(genericCollection, new object[2]
				{
					index,
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

		public GenericCollectionToNonGenericAdapter(object genericCollection, Type genericCollectionType, Type genericListType)
		{
			this.genericCollection = genericCollection;
			addMethod = genericCollectionType.GetPublicInstanceMethod("Add");
			countGetter = genericCollectionType.GetPublicProperty("Count").GetGetMethod();
			if (genericListType != null)
			{
				indexerSetter = genericListType.GetPublicProperty("Item").GetSetMethod();
			}
		}

		public int Add(object value)
		{
			int result = (int)countGetter.Invoke(genericCollection, null);
			addMethod.Invoke(genericCollection, new object[1]
			{
				value
			});
			return result;
		}

		public void Clear()
		{
			throw new NotSupportedException();
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
			throw new NotSupportedException();
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable)genericCollection).GetEnumerator();
		}
	}
}
