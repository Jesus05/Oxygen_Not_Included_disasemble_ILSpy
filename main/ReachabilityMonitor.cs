using UnityEngine;

public class ReachabilityMonitor : GameStateMachine<ReachabilityMonitor, ReachabilityMonitor.Instance, Workable>
{
	private class UpdateReachabilityCB : UpdateBucketWithUpdater<Instance>.IUpdater
	{
		public void Update(Instance smi, float dt)
		{
			smi.UpdateReachability();
		}
	}

	public new class Instance : GameInstance
	{
		public Instance(Workable workable)
			: base(workable)
		{
			UpdateReachability();
		}

		public void TriggerEvent()
		{
			bool flag = base.sm.isReachable.Get(base.smi);
			Trigger(-1432940121, flag);
		}

		public void UpdateReachability()
		{
			if ((Object)base.master != (Object)null)
			{
				int cell = Grid.PosToCell(base.master);
				CellOffset[] offsets = base.master.GetOffsets(cell);
				MinionGroupProber minionGroupProber = MinionGroupProber.Get();
				bool value = minionGroupProber.IsReachable(cell) || minionGroupProber.IsReachable(cell, offsets);
				base.sm.isReachable.Set(value, base.smi);
			}
		}
	}

	public State reachable;

	public State unreachable;

	public BoolParameter isReachable = new BoolParameter(false);

	private static UpdateReachabilityCB updateReachabilityCB = new UpdateReachabilityCB();

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = unreachable;
		base.serializable = false;
		root.FastUpdate("UpdateReachability", updateReachabilityCB, UpdateRate.SIM_1000ms, true);
		reachable.ToggleTag(GameTags.Reachable).Enter("TriggerEvent", delegate(Instance smi)
		{
			smi.TriggerEvent();
		}).ParamTransition(isReachable, unreachable, (Instance smi, bool p) => !p);
		unreachable.Enter("TriggerEvent", delegate(Instance smi)
		{
			smi.TriggerEvent();
		}).ParamTransition(isReachable, reachable, (Instance smi, bool p) => p);
	}
}
