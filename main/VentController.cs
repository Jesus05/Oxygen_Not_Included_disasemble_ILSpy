public class VentController : GameStateMachine<VentController, VentController.Instance>
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

	public BoolParameter isAnimating;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		off.PlayAnim("off").EventTransition(GameHashes.VentAnimatingChanged, working_pre, (Instance smi) => smi.GetComponent<Exhaust>().IsAnimating());
		working_pre.PlayAnim("working_pre").OnAnimQueueComplete(working_loop);
		working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.VentAnimatingChanged, working_pst, (Instance smi) => !smi.GetComponent<Exhaust>().IsAnimating());
		working_pst.PlayAnim("working_pst").OnAnimQueueComplete(off);
	}
}
