using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicTemperatureSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	private HandleVector<int>.Handle structureTemperature;

	private int simUpdateCounter;

	[Serialize]
	public float thresholdTemperature = 280f;

	[Serialize]
	public bool activateOnWarmerThan;

	public float minTemp;

	public float maxTemp = 373.15f;

	private const int NumFrameDelay = 8;

	private float[] temperatures = new float[8];

	private float averageTemp;

	private bool wasOn;

	public float StructureTemperature => GameComps.StructureTemperatures.GetData(structureTemperature).Temperature;

	public float Threshold
	{
		get
		{
			return thresholdTemperature;
		}
		set
		{
			thresholdTemperature = value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateOnWarmerThan;
		}
		set
		{
			activateOnWarmerThan = value;
		}
	}

	public float CurrentValue => GetTemperature();

	public float RangeMin => minTemp;

	public float RangeMax => maxTemp;

	public LocString Title => UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.TITLE;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_BELOW;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		base.OnToggle += OnSwitchToggled;
		UpdateVisualState(true);
		wasOn = switchedOn;
	}

	public void Sim200ms(float dt)
	{
		if (simUpdateCounter < 8)
		{
			int i = Grid.PosToCell(this);
			if (Grid.Mass[i] > 0f)
			{
				temperatures[simUpdateCounter] = Grid.Temperature[i];
				simUpdateCounter++;
			}
		}
		else
		{
			simUpdateCounter = 0;
			averageTemp = 0f;
			for (int j = 0; j < 8; j++)
			{
				averageTemp += temperatures[j];
			}
			averageTemp /= 8f;
			if (activateOnWarmerThan)
			{
				if ((averageTemp > thresholdTemperature && !base.IsSwitchedOn) || (averageTemp < thresholdTemperature && base.IsSwitchedOn))
				{
					Toggle();
				}
			}
			else if ((averageTemp > thresholdTemperature && base.IsSwitchedOn) || (averageTemp < thresholdTemperature && !base.IsSwitchedOn))
			{
				Toggle();
			}
		}
	}

	public float GetTemperature()
	{
		return averageTemp;
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateVisualState(false);
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
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

	public float GetRangeMinInputField()
	{
		return GameUtil.GetConvertedTemperature(RangeMin);
	}

	public float GetRangeMaxInputField()
	{
		return GameUtil.GetConvertedTemperature(RangeMax);
	}

	public string Format(float value, bool units)
	{
		bool displayUnits = units;
		return GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, displayUnits);
	}

	public float ProcessedSliderValue(float input)
	{
		return Mathf.Round(input);
	}

	public float ProcessedInputValue(float input)
	{
		return GameUtil.GetTemperatureConvertedToKelvin(input);
	}

	public LocString ThresholdValueUnits()
	{
		LocString result = null;
		switch (GameUtil.temperatureUnit)
		{
		case GameUtil.TemperatureUnit.Celsius:
			result = UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
			break;
		case GameUtil.TemperatureUnit.Fahrenheit:
			result = UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
			break;
		case GameUtil.TemperatureUnit.Kelvin:
			result = UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
			break;
		}
		return result;
	}
}
