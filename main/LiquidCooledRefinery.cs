using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class LiquidCooledRefinery : ComplexFabricator
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

		public BoolParameter outputBlocked;

		public State waiting_for_coolant;

		public State ready;

		public State output_blocked;

		public override void InitializeStates(out BaseState default_state)
		{
			if (waitingForCoolantStatus == null)
			{
				waitingForCoolantStatus = new StatusItem("waitingForCoolantStatus", BUILDING.STATUSITEMS.ENOUGH_COOLANT.NAME, BUILDING.STATUSITEMS.ENOUGH_COOLANT.TOOLTIP, "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
				waitingForCoolantStatus.resolveStringCallback = delegate(string str, object obj)
				{
					LiquidCooledRefinery liquidCooledRefinery = (LiquidCooledRefinery)obj;
					return string.Format(str, liquidCooledRefinery.coolantTag.ProperName(), GameUtil.GetFormattedMass(liquidCooledRefinery.minCoolantMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				};
			}
			default_state = waiting_for_coolant;
			waiting_for_coolant.ToggleStatusItem(waitingForCoolantStatus, (StatesInstance smi) => smi.master).EventTransition(GameHashes.OnStorageChange, ready, (StatesInstance smi) => smi.master.HasEnoughCoolant()).ParamTransition(outputBlocked, output_blocked, GameStateMachine<States, StatesInstance, LiquidCooledRefinery, object>.IsTrue);
			ready.EventTransition(GameHashes.OnStorageChange, waiting_for_coolant, (StatesInstance smi) => !smi.master.HasEnoughCoolant()).ParamTransition(outputBlocked, output_blocked, GameStateMachine<States, StatesInstance, LiquidCooledRefinery, object>.IsTrue).Enter(delegate(StatesInstance smi)
			{
				smi.master.UpdateMachineOrders(false);
			});
			output_blocked.ToggleStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, (object)null).ParamTransition(outputBlocked, waiting_for_coolant, GameStateMachine<States, StatesInstance, LiquidCooledRefinery, object>.IsFalse);
		}
	}

	[MyCmpReq]
	private ConduitConsumer conduitConsumer;

	public static readonly Operational.Flag coolantOutputPipeEmpty = new Operational.Flag("coolantOutputPipeEmpty", Operational.Flag.Type.Requirement);

	private int outputCell;

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
		ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
		liquidConduitFlow.AddConduitUpdater(OnConduitUpdate, ConduitFlowPriority.Default);
		Building component2 = GetComponent<Building>();
		outputCell = component2.GetUtilityOutputCell();
		workable.OnWorkTickActions = delegate
		{
			float percentComplete = workable.GetPercentComplete();
			meter_metal.SetPositionPercent(percentComplete);
		};
	}

	protected override void OnCleanUp()
	{
		Game.Instance.liquidConduitFlow.RemoveConduitUpdater(OnConduitUpdate);
		base.OnCleanUp();
	}

	private void OnConduitUpdate(float dt)
	{
		ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
		ConduitFlow.ConduitContents contents = liquidConduitFlow.GetContents(outputCell);
		bool flag = contents.mass > 0f;
		smi.sm.outputBlocked.Set(flag, smi);
		operational.SetFlag(coolantOutputPipeEmpty, !flag);
	}

	public bool HasEnoughCoolant()
	{
		float amountAvailable = inStorage.GetAmountAvailable(coolantTag);
		amountAvailable += buildStorage.GetAmountAvailable(coolantTag);
		return amountAvailable >= minCoolantMass;
	}

	private void OnStorageChange(object data)
	{
		float amountAvailable = inStorage.GetAmountAvailable(coolantTag);
		float capacityKG = conduitConsumer.capacityKG;
		float positionPercent = Mathf.Clamp01(amountAvailable / capacityKG);
		if (meter_coolant != null)
		{
			meter_coolant.SetPositionPercent(positionPercent);
		}
	}

	protected override bool HasIngredients(MachineOrder order, Storage storage)
	{
		float amountAvailable = storage.GetAmountAvailable(coolantTag);
		return amountAvailable >= minCoolantMass && base.HasIngredients(order, storage);
	}

	protected override void TransferCurrentRecipeIngredientsForBuild()
	{
		base.TransferCurrentRecipeIngredientsForBuild();
		while (buildStorage.GetAmountAvailable(coolantTag) < minCoolantMass && inStorage.GetAmountAvailable(coolantTag) > 0f)
		{
			inStorage.Transfer(buildStorage, coolantTag, minCoolantMass, false, true);
		}
	}

	protected override List<GameObject> SpawnOrderProduct(UserOrder completed_order)
	{
		List<GameObject> list = base.SpawnOrderProduct(completed_order);
		PrimaryElement component = list[0].GetComponent<PrimaryElement>();
		component.Temperature = outputTemperature;
		float num = GameUtil.CalculateEnergyDeltaForElementChange(component.Element.specificHeatCapacity, component.Mass, component.Element.highTemp, outputTemperature);
		ListPool<GameObject, LiquidCooledRefinery>.PooledList pooledList = ListPool<GameObject, LiquidCooledRefinery>.Allocate();
		buildStorage.Find(coolantTag, pooledList);
		float num2 = 0f;
		foreach (GameObject item in pooledList)
		{
			PrimaryElement component2 = item.GetComponent<PrimaryElement>();
			if (component2.Mass != 0f)
			{
				num2 = component2.Mass * component2.Element.specificHeatCapacity;
			}
		}
		foreach (GameObject item2 in pooledList)
		{
			PrimaryElement component3 = item2.GetComponent<PrimaryElement>();
			if (component3.Mass != 0f)
			{
				float num3 = component3.Mass * component3.Element.specificHeatCapacity / num2;
				float kilowatts = (0f - num) * num3 * thermalFudge;
				float num4 = GameUtil.CalculateTemperatureChange(component3.Element.specificHeatCapacity, component3.Mass, kilowatts);
				float temperature = component3.Temperature;
				component3.Temperature += num4;
			}
		}
		buildStorage.Transfer(outStorage, coolantTag, minCoolantMass, false, true);
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
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.REFINEMENT_ENERGY, GameUtil.GetFormattedJoules(num, "F1", GameUtil.TimeSlice.None)), string.Format(format, GameUtil.GetFormattedJoules(num, "F1", GameUtil.TimeSlice.None), primaryElement.GetProperName(), GameUtil.GetFormattedTemperature(temp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true, false)), Descriptor.DescriptorType.Effect, false));
		return list;
	}
}
