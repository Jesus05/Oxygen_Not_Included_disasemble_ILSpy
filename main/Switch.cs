using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Switch : KMonoBehaviour, ISaveLoadable, IToggleHandler
{
	[SerializeField]
	public bool manuallyControlled = true;

	[SerializeField]
	public bool defaultState = true;

	[Serialize]
	protected bool switchedOn = true;

	[MyCmpAdd]
	private Toggleable openSwitch;

	private int openToggleIndex;

	private static readonly EventSystem.IntraObjectHandler<Switch> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Switch>(delegate(Switch component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public bool IsSwitchedOn => switchedOn;

	public event Action<bool> OnToggle;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		switchedOn = defaultState;
	}

	protected override void OnSpawn()
	{
		openToggleIndex = openSwitch.SetTarget(this);
		if (this.OnToggle != null)
		{
			this.OnToggle(switchedOn);
		}
		if (manuallyControlled)
		{
			Subscribe(493375141, OnRefreshUserMenuDelegate);
		}
		UpdateSwitchStatus();
	}

	public void HandleToggle()
	{
		Toggle();
	}

	public bool IsHandlerOn()
	{
		return switchedOn;
	}

	private void OnMinionToggle()
	{
		if (!DebugHandler.InstantBuildMode)
		{
			openSwitch.Toggle(openToggleIndex);
		}
		else
		{
			Toggle();
		}
	}

	protected virtual void Toggle()
	{
		SetState(!switchedOn);
	}

	protected virtual void SetState(bool on)
	{
		if (switchedOn != on)
		{
			switchedOn = on;
			UpdateSwitchStatus();
			if (this.OnToggle != null)
			{
				this.OnToggle(switchedOn);
			}
			if (manuallyControlled)
			{
				Game.Instance.userMenu.Refresh(base.gameObject);
			}
		}
	}

	protected virtual void OnRefreshUserMenu(object data)
	{
		LocString loc_string = (!switchedOn) ? BUILDINGS.PREFABS.SWITCH.TURN_ON : BUILDINGS.PREFABS.SWITCH.TURN_OFF;
		LocString loc_string2 = (!switchedOn) ? BUILDINGS.PREFABS.SWITCH.TURN_ON_TOOLTIP : BUILDINGS.PREFABS.SWITCH.TURN_OFF_TOOLTIP;
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string iconName = "action_power";
		string text = loc_string;
		System.Action on_click = OnMinionToggle;
		Action shortcutKey = Action.ToggleEnabled;
		string tooltipText = loc_string2;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, shortcutKey, null, null, null, tooltipText, true), 1f);
	}

	protected virtual void UpdateSwitchStatus()
	{
		StatusItem status_item = (!switchedOn) ? Db.Get().BuildingStatusItems.SwitchStatusInactive : Db.Get().BuildingStatusItems.SwitchStatusActive;
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, null);
	}
}
