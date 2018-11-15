public class SafeCellMonitor : GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private SafeCellSensor safeCellSensor;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			safeCellSensor = GetComponent<Sensors>().GetSensor<SafeCellSensor>();
		}

		public bool IsAreaUnsafe()
		{
			return safeCellSensor.HasSafeCell();
		}
	}

	public State satisfied;

	public State danger;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = false;
		root.ToggleUrge(Db.Get().Urges.MoveToSafety);
		satisfied.EventTransition(GameHashes.SafeCellDetected, danger, (Instance smi) => smi.IsAreaUnsafe());
		danger.EventTransition(GameHashes.SafeCellLost, satisfied, (Instance smi) => !smi.IsAreaUnsafe()).ToggleChore((Instance smi) => new MoveToSafetyChore(smi.master), satisfied);
	}
}
