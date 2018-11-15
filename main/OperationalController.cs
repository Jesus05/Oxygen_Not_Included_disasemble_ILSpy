public class OperationalController : GameStateMachine<OperationalController, OperationalController.Instance>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, (object)def)
		{
		}
	}

	public State off;

	public State working_pre;

	public State working_loop;

	public State working_pst;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		root.EventTransition(GameHashes.OperationalChanged, off, (Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, working_pre, (Instance smi) => smi.GetComponent<Operational>().IsOperational);
		working_pre.PlayAnim("working_pre").OnAnimQueueComplete(working_loop);
		working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, working_pst, (Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		working_pst.PlayAnim("working_pst").OnAnimQueueComplete(off);
	}
}
