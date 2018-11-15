using STRINGS;
using System.Runtime.CompilerServices;

internal class CreatureSleepStates : GameStateMachine<CreatureSleepStates, CreatureSleepStates.Instance, IStateMachineTarget, CreatureSleepStates.Def>
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
		root.ToggleStatusItem(CREATURES.STATUSITEMS.SLEEPING.NAME, CREATURES.STATUSITEMS.SLEEPING.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: SimViewMode.None, status_overlays: 63486, resolve_string_callback: null, resolve_tooltip_callback: null);
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
