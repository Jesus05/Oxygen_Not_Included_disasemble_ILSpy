using Klei.AI;
using System;
using System.Runtime.CompilerServices;

public class SteppedInMonitor : GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Effects effects;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			effects = GetComponent<Effects>();
		}
	}

	public State satisfied;

	public State wetFloor;

	public State wetBody;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<SteppedInMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static StateMachine<SteppedInMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache6;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache7;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache8;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.Transition(wetFloor, IsFloorWet, UpdateRate.SIM_200ms).Transition(wetBody, IsSubmerged, UpdateRate.SIM_200ms);
		wetFloor.Enter(GetWetFeet).Update(GetWetFeet, UpdateRate.SIM_1000ms, false).Transition(satisfied, GameStateMachine<SteppedInMonitor, Instance, IStateMachineTarget, object>.Not(IsFloorWet), UpdateRate.SIM_200ms)
			.Transition(wetBody, IsSubmerged, UpdateRate.SIM_200ms);
		wetBody.Enter(GetSoaked).Update(GetSoaked, UpdateRate.SIM_1000ms, false).Transition(wetFloor, GameStateMachine<SteppedInMonitor, Instance, IStateMachineTarget, object>.Not(IsSubmerged), UpdateRate.SIM_200ms);
	}

	private static void GetWetFeet(Instance smi, float dt)
	{
		GetWetFeet(smi);
	}

	private static void GetWetFeet(Instance smi)
	{
		if (!smi.effects.HasEffect("SoakingWet"))
		{
			smi.effects.Add("WetFeet", true);
		}
	}

	private static void GetSoaked(Instance smi, float dt)
	{
		GetSoaked(smi);
	}

	private static void GetSoaked(Instance smi)
	{
		if (smi.effects.HasEffect("WetFeet"))
		{
			smi.effects.Remove("WetFeet");
		}
		smi.effects.Add("SoakingWet", true);
	}

	private static bool IsFloorWet(Instance smi)
	{
		int num = Grid.PosToCell(smi);
		return Grid.IsValidCell(num) && Grid.Element[num].IsLiquid;
	}

	private static bool IsSubmerged(Instance smi)
	{
		int cell = Grid.PosToCell(smi);
		int num = Grid.CellAbove(cell);
		return Grid.IsValidCell(num) && Grid.Element[num].IsLiquid;
	}
}
