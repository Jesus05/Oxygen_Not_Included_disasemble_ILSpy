public class DropUnusedInventoryMonitor : GameStateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}

	public State satisfied;

	public State hasinventory;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.EventTransition(GameHashes.OnStorageChange, hasinventory, (Instance smi) => smi.GetComponent<Storage>().Count > 0);
		hasinventory.EventTransition(GameHashes.OnStorageChange, hasinventory, (Instance smi) => smi.GetComponent<Storage>().Count == 0).ToggleChore((Instance smi) => new DropUnusedInventoryChore(Db.Get().ChoreTypes.DropUnusedInventory, smi.master), satisfied);
	}
}
