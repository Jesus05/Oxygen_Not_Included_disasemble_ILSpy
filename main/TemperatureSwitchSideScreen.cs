using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureSwitchSideScreen : SideScreenContent, IRender200ms
{
	private TemperatureControlledSwitch targetTemperatureSwitch;

	[SerializeField]
	private LocText currentTemperature;

	[SerializeField]
	private LocText targetTemperature;

	[SerializeField]
	private KToggle coolerToggle;

	[SerializeField]
	private KToggle warmerToggle;

	[SerializeField]
	private KSlider targetTemperatureSlider;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		coolerToggle.onClick += delegate
		{
			OnConditionButtonClicked(false);
		};
		warmerToggle.onClick += delegate
		{
			OnConditionButtonClicked(true);
		};
		LocText component = coolerToggle.transform.GetChild(0).GetComponent<LocText>();
		LocText component2 = warmerToggle.transform.GetChild(0).GetComponent<LocText>();
		component.SetText(UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.COLDER_BUTTON);
		component2.SetText(UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.WARMER_BUTTON);
		Slider.SliderEvent sliderEvent = new Slider.SliderEvent();
		sliderEvent.AddListener(OnTargetTemperatureChanged);
		targetTemperatureSlider.onValueChanged = sliderEvent;
	}

	public void Render200ms(float dt)
	{
		if (!((Object)targetTemperatureSwitch == (Object)null))
		{
			UpdateLabels();
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<TemperatureControlledSwitch>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		if ((Object)target == (Object)null)
		{
			Debug.LogError("Invalid gameObject received", null);
		}
		else
		{
			targetTemperatureSwitch = target.GetComponent<TemperatureControlledSwitch>();
			if ((Object)targetTemperatureSwitch == (Object)null)
			{
				Debug.LogError("The gameObject received does not contain a TimedSwitch component", null);
			}
			else
			{
				UpdateLabels();
				UpdateTargetTemperatureLabel();
				OnConditionButtonClicked(targetTemperatureSwitch.activateOnWarmerThan);
			}
		}
	}

	private void OnTargetTemperatureChanged(float new_value)
	{
		targetTemperatureSwitch.thresholdTemperature = new_value;
		UpdateTargetTemperatureLabel();
	}

	private void OnConditionButtonClicked(bool isWarmer)
	{
		targetTemperatureSwitch.activateOnWarmerThan = isWarmer;
		if (isWarmer)
		{
			coolerToggle.isOn = false;
			warmerToggle.isOn = true;
			coolerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			warmerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		}
		else
		{
			coolerToggle.isOn = true;
			warmerToggle.isOn = false;
			coolerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
			warmerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
		}
	}

	private void UpdateTargetTemperatureLabel()
	{
		targetTemperature.text = GameUtil.GetFormattedTemperature(targetTemperatureSwitch.thresholdTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
	}

	private void UpdateLabels()
	{
		currentTemperature.text = string.Format(UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.CURRENT_TEMPERATURE, GameUtil.GetFormattedTemperature(targetTemperatureSwitch.GetTemperature(), GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
	}
}
