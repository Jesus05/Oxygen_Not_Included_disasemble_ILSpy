using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicGateFilter : LogicGate, ISingleSliderControl, ISliderControl
{
	private SchedulerHandle schedulerHandle;

	[Serialize]
	private bool input_was_previously_negative;

	[Serialize]
	private float delayAmount = 5f;

	private MeterController meter;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicGateFilter> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicGateFilter>(delegate(LogicGateFilter component, object data)
	{
		component.OnCopySettings(data);
	});

	public float DelayAmount
	{
		get
		{
			return delayAmount;
		}
		set
		{
			delayAmount = value;
			if (schedulerHandle.IsValid && schedulerHandle.TimeRemaining > delayAmount)
			{
				schedulerHandle.ClearScheduler();
				schedulerHandle = GameScheduler.Instance.Schedule("logic delay", delayAmount, OnDelay, null, null);
			}
		}
	}

	public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.LOGIC_DELAY_SIDE_SCREEN.TITLE";

	public string SliderUnits => UI.UNITSUFFIXES.SECOND;

	public float GetSliderMin(int index)
	{
		return 0.1f;
	}

	public float GetSliderMax(int index)
	{
		return 200f;
	}

	public float GetSliderValue(int index)
	{
		return delayAmount;
	}

	public void SetSliderValue(float value, int index)
	{
		delayAmount = value;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.LOGIC_DELAY_SIDE_SCREEN.TOOLTIP";
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		LogicGateFilter component = gameObject.GetComponent<LogicGateFilter>();
		if ((Object)component != (Object)null)
		{
			DelayAmount = component.DelayAmount;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		meter = new MeterController((KAnimControllerBase)component, "meter_target", "meter", Meter.Offset.UserSpecified, Grid.SceneLayer.LogicWireBridgesFront, Vector3.zero, (string[])null);
		meter.SetPositionPercent(0f);
	}

	private void Update()
	{
		if (schedulerHandle.IsValid)
		{
			float timeRemaining = schedulerHandle.TimeRemaining;
			meter.SetPositionPercent(timeRemaining / delayAmount);
		}
	}

	protected override int GetCustomValue(int val1, int val2)
	{
		if (val1 == 0)
		{
			input_was_previously_negative = true;
			if (schedulerHandle.IsValid)
			{
				schedulerHandle.ClearScheduler();
			}
			meter.SetPositionPercent(1f);
		}
		else if (!schedulerHandle.IsValid)
		{
			if (input_was_previously_negative)
			{
				schedulerHandle = GameScheduler.Instance.Schedule("logic delay", delayAmount, OnDelay, null, null);
			}
			input_was_previously_negative = false;
		}
		return (val1 != 0 && !(schedulerHandle.TimeRemaining > 0f)) ? 1 : 0;
	}

	private void OnDelay(object data)
	{
		if (!cleaningUp)
		{
			meter.SetPositionPercent(0f);
			if (outputValue != 1)
			{
				int outputCell = base.OutputCell;
				LogicCircuitNetwork logicCircuitNetwork = Game.Instance.logicCircuitSystem.GetNetworkForCell(outputCell) as LogicCircuitNetwork;
				if (logicCircuitNetwork != null)
				{
					outputValue = 1;
					RefreshAnimation();
				}
			}
		}
	}
}
