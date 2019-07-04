using System;
using UnityEngine;

public class SighChore : Chore<SighChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SighChore, object>.GameInstance
	{
		public StatesInstance(SighChore master, GameObject sigher)
			: base(master)
		{
			base.sm.sigher.Set(sigher, base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SighChore>
	{
		public TargetParameter sigher;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			Target(sigher);
			root.PlayAnim("emote_depressed").OnAnimQueueComplete(null);
		}
	}

	public SighChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Sigh, target, target.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
