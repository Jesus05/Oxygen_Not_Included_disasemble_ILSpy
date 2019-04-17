using System;
using UnityEngine;

public class PutOnHatChore : Chore<PutOnHatChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PutOnHatChore, object>.GameInstance
	{
		public StatesInstance(PutOnHatChore master, GameObject duplicant)
			: base(master)
		{
			base.sm.duplicant.Set(duplicant, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PutOnHatChore>
	{
		public TargetParameter duplicant;

		public State applyHat_pre;

		public State applyHat;

		public State complete;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = applyHat_pre;
			Target(duplicant);
			applyHat_pre.ToggleAnims("anim_hat_kanim", 0f).Enter(delegate(StatesInstance smi)
			{
				duplicant.Get(smi).GetComponent<MinionResume>().ApplyTargetHat();
			}).PlayAnim("hat_first")
				.OnAnimQueueComplete(applyHat);
			applyHat.ToggleAnims("anim_hat_kanim", 0f).PlayAnim("working_pst").OnAnimQueueComplete(complete);
			complete.ReturnSuccess();
		}
	}

	public PutOnHatChore(IStateMachineTarget target, ChoreType chore_type)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, (Tag[])null, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
