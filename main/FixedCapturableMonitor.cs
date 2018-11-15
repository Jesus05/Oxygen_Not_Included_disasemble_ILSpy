public class FixedCapturableMonitor : GameStateMachine<FixedCapturableMonitor, FixedCapturableMonitor.Instance, IStateMachineTarget, FixedCapturableMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public FixedCapturePoint.Instance targetCapturePoint;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public bool ShouldGoGetCaptured()
		{
			return targetCapturePoint != null && targetCapturePoint.IsRunning() && targetCapturePoint.shouldCreatureGoGetCaptured;
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.Creatures.WantsToGetCaptured, (Instance smi) => smi.ShouldGoGetCaptured(), null);
	}
}
