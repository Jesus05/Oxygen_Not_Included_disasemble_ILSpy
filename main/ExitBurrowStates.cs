using STRINGS;
using System.Runtime.CompilerServices;

internal class ExitBurrowStates : GameStateMachine<ExitBurrowStates, ExitBurrowStates.Instance, IStateMachineTarget, ExitBurrowStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToExitBurrow);
		}
	}

	private State exiting;

	private State behaviourcomplete;

	[CompilerGenerated]
	private static StateMachine<ExitBurrowStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = exiting;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.EMERGING.NAME, CREATURES.STATUSITEMS.EMERGING.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 63486, resolve_string_callback: null, resolve_tooltip_callback: null);
		exiting.PlayAnim("emerge").Enter(MoveToCellAbove).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToExitBurrow, false);
	}

	private static void MoveToCellAbove(Instance smi)
	{
		smi.transform.SetPosition(Grid.CellToPosCBC(Grid.CellAbove(Grid.PosToCell(smi.transform.GetPosition())), Grid.SceneLayer.Creatures));
	}
}
