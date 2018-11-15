using UnityEngine;

public class SplashTransitionLayer : TransitionDriver.OverrideLayer
{
	private float lastSplashTime;

	private const float SPLASH_INTERVAL = 1f;

	public SplashTransitionLayer(Navigator navigator)
		: base(navigator)
	{
		lastSplashTime = Time.time;
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void RefreshSplashes(Navigator navigator, Navigator.ActiveTransition transition)
	{
		if (!((Object)navigator == (Object)null) && transition.end != NavType.Tube)
		{
			Vector3 position = navigator.transform.GetPosition();
			if (lastSplashTime + 1f < Time.time && Grid.Element[Grid.PosToCell(position)].IsLiquid)
			{
				lastSplashTime = Time.time;
				KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("splash_step_kanim", position + new Vector3(0f, 0.75f, -0.1f), null, false, Grid.SceneLayer.Front, false);
				kBatchedAnimController.Play("fx1", KAnim.PlayMode.Once, 1f, 0f);
				kBatchedAnimController.destroyOnAnimComplete = true;
			}
		}
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		RefreshSplashes(navigator, transition);
	}

	public override void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.UpdateTransition(navigator, transition);
		RefreshSplashes(navigator, transition);
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		RefreshSplashes(navigator, transition);
	}
}
