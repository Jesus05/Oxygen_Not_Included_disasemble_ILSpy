using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Clearable : Workable, ISaveLoadable, IRender200ms
{
	[MyCmpReq]
	private Pickupable pickupable;

	[MyCmpReq]
	private KSelectable selectable;

	[Serialize]
	private bool isMarkedForClear;

	private HandleVector<int>.Handle clearHandle;

	public bool isClearable = true;

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnStore(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnAbsorb(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Clearable> OnEquippedDelegate = new EventSystem.IntraObjectHandler<Clearable>(delegate(Clearable component, object data)
	{
		component.OnEquipped(data);
	});

	protected override void OnPrefabInit()
	{
		Subscribe(2127324410, OnCancelDelegate);
		Subscribe(856640610, OnStoreDelegate);
		Subscribe(-2064133523, OnAbsorbDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-1617557748, OnEquippedDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.Clearing;
		simRenderLoadBalance = true;
		autoRegisterSimRender = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (isMarkedForClear)
		{
			if (this.HasTag(GameTags.Stored))
			{
				isMarkedForClear = false;
			}
			else
			{
				MarkForClear(true);
			}
		}
	}

	private void OnStore(object data)
	{
		CancelClearing();
	}

	private void OnCancel(object data)
	{
		for (ObjectLayerListItem objectLayerListItem = pickupable.objectLayerListItem; objectLayerListItem != null; objectLayerListItem = objectLayerListItem.nextItem)
		{
			if ((UnityEngine.Object)objectLayerListItem.gameObject != (UnityEngine.Object)null)
			{
				objectLayerListItem.gameObject.GetComponent<Clearable>().CancelClearing();
			}
		}
	}

	public void CancelClearing()
	{
		if (isMarkedForClear)
		{
			isMarkedForClear = false;
			GetComponent<KPrefabID>().RemoveTag(GameTags.Garbage);
			Prioritizable.RemoveRef(base.gameObject);
			if (clearHandle.IsValid())
			{
				GlobalChoreProvider.Instance.UnregisterClearable(clearHandle);
				clearHandle.Clear();
			}
			RefreshClearableStatus();
			SimAndRenderScheduler.instance.Remove(this);
		}
	}

	public void MarkForClear(bool force = false)
	{
		if (isClearable && (!isMarkedForClear || force) && !pickupable.IsEntombed && !clearHandle.IsValid() && !this.HasTag(GameTags.Stored))
		{
			Prioritizable.AddRef(base.gameObject);
			GetComponent<KPrefabID>().AddTag(GameTags.Garbage);
			isMarkedForClear = true;
			clearHandle = GlobalChoreProvider.Instance.RegisterClearable(this);
			RefreshClearableStatus();
			SimAndRenderScheduler.instance.Add(this, simRenderLoadBalance);
		}
	}

	private void OnClickClear()
	{
		MarkForClear(false);
	}

	private void OnClickCancel()
	{
		CancelClearing();
	}

	private void OnEquipped(object data)
	{
		CancelClearing();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (clearHandle.IsValid())
		{
			GlobalChoreProvider.Instance.UnregisterClearable(clearHandle);
			clearHandle.Clear();
		}
	}

	private void OnRefreshUserMenu(object data)
	{
		if (isClearable && !((UnityEngine.Object)GetComponent<Health>() != (UnityEngine.Object)null) && !this.HasTag(GameTags.Stored))
		{
			object buttonInfo;
			if (isMarkedForClear)
			{
				string iconName = "action_move_to_storage";
				string text = UI.USERMENUACTIONS.CLEAR.NAME_OFF;
				System.Action on_click = OnClickCancel;
				string tooltipText = UI.USERMENUACTIONS.CLEAR.TOOLTIP_OFF;
				buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
			}
			else
			{
				string tooltipText = "action_move_to_storage";
				string text = UI.USERMENUACTIONS.CLEAR.NAME;
				System.Action on_click = OnClickClear;
				string iconName = UI.USERMENUACTIONS.CLEAR.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
			}
			KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
			Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
		}
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
		{
			Clearable component = pickupable.GetComponent<Clearable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.isMarkedForClear)
			{
				MarkForClear(false);
			}
		}
	}

	public void Render200ms(float dt)
	{
		RefreshClearableStatus();
	}

	public void RefreshClearableStatus()
	{
		if (isMarkedForClear)
		{
			bool flag = GlobalChoreProvider.Instance.ClearableHasDestination(pickupable);
			selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClear, flag, this);
			selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClearNoStorage, !flag, this);
		}
		else
		{
			selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClear, false, this);
			selectable.ToggleStatusItem(Db.Get().MiscStatusItems.PendingClearNoStorage, false, this);
		}
	}
}
