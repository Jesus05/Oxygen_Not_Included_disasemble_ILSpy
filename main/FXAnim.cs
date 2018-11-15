using UnityEngine;

public class FXAnim : GameStateMachine<FXAnim, FXAnim.Instance>
{
	public new class Instance : GameInstance
	{
		private string anim;

		private KAnim.PlayMode mode;

		private KBatchedAnimController animController;

		public Instance(IStateMachineTarget master, string kanim_file, string anim, KAnim.PlayMode mode, Vector3 offset, Color32 tint_colour)
			: base(master)
		{
			animController = FXHelpers.CreateEffect(kanim_file, base.smi.master.transform.GetPosition() + offset, base.smi.master.transform, false, Grid.SceneLayer.Front, false);
			animController.gameObject.Subscribe(-1061186183, OnAnimQueueComplete);
			animController.TintColour = tint_colour;
			base.sm.fx.Set(controller.gameObject, base.smi);
			this.anim = anim;
			this.mode = mode;
		}

		public void Enter()
		{
			animController.Play(anim, mode, 1f, 0f);
		}

		public void Exit()
		{
			DestroyFX();
		}

		private void OnAnimQueueComplete(object data)
		{
			DestroyFX();
		}

		private void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
		}
	}

	public TargetParameter fx;

	public State loop;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = loop;
		Target(fx);
		loop.Enter(delegate(Instance smi)
		{
			smi.Enter();
		}).EventTransition(GameHashes.AnimQueueComplete, loop, null).Exit("Post", delegate(Instance smi)
		{
			smi.Exit();
		});
	}
}
