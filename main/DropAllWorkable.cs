using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DropAllWorkable : Workable
{
	private Chore chore;

	private bool showCmd;

	private Storage[] storages;

	private static readonly EventSystem.IntraObjectHandler<DropAllWorkable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<DropAllWorkable>(delegate(DropAllWorkable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<DropAllWorkable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<DropAllWorkable>(delegate(DropAllWorkable component, object data)
	{
		component.OnStorageChange(data);
	});

	protected DropAllWorkable()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
		synchronizeAnims = false;
		SetWorkTime(0.1f);
	}

	private Storage[] GetStorages()
	{
		if (storages == null)
		{
			storages = GetComponents<Storage>();
		}
		return storages;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		showCmd = GetNewShowCmd();
	}

	public void DropAll()
	{
		if (DebugHandler.InstantBuildMode)
		{
			OnCompleteWork(null);
		}
		else if (chore == null)
		{
			chore = new WorkChore<DropAllWorkable>(Db.Get().ChoreTypes.EmptyStorage, this, null, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		}
		else
		{
			chore.Cancel("Cancelled emptying");
			chore = null;
			GetComponent<KSelectable>().RemoveStatusItem(workerStatusItem, false);
			ShowProgressBar(false);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage[] array = GetStorages();
		for (int i = 0; i < array.Length; i++)
		{
			List<GameObject> list = new List<GameObject>(array[i].items);
			for (int j = 0; j < list.Count; j++)
			{
				GameObject gameObject = array[i].Drop(list[j]);
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					Pickupable component = gameObject.GetComponent<Pickupable>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.TryToOffsetIfBuried();
					}
				}
			}
		}
		chore = null;
		Trigger(-1957399615, null);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (showCmd)
		{
			object buttonInfo;
			if (chore == null)
			{
				string iconName = "action_empty_contents";
				string text = UI.USERMENUACTIONS.EMPTYSTORAGE.NAME;
				System.Action on_click = DropAll;
				string tooltipText = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
			}
			else
			{
				string tooltipText = "action_empty_contents";
				string text = UI.USERMENUACTIONS.EMPTYSTORAGE.NAME_OFF;
				System.Action on_click = DropAll;
				string iconName = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP_OFF;
				buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
			}
			KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
			Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
		}
	}

	private bool GetNewShowCmd()
	{
		bool flag = false;
		Storage[] array = GetStorages();
		for (int i = 0; i < array.Length; i++)
		{
			flag = (flag || !array[i].IsEmpty());
		}
		return flag;
	}

	private void OnStorageChange(object data)
	{
		bool newShowCmd = GetNewShowCmd();
		if (newShowCmd != showCmd)
		{
			showCmd = newShowCmd;
			Game.Instance.userMenu.Refresh(base.gameObject);
		}
	}
}
