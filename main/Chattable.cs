using UnityEngine;

public class Chattable : KMonoBehaviour, IApproachable
{
	public CellOffset[] GetOffsets()
	{
		return OffsetGroups.Chat;
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	Transform get_transform()
	{
		return base.transform;
	}

	Transform IApproachable.get_transform()
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_transform
		return this.get_transform();
	}
}
