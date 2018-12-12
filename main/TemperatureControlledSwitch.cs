using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class TemperatureControlledSwitch : CircuitSwitch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	private HandleVector<int>.Handle structureTemperature;

	private int simUpdateCounter = 0;

	[Serialize]
	public float thresholdTemperature = 280f;

	[Serialize]
	public bool activateOnWarmerThan;

	public float minTemp = 0f;

	public float maxTemp = 373.15f;

	private const int NumFrameDelay = 8;

	private float[] temperatures = new float[8];

	private float averageTemp;

	public float StructureTemperature => GameComps.StructureTemperatures.GetPayload(structureTemperature).Temperature;

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

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.InputField;

	public int IncrementScale => 1;

	public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(RangeMax);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
	}

	public void Sim200ms(float dt)
	{
		if (simUpdateCounter < 8)
		{
			temperatures[simUpdateCounter] = Grid.Temperature[Grid.PosToCell(this)];
			simUpdateCounter++;
		}
		else
		{
			simUpdateCounter = 0;
			averageTemp = 0f;
			for (int i = 0; i < 8; i++)
			{
				averageTemp += temperatures[i];
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

	public float GetRangeMinInputField()
	{
		return GameUtil.GetConvertedTemperature(RangeMin, false);
	}

	public float GetRangeMaxInputField()
	{
		return GameUtil.GetConvertedTemperature(RangeMax, false);
	}

	public string Format(float value, bool units)
	{
		bool displayUnits = units;
		return GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, displayUnits, false);
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
