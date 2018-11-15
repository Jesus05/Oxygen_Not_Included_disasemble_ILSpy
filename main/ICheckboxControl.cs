public interface ICheckboxControl
{
	string CheckboxTitleKey
	{
		get;
	}

	string CheckboxLabel
	{
		get;
	}

	string CheckboxTooltip
	{
		get;
	}

	bool GetCheckboxValue();

	void SetCheckboxValue(bool value);
}
