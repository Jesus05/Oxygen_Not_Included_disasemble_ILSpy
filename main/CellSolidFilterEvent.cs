using System.Diagnostics;

public class CellSolidFilterEvent : CellEvent
{
	public CellSolidFilterEvent(string id, bool enable_logging = true)
		: base(id, "filtered", false, enable_logging)
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
		return GetMessagePrefix() + "Filtered Solid Event solid=" + cellEventInstance.data.ToString();
	}
}
