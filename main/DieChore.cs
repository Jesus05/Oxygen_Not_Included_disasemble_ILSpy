using System;

public class DieChore : Chore<DieChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, DieChore, object>.GameInstance
	{
		public StatesInstance(DieChore master, Death death)
			: base(master)
		{
			base.sm.death.Set(death, base.smi);
		}

		public void PlayPreAnim()
		{
			string preAnim = base.sm.death.Get(base.smi).preAnim;
			GetComponent<KAnimControllerBase>().Play(preAnim, KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, DieChore>
	{
		public State dying;

		public State dead;

		public ResourceParameter<Death> death;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = dying;
			dying.OnAnimQueueComplete(dead).Enter("PlayAnim", delegate(StatesInstance smi)
			{
				smi.PlayPreAnim();
			});
			dead.ReturnSuccess();
		}
	}

	public DieChore(IStateMachineTarget master, Death death)
		: base(Db.Get().ChoreTypes.Die, master, master.GetComponent<ChoreProvider>(), false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.emergency, 0, false, true, 0, (Tag[])null)
	{
		showAvailabilityInHoverText = false;
		smi = new StatesInstance(this, death);
	}
}
