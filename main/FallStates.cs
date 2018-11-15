using STRINGS;
using System;
using System.Runtime.CompilerServices;

internal class FallStates : GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>
{
	public class Def : BaseDef
	{
		public Func<Instance, string> getLandAnim = (Instance smi) => "idle_loop";
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Falling);
		}
	}

	private State loop;

	private State snaptoground;

	private State pst;

	[CompilerGenerated]
	private static StateMachine<FallStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = loop;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.FALLING.NAME, CREATURES.STATUSITEMS.FALLING.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: SimViewMode.None, status_overlays: 63486, resolve_string_callback: null, resolve_tooltip_callback: null);
		loop.PlayAnim((Instance smi) => smi.GetSMI<CreatureFallMonitor.Instance>().anim, KAnim.PlayMode.Loop).ToggleGravity().EventTransition(GameHashes.Landed, snaptoground, null)
			.Transition(pst, (Instance smi) => smi.GetSMI<CreatureFallMonitor.Instance>().CanSwimAtCurrentLocation(true), UpdateRate.SIM_33ms);
		snaptoground.Enter(delegate(Instance smi)
		{
			smi.GetSMI<CreatureFallMonitor.Instance>().SnapToGround();
		}).GoTo(pst);
		pst.Enter(PlayLandAnim).BehaviourComplete(GameTags.Creatures.Falling, false);
	}

	private static void PlayLandAnim(Instance smi)
	{
		smi.GetComponent<KBatchedAnimController>().Queue(smi.def.getLandAnim(smi), KAnim.PlayMode.Loop, 1f, 0f);
	}
}
