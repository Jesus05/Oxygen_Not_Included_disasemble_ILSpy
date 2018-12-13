using STRINGS;
using System.Runtime.CompilerServices;

internal class GrowUpStates : GameStateMachine<GrowUpStates, GrowUpStates.Instance, IStateMachineTarget, GrowUpStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.GrowUpBehaviour);
		}
	}

	public State grow_up_pre;

	public State spawn_adult;

	[CompilerGenerated]
	private static StateMachine<GrowUpStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = grow_up_pre;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.GROWINGUP.NAME, CREATURES.STATUSITEMS.GROWINGUP.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 63486, resolve_string_callback: null, resolve_tooltip_callback: null);
		grow_up_pre.QueueAnim("growup_pre", false, null).OnAnimQueueComplete(spawn_adult);
		spawn_adult.Enter(SpawnAdult);
	}

	private static void SpawnAdult(Instance smi)
	{
		smi.GetSMI<BabyMonitor.Instance>().SpawnAdult();
	}
}
