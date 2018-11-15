using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class AccessControl : KMonoBehaviour, ISaveLoadable
{
	public enum Permission
	{
		Both,
		GoLeft,
		GoRight,
		Neither
	}

	[MyCmpGet]
	private Operational operational;

	[MyCmpReq]
	private KSelectable selectable;

	[Serialize]
	private List<KeyValuePair<Ref<KPrefabID>, Permission>> savedPermissions = new List<KeyValuePair<Ref<KPrefabID>, Permission>>();

	[Serialize]
	private Permission _defaultPermission;

	[Serialize]
	public bool controlEnabled;

	public Door.ControlState overrideAccess;

	private static StatusItem accessControlActive;

	private static readonly EventSystem.IntraObjectHandler<AccessControl> OnControlStateChangedDelegate = new EventSystem.IntraObjectHandler<AccessControl>(delegate(AccessControl component, object data)
	{
		component.OnControlStateChanged(data);
	});

	public Permission DefaultPermission
	{
		get
		{
			return _defaultPermission;
		}
		set
		{
			_defaultPermission = value;
			SetStatusItem();
		}
	}

	public bool Online => true;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (accessControlActive == null)
		{
			accessControlActive = new StatusItem("accessControlActive", BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.NAME, BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486);
		}
		Subscribe(279163026, OnControlStateChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SetStatusItem();
	}

	private void OnControlStateChanged(object data)
	{
		overrideAccess = (Door.ControlState)data;
	}

	public void SetPermission(GameObject key, Permission permission)
	{
		KPrefabID component = key.GetComponent<KPrefabID>();
		if (!((Object)component == (Object)null))
		{
			bool flag = false;
			for (int i = 0; i < savedPermissions.Count; i++)
			{
				if (savedPermissions[i].Key.GetId() == component.InstanceID)
				{
					flag = true;
					KeyValuePair<Ref<KPrefabID>, Permission> keyValuePair = savedPermissions[i];
					savedPermissions[i] = new KeyValuePair<Ref<KPrefabID>, Permission>(keyValuePair.Key, permission);
					break;
				}
			}
			if (!flag)
			{
				savedPermissions.Add(new KeyValuePair<Ref<KPrefabID>, Permission>(new Ref<KPrefabID>(component), permission));
			}
			SetStatusItem();
		}
	}

	public Permission GetPermission(GameObject key)
	{
		switch (overrideAccess)
		{
		case Door.ControlState.Closed:
			return Permission.Neither;
		case Door.ControlState.Opened:
			return Permission.Both;
		default:
			return GetSetPermission(key);
		}
	}

	public Permission GetSetPermission(GameObject key)
	{
		Permission result = DefaultPermission;
		KPrefabID component = key.GetComponent<KPrefabID>();
		if ((Object)component != (Object)null)
		{
			for (int i = 0; i < savedPermissions.Count; i++)
			{
				if (savedPermissions[i].Key.GetId() == component.InstanceID)
				{
					result = savedPermissions[i].Value;
					break;
				}
			}
		}
		return result;
	}

	public void ClearPermission(GameObject key)
	{
		Permission defaultPermission = DefaultPermission;
		KPrefabID component = key.GetComponent<KPrefabID>();
		if ((Object)component != (Object)null)
		{
			for (int i = 0; i < savedPermissions.Count; i++)
			{
				if (savedPermissions[i].Key.GetId() == component.InstanceID)
				{
					savedPermissions.RemoveAt(i);
					break;
				}
			}
		}
		SetStatusItem();
	}

	public bool IsDefaultPermission(GameObject key)
	{
		bool flag = false;
		KPrefabID component = key.GetComponent<KPrefabID>();
		if ((Object)component != (Object)null)
		{
			for (int i = 0; i < savedPermissions.Count; i++)
			{
				if (savedPermissions[i].Key.GetId() == component.InstanceID)
				{
					flag = true;
					break;
				}
			}
		}
		return !flag;
	}

	private void SetStatusItem()
	{
		if (_defaultPermission != 0 || savedPermissions.Count > 0)
		{
			selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, accessControlActive, null);
		}
		else
		{
			selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, null, null);
		}
	}
}
