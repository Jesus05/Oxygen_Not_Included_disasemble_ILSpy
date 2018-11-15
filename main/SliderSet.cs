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
			component.SetSimpleTooltip(Strings.Get(target.GetSliderTooltipKey(index)));
		}
		unitsLabel.text = target.SliderUnits;
		minLabel.text = target.GetSliderMin(index) + target.SliderUnits;
		maxLabel.text = target.GetSliderMax(index) + target.SliderUnits;
		numberInput.minValue = target.GetSliderMin(index);
		numberInput.maxValue = target.GetSliderMax(index);
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
		SetValue(valueSlider.value);
	}

	private void ReceiveValueFromInput()
	{
		float value = Mathf.Round(numberInput.currentValue * 10f) / 10f;
		valueSlider.value = value;
		SetValue(value);
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
	}

	private void UpdateLabel(float value)
	{
		float num = Mathf.Round(value * 10f) / 10f;
		numberInput.SetDisplayValue(num.ToString());
	}
}
