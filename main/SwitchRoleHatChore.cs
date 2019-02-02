using System;
using UnityEngine;

public class SwitchRoleHatChore : Chore<SwitchRoleHatChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SwitchRoleHatChore, object>.GameInstance
	{
		public StatesInstance(SwitchRoleHatChore master, GameObject duplicant)
			: base(master)
		{
			base.sm.duplicant.Set(duplicant, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SwitchRoleHatChore>
	{
		public TargetParameter duplicant;

		public State remove_hat;

		public State start;

		public State delay;

		public State delay_pst;

		public State applyHat_pre;

		public State applyHat;

		public State complete;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = start;
			Target(duplicant);
			start.Enter(delegate(StatesInstance smi)
			{
				if (duplicant.Get(smi).GetComponent<MinionResume>().CurrentRole == "NoRole")
				{
					smi.GoTo(delay);
				}
				else
				{
					smi.GoTo(remove_hat);
				}
			});
			remove_hat.ToggleAnims("anim_hat_kanim", 0f).PlayAnim("hat_off").OnAnimQueueComplete(delay);
			delay.ToggleThought(Db.Get().Thoughts.NewRole, null).ToggleExpression(Db.Get().Expressions.Happy, null).ToggleAnims("anim_selfish_kanim", 0f)
				.QueueAnim("working_pre", false, null)
				.QueueAnim("working_loop", false, null)
				.QueueAnim("working_pst", false, null)
				.OnAnimQueueComplete(applyHat_pre);
			applyHat_pre.ToggleAnims("anim_hat_kanim", 0f).Enter(delegate(StatesInstance smi)
			{
				RoleManager.ApplyRoleHat(Game.Instance.roleManager.GetRole(duplicant.Get(smi).GetComponent<MinionResume>().CurrentRole), duplicant.Get(smi).GetComponent<Accessorizer>(), duplicant.Get(smi).GetComponent<KBatchedAnimController>());
			}).PlayAnim("hat_first")
				.OnAnimQueueComplete(applyHat);
			applyHat.ToggleAnims("anim_hat_kanim", 0f).PlayAnim("working_pst").OnAnimQueueComplete(complete);
			complete.ReturnSuccess();
		}
	}

	public SwitchRoleHatChore(IStateMachineTarget target, ChoreType chore_type)
		: base(chore_type, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, (Tag[])null, false, ReportManager.ReportType.WorkTime)
	{
		smi = new StatesInstance(this, target.gameObject);
	}
}
