public interface IThresholdSwitch
{
	float Threshold
	{
		get;
		set;
	}

	bool ActivateAboveThreshold
	{
		get;
		set;
	}

	float CurrentValue
	{
		get;
	}

	float RangeMin
	{
		get;
	}

	float RangeMax
	{
		get;
	}

	LocString Title
	{
		get;
	}

	LocString ThresholdValueName
	{
		get;
	}

	string AboveToolTip
	{
		get;
	}

	string BelowToolTip
	{
		get;
	}

	ThresholdScreenLayoutType LayoutType
	{
		get;
	}

	int IncrementScale
	{
		get;
	}

	float GetRangeMinInputField();

	float GetRangeMaxInputField();

	LocString ThresholdValueUnits();

	string Format(float value, bool units);

	float ProcessedSliderValue(float input);

	float ProcessedInputValue(float input);
}
