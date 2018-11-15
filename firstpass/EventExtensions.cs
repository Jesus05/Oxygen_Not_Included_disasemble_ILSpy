using UnityEngine;

public static class EventExtensions
{
	public static int Subscribe<ComponentType>(this GameObject go, int hash, EventSystem.IntraObjectHandler<ComponentType> handler)
	{
		return KObjectManager.Instance.GetOrCreateObject(go).GetEventSystem().Subscribe(hash, handler);
	}

	public static void Trigger(this GameObject go, int hash, object data = null)
	{
		KObject kObject = KObjectManager.Instance.Get(go);
		if (kObject != null && kObject.hasEventSystem)
		{
			kObject.GetEventSystem().Trigger(go, hash, data);
		}
	}
}
