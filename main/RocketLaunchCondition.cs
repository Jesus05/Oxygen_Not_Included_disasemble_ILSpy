public abstract class RocketLaunchCondition
{
	public abstract bool EvaluateLaunchCondition();

	public abstract string GetLaunchStatusMessage(bool ready);

	public abstract string GetLaunchStatusTooltip(bool ready);

	public virtual RocketLaunchCondition GetParentCondition()
	{
		return null;
	}
}
