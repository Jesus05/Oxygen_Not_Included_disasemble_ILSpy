public interface IActivationRangeTarget
{
	float ActivateValue
	{
		get;
		set;
	}

	float DeactivateValue
	{
		get;
		set;
	}

	float MinValue
	{
		get;
	}

	float MaxValue
	{
		get;
	}

	bool UseWholeNumbers
	{
		get;
	}

	string ActivationRangeTitleText
	{
		get;
	}

	string ActivateSliderLabelText
	{
		get;
	}

	string DeactivateSliderLabelText
	{
		get;
	}

	string ActivateTooltip
	{
		get;
	}

	string DeactivateTooltip
	{
		get;
	}
}
