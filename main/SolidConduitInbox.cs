using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidConduitInbox : StateMachineComponent<SolidConduitInbox.SMInstance>, ISim1000ms
{
	public class SMInstance : GameStateMachine<States, SMInstance, SolidConduitInbox, object>.GameInstance
	{
		public SMInstance(SolidConduitInbox master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, SolidConduitInbox>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State working;

			public State post;
		}

		public State off;

		public ReadyStates on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			root.DoNothing();
			off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (SMInstance smi) => smi.GetComponent<Operational>().IsOperational);
			on.DefaultState(on.idle).EventTransition(GameHashes.OperationalChanged, off, (SMInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			on.idle.PlayAnim("on").EventTransition(GameHashes.ActiveChanged, on.working, (SMInstance smi) => smi.GetComponent<Operational>().IsActive);
			on.working.PlayAnim("working_pre").QueueAnim("working_loop", true, null).EventTransition(GameHashes.ActiveChanged, on.post, (SMInstance smi) => !smi.GetComponent<Operational>().IsActive);
			on.post.PlayAnim("working_pst").OnAnimQueueComplete(on);
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private SolidConduitDispenser dispenser;

	[MyCmpAdd]
	private Storage storage;

	private FilteredStorage filteredStorage;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		filteredStorage = new FilteredStorage(this, null, null, null, false, Db.Get().ChoreTypes.StorageFetch);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		filteredStorage.FilterChanged();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void Sim1000ms(float dt)
	{
		if (operational.IsOperational && dispenser.IsDispensing)
		{
			operational.SetActive(true, false);
		}
		else
		{
			operational.SetActive(false, false);
		}
	}
}
