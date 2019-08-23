using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicCritterCountSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	private bool wasOn;

	private bool countEggs = true;

	[Serialize]
	public int countThreshold;

	[Serialize]
	public bool activateOnGreaterThan = true;

	private int currentCount;

	private KSelectable selectable;

	private Guid roomStatusGUID;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicCritterCountSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicCritterCountSensor>(delegate(LogicCritterCountSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public float Threshold
	{
		get
		{
			return (float)countThreshold;
		}
		set
		{
			countThreshold = (int)value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateOnGreaterThan;
		}
		set
		{
			activateOnGreaterThan = value;
		}
	}

	public float CurrentValue => (float)currentCount;

	public float RangeMin => 0f;

	public float RangeMax => 64f;

	public LocString Title => UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TITLE;

	public LocString ThresholdValueName => UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.VALUE_NAME;

	public string AboveToolTip => UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(RangeMax);

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
			activateOnGreaterThan = component.activateOnGreaterThan;
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
			currentCount = roomOfGameObject.cavity.creatures.Count;
			if (countEggs)
			{
				currentCount += roomOfGameObject.cavity.eggs.Count;
			}
			SetState(currentCount > countThreshold);
			bool state = (!activateOnGreaterThan) ? (currentCount < countThreshold) : (currentCount > countThreshold);
			SetState(state);
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

	public float GetRangeMinInputField()
	{
		return RangeMin;
	}

	public float GetRangeMaxInputField()
	{
		return RangeMax;
	}

	public string Format(float value, bool units)
	{
		return value.ToString();
	}

	public float ProcessedSliderValue(float input)
	{
		return Mathf.Round(input);
	}

	public float ProcessedInputValue(float input)
	{
		return Mathf.Round(input);
	}

	public LocString ThresholdValueUnits()
	{
		return string.Empty;
	}
}
