using UnityEngine;

public class FliesFX : GameStateMachine<FliesFX, FliesFX.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Vector3 offset)
			: base(master)
		{
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("fly_swarm_kanim", base.smi.master.transform.GetPosition() + offset, base.smi.master.transform, false, Grid.SceneLayer.Front, false);
			base.sm.fx.Set(kBatchedAnimController.gameObject, base.smi);
		}

		public void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
		}
	}

	public TargetParameter fx;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		Target(fx);
		root.PlayAnim("swarm_pre").QueueAnim("swarm_loop", true, null).Exit("DestroyFX", delegate(Instance smi)
		{
			smi.DestroyFX();
		});
	}
}
