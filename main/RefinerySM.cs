public class RefinerySM : StateMachineComponent<RefinerySM.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RefinerySM, object>.GameInstance
	{
		public StatesInstance(RefinerySM master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RefinerySM>
	{
		public class IdleStates : State
		{
			public State idleQueue;

			public State waitingForMaterial;

			public State waitingForWorker;
		}

		public class OperatingStates : State
		{
			public State working_pre;

			public State working_loop;

			public State working_pst;

			public State working_pst_complete;
		}

		public IdleStates off;

		public IdleStates idle;

		public OperatingStates operating;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, idle, (StatesInstance smi) => smi.GetComponent<Operational>().IsOperational);
			idle.DefaultState(idle.idleQueue).PlayAnim("off").EventTransition(GameHashes.OperationalChanged, off, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational)
				.EventTransition(GameHashes.ActiveChanged, operating, (StatesInstance smi) => smi.GetComponent<Operational>().IsActive);
			idle.idleQueue.ToggleStatusItem(Db.Get().BuildingStatusItems.FabricatorEmpty, (object)null).EventTransition(GameHashes.FabricatorOrdersUpdated, idle.waitingForMaterial, (StatesInstance smi) => smi.master.refinery.NumOrders > 0);
			idle.waitingForMaterial.EventTransition(GameHashes.FabricatorOrdersUpdated, idle.waitingForWorker, (StatesInstance smi) => smi.master.refinery.WaitingForWorker);
			idle.waitingForWorker.ToggleStatusItem(Db.Get().BuildingStatusItems.PendingWork, (object)null).EventTransition(GameHashes.FabricatorOrdersUpdated, idle.idleQueue, (StatesInstance smi) => !smi.master.refinery.WaitingForWorker).EnterTransition(operating, (StatesInstance smi) => !smi.master.refinery.duplicantOperated);
			operating.DefaultState(operating.working_pre);
			operating.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(operating.working_loop);
			operating.working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, operating.working_pst, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, operating.working_pst, (StatesInstance smi) => !smi.GetComponent<Operational>().IsActive);
			operating.working_pst.PlayAnim("working_pst").WorkableCompleteTransition((StatesInstance smi) => smi.master.refinery.GetWorkable, operating.working_pst_complete).OnAnimQueueComplete(idle);
			operating.working_pst_complete.PlayAnim("working_pst_complete").OnAnimQueueComplete(idle);
		}
	}

	[MyCmpGet]
	private Refinery refinery;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
