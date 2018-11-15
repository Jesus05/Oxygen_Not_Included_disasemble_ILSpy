using UnityEngine;

[SkipSaveFileSerialization]
public class PlanterBox : StateMachineComponent<PlanterBox.SMInstance>
{
	public class SMInstance : GameStateMachine<States, SMInstance, PlanterBox, object>.GameInstance
	{
		public SMInstance(PlanterBox master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, PlanterBox>
	{
		public State empty;

		public State full;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = empty;
			empty.EventTransition(GameHashes.OccupantChanged, full, (SMInstance smi) => (Object)smi.master.plantablePlot.Occupant != (Object)null).PlayAnim("off");
			full.EventTransition(GameHashes.OccupantChanged, empty, (SMInstance smi) => (Object)smi.master.plantablePlot.Occupant == (Object)null).PlayAnim("on");
		}
	}

	[MyCmpReq]
	private PlantablePlot plantablePlot;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
