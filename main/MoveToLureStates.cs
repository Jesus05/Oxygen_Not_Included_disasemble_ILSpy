using STRINGS;
using System;
using System.Runtime.CompilerServices;

internal class MoveToLureStates : GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.MoveToLure);
		}
	}

	public State move;

	public State behaviourcomplete;

	public TargetParameter target;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<Instance, CellOffset[]> _003C_003Ef__mg_0024cache1;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = move;
		State state = root.Enter("SetLure", delegate(Instance smi)
		{
			target.Set(smi.GetSMI<LureableMonitor.Instance>().GetTargetLure(), smi);
		});
		string name = CREATURES.STATUSITEMS.CONSIDERINGLURE.NAME;
		string tooltip = CREATURES.STATUSITEMS.CONSIDERINGLURE.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 63486, null, null, main);
		move.MoveTo(GetLureCell, GetLureOffsets, behaviourcomplete, null, false);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.MoveToLure, false);
	}

	private static Lure.Instance GetTargetLure(Instance smi)
	{
		return smi.GetSMI<LureableMonitor.Instance>().GetTargetLure().GetSMI<Lure.Instance>();
	}

	private static int GetLureCell(Instance smi)
	{
		return Grid.PosToCell(GetTargetLure(smi).transform.GetPosition());
	}

	private static CellOffset[] GetLureOffsets(Instance smi)
	{
		return GetTargetLure(smi).def.lurePoints;
	}
}
