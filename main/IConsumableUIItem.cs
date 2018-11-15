public interface IConsumableUIItem
{
	string ConsumableId
	{
		get;
	}

	string ConsumableName
	{
		get;
	}

	int MajorOrder
	{
		get;
	}

	int MinorOrder
	{
		get;
	}

	bool Display
	{
		get;
	}
}
