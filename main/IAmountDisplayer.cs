using Klei.AI;

public interface IAmountDisplayer
{
	IAttributeFormatter Formatter
	{
		get;
	}

	string GetValueString(Amount master, AmountInstance instance);

	string GetDescription(Amount master, AmountInstance instance);

	string GetTooltip(Amount master, AmountInstance instance);
}
