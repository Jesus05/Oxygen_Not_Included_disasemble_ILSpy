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
				Output.LogWarningWithObj(instance.gameObject, $"KPID instance id {instance.InstanceID} was previously used by {prefabIdMap[instance.InstanceID].gameObject} but we're trying to add it from {instance.name}. Conflict!");
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
