[SkipSaveFileSerialization]
public class Workaholic : StateMachineComponent<Workaholic.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Workaholic, object>.GameInstance
	{
		public StatesInstance(Workaholic master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Workaholic>
	{
		public State satisfied;

		public State suffering;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = satisfied;
			root.Update("WorkaholicCheck", delegate(StatesInstance smi, float dt)
			{
				if (smi.master.IsUncomfortable())
				{
					smi.GoTo(suffering);
				}
				else
				{
					smi.GoTo(satisfied);
				}
			}, UpdateRate.SIM_1000ms, false);
			suffering.AddEffect("Restless").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			satisfied.DoNothing();
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		ChoreDriver component = base.smi.master.GetComponent<ChoreDriver>();
		return component.GetCurrentChore() is IdleChore;
	}
}
