public interface IEnergyConsumer
{
	float WattsUsed
	{
		get;
	}

	float WattsNeededWhenActive
	{
		get;
	}

	int PowerSortOrder
	{
		get;
	}

	string Name
	{
		get;
	}

	int PowerCell
	{
		get;
	}

	bool IsConnected
	{
		get;
	}

	bool IsPowered
	{
		get;
	}

	void SetConnectionStatus(CircuitManager.ConnectionStatus status);
}
