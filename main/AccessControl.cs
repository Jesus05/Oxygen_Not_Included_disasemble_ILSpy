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
			SetGridRestrictions(null, _defaultPermission);
		}
	}

	public bool Online => true;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (accessControlActive == null)
		{
			accessControlActive = new StatusItem("accessControlActive", BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.NAME, BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022);
		}
		Subscribe(279163026, OnControlStateChangedDelegate);
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RegisterInGrid(true);
		SetGridRestrictions(null, DefaultPermission);
		foreach (KeyValuePair<Ref<KPrefabID>, Permission> savedPermission in savedPermissions)
		{
			SetGridRestrictions(savedPermission.Key.Get(), savedPermission.Value);
		}
		ListPool<Tuple<MinionAssignablesProxy, Permission>, AccessControl>.PooledList pooledList = ListPool<Tuple<MinionAssignablesProxy, Permission>, AccessControl>.Allocate();
		for (int num = savedPermissions.Count - 1; num >= 0; num--)
		{
			KPrefabID kPrefabID = savedPermissions[num].Key.Get();
			if ((Object)kPrefabID != (Object)null)
			{
				MinionIdentity component = kPrefabID.GetComponent<MinionIdentity>();
				if ((Object)component != (Object)null)
				{
					pooledList.Add(new Tuple<MinionAssignablesProxy, Permission>(component.assignableProxy.Get(), savedPermissions[num].Value));
					savedPermissions.RemoveAt(num);
					ClearGridRestrictions(kPrefabID);
				}
			}
		}
		foreach (Tuple<MinionAssignablesProxy, Permission> item in pooledList)
		{
			SetPermission(item.first, item.second);
		}
		pooledList.Recycle();
		SetStatusItem();
	}

	protected override void OnCleanUp()
	{
		RegisterInGrid(false);
		base.OnCleanUp();
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
					SetPermission(savedPermission.Key.Get().GetComponent<MinionAssignablesProxy>(), savedPermission.Value);
				}
			}
			_defaultPermission = component._defaultPermission;
			SetGridRestrictions(null, DefaultPermission);
		}
	}

	public void SetPermission(MinionAssignablesProxy key, Permission permission)
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
			SetGridRestrictions(component, permission);
		}
	}

	private void RegisterInGrid(bool register)
	{
		Building component = GetComponent<Building>();
		if (!((Object)component == (Object)null))
		{
			if (register)
			{
				Rotatable component2 = GetComponent<Rotatable>();
				Grid.Restriction.Orientation orientation = (!((Object)component2 == (Object)null) && component2.GetOrientation() != 0) ? Grid.Restriction.Orientation.Horizontal : Grid.Restriction.Orientation.Vertical;
				int[] placementCells = component.PlacementCells;
				foreach (int cell in placementCells)
				{
					Grid.RegisterRestriction(cell, orientation);
				}
			}
			else
			{
				int[] placementCells2 = component.PlacementCells;
				foreach (int cell2 in placementCells2)
				{
					Grid.UnregisterRestriction(cell2);
				}
			}
		}
	}

	private void SetGridRestrictions(KPrefabID kpid, Permission permission)
	{
		Building component = GetComponent<Building>();
		if (!((Object)component == (Object)null))
		{
			int minion = (!((Object)kpid != (Object)null)) ? (-1) : kpid.InstanceID;
			Grid.Restriction.Directions directions = (Grid.Restriction.Directions)0;
			switch (permission)
			{
			case Permission.Both:
				directions = (Grid.Restriction.Directions)0;
				break;
			case Permission.GoLeft:
				directions = Grid.Restriction.Directions.Right;
				break;
			case Permission.GoRight:
				directions = Grid.Restriction.Directions.Left;
				break;
			case Permission.Neither:
				directions = (Grid.Restriction.Directions.Left | Grid.Restriction.Directions.Right);
				break;
			}
			int[] placementCells = component.PlacementCells;
			foreach (int cell in placementCells)
			{
				Grid.SetRestriction(cell, minion, directions);
			}
		}
	}

	private void ClearGridRestrictions(KPrefabID kpid)
	{
		Building component = GetComponent<Building>();
		if (!((Object)component == (Object)null))
		{
			int minion = (!((Object)kpid != (Object)null)) ? (-1) : kpid.InstanceID;
			int[] placementCells = component.PlacementCells;
			foreach (int cell in placementCells)
			{
				Grid.ClearRestriction(cell, minion);
			}
		}
	}

	public Permission GetPermission(Navigator minion)
	{
		switch (overrideAccess)
		{
		case Door.ControlState.Locked:
			return Permission.Neither;
		case Door.ControlState.Opened:
			return Permission.Both;
		default:
			return GetSetPermission(GetKeyForNavigator(minion));
		}
	}

	private MinionAssignablesProxy GetKeyForNavigator(Navigator minion)
	{
		MinionIdentity component = minion.GetComponent<MinionIdentity>();
		return component.assignableProxy.Get();
	}

	public Permission GetSetPermission(MinionAssignablesProxy key)
	{
		return GetSetPermission(key.GetComponent<KPrefabID>());
	}

	private Permission GetSetPermission(KPrefabID kpid)
	{
		Permission result = DefaultPermission;
		if ((Object)kpid != (Object)null)
		{
			for (int i = 0; i < savedPermissions.Count; i++)
			{
				if (savedPermissions[i].Key.GetId() == kpid.InstanceID)
				{
					result = savedPermissions[i].Value;
					break;
				}
			}
		}
		return result;
	}

	public void ClearPermission(MinionAssignablesProxy key)
	{
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
		ClearGridRestrictions(component);
	}

	public bool IsDefaultPermission(MinionAssignablesProxy key)
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
