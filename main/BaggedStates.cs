using KSerialization;
using STRINGS;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BaggedStates : GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>
{
	public class Def : BaseDef
	{
		public float escapeTime = 300f;
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public float baggedTime;

		public static readonly Chore.Precondition IsBagged = new Chore.Precondition
		{
			id = "IsBagged",
			fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Creatures.Bagged);
			}
		};

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(IsBagged, null);
		}

		public void UpdateFaller(bool bagged)
		{
			bool flag = bagged && !base.gameObject.HasTag(GameTags.Stored);
			bool flag2 = GameComps.Fallers.Has(base.gameObject);
			if (flag != flag2)
			{
				if (flag)
				{
					GameComps.Fallers.Add(base.gameObject, Vector2.zero);
				}
				else
				{
					GameComps.Fallers.Remove(base.gameObject);
				}
			}
		}
	}

	public State bagged;

	public State escape;

	[CompilerGenerated]
	private static StateMachine<BaggedStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<BaggedStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static StateMachine<BaggedStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static StateMachine<BaggedStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache4;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = bagged;
		base.serializable = true;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.BAGGED.NAME, CREATURES.STATUSITEMS.BAGGED.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 129022, resolve_string_callback: null, resolve_tooltip_callback: null);
		bagged.Enter(BagStart).ToggleTag(GameTags.Creatures.Deliverable).PlayAnim("trussed", KAnim.PlayMode.Loop)
			.TagTransition(GameTags.Creatures.Bagged, null, true)
			.Transition(escape, ShouldEscape, UpdateRate.SIM_4000ms)
			.EventHandler(GameHashes.OnStore, OnStore)
			.Exit(BagEnd);
		escape.Enter(Unbag).PlayAnim("escape").OnAnimQueueComplete(null);
	}

	private static void BagStart(Instance smi)
	{
		if (smi.baggedTime == 0f)
		{
			smi.baggedTime = GameClock.Instance.GetTime();
		}
		smi.UpdateFaller(true);
	}

	private static void BagEnd(Instance smi)
	{
		smi.baggedTime = 0f;
		smi.UpdateFaller(false);
	}

	private static void Unbag(Instance smi)
	{
		Baggable component = smi.gameObject.GetComponent<Baggable>();
		if ((bool)component)
		{
			component.Free();
		}
	}

	private static void OnStore(Instance smi)
	{
		smi.UpdateFaller(true);
	}

	private static bool ShouldEscape(Instance smi)
	{
		if (smi.gameObject.HasTag(GameTags.Stored))
		{
			return false;
		}
		float num = GameClock.Instance.GetTime() - smi.baggedTime;
		if (num < smi.def.escapeTime)
		{
			return false;
		}
		return true;
	}
}
