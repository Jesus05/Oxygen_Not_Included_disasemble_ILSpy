using System;
using UnityEngine;

public class DropUnusedInventoryChore : Chore<DropUnusedInventoryChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, DropUnusedInventoryChore, object>.GameInstance
	{
		public StatesInstance(DropUnusedInventoryChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, DropUnusedInventoryChore>
	{
		public State dropping;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = dropping;
			dropping.Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Storage>().DropAll(false, false, default(Vector3), true);
			}).GoTo(success);
			success.ReturnSuccess();
		}
	}

	public DropUnusedInventoryChore(ChoreType chore_type, IStateMachineTarget target)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), true, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this);
	}
}
