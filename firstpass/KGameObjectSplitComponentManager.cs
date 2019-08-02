using UnityEngine;

public abstract class KGameObjectSplitComponentManager<Header, Payload> : KSplitComponentManager<Header, Payload> where Header : new()where Payload : new()
{
	public HandleVector<int>.Handle Add(GameObject go, Header header, ref Payload payload)
	{
		return InternalAddComponent(go, header, ref payload);
	}

	public virtual void Remove(GameObject go)
	{
		HandleVector<int>.Handle handle = GetHandle(go);
		CleanupInfo cleanupInfo = new CleanupInfo(go, handle);
		if (!KComponentCleanUp.InCleanUpPhase)
		{
			cleanupList.Add(cleanupInfo);
		}
		else
		{
			RemoveFromCleanupList(go);
			OnCleanUp(handle);
			InternalRemoveComponent(cleanupInfo);
		}
	}

	public HandleVector<int>.Handle GetHandle(GameObject obj)
	{
		return GetHandle((object)obj);
	}

	public HandleVector<int>.Handle GetHandle(MonoBehaviour obj)
	{
		return GetHandle((object)obj.gameObject);
	}
}
