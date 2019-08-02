using System;
using System.Collections.Generic;
using System.Diagnostics;

public class HandleVector<T>
{
	[DebuggerDisplay("{index}")]
	public struct Handle : IComparable<Handle>, IEquatable<Handle>
	{
		private const int InvalidIndex = 0;

		private int _index;

		public static readonly Handle InvalidHandle = new Handle
		{
			_index = 0
		};

		public int index
		{
			get
			{
				return _index - 1;
			}
			set
			{
				_index = value + 1;
			}
		}

		public bool IsValid()
		{
			return _index != 0;
		}

		public void Clear()
		{
			_index = 0;
		}

		public int CompareTo(Handle obj)
		{
			return _index - obj._index;
		}

		public override bool Equals(object obj)
		{
			Handle handle = (Handle)obj;
			return _index == handle._index;
		}

		public bool Equals(Handle other)
		{
			return _index == other._index;
		}

		public override int GetHashCode()
		{
			return _index;
		}

		public static bool operator ==(Handle x, Handle y)
		{
			return x._index == y._index;
		}

		public static bool operator !=(Handle x, Handle y)
		{
			return x._index != y._index;
		}
	}

	public static readonly Handle InvalidHandle = Handle.InvalidHandle;

	protected Stack<Handle> freeHandles;

	protected List<T> items;

	protected List<byte> versions;

	public List<T> Items => items;

	public Stack<Handle> Handles => freeHandles;

	public HandleVector(int initial_size)
	{
		freeHandles = new Stack<Handle>(initial_size);
		items = new List<T>(initial_size);
		versions = new List<byte>(initial_size);
		Initialize(initial_size);
	}

	public virtual void Clear()
	{
		items.Clear();
		freeHandles.Clear();
		versions.Clear();
	}

	private void Initialize(int size)
	{
		for (int num = size - 1; num >= 0; num--)
		{
			freeHandles.Push(new Handle
			{
				index = num
			});
			items.Add(default(T));
			versions.Add(0);
		}
	}

	public virtual Handle Add(T item)
	{
		Handle handle;
		if (freeHandles.Count > 0)
		{
			handle = freeHandles.Pop();
			UnpackHandle(handle, out byte _, out int index);
			items[index] = item;
		}
		else
		{
			versions.Add(0);
			handle = PackHandle(items.Count);
			items.Add(item);
		}
		return handle;
	}

	public virtual T Release(Handle handle)
	{
		if (!handle.IsValid())
		{
			return default(T);
		}
		UnpackHandle(handle, out byte version, out int index);
		version = (byte)(version + 1);
		versions[index] = version;
		Debug.Assert(index >= 0);
		Debug.Assert(index < 16777216);
		handle = PackHandle(index);
		freeHandles.Push(handle);
		T result = items[index];
		items[index] = default(T);
		return result;
	}

	public T GetItem(Handle handle)
	{
		UnpackHandle(handle, out byte _, out int index);
		return items[index];
	}

	private Handle PackHandle(int index)
	{
		Debug.Assert(index < 16777216);
		byte b = versions[index];
		versions[index] = b;
		Handle invalidHandle = InvalidHandle;
		invalidHandle.index = ((b << 24) | index);
		return invalidHandle;
	}

	public void UnpackHandle(Handle handle, out byte version, out int index)
	{
		version = (byte)(handle.index >> 24);
		index = (handle.index & 0xFFFFFF);
		if (versions[index] != version)
		{
			throw new ArgumentException("Accessing mismatched handle version. Expected version=" + versions[index].ToString() + " but got version=" + version.ToString());
		}
	}

	public void UnpackHandleUnchecked(Handle handle, out byte version, out int index)
	{
		version = (byte)(handle.index >> 24);
		index = (handle.index & 0xFFFFFF);
	}

	public bool IsValid(Handle handle)
	{
		return (handle.index & 0xFFFFFF) != 16777215;
	}

	public bool IsVersionValid(Handle handle)
	{
		byte b = (byte)(handle.index >> 24);
		int index = handle.index & 0xFFFFFF;
		return b == versions[index];
	}
}
