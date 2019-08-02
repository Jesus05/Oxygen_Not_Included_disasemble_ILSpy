using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class IceCooledFan : StateMachineComponent<IceCooledFan.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, IceCooledFan, object>.GameInstance
	{
		public StatesInstance(IceCooledFan smi)
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

	public class States : GameStateMachine<States, StatesInstance, IceCooledFan>
	{
		public class Workable : State
		{
			public State waiting;

			public State cooling;
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
			workable.ToggleChore(CreateUseChore, work_pst).EventTransition(GameHashes.ActiveChanged, workable.cooling, (StatesInstance smi) => (Object)smi.master.workable.worker != (Object)null).EventTransition(GameHashes.OperationalChanged, workable.cooling, (StatesInstance smi) => (Object)smi.master.workable.worker != (Object)null)
				.Transition(unworkable, (StatesInstance smi) => !smi.IsWorkable(), UpdateRate.SIM_200ms);
			workable.cooling.EventTransition(GameHashes.OperationalChanged, unworkable, (StatesInstance smi) => (Object)smi.master.workable.worker == (Object)null).EventHandler(GameHashes.ActiveChanged, delegate(StatesInstance smi)
			{
				smi.master.CheckWorking();
			}).Enter(delegate(StatesInstance smi)
			{
				smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(true, "Working");
				if (!smi.EnvironmentNeedsCooling() || !smi.master.HasMaterial() || !smi.EnvironmentHighEnoughPressure())
				{
					smi.GoTo(unworkable);
				}
			})
				.Update("IceCooledFanCooling", delegate(StatesInstance smi, float dt)
				{
					smi.master.DoCooling(dt);
				}, UpdateRate.SIM_200ms, false)
				.Exit(delegate(StatesInstance smi)
				{
					if (!smi.master.HasMaterial())
					{
						smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(false, "Working");
					}
					smi.master.liquidStorage.DropAll(false, false, default(Vector3), true);
				});
			work_pst.ScheduleGoTo(2f, unworkable);
			unworkable.Update("IceFanUnworkableStatusItems", delegate(StatesInstance smi, float dt)
			{
				smi.master.UpdateUnworkableStatusItems();
			}, UpdateRate.SIM_200ms, false).Transition(workable.waiting, (StatesInstance smi) => smi.IsWorkable(), UpdateRate.SIM_200ms).Enter(delegate(StatesInstance smi)
			{
				smi.master.UpdateUnworkableStatusItems();
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.UpdateUnworkableStatusItems();
				});
		}

		private Chore CreateUseChore(StatesInstance smi)
		{
			return new WorkChore<IceCooledFanWorkable>(Db.Get().ChoreTypes.IceCooledFan, smi.master.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		}
	}

	[SerializeField]
	public float minCooledTemperature;

	[SerializeField]
	public float minEnvironmentMass;

	[SerializeField]
	public float coolingRate;

	[SerializeField]
	public float targetTemperature;

	[SerializeField]
	public Vector2I minCoolingRange;

	[SerializeField]
	public Vector2I maxCoolingRange;

	[SerializeField]
	public Storage iceStorage;

	[SerializeField]
	public Storage liquidStorage;

	[SerializeField]
	public Tag consumptionTag;

	private float LOW_ICE_TEMP = 173.15f;

	[MyCmpAdd]
	private IceCooledFanWorkable workable;

	[MyCmpGet]
	private Operational operational;

	private MeterController meter;

	public bool HasMaterial()
	{
		UpdateMeter();
		return iceStorage.MassStored() > 0f;
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
		base.smi.StartSM();
		GetComponent<ManualDeliveryKG>().SetStorage(iceStorage);
	}

	private void UpdateMeter()
	{
		float num = 0f;
		foreach (GameObject item in iceStorage.items)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			num += component.Temperature;
		}
		num /= (float)iceStorage.items.Count;
		float num2 = Mathf.Clamp01((num - LOW_ICE_TEMP) / (targetTemperature - LOW_ICE_TEMP));
		meter.SetPositionPercent(1f - num2);
	}

	private void DoCooling(float dt)
	{
		float kilowatts = coolingRate * dt;
		foreach (GameObject item in iceStorage.items)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			GameUtil.DeltaThermalEnergy(component, kilowatts, targetTemperature);
		}
		for (int num = iceStorage.items.Count; num > 0; num--)
		{
			GameObject gameObject = iceStorage.items[num - 1];
			if ((Object)gameObject != (Object)null && gameObject.GetComponent<PrimaryElement>().Temperature > gameObject.GetComponent<PrimaryElement>().Element.highTemp && gameObject.GetComponent<PrimaryElement>().Element.HasTransitionUp)
			{
				PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
				iceStorage.AddLiquid(component2.Element.highTempTransitionTarget, component2.Mass, component2.Temperature, component2.DiseaseIdx, component2.DiseaseCount, false, true);
				iceStorage.ConsumeIgnoringDisease(gameObject);
			}
		}
		for (int num2 = iceStorage.items.Count; num2 > 0; num2--)
		{
			GameObject gameObject2 = iceStorage.items[num2 - 1];
			if ((Object)gameObject2 != (Object)null && gameObject2.GetComponent<PrimaryElement>().Temperature >= targetTemperature)
			{
				iceStorage.Transfer(gameObject2, liquidStorage, true, true);
			}
		}
		if (!liquidStorage.IsEmpty())
		{
			Storage storage = liquidStorage;
			Vector3 offset = new Vector3(1f, 0f, 0f);
			storage.DropAll(false, false, offset, true);
		}
		UpdateMeter();
	}
}
