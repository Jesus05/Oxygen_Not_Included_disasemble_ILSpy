using UnityEngine;

public class UpgradeFX : GameStateMachine<UpgradeFX, UpgradeFX.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Vector3 offset)
			: base(master)
		{
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("upgrade_fx_kanim", master.gameObject.transform.GetPosition() + offset, master.gameObject.transform, true, Grid.SceneLayer.Front, false);
			base.sm.fx.Set(kBatchedAnimController.gameObject, base.smi);
		}

		public void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
		}

		public Reactable CreateReactable()
		{
			return new EmoteReactable(base.master.gameObject, "UpgradeFX", Db.Get().ChoreTypes.Emote, "anim_cheer_kanim", 15, 8, 0f, 20f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
			{
				anim = (HashedString)"cheer_pre"
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = (HashedString)"cheer_loop"
			}).AddStep(new EmoteReactable.EmoteStep
			{
				anim = (HashedString)"cheer_pst"
			});
		}
	}

	public TargetParameter fx;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		Target(fx);
		root.PlayAnim("upgrade").OnAnimQueueComplete(null).ToggleReactable((Instance smi) => smi.CreateReactable())
			.Exit("DestroyFX", delegate(Instance smi)
			{
				smi.DestroyFX();
			});
	}
}
