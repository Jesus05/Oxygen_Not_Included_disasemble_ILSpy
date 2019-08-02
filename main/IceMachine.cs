using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class IceMachine : StateMachineComponent<IceMachine.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, IceMachine, object>.GameInstance
	{
		private MeterController meter;

		public Chore emptyChore;

		public StatesInstance(IceMachine smi)
			: base(smi)
		{
			meter = new MeterController(base.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_OL", "meter_frame", "meter_fill");
			UpdateMeter();
			Subscribe(-1697596308, OnStorageChange);
		}

		private void OnStorageChange(object data)
		{
			UpdateMeter();
		}

		public void UpdateMeter()
		{
			meter.SetPositionPercent(Mathf.Clamp01(base.smi.master.iceStorage.MassStored() / base.smi.master.iceStorage.Capacity()));
		}

		public void UpdateIceState()
		{
			bool value = false;
			for (int num = base.smi.master.waterStorage.items.Count; num > 0; num--)
			{
				GameObject gameObject = base.smi.master.waterStorage.items[num - 1];
				if ((bool)gameObject && gameObject.GetComponent<PrimaryElement>().Temperature <= base.smi.master.targetTemperature)
				{
					value = true;
				}
			}
			base.sm.doneFreezingIce.Set(value, this);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, IceMachine>
	{
		public class OnStates : State
		{
			public State waiting;

			public State working_pre;

			public State working;

			public State working_pst;
		}

		public BoolParameter doneFreezingIce;

		public State off;

		public OnStates on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			base.serializable = true;
			off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (StatesInstance smi) => smi.master.operational.IsOperational);
			on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, off, (StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(on.waiting);
			on.waiting.EventTransition(GameHashes.OnStorageChange, on.working_pre, (StatesInstance smi) => smi.master.CanMakeIce());
			on.working_pre.Enter(delegate(StatesInstance smi)
			{
				smi.UpdateIceState();
			}).PlayAnim("working_pre").OnAnimQueueComplete(on.working);
			on.working.QueueAnim("working_loop", true, null).Update("UpdateWorking", delegate(StatesInstance smi, float dt)
			{
				smi.master.MakeIce(smi, dt);
			}, UpdateRate.SIM_200ms, false).ParamTransition(doneFreezingIce, on.working_pst, GameStateMachine<States, StatesInstance, IceMachine, object>.IsTrue)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(true, false);
					smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(true, "Working");
				})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
					smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(false, "Done Working");
				});
			on.working_pst.Exit(DoTransfer).PlayAnim("working_pst").OnAnimQueueComplete(on);
		}

		private void DoTransfer(StatesInstance smi)
		{
			for (int num = smi.master.waterStorage.items.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = smi.master.waterStorage.items[num];
				if ((bool)gameObject && gameObject.GetComponent<PrimaryElement>().Temperature <= smi.master.targetTemperature)
				{
					smi.master.waterStorage.Transfer(gameObject, smi.master.iceStorage, false, true);
				}
			}
			smi.UpdateMeter();
		}
	}

	[MyCmpGet]
	private Operational operational;

	public Storage waterStorage;

	public Storage iceStorage;

	public float targetTemperature;

	public float heatRemovalRate;

	private static StatusItem iceStorageFullStatusItem;

	public void SetStorages(Storage waterStorage, Storage iceStorage)
	{
		this.waterStorage = waterStorage;
		this.iceStorage = iceStorage;
	}

	private bool CanMakeIce()
	{
		bool flag = (Object)waterStorage != (Object)null && waterStorage.GetMassAvailable(SimHashes.Water) >= 0.1f;
		bool flag2 = (Object)iceStorage != (Object)null && iceStorage.IsFull();
		return flag && !flag2;
	}

	private void MakeIce(StatesInstance smi, float dt)
	{
		float num = heatRemovalRate * dt / (float)waterStorage.items.Count;
		foreach (GameObject item in waterStorage.items)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			GameUtil.DeltaThermalEnergy(component, 0f - num, smi.master.targetTemperature);
		}
		for (int num2 = waterStorage.items.Count; num2 > 0; num2--)
		{
			GameObject gameObject = waterStorage.items[num2 - 1];
			if ((bool)gameObject && gameObject.GetComponent<PrimaryElement>().Temperature < gameObject.GetComponent<PrimaryElement>().Element.lowTemp)
			{
				PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
				waterStorage.AddOre(component2.Element.lowTempTransitionTarget, component2.Mass, component2.Temperature, component2.DiseaseIdx, component2.DiseaseCount, false, true);
				waterStorage.ConsumeIgnoringDisease(gameObject);
			}
		}
		smi.UpdateIceState();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
