using System.Collections.Generic;

public static class QueuePool<ObjectType, PoolIdentifier>
{
	public class PooledQueue : Queue<ObjectType>
	{
		public void Recycle()
		{
			QueuePool<ObjectType, PoolIdentifier>.Free(this);
		}
	}

	private static ContainerPool<PooledQueue, PoolIdentifier> pool = new ContainerPool<PooledQueue, PoolIdentifier>();

	public static PooledQueue Allocate()
	{
		return pool.Allocate();
	}

	private static void Free(PooledQueue queue)
	{
		queue.Clear();
		pool.Free(queue);
	}

	public static ContainerPool GetPool()
	{
		return pool;
	}
}
