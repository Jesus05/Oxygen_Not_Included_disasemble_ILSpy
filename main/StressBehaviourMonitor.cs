using System;
using TUNING;

public class StressBehaviourMonitor : GameStateMachine<StressBehaviourMonitor, StressBehaviourMonitor.Instance>
{
	public class StressedState : State
	{
		public TierOneStates tierOne;

		public TierTwoStates tierTwo;
	}

	public class TierOneStates : State
	{
		public State actingOut;

		public State reprieve;
	}

	public class TierTwoStates : State
	{
		public State actingOut;

		public State reprieve;
	}

	public new class Instance : GameInstance
	{
		public Func<ChoreProvider, Chore> tierOneStressChoreCreator;

		public Func<ChoreProvider, Chore> tierTwoStressChoreCreator;

		public string tierOneLocoAnim = "";

		public float tierTwoReprieveDuration;

		public Instance(IStateMachineTarget master, Func<ChoreProvider, Chore> tier_one_stress_chore_creator, Func<ChoreProvider, Chore> tier_two_stress_chore_creator, string tier_one_loco_anim, float tier_two_reprieve_duration = 3f)
			: base(master)
		{
			tierOneLocoAnim = tier_one_loco_anim;
			tierTwoReprieveDuration = tier_two_reprieve_duration;
			tierOneStressChoreCreator = tier_one_stress_chore_creator;
			tierTwoStressChoreCreator = tier_two_stress_chore_creator;
		}

		public Chore CreateTierOneStressChore()
		{
			return tierOneStressChoreCreator(GetComponent<ChoreProvider>());
		}

		public Chore CreateTierTwoStressChore()
		{
			return tierTwoStressChoreCreator(GetComponent<ChoreProvider>());
		}
	}

	public FloatParameter timeInTierTwoStressResponse;

	public State satisfied;

	public StressedState stressed;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.EventTransition(GameHashes.Died, (State)null, (Transition.ConditionCallback)null);
		satisfied.EventTransition(GameHashes.Stressed, stressed, (Instance smi) => smi.gameObject.GetSMI<StressMonitor.Instance>() != null && smi.gameObject.GetSMI<StressMonitor.Instance>().IsStressed());
		stressed.DefaultState(stressed.tierOne).ToggleExpression(Db.Get().Expressions.Unhappy, null).ToggleAnims((Instance smi) => smi.tierOneLocoAnim)
			.Transition(satisfied, (Instance smi) => smi.gameObject.GetSMI<StressMonitor.Instance>() != null && !smi.gameObject.GetSMI<StressMonitor.Instance>().IsStressed(), UpdateRate.SIM_200ms);
		stressed.tierOne.DefaultState(stressed.tierOne.actingOut).EventTransition(GameHashes.StressedHadEnough, stressed.tierTwo, null);
		stressed.tierOne.actingOut.ToggleChore((Instance smi) => smi.CreateTierOneStressChore(), stressed.tierOne.reprieve);
		stressed.tierOne.reprieve.ScheduleGoTo(30f, stressed.tierOne.actingOut);
		stressed.tierTwo.DefaultState(stressed.tierTwo.actingOut).Update(delegate(Instance smi, float dt)
		{
			smi.sm.timeInTierTwoStressResponse.Set(smi.sm.timeInTierTwoStressResponse.Get(smi) + dt, smi);
		}, UpdateRate.SIM_200ms, false).Exit("ResetStress", delegate(Instance smi)
		{
			Db.Get().Amounts.Stress.Lookup(smi.gameObject).SetValue(STRESS.ACTING_OUT_RESET);
		});
		stressed.tierTwo.actingOut.ToggleChore((Instance smi) => smi.CreateTierTwoStressChore(), stressed.tierTwo.reprieve);
		stressed.tierTwo.reprieve.ToggleChore((Instance smi) => new StressIdleChore(smi.master), null).Enter(delegate(Instance smi)
		{
			if (smi.sm.timeInTierTwoStressResponse.Get(smi) >= 150f)
			{
				smi.sm.timeInTierTwoStressResponse.Set(0f, smi);
				smi.GoTo(stressed);
			}
		}).ScheduleGoTo((Instance smi) => smi.tierTwoReprieveDuration, stressed.tierTwo);
	}
}
