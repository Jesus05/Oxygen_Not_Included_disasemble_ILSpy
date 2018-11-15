[SkipSaveFileSerialization]
public class Claustrophobic : StateMachineComponent<Claustrophobic.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Claustrophobic, object>.GameInstance
	{
		public StatesInstance(Claustrophobic master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Claustrophobic>
	{
		public State satisfied;

		public State suffering;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = satisfied;
			root.Update("ClaustrophobicCheck", delegate(StatesInstance smi, float dt)
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
			suffering.AddEffect("Claustrophobic").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			satisfied.DoNothing();
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		int num = 4;
		int cell = Grid.PosToCell(base.gameObject);
		for (int i = 0; i < num - 1; i++)
		{
			int num2 = Grid.OffsetCell(cell, 0, i);
			if (Grid.IsValidCell(num2) && Grid.Solid[num2])
			{
				return true;
			}
			if (Grid.IsValidCell(Grid.CellRight(cell)) && Grid.IsValidCell(Grid.CellLeft(cell)) && Grid.Solid[Grid.CellRight(cell)] && Grid.Solid[Grid.CellLeft(cell)])
			{
				return true;
			}
		}
		return false;
	}
}
