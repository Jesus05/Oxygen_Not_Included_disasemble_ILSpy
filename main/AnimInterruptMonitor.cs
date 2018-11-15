using System;
using System.Runtime.CompilerServices;

public class AnimInterruptMonitor : GameStateMachine<AnimInterruptMonitor, AnimInterruptMonitor.Instance, IStateMachineTarget, AnimInterruptMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public HashedString anim;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public void PlayAnim(HashedString anim)
		{
			this.anim = anim;
			GetComponent<CreatureBrain>().UpdateBrain();
		}
	}

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<Instance> _003C_003Ef__mg_0024cache1;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.Creatures.Behaviours.PlayInterruptAnim, ShoulPlayAnim, ClearAnim);
	}

	private static bool ShoulPlayAnim(Instance smi)
	{
		return smi.anim.IsValid;
	}

	private static void ClearAnim(Instance smi)
	{
		smi.anim = HashedString.Invalid;
	}
}
