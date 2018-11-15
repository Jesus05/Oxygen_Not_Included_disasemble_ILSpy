using System.Collections.Generic;
using UnityEngine;

public interface IBuildQueueOrder
{
	Tag Result
	{
		get;
	}

	Sprite Icon
	{
		get;
	}

	Color IconColor
	{
		get;
	}

	bool Infinite
	{
		get;
	}

	Dictionary<Tag, float> CheckMaterialRequirements();

	Dictionary<Tag, float> GetMaterialRequirements();
}
