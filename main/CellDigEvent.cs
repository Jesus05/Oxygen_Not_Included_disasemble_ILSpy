using System.Diagnostics;

public class CellDigEvent : CellEvent
{
	public CellDigEvent(bool enable_logging = true)
		: base("Dig", "Dig", true, enable_logging)
	{
	}

	[Conditional("ENABLE_CELL_EVENT_LOGGER")]
	public void Log(int cell, int callback_id)
	{
		if (enableLogging)
		{
			CellEventInstance ev = new CellEventInstance(cell, 0, 0, this);
			CellEventLogger.Instance.Add(ev);
		}
	}

	public override string GetDescription(EventInstanceBase ev)
	{
		return GetMessagePrefix() + "Dig=true";
	}
}
