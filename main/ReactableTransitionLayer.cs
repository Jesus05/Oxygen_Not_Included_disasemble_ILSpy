public class ReactableTransitionLayer : TransitionDriver.OverrideLayer
{
	public ReactableTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		ReactionMonitor.Instance reaction_monitor = navigator.GetSMI<ReactionMonitor.Instance>();
		reaction_monitor.PollForReactables(transition);
		if (reaction_monitor.IsReacting())
		{
			transition.anim = null;
			transition.isLooping = false;
			transition.end = transition.start;
			transition.speed = 1f;
			transition.animSpeed = 1f;
			transition.x = 0;
			transition.y = 0;
			transition.isCompleteCB = (() => !reaction_monitor.IsReacting());
		}
	}
}
