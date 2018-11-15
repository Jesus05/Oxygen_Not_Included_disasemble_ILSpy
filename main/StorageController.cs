public class StorageController : GameStateMachine<StorageController, StorageController.Instance>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master)
		{
		}
	}

	public State off;

	public State on;

	public State working;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		root.EventTransition(GameHashes.OnStorageInteracted, working, null);
		off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (Instance smi) => smi.GetComponent<Operational>().IsOperational);
		on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, off, (Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		working.PlayAnim("working").OnAnimQueueComplete(off);
	}
}
