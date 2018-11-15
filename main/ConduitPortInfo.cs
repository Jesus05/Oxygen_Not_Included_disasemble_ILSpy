using System;

[Serializable]
public class ConduitPortInfo
{
	public ConduitType conduitType;

	public CellOffset offset;

	public ConduitPortInfo(ConduitType type, CellOffset offset)
	{
		conduitType = type;
		this.offset = offset;
	}
}
