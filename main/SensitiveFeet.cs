using UnityEngine;

[SkipSaveFileSerialization]
public class SensitiveFeet : StateMachineComponent<SensitiveFeet.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SensitiveFeet, object>.GameInstance
	{
		public StatesInstance(SensitiveFeet master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SensitiveFeet>
	{
		public State satisfied;

		public State suffering;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = satisfied;
			root.Update("SensitiveFeetCheck", delegate(StatesInstance smi, float dt)
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
			suffering.AddEffect("UncomfortableFeet").ToggleExpression(Db.Get().Expressions.Uncomfortable, null);
			satisfied.DoNothing();
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		int num = Grid.CellBelow(Grid.PosToCell(base.gameObject));
		if (Grid.IsValidCell(num) && Grid.Solid[num] && (Object)Grid.Objects[num, 9] == (Object)null)
		{
			return true;
		}
		return false;
	}
}
