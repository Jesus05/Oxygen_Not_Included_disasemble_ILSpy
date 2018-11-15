using UnityEngine;

public class LogicPortVisualizer : ILogicUIElement, IUniformGridObject
{
	private int cell;

	private LogicPortSpriteType spriteType;

	public LogicPortVisualizer(int cell, LogicPortSpriteType sprite_type)
	{
		this.cell = cell;
		spriteType = sprite_type;
	}

	public int GetLogicUICell()
	{
		return cell;
	}

	public Vector2 PosMin()
	{
		return Grid.CellToPos2D(cell);
	}

	public Vector2 PosMax()
	{
		return Grid.CellToPos2D(cell);
	}

	public LogicPortSpriteType GetLogicPortSpriteType()
	{
		return spriteType;
	}
}
