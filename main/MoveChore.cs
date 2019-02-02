using System;
using UnityEngine;

public class MoveChore : Chore<MoveChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, MoveChore, object>.GameInstance
	{
		public Func<StatesInstance, int> getCellCallback;

		public StatesInstance(MoveChore master, GameObject mover, Func<StatesInstance, int> get_cell_callback, bool update_cell = false)
			: base(master)
		{
			getCellCallback = get_cell_callback;
			base.sm.mover.Set(mover, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, MoveChore>
	{
		public ApproachSubState<IApproachable> approach;

		public TargetParameter mover;

		public TargetParameter locator;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approach;
			Target(mover);
			root.MoveTo((StatesInstance smi) => smi.getCellCallback(smi), null, null, false);
		}
	}

	public MoveChore(IStateMachineTarget target, ChoreType chore_type, Func<StatesInstance, int> get_cell_callback, bool update_cell = false)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, (Tag[])null, false, ReportManager.ReportType.WorkTime)
	{
		smi = new StatesInstance(this, target.gameObject, get_cell_callback, update_cell);
	}
}
