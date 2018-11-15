using UnityEngine;

public class FlopMonitor : GameStateMachine<FlopMonitor, FlopMonitor.Instance, IStateMachineTarget, FlopMonitor.Def>
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

		public bool ShouldBeginFlopping()
		{
			Vector3 position = base.transform.GetPosition();
			position.y += CreatureFallMonitor.FLOOR_DISTANCE;
			int cell = Grid.PosToCell(base.transform.GetPosition());
			return Grid.Solid[Grid.PosToCell(position)] && !Grid.IsSubstantialLiquid(cell, 0.35f) && !Grid.IsLiquid(Grid.CellAbove(cell));
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.Creatures.Flopping, (Instance smi) => smi.ShouldBeginFlopping(), null);
	}
}
