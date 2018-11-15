using Klei.AI;

public class UrgeMonitor : GameStateMachine<UrgeMonitor, UrgeMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private AmountInstance amountInstance;

		private Urge urge;

		private ScheduleBlockType scheduleBlock;

		private Schedulable schedulable;

		private float inScheduleThreshold;

		private float outOfScheduleThreshold;

		private bool isThresholdMinimum;

		public Instance(IStateMachineTarget master, Urge urge, Amount amount, ScheduleBlockType schedule_block, float in_schedule_threshold, float out_of_schedule_threshold, bool is_threshold_minimum)
			: base(master)
		{
			this.urge = urge;
			scheduleBlock = schedule_block;
			schedulable = GetComponent<Schedulable>();
			amountInstance = base.gameObject.GetAmounts().Get(amount);
			isThresholdMinimum = is_threshold_minimum;
			inScheduleThreshold = in_schedule_threshold;
			outOfScheduleThreshold = out_of_schedule_threshold;
		}

		private float GetThreshold()
		{
			if (schedulable.IsAllowed(scheduleBlock))
			{
				return inScheduleThreshold;
			}
			return outOfScheduleThreshold;
		}

		public Urge GetUrge()
		{
			return urge;
		}

		public bool HasUrge()
		{
			if (isThresholdMinimum)
			{
				return amountInstance.value >= GetThreshold();
			}
			return amountInstance.value <= GetThreshold();
		}
	}

	public State satisfied;

	public State hasurge;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.Transition(hasurge, (Instance smi) => smi.HasUrge(), UpdateRate.SIM_200ms);
		hasurge.Transition(satisfied, (Instance smi) => !smi.HasUrge(), UpdateRate.SIM_200ms).ToggleUrge((Instance smi) => smi.GetUrge());
	}
}
