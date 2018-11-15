public class User : KMonoBehaviour
{
	public void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		if (status == StateMachine.Status.Success)
		{
			Trigger(58624316, null);
		}
		else
		{
			Trigger(1572098533, null);
		}
	}
}
