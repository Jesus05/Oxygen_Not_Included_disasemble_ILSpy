public class FabricatorSM : StateMachineComponent<FabricatorSM.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, FabricatorSM, object>.GameInstance
	{
		public StatesInstance(FabricatorSM master)
			: base(master)
		{
			master.fabricator = master.GetComponent<IHasBuildQueue>();
		}
	}

	public class States : GameStateMachine<States, StatesInstance, FabricatorSM>
	{
		public State idleQueue;

		public State waitingForMaterial;

		public State waitingForWorker;

		public State operating;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idleQueue;
			root.Transition(idleQueue, (StatesInstance smi) => smi.master.fabricator.NumOrders == 0, UpdateRate.SIM_200ms);
			idleQueue.ToggleStatusItem(Db.Get().BuildingStatusItems.FabricatorEmpty, (object)null).Transition(waitingForMaterial, (StatesInstance smi) => smi.master.fabricator.NumOrders > 0, UpdateRate.SIM_200ms);
			waitingForMaterial.Transition(waitingForWorker, (StatesInstance smi) => smi.master.fabricator.WaitingForWorker, UpdateRate.SIM_200ms);
			waitingForWorker.ToggleStatusItem(Db.Get().BuildingStatusItems.PendingWork, (object)null).Transition(waitingForMaterial, (StatesInstance smi) => !smi.master.fabricator.WaitingForWorker, UpdateRate.SIM_200ms).Transition(operating, (StatesInstance smi) => smi.master.fabricator.HasWorker, UpdateRate.SIM_200ms);
			operating.Transition(waitingForWorker, (StatesInstance smi) => !smi.master.fabricator.HasWorker, UpdateRate.SIM_200ms);
		}
	}

	private IHasBuildQueue fabricator;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
