using Klei.AI;
using UnityEngine;

public class StaminaMonitor : GameStateMachine<StaminaMonitor, StaminaMonitor.Instance>
{
	public class SleepyState : State
	{
		public State needssleep;

		public State sleeping;
	}

	public new class Instance : GameInstance
	{
		private ChoreDriver choreDriver;

		private Schedulable schedulable;

		public AmountInstance stamina;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			stamina = Db.Get().Amounts.Stamina.Lookup(base.gameObject);
			choreDriver = GetComponent<ChoreDriver>();
			schedulable = GetComponent<Schedulable>();
		}

		public bool NeedsToSleep()
		{
			return stamina.value <= 0f;
		}

		public bool WantsToSleep()
		{
			return choreDriver.HasChore() && choreDriver.GetCurrentChore().SatisfiesUrge(Db.Get().Urges.Sleep);
		}

		public void TryExitSleepState()
		{
			if (!NeedsToSleep() && !WantsToSleep())
			{
				base.smi.GoTo(base.smi.sm.satisfied);
			}
		}

		public bool IsSleeping()
		{
			bool result = false;
			if (WantsToSleep())
			{
				Worker component = choreDriver.GetComponent<Worker>();
				Workable workable = component.workable;
				if ((Object)workable != (Object)null)
				{
					result = true;
				}
			}
			return result;
		}

		public bool ShouldExitSleep()
		{
			if (!schedulable.IsAllowed(Db.Get().ScheduleBlockTypes.Sleep))
			{
				Narcolepsy component = GetComponent<Narcolepsy>();
				if ((Object)component != (Object)null && component.IsNarcolepsing())
				{
					return false;
				}
				if (!(stamina.value < stamina.GetMax()))
				{
					return true;
				}
				return false;
			}
			return false;
		}
	}

	public State satisfied;

	public SleepyState sleepy;

	private const float OUTSIDE_SCHEDULE_STAMINA_THRESHOLD = 0f;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = true;
		root.ToggleStateMachine((Instance smi) => new UrgeMonitor.Instance(smi.master, Db.Get().Urges.Sleep, Db.Get().Amounts.Stamina, Db.Get().ScheduleBlockTypes.Sleep, 100f, 0f, false)).ToggleStateMachine((Instance smi) => new SleepChoreMonitor.Instance(smi.master));
		satisfied.Transition(sleepy, (Instance smi) => smi.NeedsToSleep() || smi.WantsToSleep(), UpdateRate.SIM_200ms);
		sleepy.Update("Check Sleep State", delegate(Instance smi, float dt)
		{
			smi.TryExitSleepState();
		}, UpdateRate.SIM_1000ms, false).DefaultState(sleepy.needssleep);
		sleepy.needssleep.Transition(sleepy.sleeping, (Instance smi) => smi.IsSleeping(), UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.Tired, null).ToggleStatusItem(Db.Get().DuplicantStatusItems.Tired, (object)null)
			.ToggleThought(Db.Get().Thoughts.Sleepy, null);
		sleepy.sleeping.Transition(satisfied, (Instance smi) => !smi.IsSleeping(), UpdateRate.SIM_200ms);
	}
}
