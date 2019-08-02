using System.Collections.Generic;
using UnityEngine;

public struct StructureTemperaturePayload
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

	public int simHandleCopy;

	public bool enabled;

	public bool bypass;

	public bool isActiveStatusItemSet;

	public bool overrideExtents;

	private PrimaryElement primaryElementBacking;

	public Overheatable overheatable;

	public Building building;

	public Operational operational;

	public List<EnergySource> energySourcesKW;

	public float pendingEnergyModifications;

	public float maxTemperature;

	public Extents overriddenExtents;

	public PrimaryElement primaryElement
	{
		get
		{
			return primaryElementBacking;
		}
		set
		{
			if ((Object)primaryElementBacking != (Object)value)
			{
				primaryElementBacking = value;
				overheatable = primaryElementBacking.GetComponent<Overheatable>();
			}
		}
	}

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

	public StructureTemperaturePayload(GameObject go)
	{
		simHandleCopy = -1;
		enabled = true;
		bypass = false;
		overrideExtents = false;
		overriddenExtents = default(Extents);
		primaryElementBacking = go.GetComponent<PrimaryElement>();
		overheatable = ((!((Object)primaryElementBacking != (Object)null)) ? null : primaryElementBacking.GetComponent<Overheatable>());
		building = go.GetComponent<Building>();
		operational = go.GetComponent<Operational>();
		pendingEnergyModifications = 0f;
		maxTemperature = 10000f;
		energySourcesKW = null;
		isActiveStatusItemSet = false;
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
}
