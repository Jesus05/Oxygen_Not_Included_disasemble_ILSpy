using STRINGS;
using System;
using System.Runtime.CompilerServices;

public class DebugGoToStates : GameStateMachine<DebugGoToStates, DebugGoToStates.Instance, IStateMachineTarget, DebugGoToStates.Def>
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
		State state = moving.MoveTo(GetTargetCell, behaviourcomplete, behaviourcomplete, true);
		string name = CREATURES.STATUSITEMS.DEBUGGOTO.NAME;
		string tooltip = CREATURES.STATUSITEMS.DEBUGGOTO.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main);
		behaviourcomplete.BehaviourComplete(GameTags.HasDebugDestination, false);
	}

	private static int GetTargetCell(Instance smi)
	{
		return smi.GetSMI<CreatureDebugGoToMonitor.Instance>().targetCell;
	}
}
