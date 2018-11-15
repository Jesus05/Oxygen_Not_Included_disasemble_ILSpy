public class QuarantineFeedableMonitor : GameStateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool IsHungry()
		{
			return GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Eat);
		}
	}

	public State satisfied;

	public State hungry;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = true;
		satisfied.EventTransition(GameHashes.AddUrge, hungry, (Instance smi) => smi.IsHungry());
		hungry.EventTransition(GameHashes.RemoveUrge, satisfied, (Instance smi) => !smi.IsHungry());
	}
}
