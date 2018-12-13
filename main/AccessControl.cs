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

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

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

	private static readonly EventSystem.IntraObjectHandler<AccessControl> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<AccessControl>(delegate(AccessControl component, object data)
	{
		component.OnCopySettings(data);
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
			accessControlActive = new StatusItem("accessControlActive", BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.NAME, BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 63486);
		}
		Subscribe(279163026, OnControlStateChangedDelegate);
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		List<Tuple<MinionAssignablesProxy, Permission>> list = new List<Tuple<MinionAssignablesProxy, Permission>>();
		for (int num = savedPermissions.Count - 1; num >= 0; num--)
		{
			KPrefabID kPrefabID = savedPermissions[num].Key.Get();
			if ((Object)kPrefabID != (Object)null)
			{
				MinionIdentity component = kPrefabID.GetComponent<MinionIdentity>();
				if ((Object)component != (Object)null)
				{
					list.Add(new Tuple<MinionAssignablesProxy, Permission>(component.assignableProxy.Get(), savedPermissions[num].Value));
					savedPermissions.RemoveAt(num);
				}
			}
		}
		foreach (Tuple<MinionAssignablesProxy, Permission> item in list)
		{
			SetPermission(item.first.gameObject, item.second);
		}
		SetStatusItem();
	}

	private void OnControlStateChanged(object data)
	{
		overrideAccess = (Door.ControlState)data;
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		AccessControl component = gameObject.GetComponent<AccessControl>();
		if ((Object)component != (Object)null)
		{
			savedPermissions.Clear();
			foreach (KeyValuePair<Ref<KPrefabID>, Permission> savedPermission in component.savedPermissions)
			{
				if ((Object)savedPermission.Key.Get() != (Object)null)
				{
					SetPermission(savedPermission.Key.Get().gameObject, savedPermission.Value);
				}
			}
			_defaultPermission = component._defaultPermission;
		}
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
