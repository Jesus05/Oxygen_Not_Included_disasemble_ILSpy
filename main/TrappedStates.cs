using STRINGS;

internal class TrappedStates : GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public static readonly Chore.Precondition IsTrapped = new Chore.Precondition
		{
			id = "IsTrapped",
			fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Trapped);
			}
		};

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(IsTrapped, null);
		}
	}

	private State trapped;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = trapped;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.TRAPPED.NAME, CREATURES.STATUSITEMS.TRAPPED.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 63486, resolve_string_callback: null, resolve_tooltip_callback: null);
		trapped.ToggleTag(GameTags.Creatures.Deliverable).PlayAnim("trapped", KAnim.PlayMode.Loop).TagTransition(GameTags.Trapped, null, true);
	}
}
