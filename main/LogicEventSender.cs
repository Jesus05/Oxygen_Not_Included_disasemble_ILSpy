using System;
using UnityEngine;

internal class LogicEventSender : ILogicEventSender, ILogicUIElement, ILogicNetworkConnection, IUniformGridObject
{
	private HashedString id;

	private int cell;

	private int logicValue;

	private Action<int> onValueChanged;

	private Action<int, bool> onConnectionChanged;

	private LogicPortSpriteType spriteType;

	public HashedString ID => id;

	public LogicEventSender(HashedString id, int cell, Action<int> on_value_changed, Action<int, bool> on_connection_changed, LogicPortSpriteType sprite_type)
	{
		this.id = id;
		this.cell = cell;
		onValueChanged = on_value_changed;
		onConnectionChanged = on_connection_changed;
		spriteType = sprite_type;
	}

	public int GetLogicCell()
	{
		return cell;
	}

	public int GetLogicValue()
	{
		return logicValue;
	}

	public int GetLogicUICell()
	{
		return GetLogicCell();
	}

	public LogicPortSpriteType GetLogicPortSpriteType()
	{
		return spriteType;
	}

	public Vector2 PosMin()
	{
		return Grid.CellToPos2D(cell);
	}

	public Vector2 PosMax()
	{
		return Grid.CellToPos2D(cell);
	}

	public void SetValue(int value)
	{
		logicValue = value;
		onValueChanged(value);
	}

	public void LogicTick()
	{
	}

	public void OnLogicNetworkConnectionChanged(bool connected)
	{
		if (onConnectionChanged != null)
		{
			onConnectionChanged(cell, connected);
		}
	}
}
