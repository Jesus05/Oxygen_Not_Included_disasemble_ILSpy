using System.Collections.Generic;

public interface IFetchList
{
	Storage Destination
	{
		get;
	}

	float GetMinimumAmount(Tag tag);

	Dictionary<Tag, float> GetRemaining();

	Dictionary<Tag, float> GetRemainingMinimum();
}
