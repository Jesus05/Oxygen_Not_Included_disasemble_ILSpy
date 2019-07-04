using STRINGS;
using System.Runtime.CompilerServices;

public class CreatureSleepStates : GameStateMachine<CreatureSleepStates, CreatureSleepStates.Instance, IStateMachineTarget, CreatureSleepStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.SleepBehaviour);
		}
	}

	public State pre;

	public State loop;

	public State pst;

	public State behaviourcomplete;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = pre;
		State root = base.root;
		string name = CREATURES.STATUSITEMS.SLEEPING.NAME;
		string tooltip = CREATURES.STATUSITEMS.SLEEPING.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main);
		pre.QueueAnim("sleep_pre", false, null).OnAnimQueueComplete(loop);
		loop.QueueAnim("sleep_loop", true, null).Transition(pst, ShouldWakeUp, UpdateRate.SIM_1000ms);
		pst.QueueAnim("sleep_pst", false, null).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.SleepBehaviour, false);
	}

	public static bool ShouldWakeUp(Instance smi)
	{
		return !GameClock.Instance.IsNighttime();
	}
}
