using KSerialization;

public class GenericMessage : Message
{
	[Serialize]
	private string title;

	[Serialize]
	private string tooltip;

	[Serialize]
	private string body;

	public GenericMessage(string _title, string _body, string _tooltip)
	{
		title = _title;
		body = _body;
		tooltip = _tooltip;
	}

	public GenericMessage()
	{
	}

	public override string GetSound()
	{
		return null;
	}

	public override string GetMessageBody()
	{
		return body;
	}

	public override string GetTooltip()
	{
		return tooltip;
	}

	public override string GetTitle()
	{
		return title;
	}
}
