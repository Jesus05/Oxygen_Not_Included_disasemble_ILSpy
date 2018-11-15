public class PoweredController : GameStateMachine<PoweredController, PoweredController.Instance>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public bool ShowWorkingStatus;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, (object)def)
		{
		}
	}

	public State off;

	public State on;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (Instance smi) => smi.GetComponent<Operational>().IsOperational);
		on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, off, (Instance smi) => !smi.GetComponent<Operational>().IsOperational);
	}
}
