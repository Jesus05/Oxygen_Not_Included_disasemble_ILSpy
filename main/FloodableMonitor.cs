public class FloodableMonitor : GameStateMachine<FloodableMonitor, FloodableMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}

	public State satisfied;

	public State flooded;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = false;
		satisfied.DoNothing();
		flooded.DoNothing();
	}
}
