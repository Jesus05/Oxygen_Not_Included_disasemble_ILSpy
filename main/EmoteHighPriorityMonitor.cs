public class EmoteHighPriorityMonitor : GameStateMachine<EmoteHighPriorityMonitor, EmoteHighPriorityMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void OnStartChore(object o)
		{
			Chore chore = (Chore)o;
			if (chore.SatisfiesUrge(Db.Get().Urges.EmoteHighPriority))
			{
				GoTo(base.sm.resetting);
			}
		}
	}

	public State ready;

	public State resetting;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = ready;
		base.serializable = true;
		ready.ToggleUrge(Db.Get().Urges.EmoteHighPriority).EventHandler(GameHashes.BeginChore, delegate(Instance smi, object o)
		{
			smi.OnStartChore(o);
		});
		resetting.GoTo(ready);
	}
}
