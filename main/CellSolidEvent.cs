using System.Diagnostics;

public class CellSolidEvent : CellEvent
{
	public CellSolidEvent(string id, string reason, bool is_send, bool enable_logging = true)
		: base(id, reason, is_send, enable_logging)
	{
	}

	[Conditional("ENABLE_CELL_EVENT_LOGGER")]
	public void Log(int cell, bool solid)
	{
		if (enableLogging)
		{
			CellEventInstance ev = new CellEventInstance(cell, solid ? 1 : 0, 0, this);
			CellEventLogger.Instance.Add(ev);
		}
	}

	public override string GetDescription(EventInstanceBase ev)
	{
		CellEventInstance cellEventInstance = ev as CellEventInstance;
		if (cellEventInstance.data == 1)
		{
			return GetMessagePrefix() + "Solid=true (" + reason + ")";
		}
		return GetMessagePrefix() + "Solid=false (" + reason + ")";
	}
}
