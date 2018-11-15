using System.Runtime.CompilerServices;
using UnityEngine;

public class CallAdultMonitor : GameStateMachine<CallAdultMonitor, CallAdultMonitor.Instance, IStateMachineTarget, CallAdultMonitor.Def>
{
	public class Def : BaseDef
	{
		public float callMinInterval = 120f;

		public float callMaxInterval = 240f;
	}

	public new class Instance : GameInstance
	{
		public float nextCallTime;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			RefreshCallTime();
		}

		public void RefreshCallTime()
		{
			nextCallTime = Time.time + Random.value * (base.def.callMaxInterval - base.def.callMinInterval) + base.def.callMinInterval;
		}
	}

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.Creatures.Behaviours.CallAdultBehaviour, ShouldCallAdult, delegate(Instance smi)
		{
			smi.RefreshCallTime();
		});
	}

	public static bool ShouldCallAdult(Instance smi)
	{
		return Time.time >= smi.nextCallTime;
	}
}
