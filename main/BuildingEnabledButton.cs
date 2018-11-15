using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class BuildingEnabledButton : KMonoBehaviour, ISaveLoadable, IToggleHandler
{
	[MyCmpAdd]
	private Toggleable Toggleable;

	[MyCmpReq]
	private Operational Operational;

	private int ToggleIdx;

	[Serialize]
	private bool buildingEnabled = true;

	public static readonly Operational.Flag EnabledFlag = new Operational.Flag("building_enabled", Operational.Flag.Type.Functional);

	private static readonly EventSystem.IntraObjectHandler<BuildingEnabledButton> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BuildingEnabledButton>(delegate(BuildingEnabledButton component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public bool IsEnabled
	{
		get
		{
			return (UnityEngine.Object)Operational != (UnityEngine.Object)null && Operational.GetFlag(EnabledFlag);
		}
		set
		{
			Operational.SetFlag(EnabledFlag, value);
			Game.Instance.userMenu.Refresh(base.gameObject);
			buildingEnabled = value;
			GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.BuildingDisabled, !buildingEnabled, null);
			Trigger(1088293757, buildingEnabled);
		}
	}

	public bool WaitingForDisable => IsEnabled && Toggleable.IsToggleQueued(ToggleIdx);

	protected override void OnPrefabInit()
	{
		ToggleIdx = Toggleable.SetTarget(this);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	protected override void OnSpawn()
	{
		IsEnabled = buildingEnabled;
	}

	public void HandleToggle()
	{
		Prioritizable.RemoveRef(base.gameObject);
		OnToggle();
	}

	public bool IsHandlerOn()
	{
		return IsEnabled;
	}

	private void OnToggle()
	{
		IsEnabled = !IsEnabled;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	private void OnMenuToggle()
	{
		if (!Toggleable.IsToggleQueued(ToggleIdx))
		{
			if (IsEnabled)
			{
				Trigger(2108245096, "BuildingDisabled");
			}
			Prioritizable.AddRef(base.gameObject);
		}
		else
		{
			Prioritizable.RemoveRef(base.gameObject);
		}
		Toggleable.Toggle(ToggleIdx);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		bool isEnabled = IsEnabled;
		bool flag = Toggleable.IsToggleQueued(ToggleIdx);
		KIconButtonMenu.ButtonInfo buttonInfo = null;
		if ((isEnabled && !flag) || (!isEnabled && flag))
		{
			string iconName = "action_building_disabled";
			string text = UI.USERMENUACTIONS.ENABLEBUILDING.NAME;
			System.Action on_click = OnMenuToggle;
			Action shortcutKey = Action.ToggleEnabled;
			string tooltipText = UI.USERMENUACTIONS.ENABLEBUILDING.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, shortcutKey, null, null, null, tooltipText, true);
		}
		else
		{
			string tooltipText = "action_building_disabled";
			string text = UI.USERMENUACTIONS.ENABLEBUILDING.NAME_OFF;
			System.Action on_click = OnMenuToggle;
			Action shortcutKey = Action.ToggleEnabled;
			string iconName = UI.USERMENUACTIONS.ENABLEBUILDING.TOOLTIP_OFF;
			buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, shortcutKey, null, null, null, iconName, true);
		}
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}
}
