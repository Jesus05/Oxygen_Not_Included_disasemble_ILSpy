using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DisinfectThresholdDiagram : MonoBehaviour
{
	[SerializeField]
	private KNumberInputField inputField;

	[SerializeField]
	private KSlider slider;

	[SerializeField]
	private LocText minLabel;

	[SerializeField]
	private LocText maxLabel;

	[SerializeField]
	private LocText unitsLabel;

	[SerializeField]
	private LocText thresholdPrefix;

	[SerializeField]
	private ToolTip toolTip;

	[SerializeField]
	private KToggle toggle;

	[SerializeField]
	private Image disabledImage;

	private static int MAX_VALUE = 1000000;

	private static int SLIDER_CONVERSION = 1000;

	private void Start()
	{
		inputField.minValue = 0f;
		inputField.maxValue = (float)MAX_VALUE;
		inputField.currentValue = (float)SaveGame.Instance.minGermCountForDisinfect;
		inputField.SetDisplayValue(SaveGame.Instance.minGermCountForDisinfect.ToString());
		inputField.onEndEdit += delegate
		{
			ReceiveValueFromInput(inputField.currentValue);
		};
		inputField.decimalPlaces = 1;
		inputField.Activate();
		slider.minValue = 0f;
		slider.maxValue = (float)(MAX_VALUE / SLIDER_CONVERSION);
		slider.wholeNumbers = true;
		slider.value = (float)(SaveGame.Instance.minGermCountForDisinfect / SLIDER_CONVERSION);
		slider.onReleaseHandle += OnReleaseHandle;
		slider.onDrag += delegate
		{
			ReceiveValueFromSlider(slider.value);
		};
		slider.onPointerDown += delegate
		{
			ReceiveValueFromSlider(slider.value);
		};
		slider.onMove += delegate
		{
			ReceiveValueFromSlider(slider.value);
			OnReleaseHandle();
		};
		unitsLabel.SetText(UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.UNITS);
		minLabel.SetText(UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.MIN_LABEL);
		maxLabel.SetText(UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.MAX_LABEL);
		thresholdPrefix.SetText(UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.THRESHOLD_PREFIX);
		toolTip.OnToolTip = delegate
		{
			toolTip.ClearMultiStringTooltip();
			if (SaveGame.Instance.enableAutoDisinfect)
			{
				toolTip.AddMultiStringTooltip(UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.TOOLTIP.ToString().Replace("{NumberOfGerms}", SaveGame.Instance.minGermCountForDisinfect.ToString()), null);
			}
			else
			{
				toolTip.AddMultiStringTooltip(UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.TOOLTIP_DISABLED.ToString(), null);
			}
			return string.Empty;
		};
		disabledImage.gameObject.SetActive(!SaveGame.Instance.enableAutoDisinfect);
		toggle.isOn = SaveGame.Instance.enableAutoDisinfect;
		toggle.onValueChanged += OnClickToggle;
	}

	private void OnReleaseHandle()
	{
		float num = (float)((int)slider.value * SLIDER_CONVERSION);
		SaveGame.Instance.minGermCountForDisinfect = (int)num;
		inputField.SetDisplayValue(num.ToString());
	}

	private void ReceiveValueFromSlider(float new_value)
	{
		SaveGame.Instance.minGermCountForDisinfect = (int)new_value * SLIDER_CONVERSION;
		inputField.SetDisplayValue((new_value * (float)SLIDER_CONVERSION).ToString());
	}

	private void ReceiveValueFromInput(float new_value)
	{
		slider.value = new_value / (float)SLIDER_CONVERSION;
		SaveGame.Instance.minGermCountForDisinfect = (int)new_value;
	}

	private void OnClickToggle(bool new_value)
	{
		SaveGame.Instance.enableAutoDisinfect = new_value;
		disabledImage.gameObject.SetActive(!SaveGame.Instance.enableAutoDisinfect);
	}
}
