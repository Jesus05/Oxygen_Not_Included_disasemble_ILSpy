using System.Runtime.CompilerServices;
using UnityEngine;

public class EggIncubatorStates : GameStateMachine<EggIncubatorStates, EggIncubatorStates.Instance>
{
	public class EggStates : State
	{
		public State incubating;

		public State lose_power;

		public State unpowered;
	}

	public class BabyStates : State
	{
		public State idle;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}

	public BoolParameter readyToHatch;

	public State empty;

	public EggStates egg;

	public BabyStates baby;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache6;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache7;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = empty;
		empty.PlayAnim("off", KAnim.PlayMode.Loop).EventTransition(GameHashes.OccupantChanged, egg, HasEgg).EventTransition(GameHashes.OccupantChanged, baby, HasBaby);
		egg.DefaultState(egg.unpowered).EventTransition(GameHashes.OccupantChanged, empty, GameStateMachine<EggIncubatorStates, Instance, IStateMachineTarget, object>.Not(HasAny)).EventTransition(GameHashes.OccupantChanged, baby, HasBaby)
			.ToggleStatusItem(Db.Get().BuildingStatusItems.IncubatorProgress, (Instance smi) => smi.master.GetComponent<EggIncubator>());
		egg.lose_power.PlayAnim("no_power_pre").EventTransition(GameHashes.OperationalChanged, egg.incubating, IsOperational).OnAnimQueueComplete(egg.unpowered);
		egg.unpowered.PlayAnim("no_power_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, egg.incubating, IsOperational);
		egg.incubating.PlayAnim("no_power_pst").QueueAnim("working_loop", true, null).EventTransition(GameHashes.OperationalChanged, egg.lose_power, GameStateMachine<EggIncubatorStates, Instance, IStateMachineTarget, object>.Not(IsOperational));
		baby.DefaultState(baby.idle).EventTransition(GameHashes.OccupantChanged, empty, GameStateMachine<EggIncubatorStates, Instance, IStateMachineTarget, object>.Not(HasBaby));
		baby.idle.PlayAnim("no_power_pre").QueueAnim("no_power_loop", true, null);
	}

	public static bool IsOperational(Instance smi)
	{
		return smi.GetComponent<Operational>().IsOperational;
	}

	public static bool HasEgg(Instance smi)
	{
		GameObject occupant = smi.GetComponent<EggIncubator>().Occupant;
		return (bool)occupant && occupant.HasTag(GameTags.Egg);
	}

	public static bool HasBaby(Instance smi)
	{
		GameObject occupant = smi.GetComponent<EggIncubator>().Occupant;
		return (bool)occupant && occupant.HasTag(GameTags.Creature);
	}

	public static bool HasAny(Instance smi)
	{
		GameObject occupant = smi.GetComponent<EggIncubator>().Occupant;
		return occupant;
	}
}
