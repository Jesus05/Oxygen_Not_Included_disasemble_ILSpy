public abstract class RocketFlightCondition
{
	public abstract bool EvaluateFlightCondition();

	public abstract StatusItem GetFailureStatusItem();

	public RocketFlightCondition GetParentCondition()
	{
		return null;
	}
}
