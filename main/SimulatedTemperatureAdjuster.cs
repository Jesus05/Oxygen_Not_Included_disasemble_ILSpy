using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedTemperatureAdjuster
{
	private float temperature;

	private float heatCapacity;

	private float thermalConductivity;

	private bool operational;

	private Storage storage;

	public SimulatedTemperatureAdjuster(float simulated_temperature, float heat_capacity, float thermal_conductivity, Storage storage)
	{
		temperature = simulated_temperature;
		heatCapacity = heat_capacity;
		thermalConductivity = thermal_conductivity;
		this.storage = storage;
		storage.gameObject.Subscribe(-592767678, OnOperationalChanged);
		storage.gameObject.Subscribe(-1697596308, OnStorageChanged);
		operational = true;
		Operational component = storage.gameObject.GetComponent<Operational>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			operational = component.IsOperational;
		}
		OnOperationalChanged(operational);
	}

	public List<Descriptor> GetDescriptors()
	{
		return GetDescriptors(temperature);
	}

	public static List<Descriptor> GetDescriptors(float temperature)
	{
		List<Descriptor> list = new List<Descriptor>();
		string formattedTemperature = GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false);
		Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.ITEM_TEMPERATURE_ADJUST, formattedTemperature), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ITEM_TEMPERATURE_ADJUST, formattedTemperature), Descriptor.DescriptorType.Effect, false);
		list.Add(item);
		return list;
	}

	private void Register(SimTemperatureTransfer stt)
	{
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Remove(stt.onSimRegistered, new Action<SimTemperatureTransfer>(OnItemSimRegistered));
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Combine(stt.onSimRegistered, new Action<SimTemperatureTransfer>(OnItemSimRegistered));
		if (Sim.IsValidHandle(stt.SimHandle))
		{
			OnItemSimRegistered(stt);
		}
	}

	private void Unregister(SimTemperatureTransfer stt)
	{
		stt.onSimRegistered = (Action<SimTemperatureTransfer>)Delegate.Remove(stt.onSimRegistered, new Action<SimTemperatureTransfer>(OnItemSimRegistered));
		if (Sim.IsValidHandle(stt.SimHandle))
		{
			SimMessages.ModifyElementChunkTemperatureAdjuster(stt.SimHandle, 0f, 0f, 0f);
		}
	}

	private void OnItemSimRegistered(SimTemperatureTransfer stt)
	{
		if (!((UnityEngine.Object)stt == (UnityEngine.Object)null) && Sim.IsValidHandle(stt.SimHandle))
		{
			float num = temperature;
			float heat_capacity = heatCapacity;
			float thermal_conductivity = thermalConductivity;
			if (!operational)
			{
				num = 0f;
				heat_capacity = 0f;
				thermal_conductivity = 0f;
			}
			SimMessages.ModifyElementChunkTemperatureAdjuster(stt.SimHandle, num, heat_capacity, thermal_conductivity);
		}
	}

	private void OnOperationalChanged(object data)
	{
		operational = (bool)data;
		if (operational)
		{
			foreach (GameObject item in storage.items)
			{
				if ((UnityEngine.Object)item != (UnityEngine.Object)null)
				{
					SimTemperatureTransfer component = item.GetComponent<SimTemperatureTransfer>();
					OnItemSimRegistered(component);
				}
			}
		}
		else
		{
			foreach (GameObject item2 in storage.items)
			{
				if ((UnityEngine.Object)item2 != (UnityEngine.Object)null)
				{
					SimTemperatureTransfer component2 = item2.GetComponent<SimTemperatureTransfer>();
					Unregister(component2);
				}
			}
		}
	}

	public void CleanUp()
	{
		storage.gameObject.Unsubscribe(-1697596308, OnStorageChanged);
		foreach (GameObject item in storage.items)
		{
			if ((UnityEngine.Object)item != (UnityEngine.Object)null)
			{
				SimTemperatureTransfer component = item.GetComponent<SimTemperatureTransfer>();
				Unregister(component);
			}
		}
	}

	private void OnStorageChanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		SimTemperatureTransfer component = gameObject.GetComponent<SimTemperatureTransfer>();
		if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
		{
			Pickupable component2 = gameObject.GetComponent<Pickupable>();
			if (!((UnityEngine.Object)component2 == (UnityEngine.Object)null))
			{
				if (operational && (UnityEngine.Object)component2.storage == (UnityEngine.Object)storage)
				{
					Register(component);
				}
				else
				{
					Unregister(component);
				}
			}
		}
	}
}
