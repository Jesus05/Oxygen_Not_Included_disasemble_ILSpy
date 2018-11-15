using System.Collections.Generic;

public class KPrefabIDTracker
{
	public struct Entry
	{
		public int id;

		public KPrefabID instance;
	}

	private static KPrefabIDTracker Instance;

	private Dictionary<KPrefabID, Entry> entryMap = new Dictionary<KPrefabID, Entry>();

	private Dictionary<int, KPrefabID> prefabIdMap = new Dictionary<int, KPrefabID>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public static KPrefabIDTracker Get()
	{
		if (Instance == null)
		{
			Instance = new KPrefabIDTracker();
		}
		return Instance;
	}

	public void Register(KPrefabID instance)
	{
		if (instance.InstanceID != -1)
		{
			Entry entry = default(Entry);
			entry.id = instance.InstanceID;
			entry.instance = instance;
			Entry value = entry;
			entryMap[instance] = value;
			prefabIdMap[instance.InstanceID] = instance;
		}
	}

	public void Unregister(KPrefabID instance)
	{
		entryMap.Remove(instance);
		prefabIdMap.Remove(instance.InstanceID);
	}

	public void Update(KPrefabID instance)
	{
		Entry value = default(Entry);
		if (entryMap.TryGetValue(instance, out value))
		{
			value.id = instance.InstanceID;
			value.instance = instance;
			entryMap[instance] = value;
			prefabIdMap[value.id] = instance;
		}
	}

	public KPrefabID GetInstance(int instance_id)
	{
		KPrefabID value = null;
		prefabIdMap.TryGetValue(instance_id, out value);
		return value;
	}
}
