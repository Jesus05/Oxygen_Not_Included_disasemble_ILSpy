using System;
using System.Collections.Generic;

public class NearbyCreatureMonitor : GameStateMachine<NearbyCreatureMonitor, NearbyCreatureMonitor.Instance, IStateMachineTarget>
{
	public new class Instance : GameInstance
	{
		public event Action<float, List<KPrefabID>> OnUpdateNearbyCreatures;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void UpdateNearbyCreatures(float dt)
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(base.gameObject));
			if (cavityForCell != null)
			{
				this.OnUpdateNearbyCreatures(dt, cavityForCell.creatures);
			}
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.Update("UpdateNearbyCreatures", delegate(Instance smi, float dt)
		{
			smi.UpdateNearbyCreatures(dt);
		}, UpdateRate.SIM_1000ms, false);
	}
}
