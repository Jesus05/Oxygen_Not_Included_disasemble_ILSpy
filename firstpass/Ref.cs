using KSerialization;
using System.Diagnostics;
using System.Runtime.Serialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{id}")]
public class Ref<ReferenceType> : ISaveLoadable where ReferenceType : KMonoBehaviour
{
	[Serialize]
	private int id = -1;

	private ReferenceType obj;

	public Ref(ReferenceType obj)
	{
		Set(obj);
	}

	public Ref()
	{
	}

	private void UpdateID()
	{
		ReferenceType exists = Get();
		if ((bool)(Object)exists)
		{
			id = ((Component)obj).GetComponent<KPrefabID>().InstanceID;
		}
		else
		{
			id = -1;
		}
	}

	[OnSerializing]
	public void OnSerializing()
	{
		UpdateID();
	}

	public int GetId()
	{
		UpdateID();
		return id;
	}

	public ComponentType Get<ComponentType>() where ComponentType : MonoBehaviour
	{
		ReferenceType x = this.Get();
		if (!((Object)x == (Object)null))
		{
			return ((Component)x).GetComponent<ComponentType>();
		}
		return (ComponentType)null;
	}

	public ReferenceType Get()
	{
		if ((Object)obj == (Object)null && id != -1)
		{
			KPrefabID instance = KPrefabIDTracker.Get().GetInstance(id);
			if ((Object)instance != (Object)null)
			{
				obj = ((Component)instance).GetComponent<ReferenceType>();
				if ((Object)obj == (Object)null)
				{
					id = -1;
					Debug.LogWarning("Missing " + typeof(ReferenceType).Name + " reference: " + id);
				}
			}
			else
			{
				Debug.LogWarning("Missing KPrefabID reference: " + id);
				id = -1;
			}
		}
		return obj;
	}

	public void Set(ReferenceType obj)
	{
		if ((Object)obj == (Object)null)
		{
			id = -1;
		}
		else
		{
			id = ((Component)obj).GetComponent<KPrefabID>().InstanceID;
		}
		this.obj = obj;
	}
}
