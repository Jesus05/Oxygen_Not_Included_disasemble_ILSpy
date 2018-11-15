using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class JetSuitMonitor : GameStateMachine<JetSuitMonitor, JetSuitMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Navigator navigator;

		public JetSuitTank jet_suit_tank;

		public Instance(IStateMachineTarget master, GameObject owner)
			: base(master)
		{
			base.sm.owner.Set(owner, base.smi);
			navigator = owner.GetComponent<Navigator>();
			jet_suit_tank = master.GetComponent<JetSuitTank>();
		}
	}

	public State off;

	public State flying;

	public TargetParameter owner;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static StateMachine<JetSuitMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<JetSuitMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache4;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		Target(owner);
		off.EventTransition(GameHashes.PathAdvanced, flying, ShouldStartFlying);
		flying.Enter(StartFlying).Exit(StopFlying).EventTransition(GameHashes.PathAdvanced, off, ShouldStopFlying)
			.Update(Emit, UpdateRate.SIM_200ms, false);
	}

	public static bool ShouldStartFlying(Instance smi)
	{
		return (bool)smi.navigator && smi.navigator.CurrentNavType == NavType.Hover;
	}

	public static bool ShouldStopFlying(Instance smi)
	{
		return !(bool)smi.navigator || smi.navigator.CurrentNavType != NavType.Hover;
	}

	public static void StartFlying(Instance smi)
	{
	}

	public static void StopFlying(Instance smi)
	{
	}

	public static void Emit(Instance smi, float dt)
	{
		if ((bool)smi.navigator)
		{
			GameObject gameObject = smi.sm.owner.Get(smi);
			if ((bool)gameObject)
			{
				int gameCell = Grid.PosToCell(gameObject.transform.GetPosition());
				float a = 0.1f * dt;
				a = Mathf.Min(a, smi.jet_suit_tank.amount);
				smi.jet_suit_tank.amount -= a;
				float num = a * 3f;
				if (num > 1.401298E-45f)
				{
					SimMessages.AddRemoveSubstance(gameCell, SimHashes.CarbonDioxide, CellEventLogger.Instance.ElementConsumerSimUpdate, num, 473.15f, byte.MaxValue, 0, true, -1);
				}
				if (smi.jet_suit_tank.amount == 0f)
				{
					smi.navigator.AddTag(GameTags.JetSuitOutOfFuel);
					smi.navigator.SetCurrentNavType(NavType.Floor);
				}
			}
		}
	}
}
