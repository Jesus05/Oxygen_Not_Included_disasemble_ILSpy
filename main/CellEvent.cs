public class CellEvent : EventBase
{
	public string reason;

	public bool isSend;

	public bool enableLogging;

	public CellEvent(string id, string reason, bool is_send, bool enable_logging = true)
		: base(id)
	{
		this.reason = reason;
		isSend = is_send;
		enableLogging = enable_logging;
	}

	public string GetMessagePrefix()
	{
		if (isSend)
		{
			return ">>>: ";
		}
		return "<<<: ";
	}
}
