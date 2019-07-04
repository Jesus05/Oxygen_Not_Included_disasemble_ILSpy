using KSerialization;

public class EODReportMessage : Message
{
	[Serialize]
	private int day;

	[Serialize]
	private string title;

	[Serialize]
	private string tooltip;

	public EODReportMessage(string title, string tooltip)
	{
		day = GameUtil.GetCurrentCycle();
		this.title = title;
		this.tooltip = tooltip;
	}

	public EODReportMessage()
	{
	}

	public override string GetSound()
	{
		return null;
	}

	public override string GetMessageBody()
	{
		return "";
	}

	public override string GetTooltip()
	{
		return tooltip;
	}

	public override string GetTitle()
	{
		return title;
	}

	public void OpenReport()
	{
		ManagementMenu.Instance.OpenReports(day);
	}

	public override bool ShowDialog()
	{
		return false;
	}

	public override void OnClick()
	{
		OpenReport();
	}
}
