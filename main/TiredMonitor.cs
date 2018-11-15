public class TiredMonitor : GameStateMachine<TiredMonitor, TiredMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public int disturbedDay = -1;

		public int interruptedDay = -1;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void SetInterruptDay()
		{
			interruptedDay = GameClock.Instance.GetCycle();
		}

		public bool AllowInterruptClear()
		{
			bool flag = GameClock.Instance.GetCycle() > interruptedDay + 1;
			if (flag)
			{
				interruptedDay = -1;
			}
			return flag;
		}
	}

	public State tired;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.EventTransition(GameHashes.SleepFail, tired, null);
		tired.Enter(delegate(Instance smi)
		{
			smi.SetInterruptDay();
		}).EventTransition(GameHashes.NewDay, (Instance smi) => GameClock.Instance, root, (Instance smi) => smi.AllowInterruptClear()).ToggleExpression(Db.Get().Expressions.Tired, null)
			.ToggleAnims("anim_loco_walk_slouch_kanim", 0f)
			.ToggleAnims("anim_idle_slouch_kanim", 0f);
	}
}
