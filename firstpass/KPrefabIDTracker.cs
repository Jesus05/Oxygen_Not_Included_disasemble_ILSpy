using System.Collections.Generic;

public class KPrefabIDTracker
{
	private static KPrefabIDTracker Instance;

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
			if (prefabIdMap.ContainsKey(instance.InstanceID))
			{
				Debug.LogWarningFormat(instance.gameObject, "KPID instance id {0} was previously used by {1} but we're trying to add it from {2}. Conflict!", instance.InstanceID, prefabIdMap[instance.InstanceID].gameObject, instance.name);
			}
			prefabIdMap[instance.InstanceID] = instance;
		}
	}

	public void Unregister(KPrefabID instance)
	{
		prefabIdMap.Remove(instance.InstanceID);
	}

	public KPrefabID GetInstance(int instance_id)
	{
		KPrefabID value = null;
		prefabIdMap.TryGetValue(instance_id, out value);
		return value;
	}
}
