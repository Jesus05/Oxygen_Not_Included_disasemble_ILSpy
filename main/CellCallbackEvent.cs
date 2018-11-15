using System.Diagnostics;

public class CellCallbackEvent : CellEvent
{
	public CellCallbackEvent(string id, bool is_send, bool enable_logging = true)
		: base(id, "Callback", is_send, enable_logging)
	{
	}

	[Conditional("ENABLE_CELL_EVENT_LOGGER")]
	public void Log(int cell, int callback_id)
	{
		if (enableLogging)
		{
			CellEventInstance ev = new CellEventInstance(cell, callback_id, 0, this);
			CellEventLogger.Instance.Add(ev);
		}
	}

	public override string GetDescription(EventInstanceBase ev)
	{
		CellEventInstance cellEventInstance = ev as CellEventInstance;
		return GetMessagePrefix() + "Callback=" + cellEventInstance.data.ToString();
	}
}
