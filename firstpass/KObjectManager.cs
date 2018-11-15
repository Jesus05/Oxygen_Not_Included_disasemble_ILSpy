using System.Collections.Generic;
using UnityEngine;

public class KObjectManager : MonoBehaviour
{
	private Dictionary<int, KObject> objects = new Dictionary<int, KObject>();

	private List<int> pendingDestroys = new List<int>();

	public static KObjectManager Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Cleanup();
		Instance = null;
	}

	public void Cleanup()
	{
		foreach (KeyValuePair<int, KObject> @object in objects)
		{
			@object.Value.OnCleanUp();
		}
		objects.Clear();
		pendingDestroys.Clear();
	}

	public KObject GetOrCreateObject(GameObject go)
	{
		int instanceID = go.GetInstanceID();
		KObject value = null;
		if (!objects.TryGetValue(instanceID, out value))
		{
			value = new KObject(go);
			objects[instanceID] = value;
		}
		return value;
	}

	public KObject Get(GameObject go)
	{
		KObject value = null;
		objects.TryGetValue(go.GetInstanceID(), out value);
		return value;
	}

	public void QueueDestroy(KObject obj)
	{
		int id = obj.id;
		if (!pendingDestroys.Contains(id))
		{
			pendingDestroys.Add(id);
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < pendingDestroys.Count; i++)
		{
			int key = pendingDestroys[i];
			KObject value = null;
			if (objects.TryGetValue(key, out value))
			{
				objects.Remove(key);
				value.OnCleanUp();
			}
		}
		pendingDestroys.Clear();
	}

	public void DumpEventData()
	{
	}
}
