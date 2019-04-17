using UnityEngine;

public class RedAlertMonitor : GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void EnableRedAlert()
		{
			ChoreDriver component = GetComponent<ChoreDriver>();
			if ((Object)component != (Object)null)
			{
				Chore currentChore = component.GetCurrentChore();
				if (currentChore != null)
				{
					bool flag = false;
					for (int i = 0; i < currentChore.GetPreconditions().Count; i++)
					{
						Chore.PreconditionInstance preconditionInstance = currentChore.GetPreconditions()[i];
						if (preconditionInstance.id == ChorePreconditions.instance.IsNotRedAlert.id)
						{
							flag = true;
						}
					}
					if (flag)
					{
						component.StopChore();
					}
				}
			}
		}
	}

	public State off;

	public State on;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		base.serializable = true;
		off.EventTransition(GameHashes.EnteredRedAlert, (Instance smi) => Game.Instance, on, (Instance smi) => VignetteManager.Instance.Get().IsRedAlert());
		on.EventTransition(GameHashes.ExitedRedAlert, (Instance smi) => Game.Instance, off, (Instance smi) => !VignetteManager.Instance.Get().IsRedAlert()).Enter("EnableRedAlert", delegate(Instance smi)
		{
			smi.EnableRedAlert();
		}).ToggleEffect("RedAlert")
			.ToggleExpression(Db.Get().Expressions.RedAlert, null);
	}
}
