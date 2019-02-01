[SkipSaveFileSerialization]
public class SolitarySleeper : StateMachineComponent<SolitarySleeper.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SolitarySleeper, object>.GameInstance
	{
		public StatesInstance(SolitarySleeper master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SolitarySleeper>
	{
		public State satisfied;

		public State suffering;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = satisfied;
			root.EventTransition(GameHashes.Died, null, (StatesInstance smi) => smi.gameObject.GetSMI<DeathMonitor.Instance>().IsDead()).EventTransition(GameHashes.NewDay, satisfied, null).Update("SolitarySleeperCheck", delegate(StatesInstance smi, float dt)
			{
				if (smi.master.IsUncomfortable())
				{
					if (smi.GetCurrentState() != suffering)
					{
						smi.GoTo(suffering);
					}
				}
				else if (smi.GetCurrentState() != satisfied)
				{
					smi.GoTo(satisfied);
				}
			}, UpdateRate.SIM_4000ms, false);
			suffering.AddEffect("PeopleTooCloseWhileSleeping").ToggleExpression(Db.Get().Expressions.Uncomfortable, null).Update("PeopleTooCloseSleepFail", delegate(StatesInstance smi, float dt)
			{
				smi.master.gameObject.Trigger(1338475637, this);
			}, UpdateRate.SIM_1000ms, false);
			satisfied.DoNothing();
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected bool IsUncomfortable()
	{
		if (base.gameObject.GetSMI<StaminaMonitor.Instance>().IsSleeping())
		{
			int num = 5;
			bool flag = true;
			bool flag2 = true;
			int cell = Grid.PosToCell(base.gameObject);
			for (int i = 1; i < num; i++)
			{
				int num2 = Grid.OffsetCell(cell, i, 0);
				int num3 = Grid.OffsetCell(cell, -i, 0);
				if (Grid.Solid[num3])
				{
					flag = false;
				}
				if (Grid.Solid[num2])
				{
					flag2 = false;
				}
				foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
				{
					if (flag && Grid.PosToCell(item.gameObject) == num3)
					{
						return true;
					}
					if (flag2 && Grid.PosToCell(item.gameObject) == num2)
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}
}
