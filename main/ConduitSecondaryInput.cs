using UnityEngine;

public class ConduitSecondaryInput : KMonoBehaviour, ISecondaryInput
{
	[SerializeField]
	public ConduitPortInfo portInfo;

	public ConduitType GetSecondaryConduitType()
	{
		return portInfo.conduitType;
	}

	public CellOffset GetSecondaryConduitOffset()
	{
		return portInfo.offset;
	}
}
