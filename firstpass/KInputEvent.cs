public class KInputEvent
{
	public KInputController Controller
	{
		get;
		private set;
	}

	public InputEventType Type
	{
		get;
		private set;
	}

	public bool Consumed
	{
		get;
		set;
	}

	public KInputEvent(KInputController controller, InputEventType event_type)
	{
		Controller = controller;
		Type = event_type;
		Consumed = false;
	}
}
