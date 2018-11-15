using System.Collections.Generic;
using UnityEngine;

public struct StructureTemperatureData
{
	public class EnergySource
	{
		public string source;

		public RunningAverage kw_accumulator;

		public float value => kw_accumulator.AverageValue;

		public EnergySource(float kj, string source)
		{
			this.source = source;
			int sampleCount = Mathf.RoundToInt(186f);
			kw_accumulator = new RunningAverage(-3.40282347E+38f, 3.40282347E+38f, sampleCount, true);
		}

		public void Accumulate(float value)
		{
			kw_accumulator.AddSample(value);
		}
	}

	public bool dirty;

	public bool isActiveBuilding;

	public bool enabled;

	public bool isActiveStatusItemSet;

	public bool overrideExtents;

	public int simHandle;

	public PrimaryElement primaryElement;

	public Building building;

	public Operational operational;

	public List<EnergySource> energySourcesKW;

	public float pendingEnergyModifications;

	public float maxTemperature;

	public Extents overriddenExtents;

	public float TotalEnergyProducedKW
	{
		get
		{
			if (energySourcesKW == null || energySourcesKW.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < energySourcesKW.Count; i++)
			{
				num += energySourcesKW[i].value;
			}
			return num;
		}
	}

	public float Temperature => primaryElement.Temperature;

	public float ExhaustKilowatts => building.Def.ExhaustKilowattsWhenActive;

	public float OperatingKilowatts => (!((Object)operational != (Object)null) || !operational.IsActive) ? 0f : building.Def.SelfHeatKilowattsWhenActive;

	public StructureTemperatureData(GameObject go)
	{
		dirty = false;
		isActiveBuilding = false;
		enabled = true;
		overrideExtents = false;
		overriddenExtents = default(Extents);
		simHandle = -1;
		primaryElement = go.GetComponent<PrimaryElement>();
		building = go.GetComponent<Building>();
		operational = go.GetComponent<Operational>();
		pendingEnergyModifications = 0f;
		maxTemperature = 10000f;
		energySourcesKW = null;
		isActiveStatusItemSet = false;
	}

	public void ModifyEnergy(float delta_kilojoules)
	{
		if (Sim.IsValidHandle(simHandle))
		{
			SimMessages.ModifyBuildingEnergy(simHandle, delta_kilojoules, 0f, 10000f);
		}
		else
		{
			pendingEnergyModifications += delta_kilojoules;
			dirty = true;
		}
	}

	public void OverrideExtents(Extents newExtents)
	{
		overrideExtents = true;
		overriddenExtents = newExtents;
	}

	public Extents GetExtents()
	{
		return (!overrideExtents) ? building.GetExtents() : overriddenExtents;
	}

	public void ApplyPendingEnergyModifications()
	{
		if (pendingEnergyModifications != 0f)
		{
			SimMessages.ModifyBuildingEnergy(simHandle, pendingEnergyModifications, 0f, 10000f);
			pendingEnergyModifications = 0f;
		}
	}
}
