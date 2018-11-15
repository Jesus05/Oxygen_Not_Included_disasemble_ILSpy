using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class EatStates : GameStateMachine<EatStates, EatStates.Instance, IStateMachineTarget, EatStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Element lastMealElement;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToEat);
		}

		public Element GetLatestMealElement()
		{
			return lastMealElement;
		}
	}

	public class EatingState : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public ApproachSubState<Pickupable> goingtoeat;

	public EatingState eating;

	public State behaviourcomplete;

	public TargetParameter target;

	[CompilerGenerated]
	private static StateMachine<EatStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static StateMachine<EatStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<EatStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static StateMachine<EatStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache4;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = goingtoeat;
		root.Enter(SetTarget).Enter(ReserveEdible).Exit(UnreserveEdible);
		goingtoeat.MoveTo(GetEdibleCell, eating, null, false).ToggleStatusItem(CREATURES.STATUSITEMS.LOOKINGFORFOOD.NAME, CREATURES.STATUSITEMS.LOOKINGFORFOOD.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: (NotificationType)0, allow_multiples: false, render_overlay: SimViewMode.None, status_overlays: 0, resolve_string_callback: null, resolve_tooltip_callback: null);
		eating.DefaultState(eating.pre).ToggleStatusItem(CREATURES.STATUSITEMS.EATING.NAME, CREATURES.STATUSITEMS.EATING.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: (NotificationType)0, allow_multiples: false, render_overlay: SimViewMode.None, status_overlays: 0, resolve_string_callback: null, resolve_tooltip_callback: null);
		eating.pre.QueueAnim("eat_pre", false, null).OnAnimQueueComplete(eating.loop);
		eating.loop.Enter(EatComplete).QueueAnim("eat_loop", true, null).ScheduleGoTo(3f, eating.pst);
		eating.pst.QueueAnim("eat_pst", false, null).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToEat, false);
	}

	private static void SetTarget(Instance smi)
	{
		smi.sm.target.Set(smi.GetSMI<SolidConsumerMonitor.Instance>().targetEdible, smi);
	}

	private static void ReserveEdible(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			DebugUtil.Assert(!gameObject.HasTag(GameTags.Creatures.ReservedByCreature), "Assert!", string.Empty, string.Empty);
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void UnreserveEdible(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			DebugUtil.Assert(gameObject.HasTag(GameTags.Creatures.ReservedByCreature), "Assert!", string.Empty, string.Empty);
			gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void EatComplete(Instance smi)
	{
		PrimaryElement primaryElement = smi.sm.target.Get<PrimaryElement>(smi);
		if ((UnityEngine.Object)primaryElement != (UnityEngine.Object)null)
		{
			smi.lastMealElement = primaryElement.Element;
		}
		smi.Trigger(1386391852, smi.sm.target.Get<KPrefabID>(smi));
	}

	private static int GetEdibleCell(Instance smi)
	{
		return Grid.PosToCell(smi.sm.target.Get(smi));
	}
}
