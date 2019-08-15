public interface ISliderControl
{
	string SliderTitleKey
	{
		get;
	}

	string SliderUnits
	{
		get;
	}

	int SliderDecimalPlaces(int index);

	float GetSliderMin(int index);

	float GetSliderMax(int index);

	float GetSliderValue(int index);

	void SetSliderValue(float percent, int index);

	string GetSliderTooltipKey(int index);

	string GetSliderTooltip();
}
