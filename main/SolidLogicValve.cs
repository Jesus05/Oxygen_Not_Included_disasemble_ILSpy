using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidLogicValve : StateMachineComponent<SolidLogicValve.StatesInstance>, ISim200ms
{
	public class States : GameStateMachine<States, StatesInstance, SolidLogicValve>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State working;
		}

		public State off;

		public ReadyStates on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			root.DoNothing();
			off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			on.DefaultState(on.idle).EventTransition(GameHashes.OperationalChanged, off, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			on.idle.PlayAnim("on").EventTransition(GameHashes.ActiveChanged, on.working, (StatesInstance smi) => smi.GetComponent<Operational>().IsActive);
			on.working.PlayAnim("on_flow", KAnim.PlayMode.Loop).EventTransition(GameHashes.ActiveChanged, on.idle, (StatesInstance smi) => !smi.GetComponent<Operational>().IsActive);
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, SolidLogicValve, object>.GameInstance
	{
		public StatesInstance(SolidLogicValve master)
			: base(master)
		{
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private SolidConduitBridge bridge;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void Sim200ms(float dt)
	{
		if (operational.IsOperational && bridge.IsDispensing)
		{
			operational.SetActive(true, false);
		}
		else
		{
			operational.SetActive(false, false);
		}
	}
}
