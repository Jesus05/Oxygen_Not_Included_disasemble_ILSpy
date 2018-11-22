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
		State root = base.root;
		string name = CREATURES.STATUSITEMS.TRAPPED.NAME;
		string tooltip = CREATURES.STATUSITEMS.TRAPPED.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 63486, null, null, main);
		trapped.ToggleTag(GameTags.Creatures.Deliverable).PlayAnim("trapped", KAnim.PlayMode.Loop).TagTransition(GameTags.Trapped, null, true);
	}
}
