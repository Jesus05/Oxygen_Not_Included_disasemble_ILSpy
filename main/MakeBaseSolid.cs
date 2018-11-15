using System.Runtime.CompilerServices;

public class MakeBaseSolid : GameStateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	[CompilerGenerated]
	private static StateMachine<MakeBaseSolid, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static StateMachine<MakeBaseSolid, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache1;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.Enter(ConvertToSolid).Exit(ConvertToVacuum);
	}

	private static void ConvertToSolid(Instance smi)
	{
		int num = Grid.PosToCell(smi.gameObject);
		PrimaryElement component = smi.GetComponent<PrimaryElement>();
		SimMessages.ReplaceAndDisplaceElement(num, component.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, component.Mass, component.Temperature, byte.MaxValue, 0, -1);
		Grid.Objects[num, 9] = smi.gameObject;
		Grid.Foundation[num] = true;
		Grid.SetSolid(num, true, CellEventLogger.Instance.SimCellOccupierForceSolid);
		Grid.RenderedByWorld[num] = false;
		World.Instance.OnSolidChanged(num);
		GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
	}

	private static void ConvertToVacuum(Instance smi)
	{
		int num = Grid.PosToCell(smi.gameObject);
		SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn, 0f, -1f, byte.MaxValue, 0, -1);
		Grid.Objects[num, 9] = null;
		Grid.Foundation[num] = false;
		Grid.SetSolid(num, false, CellEventLogger.Instance.SimCellOccupierDestroy);
		Grid.RenderedByWorld[num] = true;
		World.Instance.OnSolidChanged(num);
		GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
	}
}
