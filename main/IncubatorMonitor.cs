using System.Runtime.CompilerServices;
using UnityEngine;

public class IncubatorMonitor : GameStateMachine<IncubatorMonitor, IncubatorMonitor.Instance, IStateMachineTarget, IncubatorMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	public State not;

	public State in_incubator;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = not;
		not.EventTransition(GameHashes.OnStore, in_incubator, InIncubator);
		in_incubator.ToggleTag(GameTags.Creatures.InIncubator).EventTransition(GameHashes.OnStore, not, GameStateMachine<IncubatorMonitor, Instance, IStateMachineTarget, Def>.Not(InIncubator));
	}

	public static bool InIncubator(Instance smi)
	{
		if (!(bool)smi.gameObject.transform.parent)
		{
			return false;
		}
		EggIncubator component = smi.gameObject.transform.parent.GetComponent<EggIncubator>();
		return (Object)component != (Object)null;
	}
}
