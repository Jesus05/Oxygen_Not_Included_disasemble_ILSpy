using System;

public abstract class NewGameFlowScreen : KModalScreen
{
	public event System.Action OnNavigateForward;

	public event System.Action OnNavigateBackward;

	protected void NavigateBackward()
	{
		this.OnNavigateBackward();
	}

	protected void NavigateForward()
	{
		this.OnNavigateForward();
	}
}
