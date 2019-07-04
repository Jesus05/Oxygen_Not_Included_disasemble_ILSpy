using STRINGS;
using System.Runtime.CompilerServices;

public class GrowUpStates : GameStateMachine<GrowUpStates, GrowUpStates.Instance, IStateMachineTarget, GrowUpStates.Def>
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
		State root = base.root;
		string name = CREATURES.STATUSITEMS.GROWINGUP.NAME;
		string tooltip = CREATURES.STATUSITEMS.GROWINGUP.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main);
		grow_up_pre.QueueAnim("growup_pre", false, null).OnAnimQueueComplete(spawn_adult);
		spawn_adult.Enter(SpawnAdult);
	}

	private static void SpawnAdult(Instance smi)
	{
		smi.GetSMI<BabyMonitor.Instance>().SpawnAdult();
	}
}
