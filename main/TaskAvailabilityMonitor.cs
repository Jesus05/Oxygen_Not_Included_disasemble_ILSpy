public class TaskAvailabilityMonitor : GameStateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void RefreshStatusItem()
		{
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Idle, null);
		}
	}

	public State satisfied;

	public State unavailable;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.EventTransition(GameHashes.NewDay, (Instance smi) => GameClock.Instance, unavailable, (Instance smi) => GameClock.Instance.GetCycle() > 0);
		unavailable.Enter("RefreshStatusItem", delegate(Instance smi)
		{
			smi.RefreshStatusItem();
		}).EventHandler(GameHashes.ScheduleChanged, delegate(Instance smi)
		{
			smi.RefreshStatusItem();
		});
	}
}
