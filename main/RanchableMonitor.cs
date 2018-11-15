public class RanchableMonitor : GameStateMachine<RanchableMonitor, RanchableMonitor.Instance, IStateMachineTarget, RanchableMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public RanchStation.Instance targetRanchStation;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public bool ShouldGoGetRanched()
		{
			return targetRanchStation != null && targetRanchStation.IsRunning() && targetRanchStation.shouldCreatureGoGetRanched;
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.Creatures.WantsToGetRanched, (Instance smi) => smi.ShouldGoGetRanched(), null);
	}
}
