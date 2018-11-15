using UnityEngine;

[SkipSaveFileSerialization]
public class Climacophobic : StateMachineComponent<Climacophobic.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Climacophobic, object>.GameInstance
	{
		public StatesInstance(Climacophobic master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Climacophobic>
	{
		public State satisfied;

		public State suffering;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = satisfied;
			root.Update("ClimacophobicCheck", delegate(StatesInstance smi, float dt)
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
			suffering.AddEffect("Vertigo").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			satisfied.DoNothing();
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		int num = 5;
		int cell = Grid.PosToCell(base.gameObject);
		if (isCellLadder(cell))
		{
			int num2 = 1;
			bool flag = true;
			bool flag2 = true;
			for (int i = 1; i < num; i++)
			{
				int cell2 = Grid.OffsetCell(cell, 0, i);
				int cell3 = Grid.OffsetCell(cell, 0, -i);
				if (flag && isCellLadder(cell2))
				{
					num2++;
				}
				else
				{
					flag = false;
				}
				if (flag2 && isCellLadder(cell3))
				{
					num2++;
				}
				else
				{
					flag2 = false;
				}
			}
			return num2 >= num;
		}
		return false;
	}

	private bool isCellLadder(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		GameObject gameObject = Grid.Objects[cell, 1];
		if ((Object)gameObject == (Object)null)
		{
			return false;
		}
		if ((Object)gameObject.GetComponent<Ladder>() == (Object)null)
		{
			return false;
		}
		return true;
	}
}
