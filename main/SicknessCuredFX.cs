using UnityEngine;

public class SicknessCuredFX : GameStateMachine<SicknessCuredFX, SicknessCuredFX.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Vector3 offset)
			: base(master)
		{
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("recentlyhealed_fx_kanim", master.gameObject.transform.GetPosition() + offset, master.gameObject.transform, true, Grid.SceneLayer.Front, false);
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
		root.PlayAnim("upgrade").OnAnimQueueComplete(null).Exit("DestroyFX", delegate(Instance smi)
		{
			smi.DestroyFX();
		});
	}
}
