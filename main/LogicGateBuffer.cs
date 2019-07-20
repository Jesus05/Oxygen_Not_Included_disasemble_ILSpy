using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicGateBuffer : LogicGate, ISingleSliderControl, ISliderControl
{
	[Serialize]
	private bool input_was_previously_positive;

	[Serialize]
	private float delayAmount = 5f;

	[Serialize]
	private int delayTicksRemaining = 0;

	private MeterController meter;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicGateBuffer> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicGateBuffer>(delegate(LogicGateBuffer component, object data)
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
			int delayAmountTicks = DelayAmountTicks;
			if (delayTicksRemaining > delayAmountTicks)
			{
				delayTicksRemaining = delayAmountTicks;
			}
		}
	}

	private int DelayAmountTicks => Mathf.RoundToInt(delayAmount / LogicCircuitManager.ClockTickInterval);

	public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.LOGIC_BUFFER_SIDE_SCREEN.TITLE";

	public string SliderUnits => UI.UNITSUFFIXES.SECOND;

	public int SliderDecimalPlaces(int index)
	{
		return 1;
	}

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
		return DelayAmount;
	}

	public void SetSliderValue(float value, int index)
	{
		DelayAmount = value;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.LOGIC_BUFFER_SIDE_SCREEN.TOOLTIP";
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		LogicGateBuffer component = gameObject.GetComponent<LogicGateBuffer>();
		if ((Object)component != (Object)null)
		{
			DelayAmount = component.DelayAmount;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		meter = new MeterController((KAnimControllerBase)component, "meter_target", "meter", Meter.Offset.UserSpecified, Grid.SceneLayer.LogicGatesFront, Vector3.zero, (string[])null);
		meter.SetPositionPercent(1f);
	}

	private void Update()
	{
		float num = 0f;
		num = (input_was_previously_positive ? 0f : ((delayTicksRemaining <= 0) ? 1f : ((float)(DelayAmountTicks - delayTicksRemaining) / (float)DelayAmountTicks)));
		meter.SetPositionPercent(num);
	}

	public override void LogicTick()
	{
		if (!input_was_previously_positive && delayTicksRemaining > 0)
		{
			delayTicksRemaining--;
			if (delayTicksRemaining <= 0)
			{
				OnDelay();
			}
		}
	}

	protected override int GetCustomValue(int val1, int val2)
	{
		if (val1 != 0)
		{
			input_was_previously_positive = true;
			delayTicksRemaining = 0;
			meter.SetPositionPercent(0f);
		}
		else if (delayTicksRemaining <= 0)
		{
			if (input_was_previously_positive)
			{
				delayTicksRemaining = DelayAmountTicks;
			}
			input_was_previously_positive = false;
		}
		return (val1 != 0 || delayTicksRemaining > 0) ? 1 : 0;
	}

	private void OnDelay()
	{
		if (!cleaningUp)
		{
			delayTicksRemaining = 0;
			meter.SetPositionPercent(1f);
			if (outputValue != 0)
			{
				int outputCell = base.OutputCell;
				LogicCircuitNetwork logicCircuitNetwork = Game.Instance.logicCircuitSystem.GetNetworkForCell(outputCell) as LogicCircuitNetwork;
				if (logicCircuitNetwork != null)
				{
					outputValue = 0;
					RefreshAnimation();
				}
			}
		}
	}
}
