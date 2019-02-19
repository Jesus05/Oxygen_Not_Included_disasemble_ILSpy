using STRINGS;
using System;
using System.Runtime.CompilerServices;

internal class DebugGoToStates : GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.HasDebugDestination);
		}
	}

	public State moving;

	public State behaviourcomplete;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = moving;
		moving.MoveTo(GetTargetCell, behaviourcomplete, behaviourcomplete, true).ToggleStatusItem(CREATURES.STATUSITEMS.DEBUGGOTO.NAME, CREATURES.STATUSITEMS.DEBUGGOTO.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 63486, resolve_string_callback: null, resolve_tooltip_callback: null);
		behaviourcomplete.BehaviourComplete(GameTags.HasDebugDestination, false);
	}

	private static int GetTargetCell(Instance smi)
	{
		return smi.GetSMI<CreatureDebugGoToMonitor.Instance>().targetCell;
	}
}
