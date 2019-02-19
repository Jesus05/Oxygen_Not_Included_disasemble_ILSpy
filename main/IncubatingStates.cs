using STRINGS;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class IncubatingStates : GameStateMachine<IncubatingStates, IncubatingStates.Instance, IStateMachineTarget, IncubatingStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public int variant_time = 3;

		public static readonly Chore.Precondition IsInIncubator = new Chore.Precondition
		{
			id = "IsInIncubator",
			fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.prefabid.HasTag(GameTags.Creatures.InIncubator);
			}
		};

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(IsInIncubator, null);
		}
	}

	public class IncubatorStates : State
	{
		public State idle;

		public State choose;

		public State variant;
	}

	public IncubatorStates incubator;

	[CompilerGenerated]
	private static StateMachine<IncubatingStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache2;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = incubator;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.IN_INCUBATOR.NAME, CREATURES.STATUSITEMS.IN_INCUBATOR.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: (NotificationType)0, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 0, resolve_string_callback: null, resolve_tooltip_callback: null);
		incubator.DefaultState(incubator.idle).ToggleTag(GameTags.Creatures.Deliverable).TagTransition(GameTags.Creatures.InIncubator, null, true);
		incubator.idle.Enter("VariantUpdate", VariantUpdate).PlayAnim("incubator_idle_loop").OnAnimQueueComplete(incubator.choose);
		incubator.choose.Transition(incubator.variant, DoVariant, UpdateRate.SIM_200ms).Transition(incubator.idle, GameStateMachine<IncubatingStates, Instance, IStateMachineTarget, Def>.Not(DoVariant), UpdateRate.SIM_200ms);
		incubator.variant.PlayAnim("incubator_variant").OnAnimQueueComplete(incubator.idle);
	}

	public static bool DoVariant(Instance smi)
	{
		return smi.variant_time == 0;
	}

	public static void VariantUpdate(Instance smi)
	{
		if (smi.variant_time <= 0)
		{
			smi.variant_time = Random.Range(3, 7);
		}
		else
		{
			smi.variant_time--;
		}
	}
}
