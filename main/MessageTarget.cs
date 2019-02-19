using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MessageTarget : ISaveLoadable
{
	[Serialize]
	private Ref<KPrefabID> prefabId = new Ref<KPrefabID>();

	[Serialize]
	private Vector3 position;

	[Serialize]
	private string name;

	public MessageTarget(KPrefabID prefab_id)
	{
		prefabId.Set(prefab_id);
		position = prefab_id.transform.GetPosition();
		name = "Unknown";
		KSelectable component = prefab_id.GetComponent<KSelectable>();
		if ((Object)component != (Object)null)
		{
			name = component.GetName();
		}
		prefab_id.Subscribe(-1940207677, OnAbsorbedBy);
	}

	public Vector3 GetPosition()
	{
		if ((Object)prefabId.Get() != (Object)null)
		{
			return prefabId.Get().transform.GetPosition();
		}
		return position;
	}

	public KSelectable GetSelectable()
	{
		if ((Object)prefabId.Get() != (Object)null)
		{
			return prefabId.Get().transform.GetComponent<KSelectable>();
		}
		return null;
	}

	public string GetName()
	{
		return name;
	}

	private void OnAbsorbedBy(object data)
	{
		if ((Object)prefabId.Get() != (Object)null)
		{
			prefabId.Get().Unsubscribe(-1940207677, OnAbsorbedBy);
		}
		GameObject gameObject = (GameObject)data;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.Subscribe(-1940207677, OnAbsorbedBy);
		prefabId.Set(component);
	}

	public void OnCleanUp()
	{
		if ((Object)prefabId.Get() != (Object)null)
		{
			prefabId.Get().Unsubscribe(-1940207677, OnAbsorbedBy);
			prefabId.Set(null);
		}
	}
}
