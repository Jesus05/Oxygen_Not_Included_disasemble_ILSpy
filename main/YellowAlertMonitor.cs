public class YellowAlertMonitor : GameStateMachine<YellowAlertMonitor, YellowAlertMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void EnableYellowAlert()
		{
		}
	}

	public State off;

	public State on;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		base.serializable = true;
		off.EventTransition(GameHashes.EnteredYellowAlert, (Instance smi) => Game.Instance, on, (Instance smi) => YellowAlertManager.Instance.Get().IsOn());
		on.EventTransition(GameHashes.ExitedYellowAlert, (Instance smi) => Game.Instance, off, (Instance smi) => !YellowAlertManager.Instance.Get().IsOn()).Enter("EnableYellowAlert", delegate(Instance smi)
		{
			smi.EnableYellowAlert();
		});
	}
}
