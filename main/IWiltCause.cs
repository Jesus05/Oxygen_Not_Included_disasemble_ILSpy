public interface IWiltCause
{
	string WiltStateString
	{
		get;
	}

	WiltCondition.Condition[] Conditions
	{
		get;
	}
}
