public class IdleMonitor : GameStateMachine<IdleMonitor, IdleMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}

	public State idle;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		base.serializable = false;
		idle.ToggleRecurringChore(CreateIdleChore, null);
	}

	private Chore CreateIdleChore(Instance smi)
	{
		return new IdleChore(smi.master);
	}
}
