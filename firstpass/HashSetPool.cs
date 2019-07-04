using System.Collections.Generic;
using System.Diagnostics;

public static class HashSetPool<ObjectType, PoolIdentifier>
{
	[DebuggerDisplay("Count={Count}")]
	public class PooledHashSet : HashSet<ObjectType>
	{
		public void Recycle()
		{
			HashSetPool<ObjectType, PoolIdentifier>.Free(this);
		}
	}

	private static ContainerPool<PooledHashSet, PoolIdentifier> pool = new ContainerPool<PooledHashSet, PoolIdentifier>();

	public static PooledHashSet Allocate()
	{
		return pool.Allocate();
	}

	private static void Free(PooledHashSet hash_set)
	{
		hash_set.Clear();
		pool.Free(hash_set);
	}

	public static ContainerPool GetPool()
	{
		return pool;
	}
}
