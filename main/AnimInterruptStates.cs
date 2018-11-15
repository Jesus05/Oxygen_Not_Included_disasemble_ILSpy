internal class AnimInterruptStates : GameStateMachine<AnimInterruptStates, AnimInterruptStates.Instance, IStateMachineTarget, AnimInterruptStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.PlayInterruptAnim);
		}
	}

	public State play_anim;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = play_anim;
		play_anim.Enter(PlayAnim).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.PlayInterruptAnim, false);
	}

	private void PlayAnim(Instance smi)
	{
		smi.Get<KBatchedAnimController>().Play(smi.GetSMI<AnimInterruptMonitor.Instance>().anim, KAnim.PlayMode.Once, 1f, 0f);
	}
}
