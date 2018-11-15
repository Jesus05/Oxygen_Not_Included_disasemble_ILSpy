public class FullPuftTransitionLayer : TransitionDriver.OverrideLayer
{
	public FullPuftTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		CreatureCalorieMonitor.Instance sMI = navigator.GetSMI<CreatureCalorieMonitor.Instance>();
		if (sMI != null && sMI.stomach.IsReadyToPoop())
		{
			KBatchedAnimController component = navigator.GetComponent<KBatchedAnimController>();
			string s = HashCache.Get().Get(transition.anim.HashValue) + "_full";
			if (component.HasAnimation(s))
			{
				transition.anim = s;
			}
		}
	}
}
