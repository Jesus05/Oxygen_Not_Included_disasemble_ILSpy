using UnityEngine;

public class FlowerVase : StateMachineComponent<FlowerVase.SMInstance>
{
	public class SMInstance : GameStateMachine<States, SMInstance, FlowerVase, object>.GameInstance
	{
		public SMInstance(FlowerVase master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, FlowerVase>
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

	[MyCmpReq]
	private KBoxCollider2D boxCollider;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
