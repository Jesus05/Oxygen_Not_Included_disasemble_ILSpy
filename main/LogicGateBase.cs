using UnityEngine;

public class LogicGateBase : KMonoBehaviour
{
	public enum PortId
	{
		InputOne,
		InputTwo,
		Output
	}

	public enum Op
	{
		And,
		Or,
		Not,
		Xor,
		CustomSingle
	}

	public static LogicModeUI uiSrcData;

	[SerializeField]
	public Op op;

	public static CellOffset[] portOffsets = new CellOffset[3]
	{
		CellOffset.none,
		new CellOffset(0, 1),
		new CellOffset(1, 0)
	};

	public int InputCellOne => GetActualCell(portOffsets[0]);

	public int InputCellTwo => GetActualCell(portOffsets[1]);

	public int OutputCell => GetActualCell(portOffsets[2]);

	public bool RequiresTwoInputs => OpRequiresTwoInputs(op);

	private int GetActualCell(CellOffset offset)
	{
		Rotatable component = GetComponent<Rotatable>();
		if ((Object)component != (Object)null)
		{
			offset = component.GetRotatedCellOffset(offset);
		}
		int cell = Grid.PosToCell(base.transform.GetPosition());
		return Grid.OffsetCell(cell, offset);
	}

	public int PortCell(PortId port)
	{
		switch (port)
		{
		case PortId.InputOne:
			return InputCellOne;
		case PortId.InputTwo:
			return InputCellTwo;
		default:
			return OutputCell;
		}
	}

	public bool TryGetPortAtCell(int cell, out PortId port)
	{
		if (cell != InputCellOne)
		{
			if (cell == InputCellTwo && RequiresTwoInputs)
			{
				port = PortId.InputTwo;
				return true;
			}
			if (cell != OutputCell)
			{
				port = PortId.InputOne;
				return false;
			}
			port = PortId.Output;
			return true;
		}
		port = PortId.InputOne;
		return true;
	}

	public static bool OpRequiresTwoInputs(Op op)
	{
		if (op != Op.Not && op != Op.CustomSingle)
		{
			return true;
		}
		return false;
	}
}
