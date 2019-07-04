using STRINGS;
using UnityEngine;

public class ThresholdSwitchSideScreen : SideScreenContent, IRender200ms
{
	private GameObject target;

	private IThresholdSwitch thresholdSwitch;

	[SerializeField]
	private LocText currentValue;

	[SerializeField]
	private LocText tresholdValue;

	[SerializeField]
	private KToggle aboveToggle;

	[SerializeField]
	private KToggle belowToggle;

	[Header("Slider")]
	[SerializeField]
	private NonLinearSlider thresholdSlider;

	[Header("Number Input")]
	[SerializeField]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;

	[Header("Increment Buttons")]
	[SerializeField]
	private GameObject incrementMinor;

	[SerializeField]
	private GameObject incrementMajor;

	[SerializeField]
	private GameObject decrementMinor;

	[SerializeField]
	private GameObject decrementMajor;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		aboveToggle.onClick += delegate
		{
			OnConditionButtonClicked(true);
		};
		belowToggle.onClick += delegate
		{
			OnConditionButtonClicked(false);
		};
		LocText component = aboveToggle.transform.GetChild(0).GetComponent<LocText>();
		LocText component2 = belowToggle.transform.GetChild(0).GetComponent<LocText>();
		component.SetText(UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.ABOVE_BUTTON);
		component2.SetText(UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.BELOW_BUTTON);
		thresholdSlider.onDrag += delegate
		{
			ReceiveValueFromSlider(thresholdSlider.GetValueForPercentage(GameUtil.GetRoundedTemperatureInKelvin(thresholdSlider.value)));
		};
		thresholdSlider.onPointerDown += delegate
		{
			ReceiveValueFromSlider(thresholdSlider.GetValueForPercentage(GameUtil.GetRoundedTemperatureInKelvin(thresholdSlider.value)));
		};
		thresholdSlider.onMove += delegate
		{
			ReceiveValueFromSlider(thresholdSlider.GetValueForPercentage(GameUtil.GetRoundedTemperatureInKelvin(thresholdSlider.value)));
		};
		numberInput.onEndEdit += delegate
		{
			ReceiveValueFromInput(numberInput.currentValue);
		};
		numberInput.decimalPlaces = 1;
	}

	public void Render200ms(float dt)
	{
		if ((Object)target == (Object)null)
		{
			target = null;
		}
		else
		{
			UpdateLabels();
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IThresholdSwitch>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		target = null;
		if ((Object)new_target == (Object)null)
		{
			Debug.LogError("Invalid gameObject received");
		}
		else
		{
			target = new_target;
			thresholdSwitch = target.GetComponent<IThresholdSwitch>();
			if (thresholdSwitch == null)
			{
				target = null;
				Debug.LogError("The gameObject received does not contain a IThresholdSwitch component");
			}
			else
			{
				UpdateLabels();
				if (target.GetComponent<IThresholdSwitch>().LayoutType == ThresholdScreenLayoutType.SliderBar)
				{
					thresholdSlider.gameObject.SetActive(true);
					thresholdSlider.minValue = 0f;
					thresholdSlider.maxValue = 100f;
					thresholdSlider.SetRanges(thresholdSwitch.GetRanges);
					thresholdSlider.value = thresholdSlider.GetPercentageFromValue(thresholdSwitch.Threshold);
					thresholdSlider.GetComponentInChildren<ToolTip>();
				}
				else
				{
					thresholdSlider.gameObject.SetActive(false);
				}
				MultiToggle incrementMinorToggle = incrementMinor.GetComponent<MultiToggle>();
				incrementMinorToggle.onClick = delegate
				{
					UpdateThresholdValue(thresholdSwitch.Threshold + (float)thresholdSwitch.IncrementScale);
					incrementMinorToggle.ChangeState(1);
				};
				incrementMinorToggle.onStopHold = delegate
				{
					incrementMinorToggle.ChangeState(0);
				};
				MultiToggle incrementMajorToggle = incrementMajor.GetComponent<MultiToggle>();
				incrementMajorToggle.onClick = delegate
				{
					UpdateThresholdValue(thresholdSwitch.Threshold + 10f * (float)thresholdSwitch.IncrementScale);
					incrementMajorToggle.ChangeState(1);
				};
				incrementMajorToggle.onStopHold = delegate
				{
					incrementMajorToggle.ChangeState(0);
				};
				MultiToggle decrementMinorToggle = decrementMinor.GetComponent<MultiToggle>();
				decrementMinorToggle.onClick = delegate
				{
					UpdateThresholdValue(thresholdSwitch.Threshold - (float)thresholdSwitch.IncrementScale);
					decrementMinorToggle.ChangeState(1);
				};
				decrementMinorToggle.onStopHold = delegate
				{
					decrementMinorToggle.ChangeState(0);
				};
				MultiToggle decrementMajorToggle = decrementMajor.GetComponent<MultiToggle>();
				decrementMajorToggle.onClick = delegate
				{
					UpdateThresholdValue(thresholdSwitch.Threshold - 10f * (float)thresholdSwitch.IncrementScale);
					decrementMajorToggle.ChangeState(1);
				};
				decrementMajorToggle.onStopHold = delegate
				{
					decrementMajorToggle.ChangeState(0);
				};
				unitsLabel.text = thresholdSwitch.ThresholdValueUnits();
				numberInput.minValue = thresholdSwitch.GetRangeMinInputField();
				numberInput.maxValue = thresholdSwitch.GetRangeMaxInputField();
				numberInput.Activate();
				UpdateTargetThresholdLabel();
				OnConditionButtonClicked(thresholdSwitch.ActivateAboveThreshold);
			}
		}
	}

	private void OnThresholdValueChanged(float new_value)
	{
		thresholdSwitch.Threshold = new_value;
		UpdateTargetThresholdLabel();
	}

	private void OnConditionButtonClicked(bool activate_above_threshold)
	{
		thresholdSwitch.ActivateAboveThreshold = activate_above_threshold;
		if (activate_above_threshold)
		{
			belowToggle.isOn = true;
			aboveToggle.isOn = false;
			belowToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			aboveToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		}
		else
		{
			belowToggle.isOn = false;
			aboveToggle.isOn = true;
			belowToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
			aboveToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
		}
		UpdateTargetThresholdLabel();
	}

	private void UpdateTargetThresholdLabel()
	{
		numberInput.SetDisplayValue(thresholdSwitch.Format(thresholdSwitch.Threshold, false));
		if (thresholdSwitch.ActivateAboveThreshold)
		{
			thresholdSlider.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(thresholdSwitch.AboveToolTip, thresholdSwitch.Format(thresholdSwitch.Threshold, true)));
			thresholdSlider.GetComponentInChildren<ToolTip>().tooltipPositionOffset = new Vector2(0f, 25f);
		}
		else
		{
			thresholdSlider.GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(thresholdSwitch.BelowToolTip, thresholdSwitch.Format(thresholdSwitch.Threshold, true)));
			thresholdSlider.GetComponentInChildren<ToolTip>().tooltipPositionOffset = new Vector2(0f, 25f);
		}
	}

	private void ReceiveValueFromSlider(float newValue)
	{
		UpdateThresholdValue(thresholdSwitch.ProcessedSliderValue(newValue));
	}

	private void ReceiveValueFromInput(float newValue)
	{
		UpdateThresholdValue(thresholdSwitch.ProcessedInputValue(newValue));
	}

	private void UpdateThresholdValue(float newValue)
	{
		if (newValue < thresholdSwitch.RangeMin)
		{
			newValue = thresholdSwitch.RangeMin;
		}
		if (newValue > thresholdSwitch.RangeMax)
		{
			newValue = thresholdSwitch.RangeMax;
		}
		thresholdSwitch.Threshold = newValue;
		NonLinearSlider nonLinearSlider = thresholdSlider;
		if ((Object)nonLinearSlider != (Object)null)
		{
			thresholdSlider.value = nonLinearSlider.GetPercentageFromValue(newValue);
		}
		else
		{
			thresholdSlider.value = newValue;
		}
		UpdateTargetThresholdLabel();
	}

	private void UpdateLabels()
	{
		currentValue.text = string.Format(UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.CURRENT_VALUE, thresholdSwitch.ThresholdValueName, thresholdSwitch.Format(thresholdSwitch.CurrentValue, true));
	}

	public override string GetTitle()
	{
		if (!((Object)target != (Object)null))
		{
			return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;
		}
		return thresholdSwitch.Title;
	}
}
