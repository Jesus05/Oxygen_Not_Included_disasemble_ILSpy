using UnityEngine;

public interface IUsable
{
	Transform transform
	{
		get;
	}

	bool IsUsable();
}
