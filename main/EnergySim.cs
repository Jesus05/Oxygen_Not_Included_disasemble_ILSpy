using System.Collections.Generic;

public class EnergySim
{
	private HashSet<Generator> generators = new HashSet<Generator>();

	private HashSet<ManualGenerator> manualGenerators = new HashSet<ManualGenerator>();

	private HashSet<Battery> batteries = new HashSet<Battery>();

	private HashSet<EnergyConsumer> energyConsumers = new HashSet<EnergyConsumer>();

	public HashSet<Generator> Generators => generators;

	public void AddGenerator(Generator generator)
	{
		generators.Add(generator);
	}

	public void RemoveGenerator(Generator generator)
	{
		generators.Remove(generator);
	}

	public void AddManualGenerator(ManualGenerator manual_generator)
	{
		manualGenerators.Add(manual_generator);
	}

	public void RemoveManualGenerator(ManualGenerator manual_generator)
	{
		manualGenerators.Remove(manual_generator);
	}

	public void AddBattery(Battery battery)
	{
		batteries.Add(battery);
	}

	public void RemoveBattery(Battery battery)
	{
		batteries.Remove(battery);
	}

	public void AddEnergyConsumer(EnergyConsumer energy_consumer)
	{
		energyConsumers.Add(energy_consumer);
	}

	public void RemoveEnergyConsumer(EnergyConsumer energy_consumer)
	{
		energyConsumers.Remove(energy_consumer);
	}

	public void EnergySim200ms(float dt)
	{
		foreach (Generator generator in generators)
		{
			generator.EnergySim200ms(dt);
		}
		foreach (ManualGenerator manualGenerator in manualGenerators)
		{
			manualGenerator.EnergySim200ms(dt);
		}
		foreach (Battery battery in batteries)
		{
			battery.EnergySim200ms(dt);
		}
		foreach (EnergyConsumer energyConsumer in energyConsumers)
		{
			energyConsumer.EnergySim200ms(dt);
		}
	}
}
