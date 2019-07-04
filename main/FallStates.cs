using STRINGS;
using System;
using System.Runtime.CompilerServices;

public class FallStates : GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>
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
		State root = base.root;
		string name = CREATURES.STATUSITEMS.FALLING.NAME;
		string tooltip = CREATURES.STATUSITEMS.FALLING.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main);
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
