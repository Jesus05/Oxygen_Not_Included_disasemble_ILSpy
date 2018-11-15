public static class StateMachineExtensions
{
	public static bool IsNullOrStopped(this StateMachine.Instance smi)
	{
		return smi == null || !smi.IsRunning();
	}
}
