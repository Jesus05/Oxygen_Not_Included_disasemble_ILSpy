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
		root.ToggleStatusItem(CREATURES.STATUSITEMS.CONSIDERINGLURE.NAME, CREATURES.STATUSITEMS.CONSIDERINGLURE.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 129022, resolve_string_callback: null, resolve_tooltip_callback: null);
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
		if ((UnityEngine.Object)targetLure == (UnityEngine.Object)null)
		{
			return null;
		}
		return targetLure.GetSMI<Lure.Instance>();
	}

	private static int GetLureCell(Instance smi)
	{
		Lure.Instance targetLure = GetTargetLure(smi);
		if (targetLure == null)
		{
			return Grid.InvalidCell;
		}
		return Grid.PosToCell(targetLure);
	}

	private static CellOffset[] GetLureOffsets(Instance smi)
	{
		Lure.Instance targetLure = GetTargetLure(smi);
		if (targetLure == null)
		{
			return null;
		}
		return targetLure.def.lurePoints;
	}
}
