using STRINGS;

internal class DropElementStates : GameStateMachine<DropElementStates, DropElementStates.Instance, IStateMachineTarget, DropElementStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToDropElements);
		}
	}

	public State dropping;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = dropping;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.EXPELLING_GAS.NAME, CREATURES.STATUSITEMS.EXPELLING_GAS.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: SimViewMode.None, status_overlays: 63486, resolve_string_callback: null, resolve_tooltip_callback: null);
		dropping.PlayAnim("dirty").OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.Enter("DropElement", delegate(Instance smi)
		{
			smi.GetSMI<ElementDropperMonitor.Instance>().DropPeriodicElement();
		}).QueueAnim("idle_loop", true, null).BehaviourComplete(GameTags.Creatures.WantsToDropElements, false);
	}
}
