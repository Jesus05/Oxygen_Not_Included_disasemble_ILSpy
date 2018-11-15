using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class LiquidCooledRefinery : Refinery
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, LiquidCooledRefinery, object>.GameInstance
	{
		public StatesInstance(LiquidCooledRefinery master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, LiquidCooledRefinery>
	{
		public static StatusItem waitingForCoolantStatus;

		public State waiting_for_coolant;

		public State ready;

		public override void InitializeStates(out BaseState default_state)
		{
			if (waitingForCoolantStatus == null)
			{
				waitingForCoolantStatus = new StatusItem("waitingForCoolantStatus", BUILDING.STATUSITEMS.ENOUGH_COOLANT.NAME, BUILDING.STATUSITEMS.ENOUGH_COOLANT.TOOLTIP, "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, 63486);
				waitingForCoolantStatus.resolveStringCallback = delegate(string str, object obj)
				{
					LiquidCooledRefinery liquidCooledRefinery = (LiquidCooledRefinery)obj;
					return string.Format(str, liquidCooledRefinery.coolantTag.ProperName(), GameUtil.GetFormattedMass(liquidCooledRefinery.minCoolantMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				};
			}
			default_state = waiting_for_coolant;
			waiting_for_coolant.ToggleStatusItem(waitingForCoolantStatus, (StatesInstance smi) => smi.master).EventTransition(GameHashes.OnStorageChange, ready, (StatesInstance smi) => smi.master.HasEnoughCoolant());
			ready.EventTransition(GameHashes.OnStorageChange, waiting_for_coolant, (StatesInstance smi) => !smi.master.HasEnoughCoolant());
		}
	}

	[MyCmpReq]
	private ConduitConsumer conduitConsumer;

	public static readonly Operational.Flag enoughCoolant = new Operational.Flag("enoughCoolant", Operational.Flag.Type.Functional);

	public Tag coolantTag;

	public float minCoolantMass = 100f;

	public float thermalFudge = 0.8f;

	public float outputTemperature = 313.15f;

	private MeterController meter_coolant;

	private MeterController meter_metal;

	private StatesInstance smi;

	private static readonly EventSystem.IntraObjectHandler<LiquidCooledRefinery> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<LiquidCooledRefinery>(delegate(LiquidCooledRefinery component, object data)
	{
		component.OnStorageChange(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1697596308, OnStorageChangeDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		meter_coolant = new MeterController((KAnimControllerBase)component, "meter_target", "meter_coolant", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, (string[])null);
		meter_metal = new MeterController((KAnimControllerBase)component, "meter_target_metal", "meter_metal", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, (string[])null);
		meter_metal.SetPositionPercent(1f);
		smi = new StatesInstance(this);
		smi.StartSM();
		workable.OnWorkTickActions = delegate
		{
			float percentComplete = workable.GetPercentComplete();
			meter_metal.SetPositionPercent(percentComplete);
		};
	}

	public bool HasEnoughCoolant()
	{
		float amountAvailable = inStorage.GetAmountAvailable(coolantTag);
		return amountAvailable >= minCoolantMass;
	}

	private void OnStorageChange(object data)
	{
		float amountAvailable = inStorage.GetAmountAvailable(coolantTag);
		operational.SetFlag(enoughCoolant, amountAvailable >= minCoolantMass);
		float capacityKG = conduitConsumer.capacityKG;
		float positionPercent = Mathf.Clamp01(amountAvailable / capacityKG);
		if (meter_coolant != null)
		{
			meter_coolant.SetPositionPercent(positionPercent);
		}
	}

	protected override List<GameObject> CompleteOrder(UserOrder completed_order)
	{
		List<GameObject> list = base.CompleteOrder(completed_order);
		PrimaryElement component = list[0].GetComponent<PrimaryElement>();
		component.Temperature = outputTemperature;
		float num = GameUtil.CalculateEnergyDeltaForElementChange(component.Element.specificHeatCapacity, component.Mass, component.Element.highTemp, outputTemperature);
		inStorage.Transfer(outStorage, coolantTag, minCoolantMass, false, true);
		ListPool<GameObject, LiquidCooledRefinery>.PooledList pooledList = ListPool<GameObject, LiquidCooledRefinery>.Allocate();
		outStorage.Find(coolantTag, pooledList);
		foreach (GameObject item in pooledList)
		{
			PrimaryElement component2 = item.GetComponent<PrimaryElement>();
			if (component2.Mass != 0f)
			{
				float num2 = component2.Mass / minCoolantMass;
				float kilowatts = (0f - num) * num2 * thermalFudge;
				float num3 = GameUtil.CalculateTemperatureChange(component2.Element.specificHeatCapacity, component2.Mass, kilowatts);
				float temperature = component2.Temperature;
				component2.Temperature += num3;
			}
		}
		pooledList.Recycle();
		return list;
	}

	public override List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> descriptors = base.GetDescriptors(def);
		descriptors.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.COOLANT, coolantTag.ProperName(), GameUtil.GetFormattedMass(minCoolantMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.COOLANT, coolantTag.ProperName(), GameUtil.GetFormattedMass(minCoolantMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
		return descriptors;
	}

	public override List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe)
	{
		List<Descriptor> list = base.AdditionalEffectsForRecipe(recipe);
		GameObject prefab = Assets.GetPrefab(recipe.results[0].material);
		PrimaryElement component = prefab.GetComponent<PrimaryElement>();
		PrimaryElement primaryElement = inStorage.FindFirstWithMass(coolantTag);
		string format = UI.BUILDINGEFFECTS.TOOLTIPS.REFINEMENT_ENERGY_HAS_COOLANT;
		if ((Object)primaryElement == (Object)null)
		{
			GameObject prefab2 = Assets.GetPrefab(GameTags.Water);
			primaryElement = prefab2.GetComponent<PrimaryElement>();
			format = UI.BUILDINGEFFECTS.TOOLTIPS.REFINEMENT_ENERGY_NO_COOLANT;
		}
		float num = 0f - GameUtil.CalculateEnergyDeltaForElementChange(component.Element.specificHeatCapacity, recipe.results[0].amount, component.Element.highTemp, outputTemperature);
		float temp = GameUtil.CalculateTemperatureChange(primaryElement.Element.specificHeatCapacity, minCoolantMass, num * thermalFudge);
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.REFINEMENT_ENERGY, GameUtil.GetFormattedJoules(num, "F1", GameUtil.TimeSlice.None)), string.Format(format, GameUtil.GetFormattedJoules(num, "F1", GameUtil.TimeSlice.None), primaryElement.GetProperName(), GameUtil.GetFormattedTemperature(temp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true)), Descriptor.DescriptorType.Effect, false));
		return list;
	}
}
