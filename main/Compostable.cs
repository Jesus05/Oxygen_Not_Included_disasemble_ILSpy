using STRINGS;
using System;
using UnityEngine;

public class Compostable : KMonoBehaviour
{
	[SerializeField]
	public bool isMarkedForCompost;

	public GameObject originalPrefab;

	public GameObject compostPrefab;

	private static readonly EventSystem.IntraObjectHandler<Compostable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Compostable>(delegate(Compostable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Compostable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Compostable>(delegate(Compostable component, object data)
	{
		component.OnStore(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (isMarkedForCompost)
		{
			MarkForCompost(false);
		}
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(856640610, OnStoreDelegate);
	}

	private void MarkForCompost(bool force = false)
	{
		RefreshStatusItem();
		GetComponent<KPrefabID>().AddTag(GameTags.MarkedForCompost);
		GetComponent<KPrefabID>().AddTag(GameTags.Compostable);
		Storage storage = GetComponent<Pickupable>().storage;
		if ((UnityEngine.Object)storage != (UnityEngine.Object)null)
		{
			storage.Drop(base.gameObject);
		}
	}

	private void OnToggleCompost()
	{
		if (!isMarkedForCompost)
		{
			Pickupable component = GetComponent<Pickupable>();
			if ((UnityEngine.Object)component.storage != (UnityEngine.Object)null)
			{
				component.storage.Drop(base.gameObject);
			}
			Pickupable pickupable = EntitySplitter.Split(component, component.TotalAmount, compostPrefab);
			if ((UnityEngine.Object)pickupable != (UnityEngine.Object)null)
			{
				SelectTool.Instance.SelectNextFrame(pickupable.GetComponent<KSelectable>(), true);
			}
		}
		else
		{
			Pickupable component2 = GetComponent<Pickupable>();
			Pickupable pickupable2 = EntitySplitter.Split(component2, component2.TotalAmount, originalPrefab);
			SelectTool.Instance.SelectNextFrame(pickupable2.GetComponent<KSelectable>(), true);
		}
	}

	private void RefreshStatusItem()
	{
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForCompost, false);
		component.RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForCompostInStorage, false);
		if (isMarkedForCompost)
		{
			if ((UnityEngine.Object)GetComponent<Pickupable>() != (UnityEngine.Object)null && (UnityEngine.Object)GetComponent<Pickupable>().storage == (UnityEngine.Object)null)
			{
				component.AddStatusItem(Db.Get().MiscStatusItems.MarkedForCompost, null);
			}
			else
			{
				component.AddStatusItem(Db.Get().MiscStatusItems.MarkedForCompostInStorage, null);
			}
		}
	}

	private void OnStore(object data)
	{
		RefreshStatusItem();
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo = null;
		if (!isMarkedForCompost)
		{
			string iconName = "action_compost";
			string text = UI.USERMENUACTIONS.COMPOST.NAME;
			System.Action on_click = OnToggleCompost;
			string tooltipText = UI.USERMENUACTIONS.COMPOST.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
		}
		else
		{
			string tooltipText = "action_compost";
			string text = UI.USERMENUACTIONS.COMPOST.NAME_OFF;
			System.Action on_click = OnToggleCompost;
			string iconName = UI.USERMENUACTIONS.COMPOST.TOOLTIP_OFF;
			buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
		}
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}
}
