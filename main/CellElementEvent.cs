using System.Diagnostics;

public class CellElementEvent : CellEvent
{
	public CellElementEvent(string id, string reason, bool is_send, bool enable_logging = true)
		: base(id, reason, is_send, enable_logging)
	{
	}

	[Conditional("ENABLE_CELL_EVENT_LOGGER")]
	public void Log(int cell, SimHashes element, int callback_id)
	{
		if (enableLogging)
		{
			CellEventInstance ev = new CellEventInstance(cell, (int)element, 0, this);
			CellEventLogger.Instance.Add(ev);
		}
	}

	public override string GetDescription(EventInstanceBase ev)
	{
		CellEventInstance cellEventInstance = ev as CellEventInstance;
		SimHashes data = (SimHashes)cellEventInstance.data;
		return GetMessagePrefix() + "Element=" + data.ToString() + " (" + reason + ")";
	}
}
