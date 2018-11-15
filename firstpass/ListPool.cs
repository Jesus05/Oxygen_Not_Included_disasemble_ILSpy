using System.Collections.Generic;

public static class ListPool<ObjectType, PoolIdentifier>
{
	public class PooledList : List<ObjectType>
	{
		public void Recycle()
		{
			ListPool<ObjectType, PoolIdentifier>.Free(this);
		}
	}

	private static ContainerPool<PooledList, PoolIdentifier> pool = new ContainerPool<PooledList, PoolIdentifier>();

	public static PooledList Allocate(List<ObjectType> objects)
	{
		PooledList pooledList = pool.Allocate();
		pooledList.AddRange(objects);
		return pooledList;
	}

	public static PooledList Allocate()
	{
		return pool.Allocate();
	}

	private static void Free(PooledList list)
	{
		list.Clear();
		pool.Free(list);
	}

	public static ContainerPool GetPool()
	{
		return pool;
	}
}
