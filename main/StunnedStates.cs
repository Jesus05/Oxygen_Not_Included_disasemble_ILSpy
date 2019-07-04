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
		State root = base.root;
		string name = CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME;
		string tooltip = CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main);
		stunned.PlayAnim("idle_loop", KAnim.PlayMode.Loop).TagTransition(GameTags.Creatures.Stunned, null, true);
	}
}
