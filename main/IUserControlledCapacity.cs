public interface IUserControlledCapacity
{
	float UserMaxCapacity
	{
		get;
		set;
	}

	float AmountStored
	{
		get;
	}

	float MinCapacity
	{
		get;
	}

	float MaxCapacity
	{
		get;
	}

	bool WholeValues
	{
		get;
	}

	LocString CapacityUnits
	{
		get;
	}
}
