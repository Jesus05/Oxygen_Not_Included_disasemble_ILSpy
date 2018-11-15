using UnityEngine;

public interface IApproachable
{
	Transform transform
	{
		get;
	}

	CellOffset[] GetOffsets();

	int GetCell();
}
