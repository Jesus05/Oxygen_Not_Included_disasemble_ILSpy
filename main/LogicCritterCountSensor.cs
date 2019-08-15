using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicCritterCountSensor : Switch, ISaveLoadable, ISim200ms, IIntSliderControl, ISliderControl
{
	private bool wasOn;

	private bool countEggs = true;

	[Serialize]
	public int countThreshold;

	private KSelectable selectable;

	private Guid roomStatusGUID;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicCritterCountSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicCritterCountSensor>(delegate(LogicCritterCountSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TITLE";

	public string SliderUnits => UI.UNITSUFFIXES.CRITTERS;

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 64f;
	}

	public float GetSliderValue(int index)
	{
		return (float)countThreshold;
	}

	public void SetSliderValue(float value, int index)
	{
		countThreshold = Mathf.RoundToInt(value);
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP";
	}

	string ISliderControl.GetSliderTooltip()
	{
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP"), countThreshold);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		selectable = GetComponent<KSelectable>();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		LogicCritterCountSensor component = gameObject.GetComponent<LogicCritterCountSensor>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			countThreshold = component.countThreshold;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += OnSwitchToggled;
		UpdateLogicCircuit();
		UpdateVisualState(true);
		wasOn = switchedOn;
	}

	public void Sim200ms(float dt)
	{
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			int num = roomOfGameObject.cavity.creatures.Count;
			if (countEggs)
			{
				num += roomOfGameObject.cavity.eggs.Count;
			}
			SetState(num > countThreshold);
			if (selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
			{
				selectable.RemoveStatusItem(roomStatusGUID, false);
			}
		}
		else
		{
			if (!selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
			{
				roomStatusGUID = selectable.AddStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom, null);
			}
			SetState(false);
		}
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState(false);
	}

	private void UpdateLogicCircuit()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		component.SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (wasOn != switchedOn || force)
		{
			wasOn = switchedOn;
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			component.Play((!switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue((!switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (!switchedOn) ? Db.Get().BuildingStatusItems.LogicSensorStatusInactive : Db.Get().BuildingStatusItems.LogicSensorStatusActive;
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, null);
	}
}
