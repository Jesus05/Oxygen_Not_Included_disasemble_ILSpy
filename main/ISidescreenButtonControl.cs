public interface ISidescreenButtonControl
{
	string SidescreenTitleKey
	{
		get;
	}

	string SidescreenStatusMessage
	{
		get;
	}

	string SidescreenButtonText
	{
		get;
	}

	void OnSidescreenButtonPressed();
}
