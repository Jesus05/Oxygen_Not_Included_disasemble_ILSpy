using Klei.AI;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BreathMonitor : GameStateMachine<BreathMonitor, BreathMonitor.Instance>
{
	public class LowBreathState : State
	{
		public State nowheretorecover;

		public State recoveryavailable;
	}

	public class SatisfiedState : State
	{
		public State full;

		public State notfull;
	}

	public new class Instance : GameInstance
	{
		public AmountInstance breath;

		public SafetyQuery query;

		public Navigator navigator;

		public OxygenBreather breather;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			breath = Db.Get().Amounts.Breath.Lookup(master.gameObject);
			query = new SafetyQuery(Game.Instance.safetyConditions.RecoverBreathChecker, GetComponent<KMonoBehaviour>(), 2147483647);
			navigator = GetComponent<Navigator>();
			breather = GetComponent<OxygenBreather>();
		}

		public int GetRecoverCell()
		{
			return base.sm.recoverBreathCell.Get(base.smi);
		}

		public float GetBreath()
		{
			return breath.value / breath.GetMax();
		}
	}

	public SatisfiedState satisfied;

	public LowBreathState lowbreath;

	public IntParameter recoverBreathCell;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static StateMachine<BreathMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static StateMachine<BreathMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static Func<Instance, bool> _003C_003Ef__mg_0024cache6;

	[CompilerGenerated]
	private static StateMachine<BreathMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache7;

	[CompilerGenerated]
	private static StateMachine<BreathMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache8;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache9;

	[CompilerGenerated]
	private static Parameter<int>.Callback _003C_003Ef__mg_0024cacheA;

	[CompilerGenerated]
	private static Parameter<int>.Callback _003C_003Ef__mg_0024cacheB;

	[CompilerGenerated]
	private static StateMachine<BreathMonitor, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cacheC;

	[CompilerGenerated]
	private static Func<Instance, Chore> _003C_003Ef__mg_0024cacheD;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.DefaultState(satisfied.full).Transition(lowbreath, IsLowBreath, UpdateRate.SIM_200ms);
		satisfied.full.Transition(satisfied.notfull, IsNotFullBreath, UpdateRate.SIM_200ms).Enter(HideBreathBar);
		satisfied.notfull.Transition(satisfied.full, IsFullBreath, UpdateRate.SIM_200ms).Enter(ShowBreathBar);
		lowbreath.DefaultState(lowbreath.nowheretorecover).Transition(satisfied, IsFullBreath, UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.RecoverBreath, IsNotInBreathableArea)
			.ToggleUrge(Db.Get().Urges.RecoverBreath)
			.ToggleThought(Db.Get().Thoughts.Suffocating, null)
			.ToggleTag(GameTags.HoldingBreath)
			.Enter(ShowBreathBar)
			.Enter(UpdateRecoverBreathCell)
			.Update(UpdateRecoverBreathCell, UpdateRate.SIM_200ms, false);
		lowbreath.nowheretorecover.ParamTransition(recoverBreathCell, lowbreath.recoveryavailable, IsValidRecoverCell);
		lowbreath.recoveryavailable.ParamTransition(recoverBreathCell, lowbreath.nowheretorecover, IsNotValidRecoverCell).Enter(UpdateRecoverBreathCell).ToggleChore(CreateRecoverBreathChore, lowbreath.nowheretorecover);
	}

	private static bool IsLowBreath(Instance smi)
	{
		return smi.breath.value < 72.72727f;
	}

	private static Chore CreateRecoverBreathChore(Instance smi)
	{
		return new RecoverBreathChore(smi.master);
	}

	private static bool IsNotFullBreath(Instance smi)
	{
		return !IsFullBreath(smi);
	}

	private static bool IsFullBreath(Instance smi)
	{
		return smi.breath.value >= smi.breath.GetMax();
	}

	private static bool IsNotInBreathableArea(Instance smi)
	{
		return !smi.breather.IsBreathableElementAtCell(Grid.PosToCell(smi), null);
	}

	private static void ShowBreathBar(Instance smi)
	{
		if ((UnityEngine.Object)NameDisplayScreen.Instance != (UnityEngine.Object)null)
		{
			NameDisplayScreen.Instance.SetBreathDisplay(smi.gameObject, smi.GetBreath, true);
		}
	}

	private static void HideBreathBar(Instance smi)
	{
		if ((UnityEngine.Object)NameDisplayScreen.Instance != (UnityEngine.Object)null)
		{
			NameDisplayScreen.Instance.SetBreathDisplay(smi.gameObject, null, false);
		}
	}

	private static bool IsValidRecoverCell(Instance smi, int cell)
	{
		return cell != Grid.InvalidCell;
	}

	private static bool IsNotValidRecoverCell(Instance smi, int cell)
	{
		return !IsValidRecoverCell(smi, cell);
	}

	private static void UpdateRecoverBreathCell(Instance smi, float dt)
	{
		UpdateRecoverBreathCell(smi);
	}

	private static void UpdateRecoverBreathCell(Instance smi)
	{
		smi.query.Reset();
		smi.navigator.RunQuery(smi.query);
		int num = smi.query.GetResultCell();
		if (!smi.breather.IsBreathableElementAtCell(num, null))
		{
			num = PathFinder.InvalidCell;
		}
		smi.sm.recoverBreathCell.Set(num, smi);
	}
}
