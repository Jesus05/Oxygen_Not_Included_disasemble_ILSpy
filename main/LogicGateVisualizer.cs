using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class LogicGateVisualizer : LogicGateBase
{
	private class IOVisualizer : ILogicUIElement, IUniformGridObject
	{
		private int cell;

		private bool input;

		public IOVisualizer(int cell, bool input)
		{
			this.cell = cell;
			this.input = input;
		}

		public int GetLogicUICell()
		{
			return cell;
		}

		public LogicPortSpriteType GetLogicPortSpriteType()
		{
			return (!input) ? LogicPortSpriteType.Output : LogicPortSpriteType.Input;
		}

		public Vector2 PosMin()
		{
			return Grid.CellToPos2D(cell);
		}

		public Vector2 PosMax()
		{
			return PosMin();
		}
	}

	private List<IOVisualizer> visChildren = new List<IOVisualizer>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Register();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Unregister();
	}

	private void Register()
	{
		Unregister();
		visChildren.Add(new IOVisualizer(base.OutputCell, false));
		visChildren.Add(new IOVisualizer(base.InputCellOne, true));
		if (base.RequiresTwoInputs)
		{
			visChildren.Add(new IOVisualizer(base.InputCellTwo, true));
		}
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		foreach (IOVisualizer visChild in visChildren)
		{
			logicCircuitManager.AddVisElem(visChild);
		}
	}

	private void Unregister()
	{
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		foreach (IOVisualizer visChild in visChildren)
		{
			logicCircuitManager.RemoveVisElem(visChild);
		}
		visChildren.Clear();
	}
}
