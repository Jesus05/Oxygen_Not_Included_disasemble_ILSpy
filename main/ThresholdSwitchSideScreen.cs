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
	private KSlider thresholdSlider;

	[Header("Number Input")]
	[SerializeField]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;

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
			ReceiveValueFromSlider(thresholdSlider.value);
		};
		thresholdSlider.onPointerDown += delegate
		{
			ReceiveValueFromSlider(thresholdSlider.value);
		};
		thresholdSlider.onMove += delegate
		{
			ReceiveValueFromSlider(thresholdSlider.value);
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
			Debug.LogError("Invalid gameObject received", null);
		}
		else
		{
			target = new_target;
			thresholdSwitch = target.GetComponent<IThresholdSwitch>();
			if (thresholdSwitch == null)
			{
				target = null;
				Debug.LogError("The gameObject received does not contain a IThresholdSwitch component", null);
			}
			else
			{
				UpdateLabels();
				thresholdSlider.minValue = thresholdSwitch.RangeMin;
				thresholdSlider.maxValue = thresholdSwitch.RangeMax;
				thresholdSlider.value = thresholdSwitch.Threshold;
				thresholdSlider.GetComponentInChildren<ToolTip>();
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
		thresholdSwitch.Threshold = newValue;
		thresholdSlider.value = newValue;
		UpdateTargetThresholdLabel();
	}

	private void UpdateLabels()
	{
		currentValue.text = string.Format(UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.CURRENT_VALUE, thresholdSwitch.ThresholdValueName, thresholdSwitch.Format(thresholdSwitch.CurrentValue, true));
	}

	public override string GetTitle()
	{
		if ((Object)target != (Object)null)
		{
			return thresholdSwitch.Title;
		}
		return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;
	}
}
