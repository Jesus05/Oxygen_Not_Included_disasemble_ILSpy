using Klei.AI;

public class CalorieMonitor : GameStateMachine<CalorieMonitor, CalorieMonitor.Instance>
{
	public class HungryState : State
	{
		public State working;

		public State normal;

		public State starving;
	}

	public new class Instance : GameInstance
	{
		public AmountInstance calories;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			calories = Db.Get().Amounts.Calories.Lookup(base.gameObject);
		}

		private float GetCalories0to1()
		{
			return calories.value / calories.GetMax();
		}

		public bool IsEatTime()
		{
			Schedulable component = base.master.GetComponent<Schedulable>();
			return component.IsAllowed(Db.Get().ScheduleBlockTypes.Eat);
		}

		public bool IsHungry()
		{
			return GetCalories0to1() < 0.825f;
		}

		public bool IsStarving()
		{
			return GetCalories0to1() < 0.25f;
		}

		public bool IsSatisfied()
		{
			return GetCalories0to1() > 0.95f;
		}

		public bool IsEating()
		{
			ChoreDriver component = base.master.GetComponent<ChoreDriver>();
			return component.HasChore() && component.GetCurrentChore().choreType.urge == Db.Get().Urges.Eat;
		}

		public bool IsDepleted()
		{
			return calories.value <= 0f;
		}

		public bool ShouldExitInfirmary()
		{
			return !IsStarving();
		}

		public void Kill()
		{
			DeathMonitor.Instance sMI = base.gameObject.GetSMI<DeathMonitor.Instance>();
			if (sMI != null)
			{
				base.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Starvation);
			}
		}
	}

	public State satisfied;

	public HungryState hungry;

	public State eating;

	public State incapacitated;

	public State depleted;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = true;
		satisfied.Transition(hungry, (Instance smi) => smi.IsHungry(), UpdateRate.SIM_200ms);
		hungry.DefaultState(hungry.normal).Transition(satisfied, (Instance smi) => smi.IsSatisfied(), UpdateRate.SIM_200ms).EventTransition(GameHashes.BeginChore, eating, (Instance smi) => smi.IsEating());
		hungry.working.EventTransition(GameHashes.ScheduleBlocksChanged, hungry.normal, (Instance smi) => smi.IsEatTime()).Transition(hungry.starving, (Instance smi) => smi.IsStarving(), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.Hungry, (object)null);
		hungry.normal.EventTransition(GameHashes.ScheduleBlocksChanged, hungry.working, (Instance smi) => !smi.IsEatTime()).Transition(hungry.starving, (Instance smi) => smi.IsStarving(), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.Hungry, (object)null)
			.ToggleUrge(Db.Get().Urges.Eat)
			.ToggleExpression(Db.Get().Expressions.Hungry, null)
			.ToggleThought(Db.Get().Thoughts.Starving, null);
		hungry.starving.Transition(hungry.normal, (Instance smi) => !smi.IsStarving(), UpdateRate.SIM_200ms).Transition(depleted, (Instance smi) => smi.IsDepleted(), UpdateRate.SIM_200ms).ToggleStatusItem(Db.Get().DuplicantStatusItems.Starving, (object)null)
			.ToggleUrge(Db.Get().Urges.Eat)
			.ToggleExpression(Db.Get().Expressions.Hungry, null)
			.ToggleThought(Db.Get().Thoughts.Starving, null);
		eating.EventTransition(GameHashes.EndChore, satisfied, (Instance smi) => !smi.IsEating());
		depleted.ToggleTag(GameTags.CaloriesDepleted).Enter(delegate(Instance smi)
		{
			smi.Kill();
		});
	}
}
