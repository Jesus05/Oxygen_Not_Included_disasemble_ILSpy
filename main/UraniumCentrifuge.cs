using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class UraniumCentrifuge : StateMachineComponent<UraniumCentrifuge.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, UraniumCentrifuge, object>.GameInstance
	{
		public StatesInstance(UraniumCentrifuge smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, UraniumCentrifuge>
	{
		public State disabled;

		public State waiting;

		public State converting;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disabled;
			root.EventTransition(GameHashes.OperationalChanged, disabled, (StatesInstance smi) => !smi.master.operational.IsOperational);
			disabled.EventTransition(GameHashes.OperationalChanged, waiting, (StatesInstance smi) => smi.master.operational.IsOperational);
			waiting.EventTransition(GameHashes.OnStorageChange, converting, (StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			converting.Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).Transition(waiting, (StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll(), UpdateRate.SIM_200ms);
		}
	}

	[MyCmpAdd]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}
}
