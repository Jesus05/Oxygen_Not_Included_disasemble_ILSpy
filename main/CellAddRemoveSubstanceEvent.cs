using System.Diagnostics;

public class CellAddRemoveSubstanceEvent : CellEvent
{
	public CellAddRemoveSubstanceEvent(string id, string reason, bool enable_logging = false)
		: base(id, reason, true, enable_logging)
	{
	}

	[Conditional("ENABLE_CELL_EVENT_LOGGER")]
	public void Log(int cell, SimHashes element, float amount, int callback_id)
	{
		if (enableLogging)
		{
			CellEventInstance ev = new CellEventInstance(cell, (int)element, (int)(amount * 1000f), this);
			CellEventLogger.Instance.Add(ev);
		}
	}

	public override string GetDescription(EventInstanceBase ev)
	{
		CellEventInstance cellEventInstance = ev as CellEventInstance;
		SimHashes data = (SimHashes)cellEventInstance.data;
		return GetMessagePrefix() + "Element=" + data.ToString() + ", Mass=" + (float)cellEventInstance.data2 / 1000f + " (" + reason + ")";
	}
}
