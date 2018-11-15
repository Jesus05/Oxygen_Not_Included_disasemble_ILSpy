using UnityEngine;

public class FarmTile : StateMachineComponent<FarmTile.SMInstance>
{
	public class SMInstance : GameStateMachine<States, SMInstance, FarmTile, object>.GameInstance
	{
		public SMInstance(FarmTile master)
			: base(master)
		{
		}

		public bool HasWater()
		{
			PrimaryElement primaryElement = base.master.storage.FindPrimaryElement(SimHashes.Water);
			return (Object)primaryElement != (Object)null && primaryElement.Mass > 0f;
		}
	}

	public class States : GameStateMachine<States, SMInstance, FarmTile>
	{
		public class FarmStates : State
		{
			public State wet;

			public State dry;
		}

		public FarmStates empty;

		public FarmStates full;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = empty;
			empty.EventTransition(GameHashes.OccupantChanged, full, (SMInstance smi) => (Object)smi.master.plantablePlot.Occupant != (Object)null);
			empty.wet.EventTransition(GameHashes.OnStorageChange, empty.dry, (SMInstance smi) => !smi.HasWater());
			empty.dry.EventTransition(GameHashes.OnStorageChange, empty.wet, (SMInstance smi) => !smi.HasWater());
			full.EventTransition(GameHashes.OccupantChanged, empty, (SMInstance smi) => (Object)smi.master.plantablePlot.Occupant == (Object)null);
			full.wet.EventTransition(GameHashes.OnStorageChange, full.dry, (SMInstance smi) => !smi.HasWater());
			full.dry.EventTransition(GameHashes.OnStorageChange, full.wet, (SMInstance smi) => !smi.HasWater());
		}
	}

	[MyCmpReq]
	private PlantablePlot plantablePlot;

	[MyCmpReq]
	private Storage storage;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
