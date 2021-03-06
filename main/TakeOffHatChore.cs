using System;
using UnityEngine;

public class TakeOffHatChore : Chore<TakeOffHatChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, TakeOffHatChore, object>.GameInstance
	{
		public StatesInstance(TakeOffHatChore master, GameObject duplicant)
			: base(master)
		{
			base.sm.duplicant.Set(duplicant, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, TakeOffHatChore>
	{
		public TargetParameter duplicant;

		public State remove_hat_pre;

		public State remove_hat;

		public State complete;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = remove_hat_pre;
			Target(duplicant);
			remove_hat_pre.Enter(delegate(StatesInstance smi)
			{
				if (duplicant.Get(smi).GetComponent<MinionResume>().CurrentHat != null)
				{
					smi.GoTo(remove_hat);
				}
				else
				{
					smi.GoTo(complete);
				}
			});
			remove_hat.ToggleAnims("anim_hat_kanim", 0f).PlayAnim("hat_off").OnAnimQueueComplete(complete);
			complete.Enter(delegate(StatesInstance smi)
			{
				smi.master.GetComponent<MinionResume>().RemoveHat();
			}).ReturnSuccess();
		}
	}

	public TakeOffHatChore(IStateMachineTarget target, ChoreType chore_type)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
