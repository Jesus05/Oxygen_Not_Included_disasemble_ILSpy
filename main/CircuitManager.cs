using STRINGS;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class CircuitManager
{
	private struct CircuitInfo
	{
		public List<Generator> generators;

		public List<IEnergyConsumer> consumers;

		public List<Battery> batteries;

		public List<Battery> inputTransformers;

		public List<Generator> outputTransformers;

		public List<WireUtilityNetworkLink>[] bridgeGroups;

		public float minBatteryPercentFull;

		public float wattsUsed;
	}

	public enum ConnectionStatus
	{
		NotConnected,
		Unpowered,
		Powered
	}

	public const ushort INVALID_ID = ushort.MaxValue;

	private const int SimUpdateSortKey = 1000;

	private const float MIN_POWERED_THRESHOLD = 0.01f;

	private bool dirty = true;

	private HashSet<Generator> generators = new HashSet<Generator>();

	private HashSet<IEnergyConsumer> consumers = new HashSet<IEnergyConsumer>();

	private HashSet<WireUtilityNetworkLink> bridges = new HashSet<WireUtilityNetworkLink>();

	private float elapsedTime;

	private List<CircuitInfo> circuitInfo = new List<CircuitInfo>();

	private List<IEnergyConsumer> consumersShadow = new List<IEnergyConsumer>();

	private List<Generator> activeGenerators = new List<Generator>();

	public void Connect(Generator generator)
	{
		if (!Game.IsQuitting())
		{
			generators.Add(generator);
			dirty = true;
		}
	}

	public void Disconnect(Generator generator)
	{
		if (!Game.IsQuitting())
		{
			generators.Remove(generator);
			dirty = true;
		}
	}

	public void Connect(IEnergyConsumer consumer)
	{
		if (!Game.IsQuitting())
		{
			consumers.Add(consumer);
			dirty = true;
		}
	}

	public void Disconnect(IEnergyConsumer consumer)
	{
		if (!Game.IsQuitting())
		{
			consumers.Remove(consumer);
			dirty = true;
		}
	}

	public void Connect(WireUtilityNetworkLink bridge)
	{
		bridges.Add(bridge);
		dirty = true;
	}

	public void Disconnect(WireUtilityNetworkLink bridge)
	{
		bridges.Remove(bridge);
		dirty = true;
	}

	public float GetPowerDraw(ushort circuitID, Generator generator)
	{
		float result = 0f;
		if (circuitID < circuitInfo.Count)
		{
			CircuitInfo value = circuitInfo[circuitID];
			circuitInfo[circuitID] = value;
			circuitInfo[circuitID] = value;
		}
		return result;
	}

	public ushort GetCircuitID(int cell)
	{
		return (ushort)(Game.Instance.electricalConduitSystem.GetNetworkForCell(cell)?.id ?? 65535);
	}

	public void Sim200msFirst(float dt)
	{
		Refresh(dt);
	}

	public void RenderEveryTick(float dt)
	{
		Refresh(dt);
	}

	private void Refresh(float dt)
	{
		UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem = Game.Instance.electricalConduitSystem;
		if (electricalConduitSystem.IsDirty || dirty)
		{
			electricalConduitSystem.Update();
			IList<UtilityNetwork> networks = electricalConduitSystem.GetNetworks();
			while (this.circuitInfo.Count < networks.Count)
			{
				CircuitInfo circuitInfo = default(CircuitInfo);
				circuitInfo.generators = new List<Generator>();
				circuitInfo.consumers = new List<IEnergyConsumer>();
				circuitInfo.batteries = new List<Battery>();
				circuitInfo.inputTransformers = new List<Battery>();
				circuitInfo.outputTransformers = new List<Generator>();
				CircuitInfo item = circuitInfo;
				item.bridgeGroups = new List<WireUtilityNetworkLink>[5];
				for (int i = 0; i < item.bridgeGroups.Length; i++)
				{
					item.bridgeGroups[i] = new List<WireUtilityNetworkLink>();
				}
				this.circuitInfo.Add(item);
			}
			Rebuild();
		}
	}

	public void Rebuild()
	{
		for (int i = 0; i < this.circuitInfo.Count; i++)
		{
			CircuitInfo value = this.circuitInfo[i];
			value.generators.Clear();
			value.consumers.Clear();
			value.batteries.Clear();
			value.inputTransformers.Clear();
			value.outputTransformers.Clear();
			value.minBatteryPercentFull = 1f;
			for (int j = 0; j < value.bridgeGroups.Length; j++)
			{
				value.bridgeGroups[j].Clear();
			}
			this.circuitInfo[i] = value;
		}
		consumersShadow.AddRange(consumers);
		List<IEnergyConsumer>.Enumerator enumerator = consumersShadow.GetEnumerator();
		while (enumerator.MoveNext())
		{
			IEnergyConsumer current = enumerator.Current;
			int powerCell = current.PowerCell;
			ushort circuitID = GetCircuitID(powerCell);
			if (circuitID != 65535)
			{
				Battery battery = current as Battery;
				if ((Object)battery != (Object)null)
				{
					Operational component = battery.GetComponent<Operational>();
					if ((Object)component == (Object)null || component.IsOperational)
					{
						CircuitInfo value2 = this.circuitInfo[circuitID];
						PowerTransformer powerTransformer = battery.powerTransformer;
						if ((Object)powerTransformer != (Object)null)
						{
							value2.inputTransformers.Add(battery);
						}
						else
						{
							value2.batteries.Add(battery);
							CircuitInfo circuitInfo = this.circuitInfo[circuitID];
							value2.minBatteryPercentFull = Mathf.Min(circuitInfo.minBatteryPercentFull, battery.PercentFull);
						}
						this.circuitInfo[circuitID] = value2;
					}
				}
				else
				{
					CircuitInfo circuitInfo2 = this.circuitInfo[circuitID];
					circuitInfo2.consumers.Add(current);
				}
			}
		}
		consumersShadow.Clear();
		HashSet<Generator>.Enumerator enumerator2 = generators.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			Generator current2 = enumerator2.Current;
			int powerCell2 = current2.PowerCell;
			ushort circuitID2 = GetCircuitID(powerCell2);
			if (circuitID2 != 65535)
			{
				if (current2.GetType() == typeof(PowerTransformer))
				{
					CircuitInfo circuitInfo3 = this.circuitInfo[circuitID2];
					circuitInfo3.outputTransformers.Add(current2);
				}
				else
				{
					CircuitInfo circuitInfo4 = this.circuitInfo[circuitID2];
					circuitInfo4.generators.Add(current2);
				}
			}
		}
		HashSet<WireUtilityNetworkLink>.Enumerator enumerator3 = bridges.GetEnumerator();
		while (enumerator3.MoveNext())
		{
			WireUtilityNetworkLink current3 = enumerator3.Current;
			current3.GetCells(out int linked_cell, out int _);
			ushort circuitID3 = GetCircuitID(linked_cell);
			if (circuitID3 != 65535)
			{
				Wire.WattageRating maxWattageRating = current3.GetMaxWattageRating();
				CircuitInfo circuitInfo5 = this.circuitInfo[circuitID3];
				circuitInfo5.bridgeGroups[(int)maxWattageRating].Add(current3);
			}
		}
		dirty = false;
	}

	private float GetBatteryJoulesAvailable(List<Battery> batteries, out int num_powered)
	{
		float result = 0f;
		num_powered = 0;
		for (int i = 0; i < batteries.Count; i++)
		{
			if (batteries[i].JoulesAvailable > 0f)
			{
				result = batteries[i].JoulesAvailable;
				num_powered = batteries.Count - i;
				break;
			}
		}
		return result;
	}

	public void Sim200msLast(float dt)
	{
		elapsedTime += dt;
		if (!(elapsedTime < 0.2f))
		{
			elapsedTime -= 0.2f;
			for (int i = 0; i < this.circuitInfo.Count; i++)
			{
				CircuitInfo value = this.circuitInfo[i];
				value.wattsUsed = 0f;
				activeGenerators.Clear();
				List<Generator> list = value.generators;
				List<IEnergyConsumer> list2 = value.consumers;
				List<Battery> batteries = value.batteries;
				List<Generator> outputTransformers = value.outputTransformers;
				batteries.Sort((Battery a, Battery b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
				bool flag = false;
				bool flag2 = list.Count > 0;
				for (int j = 0; j < list.Count; j++)
				{
					Generator generator = list[j];
					if (generator.JoulesAvailable > 0f)
					{
						flag = true;
						activeGenerators.Add(generator);
					}
				}
				activeGenerators.Sort((Generator a, Generator b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
				if (!flag)
				{
					for (int k = 0; k < outputTransformers.Count; k++)
					{
						Generator generator2 = outputTransformers[k];
						if (generator2.JoulesAvailable > 0f)
						{
							flag = true;
						}
					}
				}
				float num = 1f;
				for (int l = 0; l < batteries.Count; l++)
				{
					Battery battery = batteries[l];
					if (battery.JoulesAvailable > 0f)
					{
						flag = true;
					}
					num = Mathf.Min(num, battery.PercentFull);
				}
				for (int m = 0; m < value.inputTransformers.Count; m++)
				{
					Battery battery2 = value.inputTransformers[m];
					num = Mathf.Min(num, battery2.PercentFull);
				}
				value.minBatteryPercentFull = num;
				if (flag)
				{
					for (int n = 0; n < list2.Count; n++)
					{
						IEnergyConsumer energyConsumer = list2[n];
						float num2 = energyConsumer.WattsUsed * 0.2f;
						if (num2 > 0f)
						{
							value.wattsUsed += energyConsumer.WattsUsed;
							bool flag3 = false;
							for (int num3 = 0; num3 < activeGenerators.Count; num3++)
							{
								Generator g = activeGenerators[num3];
								num2 = PowerFromGenerator(num2, g, energyConsumer);
								if (num2 <= 0f)
								{
									flag3 = true;
									break;
								}
							}
							if (!flag3)
							{
								for (int num4 = 0; num4 < outputTransformers.Count; num4++)
								{
									Generator g2 = outputTransformers[num4];
									num2 = PowerFromGenerator(num2, g2, energyConsumer);
									if (num2 <= 0f)
									{
										flag3 = true;
										break;
									}
								}
							}
							if (!flag3)
							{
								num2 = PowerFromBatteries(num2, batteries, energyConsumer);
								flag3 = (Mathf.Abs(num2) <= 0.01f);
							}
							energyConsumer.SetConnectionStatus((!flag3) ? ConnectionStatus.Unpowered : ConnectionStatus.Powered);
						}
						else
						{
							energyConsumer.SetConnectionStatus((!flag) ? ConnectionStatus.Unpowered : ConnectionStatus.Powered);
						}
					}
				}
				else if (flag2)
				{
					for (int num5 = 0; num5 < list2.Count; num5++)
					{
						IEnergyConsumer energyConsumer2 = list2[num5];
						energyConsumer2.SetConnectionStatus(ConnectionStatus.Unpowered);
					}
				}
				else
				{
					for (int num6 = 0; num6 < list2.Count; num6++)
					{
						IEnergyConsumer energyConsumer3 = list2[num6];
						energyConsumer3.SetConnectionStatus(ConnectionStatus.NotConnected);
					}
				}
				this.circuitInfo[i] = value;
			}
			for (int num7 = 0; num7 < this.circuitInfo.Count; num7++)
			{
				CircuitInfo value2 = this.circuitInfo[num7];
				value2.batteries.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
				value2.inputTransformers.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
				float joules_used = 0f;
				foreach (Battery inputTransformer in value2.inputTransformers)
				{
					ChargeTransformer(dt, inputTransformer, value2.generators, ref joules_used);
				}
				ChargeBatteries(num7, value2.outputTransformers, value2.inputTransformers, ref joules_used);
				float joules_used2 = 0f;
				ChargeBatteries(num7, value2.generators, value2.batteries, ref joules_used2);
				ChargeBatteries(num7, value2.outputTransformers, value2.batteries, ref joules_used2);
				value2.minBatteryPercentFull = 1f;
				for (int num8 = 0; num8 < value2.batteries.Count; num8++)
				{
					Battery battery3 = value2.batteries[num8];
					float percentFull = battery3.PercentFull;
					if (percentFull < value2.minBatteryPercentFull)
					{
						value2.minBatteryPercentFull = percentFull;
					}
				}
				for (int num9 = 0; num9 < value2.inputTransformers.Count; num9++)
				{
					Battery battery4 = value2.inputTransformers[num9];
					float percentFull2 = battery4.PercentFull;
					if (percentFull2 < value2.minBatteryPercentFull)
					{
						value2.minBatteryPercentFull = percentFull2;
					}
				}
				value2.wattsUsed += joules_used / 0.2f;
				this.circuitInfo[num7] = value2;
			}
			for (int num10 = 0; num10 < this.circuitInfo.Count; num10++)
			{
				CircuitInfo value3 = this.circuitInfo[num10];
				float joules_used3 = 0f;
				for (int num11 = 0; num11 < value3.inputTransformers.Count; num11++)
				{
					Battery transformer = value3.inputTransformers[num11];
					ChargeTransformer(dt, transformer, value3.batteries, ref joules_used3);
				}
				value3.wattsUsed += joules_used3 / 0.2f;
				this.circuitInfo[num10] = value3;
			}
			for (int num12 = 0; num12 < this.circuitInfo.Count; num12++)
			{
				CircuitInfo value4 = this.circuitInfo[num12];
				bool is_connected_to_something_useful = value4.generators.Count + value4.consumers.Count + value4.outputTransformers.Count > 0;
				UpdateBatteryConnectionStatus(value4.batteries, is_connected_to_something_useful, num12);
				bool flag4 = value4.generators.Count > 0 || value4.outputTransformers.Count > 0;
				if (!flag4)
				{
					foreach (Battery battery5 in value4.batteries)
					{
						if (battery5.JoulesAvailable > 0f)
						{
							flag4 = true;
							break;
						}
					}
				}
				UpdateBatteryConnectionStatus(value4.inputTransformers, flag4, num12);
				this.circuitInfo[num12] = value4;
				for (int num13 = 0; num13 < value4.generators.Count; num13++)
				{
					Generator generator3 = value4.generators[num13];
					ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, 0f - generator3.JoulesAvailable, StringFormatter.Replace(BUILDINGS.PREFABS.GENERATOR.OVERPRODUCTION, "{Generator}", generator3.gameObject.GetProperName()), null);
				}
			}
			for (int num14 = 0; num14 < this.circuitInfo.Count; num14++)
			{
				CircuitInfo circuitInfo = this.circuitInfo[num14];
				CheckCircuitOverloaded(0.2f, num14, circuitInfo.wattsUsed);
			}
		}
	}

	private float PowerFromBatteries(float joules_needed, List<Battery> batteries, IEnergyConsumer c)
	{
		int num_powered;
		do
		{
			float batteryJoulesAvailable = GetBatteryJoulesAvailable(batteries, out num_powered);
			float num = batteryJoulesAvailable * (float)num_powered;
			float num2 = (!(num < joules_needed)) ? joules_needed : num;
			joules_needed -= num2;
			ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, 0f - num2, c.Name, null);
			float joules = num2 / (float)num_powered;
			for (int i = batteries.Count - num_powered; i < batteries.Count; i++)
			{
				Battery battery = batteries[i];
				battery.ConsumeEnergy(joules);
			}
		}
		while (joules_needed >= 0.01f && num_powered > 0);
		return joules_needed;
	}

	private float PowerFromGenerator(float joules_needed, Generator g, IEnergyConsumer c)
	{
		float num = Mathf.Min(g.JoulesAvailable, joules_needed);
		joules_needed -= num;
		g.ApplyDeltaJoules(0f - num, false);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, 0f - num, c.Name, null);
		return joules_needed;
	}

	private float GetBatteryChargeCapacity(Generator g, List<Battery> batteries, out int num_to_charge)
	{
		float result = 0f;
		num_to_charge = 0;
		for (int i = 0; i < batteries.Count; i++)
		{
			Battery battery = batteries[i];
			if ((Object)battery != (Object)null && (Object)g != (Object)null && (Object)battery.gameObject != (Object)g.gameObject && battery.Capacity > battery.JoulesAvailable)
			{
				result = battery.Capacity - battery.JoulesAvailable;
				num_to_charge = batteries.Count - i;
				break;
			}
		}
		return result;
	}

	private void ChargeBatteries(int circuit_id, List<Generator> generators, List<Battery> batteries, ref float joules_used)
	{
		if (batteries.Count != 0)
		{
			foreach (Generator generator in generators)
			{
				for (bool flag = true; flag && generator.JoulesAvailable >= 1f; flag = ChargeBattery(generator, batteries, ref joules_used))
				{
				}
			}
		}
	}

	private bool ChargeBattery(Generator g, List<Battery> batteries, ref float joules_used)
	{
		int num_to_charge;
		float batteryChargeCapacity = GetBatteryChargeCapacity(g, batteries, out num_to_charge);
		if (batteryChargeCapacity <= 0f)
		{
			return false;
		}
		float num = Mathf.Min(batteryChargeCapacity, g.JoulesAvailable / (float)num_to_charge);
		g.ApplyDeltaJoules((0f - num) * (float)num_to_charge, false);
		joules_used += num * (float)num_to_charge;
		for (int i = batteries.Count - num_to_charge; i < batteries.Count; i++)
		{
			Battery battery = batteries[i];
			if ((Object)g != (Object)null && (Object)battery != (Object)null && (Object)g.gameObject != (Object)battery.gameObject)
			{
				battery.AddEnergy(num);
			}
		}
		return true;
	}

	private void UpdateBatteryConnectionStatus(List<Battery> batteries, bool is_connected_to_something_useful, int circuit_id)
	{
		foreach (Battery battery in batteries)
		{
			if (!((Object)battery == (Object)null))
			{
				if ((Object)battery.powerTransformer == (Object)null)
				{
					battery.SetConnectionStatus(is_connected_to_something_useful ? ConnectionStatus.Powered : ConnectionStatus.NotConnected);
				}
				else
				{
					ushort circuitID = GetCircuitID(battery.PowerCell);
					if (circuitID == circuit_id)
					{
						battery.SetConnectionStatus((!is_connected_to_something_useful) ? ConnectionStatus.Unpowered : ConnectionStatus.Powered);
					}
				}
			}
		}
	}

	private void ChargeTransformer<T>(float dt, Battery transformer, List<T> energy_producers, ref float joules_used) where T : IEnergyProducer
	{
		if (energy_producers.Count > 0)
		{
			float num = Mathf.Min(transformer.Capacity - transformer.JoulesAvailable, transformer.ChargeCapacity);
			if (!(num <= 0f))
			{
				float a = 0f;
				int num2 = 0;
				for (int i = 0; i < energy_producers.Count; i++)
				{
					T val = energy_producers[i];
					if (val.JoulesAvailable > 0f)
					{
						num2 = energy_producers.Count - i;
						a = val.JoulesAvailable;
						break;
					}
				}
				if (num2 > 0)
				{
					float num3 = Mathf.Min(a, num / (float)num2);
					transformer.AddEnergy(num3 * (float)num2);
					joules_used += num3 * (float)num2;
					for (int j = energy_producers.Count - num2; j < energy_producers.Count; j++)
					{
						energy_producers[j].ConsumeEnergy(num3);
					}
				}
			}
		}
	}

	private void CheckCircuitOverloaded(float dt, int id, float watts_used)
	{
		UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem = Game.Instance.electricalConduitSystem;
		UtilityNetwork networkByID = electricalConduitSystem.GetNetworkByID(id);
		if (networkByID != null)
		{
			ElectricalUtilityNetwork electricalUtilityNetwork = (ElectricalUtilityNetwork)networkByID;
			if (electricalUtilityNetwork != null)
			{
				ElectricalUtilityNetwork electricalUtilityNetwork2 = electricalUtilityNetwork;
				CircuitInfo circuitInfo = this.circuitInfo[id];
				electricalUtilityNetwork2.UpdateOverloadTime(dt, watts_used, circuitInfo.bridgeGroups);
			}
		}
	}

	public float GetWattsUsedByCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return -1f;
		}
		float num = 0f;
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		foreach (IEnergyConsumer consumer in circuitInfo.consumers)
		{
			num += consumer.WattsUsed;
		}
		CircuitInfo circuitInfo2 = this.circuitInfo[circuitID];
		foreach (Battery inputTransformer in circuitInfo2.inputTransformers)
		{
			num += inputTransformer.WattsUsed;
		}
		return num;
	}

	public float GetWattsNeededWhenActive(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return -1f;
		}
		float num = 0f;
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		foreach (IEnergyConsumer consumer in circuitInfo.consumers)
		{
			num += consumer.WattsNeededWhenActive;
		}
		CircuitInfo circuitInfo2 = this.circuitInfo[circuitID];
		foreach (Battery inputTransformer in circuitInfo2.inputTransformers)
		{
			num += inputTransformer.WattsNeededWhenActive;
		}
		return num;
	}

	public float GetWattsGeneratedByCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return -1f;
		}
		float num = 0f;
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		List<Generator> list = circuitInfo.generators;
		foreach (Generator item in list)
		{
			if (!((Object)item == (Object)null) && item.GetComponent<Operational>().IsActive)
			{
				num += item.WattageRating;
			}
		}
		return num;
	}

	public float GetPotentialWattsGeneratedByCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return -1f;
		}
		float num = 0f;
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		List<Generator> list = circuitInfo.generators;
		foreach (Generator item in list)
		{
			num += item.WattageRating;
		}
		return num;
	}

	public bool HasPowerSource(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return false;
		}
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		List<Generator> list = circuitInfo.generators;
		CircuitInfo circuitInfo2 = this.circuitInfo[circuitID];
		List<Battery> batteries = circuitInfo2.batteries;
		return (list.Count > 0 && (Object)list.Find(FindActiveGenerator) != (Object)null) || (batteries.Count > 0 && (Object)batteries.Find(FindActiveBattery) != (Object)null);
	}

	private bool FindActiveGenerator(Generator g)
	{
		Operational component = g.GetComponent<Operational>();
		ManualGenerator component2 = g.GetComponent<ManualGenerator>();
		if ((Object)component2 == (Object)null)
		{
			return component.IsActive;
		}
		return component.IsOperational && component2.IsPowered;
	}

	private bool FindActiveBattery(Battery b)
	{
		return b.GetComponent<Operational>().IsOperational && b.PercentFull > 0f;
	}

	public float GetJoulesAvailableOnCircuit(ushort circuitID)
	{
		int num_powered;
		float batteryJoulesAvailable = GetBatteryJoulesAvailable(GetBatteriesOnCircuit(circuitID), out num_powered);
		return batteryJoulesAvailable * (float)num_powered;
	}

	public ReadOnlyCollection<Generator> GetGeneratorsOnCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return null;
		}
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		return circuitInfo.generators.AsReadOnly();
	}

	public ReadOnlyCollection<IEnergyConsumer> GetConsumersOnCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return null;
		}
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		return circuitInfo.consumers.AsReadOnly();
	}

	public ReadOnlyCollection<Battery> GetTransformersOnCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return null;
		}
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		return circuitInfo.inputTransformers.AsReadOnly();
	}

	public List<Battery> GetBatteriesOnCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return null;
		}
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		return circuitInfo.batteries;
	}

	public float GetMinBatteryPercentFullOnCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return 0f;
		}
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		return circuitInfo.minBatteryPercentFull;
	}

	public bool HasBatteries(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return false;
		}
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		int count = circuitInfo.batteries.Count;
		CircuitInfo circuitInfo2 = this.circuitInfo[circuitID];
		return count + circuitInfo2.inputTransformers.Count > 0;
	}

	public bool HasGenerators(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return false;
		}
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		int count = circuitInfo.generators.Count;
		CircuitInfo circuitInfo2 = this.circuitInfo[circuitID];
		return count + circuitInfo2.outputTransformers.Count > 0;
	}

	public bool HasGenerators()
	{
		return generators.Count > 0;
	}

	public bool HasConsumers(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return false;
		}
		CircuitInfo circuitInfo = this.circuitInfo[circuitID];
		return circuitInfo.consumers.Count > 0;
	}

	public float GetMaxSafeWattageForCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return 0f;
		}
		return (Game.Instance.electricalConduitSystem.GetNetworkByID(circuitID) as ElectricalUtilityNetwork)?.GetMaxSafeWattage() ?? 0f;
	}
}
