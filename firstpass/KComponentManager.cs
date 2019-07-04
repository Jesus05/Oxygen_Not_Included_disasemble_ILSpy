#define UNITY_ASSERTIONS
using System.Collections.Generic;
using UnityEngine.Assertions;

public abstract class KComponentManager<T> : KCompactedVector<T>, IComponentManager where T : new()
{
	protected struct CleanupInfo
	{
		public object instance;

		public HandleVector<int>.Handle handle;

		public CleanupInfo(object instance, HandleVector<int>.Handle handle)
		{
			this.instance = instance;
			this.handle = handle;
		}
	}

	protected Dictionary<object, HandleVector<int>.Handle> instanceHandleMap = new Dictionary<object, HandleVector<int>.Handle>();

	private HashSet<HandleVector<int>.Handle> spawnList = new HashSet<HandleVector<int>.Handle>();

	private HashSet<HandleVector<int>.Handle> shadowSpawnList = new HashSet<HandleVector<int>.Handle>();

	protected List<CleanupInfo> cleanupList = new List<CleanupInfo>();

	private List<CleanupInfo> shadowCleanupList = new List<CleanupInfo>();

	public string Name
	{
		get;
		set;
	}

	public KComponentManager()
		: base(0)
	{
		Name = GetType().Name;
	}

	public bool Has(object go)
	{
		if (!cleanupList.Exists((CleanupInfo x) => x.instance == go))
		{
			HandleVector<int>.Handle handle = GetHandle(go);
			if (!(handle == HandleVector<int>.InvalidHandle))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	protected HandleVector<int>.Handle InternalAddComponent(object instance, T cmp_values)
	{
		HandleVector<int>.Handle value = HandleVector<int>.InvalidHandle;
		RemoveFromCleanupList(instance);
		if (!instanceHandleMap.TryGetValue(instance, out value))
		{
			value = Allocate(cmp_values);
			instanceHandleMap[instance] = value;
		}
		else
		{
			SetData(value, cmp_values);
		}
		spawnList.Remove(value);
		OnPrefabInit(value);
		spawnList.Add(value);
		Assert.IsTrue(value.IsValid());
		return value;
	}

	protected void InternalRemoveComponent(CleanupInfo info)
	{
		if (info.instance != null)
		{
			if (!instanceHandleMap.ContainsKey(info.instance))
			{
				DebugUtil.LogErrorArgs("Tried to remove component of type", typeof(T).ToString(), "on instance", info.instance.ToString(), "but instance has not been registered yet. Handle:", info.handle);
				return;
			}
			instanceHandleMap.Remove(info.instance);
		}
		else
		{
			foreach (KeyValuePair<object, HandleVector<int>.Handle> item in instanceHandleMap)
			{
				if (item.Value == info.handle)
				{
					instanceHandleMap.Remove(item.Key);
				}
			}
		}
		Free(info.handle);
		spawnList.Remove(info.handle);
	}

	public HandleVector<int>.Handle GetHandle(object instance)
	{
		if (instanceHandleMap.TryGetValue(instance, out HandleVector<int>.Handle value))
		{
			return value;
		}
		value = HandleVector<int>.InvalidHandle;
		return value;
	}

	public void Spawn()
	{
		HashSet<HandleVector<int>.Handle> hashSet = spawnList;
		spawnList = shadowSpawnList;
		shadowSpawnList = hashSet;
		spawnList.Clear();
		foreach (CleanupInfo cleanup in cleanupList)
		{
			HandleVector<int>.Handle handle = GetHandle(cleanup);
			shadowSpawnList.Remove(handle);
		}
		foreach (HandleVector<int>.Handle shadowSpawn in shadowSpawnList)
		{
			OnSpawn(shadowSpawn);
		}
		shadowSpawnList.Clear();
	}

	public virtual void RenderEveryTick(float dt)
	{
	}

	public virtual void FixedUpdate(float dt)
	{
	}

	public virtual void Sim200ms(float dt)
	{
	}

	public void CleanUp()
	{
		shadowCleanupList.AddRange(cleanupList);
		cleanupList.Clear();
		foreach (CleanupInfo shadowCleanup in shadowCleanupList)
		{
			CleanupInfo current = shadowCleanup;
			OnCleanUp(current.handle);
			InternalRemoveComponent(current);
		}
		shadowCleanupList.Clear();
	}

	protected void RemoveFromCleanupList(object instance)
	{
		int num = 0;
		while (true)
		{
			if (num >= cleanupList.Count)
			{
				return;
			}
			CleanupInfo cleanupInfo = cleanupList[num];
			if (cleanupInfo.instance == instance)
			{
				break;
			}
			num++;
		}
		cleanupList[num] = cleanupList[cleanupList.Count - 1];
		cleanupList.RemoveAt(cleanupList.Count - 1);
	}

	public override void Clear()
	{
		base.Clear();
		spawnList.Clear();
		shadowSpawnList.Clear();
		cleanupList.Clear();
		shadowCleanupList.Clear();
		instanceHandleMap.Clear();
	}

	protected virtual void OnPrefabInit(HandleVector<int>.Handle h)
	{
	}

	protected virtual void OnSpawn(HandleVector<int>.Handle h)
	{
	}

	protected virtual void OnCleanUp(HandleVector<int>.Handle h)
	{
	}

	int get_Count()
	{
		return base.Count;
	}

	int IComponentManager.get_Count()
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_Count
		return this.get_Count();
	}
}
