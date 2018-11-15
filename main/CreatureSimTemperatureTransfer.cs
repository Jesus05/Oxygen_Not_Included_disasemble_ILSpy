using Klei;
using Klei.AI;
using STRINGS;
using System.Collections.Generic;

public class CreatureSimTemperatureTransfer : SimTemperatureTransfer, ISim200ms
{
	public AttributeModifier averageTemperatureTransferPerSecond;

	private PrimaryElement primaryElement;

	public RunningWeightedAverage average_kilowatts_exchanged;

	public List<AttributeModifier> NonSimTemperatureModifiers = new List<AttributeModifier>();

	public float deltaEnergy
	{
		get
		{
			return deltaKJ;
		}
		protected set
		{
			deltaKJ = value;
		}
	}

	public float currentExchangeWattage => deltaKJ * 5f * 1000f;

	protected override void OnPrefabInit()
	{
		primaryElement = GetComponent<PrimaryElement>();
		average_kilowatts_exchanged = new RunningWeightedAverage(-10f, 10f, 20, true);
		surfaceArea = 1f;
		thickness = 0.002f;
		groundTransferScale = 0f;
		Attributes attributes = base.gameObject.GetAttributes();
		AttributeInstance attributeInstance = attributes.Add(Db.Get().Attributes.ThermalConductivityBarrier);
		AttributeModifier modifier = new AttributeModifier(Db.Get().Attributes.ThermalConductivityBarrier.Id, thickness, DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME, false, false, true);
		attributeInstance.Add(modifier);
		averageTemperatureTransferPerSecond = new AttributeModifier("TemperatureDelta", 0f, DUPLICANTS.MODIFIERS.TEMPEXCHANGE.NAME, false, true, false);
		this.GetAttributes().Add(averageTemperatureTransferPerSecond);
		base.OnPrefabInit();
	}

	public void Sim200ms(float dt)
	{
		average_kilowatts_exchanged.AddSample(currentExchangeWattage * 0.001f);
		averageTemperatureTransferPerSecond.SetValue(SimUtil.EnergyFlowToTemperatureDelta(average_kilowatts_exchanged.GetWeightedAverage, primaryElement.Element.specificHeatCapacity, primaryElement.Mass));
		float num = 0f;
		foreach (AttributeModifier nonSimTemperatureModifier in NonSimTemperatureModifiers)
		{
			num += nonSimTemperatureModifier.Value;
		}
		if (Sim.IsValidHandle(simHandle))
		{
			SimMessages.ModifyElementChunkEnergy(simHandle, num * dt * (primaryElement.Mass * 1000f) * primaryElement.Element.specificHeatCapacity * 0.001f);
		}
	}

	public void RefreshRegistration()
	{
		SimUnregister();
		Attributes attributes = base.gameObject.GetAttributes();
		AttributeInstance attributeInstance = attributes.Get("ThermalConductivityBarrier");
		thickness = attributeInstance.GetTotalValue();
		simHandle = -1;
		SimRegister();
	}

	public static float PotentialEnergyFlowToCreature(int cell, PrimaryElement transfererPrimaryElement, SimTemperatureTransfer temperatureTransferer, float deltaTime = 1f)
	{
		return SimUtil.CalculateEnergyFlowCreatures(cell, transfererPrimaryElement.Temperature, transfererPrimaryElement.Element.specificHeatCapacity, transfererPrimaryElement.Element.thermalConductivity, temperatureTransferer.SurfaceArea, temperatureTransferer.Thickness);
	}
}
