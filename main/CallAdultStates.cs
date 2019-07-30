using STRINGS;

public class CallAdultStates : GameStateMachine<CallAdultStates, CallAdultStates.Instance, IStateMachineTarget, CallAdultStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.CallAdultBehaviour);
		}
	}

	public State pre;

	public State loop;

	public State pst;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = pre;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.SLEEPING.NAME, CREATURES.STATUSITEMS.SLEEPING.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 129022, resolve_string_callback: null, resolve_tooltip_callback: null);
		pre.QueueAnim("call_pre", false, null).OnAnimQueueComplete(loop);
		loop.QueueAnim("call_loop", false, null).OnAnimQueueComplete(pst);
		pst.QueueAnim("call_pst", false, null).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.CallAdultBehaviour, false);
	}
}
