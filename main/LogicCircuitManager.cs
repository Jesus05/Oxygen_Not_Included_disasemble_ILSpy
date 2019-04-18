using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LogicCircuitManager
{
	private struct Signal
	{
		public int cell;

		public int value;

		public Signal(int cell, int value)
		{
			this.cell = cell;
			this.value = value;
		}
	}

	public static float ClockTickInterval = 0.1f;

	private float elapsedTime;

	private UtilityNetworkManager<LogicCircuitNetwork, LogicWire> conduitSystem;

	private List<ILogicUIElement> uiVisElements = new List<ILogicUIElement>();

	public Action<ILogicUIElement> onElemAdded;

	public Action<ILogicUIElement> onElemRemoved;

	public LogicCircuitManager(UtilityNetworkManager<LogicCircuitNetwork, LogicWire> conduit_system)
	{
		conduitSystem = conduit_system;
		elapsedTime = 0f;
	}

	public void Sim200ms(float dt)
	{
		Refresh(dt);
	}

	private void Refresh(float dt)
	{
		if (conduitSystem.IsDirty)
		{
			conduitSystem.Update();
			PropagateSignals(true);
			elapsedTime = 0f;
		}
		else if (conduitSystem.GetNetworks().Count > 0 && (UnityEngine.Object)SpeedControlScreen.Instance != (UnityEngine.Object)null && !SpeedControlScreen.Instance.IsPaused)
		{
			elapsedTime += dt;
			while (elapsedTime > ClockTickInterval)
			{
				elapsedTime -= ClockTickInterval;
				PropagateSignals(false);
			}
		}
	}

	private void PropagateSignals(bool force_send_events)
	{
		IList<UtilityNetwork> networks = Game.Instance.logicCircuitSystem.GetNetworks();
		foreach (LogicCircuitNetwork item in networks)
		{
			item.UpdateLogicValue();
		}
		foreach (LogicCircuitNetwork item2 in networks)
		{
			item2.SendLogicEvents(force_send_events);
		}
	}

	public LogicCircuitNetwork GetNetworkForCell(int cell)
	{
		return conduitSystem.GetNetworkForCell(cell) as LogicCircuitNetwork;
	}

	public void AddVisElem(ILogicUIElement elem)
	{
		uiVisElements.Add(elem);
		if (onElemAdded != null)
		{
			onElemAdded(elem);
		}
	}

	public void RemoveVisElem(ILogicUIElement elem)
	{
		if (onElemRemoved != null)
		{
			onElemRemoved(elem);
		}
		uiVisElements.Remove(elem);
	}

	public ReadOnlyCollection<ILogicUIElement> GetVisElements()
	{
		return uiVisElements.AsReadOnly();
	}

	public static void ToggleNoWireConnected(bool show_missing_wire, GameObject go)
	{
		go.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoLogicWireConnected, show_missing_wire, null);
	}
}
