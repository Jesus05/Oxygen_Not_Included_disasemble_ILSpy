using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class IceMachine : StateMachineComponent<IceMachine.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, IceMachine, object>.GameInstance
	{
		public StatesInstance(IceMachine smi)
			: base(smi)
		{
		}

		public void UpdateIceState()
		{
			base.sm.shouldDropIce.Set(!base.smi.master.iceStorage.IsEmpty(), this);
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

		public BoolParameter shouldDropIce;

		public State off;

		public OnStates on;

		private static readonly HashedString[] FULL_ANIMS = new HashedString[2]
		{
			"working_pst",
			"off"
		};

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (StatesInstance smi) => smi.master.operational.IsOperational);
			on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, off, (StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(on.waiting);
			on.waiting.EventTransition(GameHashes.OnStorageChange, on.working_pre, (StatesInstance smi) => smi.master.CanMakeIce());
			on.working_pre.Enter(delegate(StatesInstance smi)
			{
				smi.UpdateIceState();
			}).PlayAnim("working_pre").OnAnimQueueComplete(on.working);
			on.working.Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
				smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(true, "Working");
			}).QueueAnim("working_loop", true, null).Update("UpdateWorking", delegate(StatesInstance smi, float dt)
			{
				smi.master.MakeIce(smi, dt);
			}, UpdateRate.SIM_200ms, false)
				.ParamTransition(shouldDropIce, on.working_pst, GameStateMachine<States, StatesInstance, IceMachine, object>.IsTrue)
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
					smi.master.gameObject.GetComponent<ManualDeliveryKG>().Pause(false, "Done Working");
				});
			on.working_pst.Exit(delegate(StatesInstance smi)
			{
				Storage iceStorage = smi.master.iceStorage;
				Vector3 offset = new Vector3(1f, 0f, 0f);
				iceStorage.DropAll(false, false, offset, true);
			}).PlayAnim("working_pst").OnAnimQueueComplete(on.waiting);
		}
	}

	[MyCmpGet]
	private Operational operational;

	private ManualDeliveryKG[] deliveryComponents;

	public Storage waterStorage;

	public Storage iceStorage;

	public float targetTemperature;

	public float energyConsumption;

	public float energyWaste;

	public void SetStorages(Storage waterStorage, Storage iceStorage)
	{
		this.waterStorage = waterStorage;
		this.iceStorage = iceStorage;
	}

	private bool CanMakeIce()
	{
		return (Object)waterStorage != (Object)null && !waterStorage.IsEmpty();
	}

	private void MakeIce(StatesInstance smi, float dt)
	{
		float num = (energyConsumption - energyWaste) * dt / (float)waterStorage.items.Count;
		foreach (GameObject item in waterStorage.items)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			GameUtil.DeltaThermalEnergy(component, 0f - num);
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
		for (int num3 = waterStorage.items.Count; num3 > 0; num3--)
		{
			GameObject gameObject2 = waterStorage.items[num3 - 1];
			if ((bool)gameObject2 && gameObject2.GetComponent<PrimaryElement>().Temperature <= targetTemperature)
			{
				waterStorage.Transfer(gameObject2, iceStorage, true, true);
			}
		}
		smi.UpdateIceState();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		deliveryComponents = GetComponents<ManualDeliveryKG>();
		base.smi.StartSM();
	}
}
