using KSerialization;
using STRINGS;
using System.Runtime.CompilerServices;

internal class BaggedStates : GameStateMachine<BaggedStates, BaggedStates.Instance, IStateMachineTarget, BaggedStates.Def>
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

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = bagged;
		base.serializable = true;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.BAGGED.NAME, CREATURES.STATUSITEMS.BAGGED.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: SimViewMode.None, status_overlays: 63486, resolve_string_callback: null, resolve_tooltip_callback: null);
		bagged.Enter(BagStart).ToggleTag(GameTags.Creatures.Deliverable).ToggleFaller()
			.PlayAnim("trussed", KAnim.PlayMode.Loop)
			.TagTransition(GameTags.Creatures.Bagged, null, true)
			.Transition(escape, ShouldEscape, UpdateRate.SIM_4000ms)
			.Exit(BagEnd);
		escape.Enter(Unbag).PlayAnim("escape").OnAnimQueueComplete(null);
	}

	private static void BagStart(Instance smi)
	{
		if (smi.baggedTime == 0f)
		{
			smi.baggedTime = GameClock.Instance.GetTime();
		}
	}

	private static void BagEnd(Instance smi)
	{
		smi.baggedTime = 0f;
	}

	private static void Unbag(Instance smi)
	{
		Baggable component = smi.gameObject.GetComponent<Baggable>();
		if ((bool)component)
		{
			component.Free();
		}
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
