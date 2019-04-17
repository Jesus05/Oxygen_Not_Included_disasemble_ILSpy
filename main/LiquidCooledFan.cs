using Klei;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LiquidCooledFan : StateMachineComponent<LiquidCooledFan.StatesInstance>, IEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, LiquidCooledFan, object>.GameInstance
	{
		public StatesInstance(LiquidCooledFan smi)
			: base(smi)
		{
		}

		public bool IsWorkable()
		{
			bool result = false;
			if (base.master.operational.IsOperational && EnvironmentNeedsCooling() && base.smi.master.HasMaterial() && base.smi.EnvironmentHighEnoughPressure())
			{
				result = true;
			}
			return result;
		}

		public bool EnvironmentNeedsCooling()
		{
			bool result = false;
			int cell = Grid.PosToCell(base.transform.GetPosition());
			for (int i = base.master.minCoolingRange.y; i < base.master.maxCoolingRange.y; i++)
			{
				for (int j = base.master.minCoolingRange.x; j < base.master.maxCoolingRange.x; j++)
				{
					CellOffset offset = new CellOffset(j, i);
					int i2 = Grid.OffsetCell(cell, offset);
					if (Grid.Temperature[i2] > base.master.minCooledTemperature)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public bool EnvironmentHighEnoughPressure()
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			for (int i = base.master.minCoolingRange.y; i < base.master.maxCoolingRange.y; i++)
			{
				for (int j = base.master.minCoolingRange.x; j < base.master.maxCoolingRange.x; j++)
				{
					CellOffset offset = new CellOffset(j, i);
					int i2 = Grid.OffsetCell(cell, offset);
					if (Grid.Mass[i2] >= base.master.minEnvironmentMass)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, LiquidCooledFan>
	{
		public class Workable : State
		{
			public State waiting;

			public State consuming;

			public State emitting;
		}

		public Workable workable;

		public State unworkable;

		public State work_pst;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unworkable;
			root.Enter(delegate(StatesInstance smi)
			{
				smi.master.workable.SetWorkTime(float.PositiveInfinity);
			});
			workable.ToggleChore(CreateUseChore, work_pst).EventTransition(GameHashes.ActiveChanged, workable.consuming, (StatesInstance smi) => (Object)smi.master.workable.worker != (Object)null).EventTransition(GameHashes.OperationalChanged, workable.consuming, (StatesInstance smi) => (Object)smi.master.workable.worker != (Object)null)
				.Transition(unworkable, (StatesInstance smi) => !smi.IsWorkable(), UpdateRate.SIM_200ms);
			work_pst.Update("LiquidFanEmitCooledContents", delegate(StatesInstance smi, float dt)
			{
				smi.master.EmitContents();
			}, UpdateRate.SIM_200ms, false).ScheduleGoTo(2f, unworkable);
			unworkable.Update("LiquidFanEmitCooledContents", delegate(StatesInstance smi, float dt)
			{
				smi.master.EmitContents();
			}, UpdateRate.SIM_200ms, false).Update("LiquidFanUnworkableStatusItems", delegate(StatesInstance smi, float dt)
			{
				smi.master.UpdateUnworkableStatusItems();
			}, UpdateRate.SIM_200ms, false).Transition(workable.waiting, (StatesInstance smi) => smi.IsWorkable(), UpdateRate.SIM_200ms)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.UpdateUnworkableStatusItems();
				})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.UpdateUnworkableStatusItems();
				});
			workable.consuming.EventTransition(GameHashes.OperationalChanged, unworkable, (StatesInstance smi) => (Object)smi.master.workable.worker == (Object)null).EventHandler(GameHashes.ActiveChanged, delegate(StatesInstance smi)
			{
				smi.master.CheckWorking();
			}).Enter(delegate(StatesInstance smi)
			{
				if (!smi.EnvironmentNeedsCooling() || !smi.master.HasMaterial() || !smi.EnvironmentHighEnoughPressure())
				{
					smi.GoTo(unworkable);
				}
				ElementConsumer component2 = smi.master.GetComponent<ElementConsumer>();
				component2.consumptionRate = smi.master.flowRate;
				component2.RefreshConsumptionRate();
			})
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.CoolContents(dt);
				}, UpdateRate.SIM_200ms, false)
				.ScheduleGoTo(12f, workable.emitting)
				.Exit(delegate(StatesInstance smi)
				{
					ElementConsumer component = smi.master.GetComponent<ElementConsumer>();
					component.consumptionRate = 0f;
					component.RefreshConsumptionRate();
				});
			workable.emitting.EventTransition(GameHashes.ActiveChanged, unworkable, (StatesInstance smi) => (Object)smi.master.workable.worker == (Object)null).EventTransition(GameHashes.OperationalChanged, unworkable, (StatesInstance smi) => (Object)smi.master.workable.worker == (Object)null).ScheduleGoTo(3f, workable.consuming)
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.CoolContents(dt);
				}, UpdateRate.SIM_200ms, false)
				.Update("LiquidFanEmitCooledContents", delegate(StatesInstance smi, float dt)
				{
					smi.master.EmitContents();
				}, UpdateRate.SIM_200ms, false);
		}

		private Chore CreateUseChore(StatesInstance smi)
		{
			return new WorkChore<LiquidCooledFanWorkable>(Db.Get().ChoreTypes.LiquidCooledFan, smi.master.workable, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		}
	}

	[SerializeField]
	public float coolingKilowatts;

	[SerializeField]
	public float minCooledTemperature;

	[SerializeField]
	public float minEnvironmentMass;

	[SerializeField]
	public float waterKGConsumedPerKJ;

	[SerializeField]
	public Vector2I minCoolingRange;

	[SerializeField]
	public Vector2I maxCoolingRange;

	private float flowRate = 0.3f;

	[SerializeField]
	public Storage gasStorage;

	[SerializeField]
	public Storage liquidStorage;

	[MyCmpAdd]
	private LiquidCooledFanWorkable workable;

	[MyCmpGet]
	private Operational operational;

	private HandleVector<int>.Handle waterConsumptionAccumulator = HandleVector<int>.InvalidHandle;

	private MeterController meter;

	public bool HasMaterial()
	{
		ListPool<GameObject, LiquidCooledFan>.PooledList pooledList = ListPool<GameObject, LiquidCooledFan>.Allocate();
		base.smi.master.gasStorage.Find(GameTags.Water, pooledList);
		if (pooledList.Count > 0)
		{
			Debug.LogWarning("Liquid Cooled fan Gas storage contains water - A duplicant probably delivered to the wrong storage - moving it to liquid storage.");
			foreach (GameObject item in pooledList)
			{
				base.smi.master.gasStorage.Transfer(item, base.smi.master.liquidStorage, false, false);
			}
		}
		pooledList.Recycle();
		UpdateMeter();
		return liquidStorage.MassStored() > 0f;
	}

	public void CheckWorking()
	{
		if ((Object)base.smi.master.workable.worker == (Object)null)
		{
			base.smi.GoTo(base.smi.sm.unworkable);
		}
	}

	private void UpdateUnworkableStatusItems()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (!base.smi.EnvironmentNeedsCooling())
		{
			if (!component.HasStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther))
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther, null);
			}
		}
		else if (component.HasStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther))
		{
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther, false);
		}
		if (!base.smi.EnvironmentHighEnoughPressure())
		{
			if (!component.HasStatusItem(Db.Get().BuildingStatusItems.UnderPressure))
			{
				component.AddStatusItem(Db.Get().BuildingStatusItems.UnderPressure, null);
			}
		}
		else if (component.HasStatusItem(Db.Get().BuildingStatusItems.UnderPressure))
		{
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.UnderPressure, false);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_waterbody", "meter_waterlevel");
		GetComponent<ElementConsumer>().EnableConsumption(true);
		base.smi.StartSM();
		base.smi.master.waterConsumptionAccumulator = Game.Instance.accumulators.Add("waterConsumptionAccumulator", this);
		GetComponent<ElementConsumer>().storage = gasStorage;
		GetComponent<ManualDeliveryKG>().SetStorage(liquidStorage);
	}

	private void UpdateMeter()
	{
		meter.SetPositionPercent(Mathf.Clamp01(liquidStorage.MassStored() / liquidStorage.capacityKg));
	}

	private void EmitContents()
	{
		if (gasStorage.items.Count != 0)
		{
			float num = 0.1f;
			float num2 = num;
			PrimaryElement primaryElement = null;
			for (int i = 0; i < gasStorage.items.Count; i++)
			{
				PrimaryElement component = gasStorage.items[i].GetComponent<PrimaryElement>();
				if (component.Mass > num2 && component.Element.IsGas)
				{
					primaryElement = component;
					num2 = primaryElement.Mass;
				}
			}
			if ((Object)primaryElement != (Object)null)
			{
				SimMessages.AddRemoveSubstance(Grid.CellRight(Grid.CellAbove(Grid.PosToCell(base.gameObject))), ElementLoader.GetElementIndex(primaryElement.ElementID), CellEventLogger.Instance.ExhaustSimUpdate, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount, true, -1);
				gasStorage.ConsumeIgnoringDisease(primaryElement.gameObject);
			}
		}
	}

	private void CoolContents(float dt)
	{
		if (gasStorage.items.Count != 0)
		{
			float num = float.PositiveInfinity;
			float num2 = 0f;
			float num3 = 0f;
			foreach (GameObject item in gasStorage.items)
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (!((Object)component == (Object)null) && !(component.Mass < 0.1f) && !(component.Temperature < minCooledTemperature))
				{
					num2 = GameUtil.GetThermalEnergy(component);
					if (num > num2)
					{
						num = num2;
					}
				}
			}
			foreach (GameObject item2 in gasStorage.items)
			{
				PrimaryElement component = item2.GetComponent<PrimaryElement>();
				if (!((Object)component == (Object)null) && !(component.Mass < 0.1f) && !(component.Temperature < minCooledTemperature))
				{
					float num4 = Mathf.Min(num, 10f);
					GameUtil.DeltaThermalEnergy(component, 0f - num4);
					num3 += num4;
				}
			}
			float num5 = Mathf.Abs(num3 * waterKGConsumedPerKJ);
			Game.Instance.accumulators.Accumulate(base.smi.master.waterConsumptionAccumulator, num5);
			if (num5 != 0f)
			{
				liquidStorage.ConsumeAndGetDisease(GameTags.Water, num5, out SimUtil.DiseaseInfo disease_info, out float _);
				SimMessages.ModifyDiseaseOnCell(Grid.PosToCell(base.gameObject), disease_info.idx, disease_info.count);
				UpdateMeter();
			}
		}
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.HEATCONSUMED, GameUtil.GetFormattedHeatEnergy(coolingKilowatts, GameUtil.HeatEnergyFormatterUnit.Automatic)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATCONSUMED, GameUtil.GetFormattedHeatEnergy(coolingKilowatts, GameUtil.HeatEnergyFormatterUnit.Automatic)), Descriptor.DescriptorType.Effect);
		list.Add(item);
		return list;
	}
}
