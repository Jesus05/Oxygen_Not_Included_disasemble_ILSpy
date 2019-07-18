using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MoveToLureStates : GameStateMachine<MoveToLureStates, MoveToLureStates.Instance, IStateMachineTarget, MoveToLureStates.Def>
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

	public State arrive_at_lure;

	public State behaviourcomplete;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Func<Instance, CellOffset[]> _003C_003Ef__mg_0024cache1;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = move;
		State root = base.root;
		string name = CREATURES.STATUSITEMS.CONSIDERINGLURE.NAME;
		string tooltip = CREATURES.STATUSITEMS.CONSIDERINGLURE.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main);
		move.MoveTo(GetLureCell, GetLureOffsets, arrive_at_lure, behaviourcomplete, false);
		arrive_at_lure.Enter(delegate(Instance smi)
		{
			Lure.Instance targetLure = GetTargetLure(smi);
			if (targetLure != null && targetLure.HasTag(GameTags.OneTimeUseLure))
			{
				targetLure.GetComponent<KPrefabID>().AddTag(GameTags.LureUsed, false);
			}
		}).GoTo(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.MoveToLure, false);
	}

	private static Lure.Instance GetTargetLure(Instance smi)
	{
		LureableMonitor.Instance sMI = smi.GetSMI<LureableMonitor.Instance>();
		GameObject targetLure = sMI.GetTargetLure();
		if (!((UnityEngine.Object)targetLure == (UnityEngine.Object)null))
		{
			return targetLure.GetSMI<Lure.Instance>();
		}
		return null;
	}

	private static int GetLureCell(Instance smi)
	{
		Lure.Instance targetLure = GetTargetLure(smi);
		if (targetLure != null)
		{
			return Grid.PosToCell(targetLure);
		}
		return Grid.InvalidCell;
	}

	private static CellOffset[] GetLureOffsets(Instance smi)
	{
		Lure.Instance targetLure = GetTargetLure(smi);
		if (targetLure != null)
		{
			return targetLure.def.lurePoints;
		}
		return null;
	}
}
