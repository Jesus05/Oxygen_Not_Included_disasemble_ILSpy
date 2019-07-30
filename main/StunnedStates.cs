using STRINGS;

public class StunnedStates : GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public static readonly Chore.Precondition IsStunned = new Chore.Precondition
		{
			id = "IsStunned",
			fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Creatures.Stunned);
			}
		};

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(IsStunned, null);
		}
	}

	public State stunned;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = stunned;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME, CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 129022, resolve_string_callback: null, resolve_tooltip_callback: null);
		stunned.PlayAnim("idle_loop", KAnim.PlayMode.Loop).TagTransition(GameTags.Creatures.Stunned, null, true);
	}
}
