using System;
using UnityEngine;

[Serializable]
public class SliderSet
{
	public KSlider valueSlider;

	public KNumberInputField numberInput;

	public LocText unitsLabel;

	public LocText minLabel;

	public LocText maxLabel;

	[NonSerialized]
	public int index;

	private ISliderControl target;

	public void SetupSlider(int index)
	{
		this.index = index;
		valueSlider.onReleaseHandle += delegate
		{
			valueSlider.value = Mathf.Round(valueSlider.value * 10f) / 10f;
			ReceiveValueFromSlider();
		};
		valueSlider.onDrag += delegate
		{
			ReceiveValueFromSlider();
		};
		valueSlider.onMove += delegate
		{
			ReceiveValueFromSlider();
		};
		valueSlider.onPointerDown += delegate
		{
			ReceiveValueFromSlider();
		};
		numberInput.onEndEdit += delegate
		{
			ReceiveValueFromInput();
		};
	}

	public void SetTarget(ISliderControl target)
	{
		this.target = target;
		ToolTip component = valueSlider.handleRect.GetComponent<ToolTip>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetSimpleTooltip(target.GetSliderTooltip());
		}
		unitsLabel.text = target.SliderUnits;
		minLabel.text = target.GetSliderMin(index) + target.SliderUnits;
		maxLabel.text = target.GetSliderMax(index) + target.SliderUnits;
		numberInput.minValue = target.GetSliderMin(index);
		numberInput.maxValue = target.GetSliderMax(index);
		numberInput.decimalPlaces = target.SliderDecimalPlaces(index);
		valueSlider.minValue = target.GetSliderMin(index);
		valueSlider.maxValue = target.GetSliderMax(index);
		valueSlider.value = target.GetSliderValue(index);
		SetValue(target.GetSliderValue(index));
		if (index == 0)
		{
			numberInput.Activate();
		}
	}

	private void ReceiveValueFromSlider()
	{
		float num = valueSlider.value;
		if (numberInput.decimalPlaces != -1)
		{
			float num2 = Mathf.Pow(10f, (float)numberInput.decimalPlaces);
			num = Mathf.Round(num * num2) / num2;
		}
		SetValue(num);
	}

	private void ReceiveValueFromInput()
	{
		float num = numberInput.currentValue;
		if (numberInput.decimalPlaces != -1)
		{
			float num2 = Mathf.Pow(10f, (float)numberInput.decimalPlaces);
			num = Mathf.Round(num * num2) / num2;
		}
		valueSlider.value = num;
		SetValue(num);
	}

	private void SetValue(float value)
	{
		float num = value;
		if (num > target.GetSliderMax(index))
		{
			num = target.GetSliderMax(index);
		}
		else if (num < target.GetSliderMin(index))
		{
			num = target.GetSliderMin(index);
		}
		UpdateLabel(num);
		target.SetSliderValue(num, index);
		ToolTip component = valueSlider.handleRect.GetComponent<ToolTip>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.SetSimpleTooltip(target.GetSliderTooltip());
		}
	}

	private void UpdateLabel(float value)
	{
		float num = Mathf.Round(value * 10f) / 10f;
		numberInput.SetDisplayValue(num.ToString());
	}
}
