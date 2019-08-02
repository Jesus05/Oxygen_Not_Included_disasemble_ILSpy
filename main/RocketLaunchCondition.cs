public abstract class RocketLaunchCondition
{
	public enum LaunchStatus
	{
		Ready,
		Warning,
		Failure
	}

	public abstract LaunchStatus EvaluateLaunchCondition();

	public abstract string GetLaunchStatusMessage(bool ready);

	public abstract string GetLaunchStatusTooltip(bool ready);

	public virtual RocketLaunchCondition GetParentCondition()
	{
		return null;
	}
}
