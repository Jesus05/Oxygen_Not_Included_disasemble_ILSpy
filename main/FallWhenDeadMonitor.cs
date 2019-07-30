using UnityEngine;

public class FallWhenDeadMonitor : GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public bool IsEntombed()
		{
			Pickupable component = GetComponent<Pickupable>();
			return (Object)component != (Object)null && component.IsEntombed;
		}

		public bool IsFalling()
		{
			int cell = Grid.PosToCell(base.master.transform.GetPosition());
			int num = Grid.CellBelow(cell);
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			return !Grid.Solid[num];
		}
	}

	public State standing;

	public State falling;

	public State entombed;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = standing;
		standing.Transition(entombed, (Instance smi) => smi.IsEntombed(), UpdateRate.SIM_200ms).Transition(falling, (Instance smi) => smi.IsFalling(), UpdateRate.SIM_200ms);
		falling.ToggleGravity(standing);
		entombed.Transition(standing, (Instance smi) => !smi.IsEntombed(), UpdateRate.SIM_200ms);
	}
}
