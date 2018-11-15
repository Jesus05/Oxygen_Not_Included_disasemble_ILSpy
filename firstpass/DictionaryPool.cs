using System.Collections.Generic;

public static class DictionaryPool<KeyType, ObjectType, PoolIdentifier>
{
	public class PooledDictionary : Dictionary<KeyType, ObjectType>
	{
		public void Recycle()
		{
			DictionaryPool<KeyType, ObjectType, PoolIdentifier>.Free(this);
		}
	}

	private static ContainerPool<PooledDictionary, PoolIdentifier> pool = new ContainerPool<PooledDictionary, PoolIdentifier>();

	public static PooledDictionary Allocate()
	{
		return pool.Allocate();
	}

	private static void Free(PooledDictionary dictionary)
	{
		dictionary.Clear();
		pool.Free(dictionary);
	}

	public static ContainerPool GetPool()
	{
		return pool;
	}
}
