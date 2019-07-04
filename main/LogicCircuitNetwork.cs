using FMOD.Studio;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LogicCircuitNetwork : UtilityNetwork
{
	private List<LogicWire> wires = new List<LogicWire>();

	private List<ILogicEventReceiver> receivers = new List<ILogicEventReceiver>();

	private List<ILogicEventSender> senders = new List<ILogicEventSender>();

	private int previousValue = -1;

	private int outputValue;

	private bool resetting = false;

	public int OutputValue => outputValue;

	public List<LogicWire> Wires => wires;

	public ReadOnlyCollection<ILogicEventSender> Senders => senders.AsReadOnly();

	public ReadOnlyCollection<ILogicEventReceiver> Receivers => receivers.AsReadOnly();

	public override void AddItem(int cell, object item)
	{
		if (item is LogicWire)
		{
			wires.Add((LogicWire)item);
		}
		else if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver item2 = (ILogicEventReceiver)item;
			receivers.Add(item2);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender item3 = (ILogicEventSender)item;
			senders.Add(item3);
		}
	}

	public override void RemoveItem(int cell, object item)
	{
		if (item is LogicWire)
		{
			wires.Remove((LogicWire)item);
		}
		else if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver item2 = item as ILogicEventReceiver;
			receivers.Remove(item2);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender item3 = (ILogicEventSender)item;
			senders.Remove(item3);
		}
	}

	public override void ConnectItem(int cell, object item)
	{
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = (ILogicEventReceiver)item;
			logicEventReceiver.OnLogicNetworkConnectionChanged(true);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = (ILogicEventSender)item;
			logicEventSender.OnLogicNetworkConnectionChanged(true);
		}
	}

	public override void DisconnectItem(int cell, object item)
	{
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = item as ILogicEventReceiver;
			logicEventReceiver.ReceiveLogicEvent(0);
			logicEventReceiver.OnLogicNetworkConnectionChanged(false);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = item as ILogicEventSender;
			logicEventSender.OnLogicNetworkConnectionChanged(false);
		}
	}

	public override void Reset(UtilityNetworkGridNode[] grid)
	{
		resetting = true;
		previousValue = -1;
		outputValue = 0;
		for (int i = 0; i < wires.Count; i++)
		{
			LogicWire logicWire = wires[i];
			if ((Object)logicWire != (Object)null)
			{
				int num = Grid.PosToCell(logicWire.transform.GetPosition());
				UtilityNetworkGridNode utilityNetworkGridNode = grid[num];
				utilityNetworkGridNode.networkIdx = -1;
				grid[num] = utilityNetworkGridNode;
			}
		}
		wires.Clear();
		senders.Clear();
		receivers.Clear();
		resetting = false;
	}

	public void UpdateLogicValue()
	{
		if (!resetting)
		{
			previousValue = outputValue;
			outputValue = 0;
			foreach (ILogicEventSender sender in senders)
			{
				sender.LogicTick();
			}
			foreach (ILogicEventSender sender2 in senders)
			{
				int logicValue = sender2.GetLogicValue();
				outputValue |= logicValue;
			}
		}
	}

	public void SendLogicEvents(bool force_send)
	{
		if (!resetting && (outputValue != previousValue || force_send))
		{
			foreach (ILogicEventReceiver receiver in receivers)
			{
				receiver.ReceiveLogicEvent(outputValue);
			}
			if (!force_send)
			{
				TriggerAudio((previousValue >= 0) ? previousValue : 0);
			}
		}
	}

	private void TriggerAudio(int old_value)
	{
		SpeedControlScreen instance = SpeedControlScreen.Instance;
		if (old_value != outputValue && (Object)instance != (Object)null && !instance.IsPaused)
		{
			GridArea visibleArea = GridVisibleArea.GetVisibleArea();
			List<LogicWire> list = new List<LogicWire>();
			for (int i = 0; i < wires.Count; i++)
			{
				if (visibleArea.Min <= (Vector2)wires[i].transform.GetPosition() && (Vector2)wires[i].transform.GetPosition() <= visibleArea.Max)
				{
					list.Add(wires[i]);
				}
			}
			if (list.Count > 0)
			{
				int index = Mathf.CeilToInt((float)(list.Count / 2));
				if ((Object)list[index] != (Object)null)
				{
					Vector3 position = list[index].transform.GetPosition();
					EventInstance instance2 = KFMOD.BeginOneShot(GlobalAssets.GetSound("Logic_Circuit_Toggle", false), position);
					instance2.setParameterValue("wireCount", (float)(wires.Count % 24));
					instance2.setParameterValue("enabled", (float)outputValue);
					KFMOD.EndOneShot(instance2);
				}
			}
		}
	}
}
