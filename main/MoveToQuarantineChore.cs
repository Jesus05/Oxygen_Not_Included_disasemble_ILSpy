using System;
using UnityEngine;

public class MoveToQuarantineChore : Chore<MoveToQuarantineChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, MoveToQuarantineChore, object>.GameInstance
	{
		public StatesInstance(MoveToQuarantineChore master, GameObject quarantined)
			: base(master)
		{
			base.sm.quarantined.Set(quarantined, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, MoveToQuarantineChore>
	{
		public TargetParameter locator;

		public TargetParameter quarantined;

		public ApproachSubState<IApproachable> approach;

		public State success;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approach;
			approach.InitializeStates(quarantined, locator, success, null, null, null);
			success.ReturnSuccess();
		}
	}

	public MoveToQuarantineChore(IStateMachineTarget target, KMonoBehaviour quarantine_area)
		: base(Db.Get().ChoreTypes.MoveToQuarantine, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, (Tag[])null, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
		base.smi.sm.locator.Set(quarantine_area.gameObject, base.smi);
	}
}
