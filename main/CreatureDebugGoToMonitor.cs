using System;
using System.Runtime.CompilerServices;

public class CreatureDebugGoToMonitor : GameStateMachine<CreatureDebugGoToMonitor, CreatureDebugGoToMonitor.Instance, IStateMachineTarget, CreatureDebugGoToMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public int targetCell = Grid.InvalidCell;

		public Instance(IStateMachineTarget target, Def def)
			: base(target, def)
		{
		}

		public void GoToCursor()
		{
			targetCell = DebugHandler.GetMouseCell();
		}
	}

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<Instance> _003C_003Ef__mg_0024cache1;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.HasDebugDestination, HasTargetCell, ClearTargetCell);
	}

	private static bool HasTargetCell(Instance smi)
	{
		return smi.targetCell != Grid.InvalidCell;
	}

	private static void ClearTargetCell(Instance smi)
	{
		smi.targetCell = Grid.InvalidCell;
	}
}
