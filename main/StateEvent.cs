public abstract class StateEvent
{
	public struct Context
	{
		public StateEvent stateEvent;

		public int data;

		public Context(StateEvent state_event)
		{
			stateEvent = state_event;
			data = 0;
		}
	}

	protected string name;

	private string debugName;

	public StateEvent(string name)
	{
		this.name = name;
		debugName = "(Event)" + name;
	}

	public virtual Context Subscribe(StateMachine.Instance smi)
	{
		return new Context(this);
	}

	public virtual void Unsubscribe(StateMachine.Instance smi, Context context)
	{
	}

	public string GetName()
	{
		return name;
	}

	public string GetDebugName()
	{
		return debugName;
	}
}
