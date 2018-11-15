using STRINGS;

public class DuplicantsLeftMessage : Message
{
	public override string GetSound()
	{
		return string.Empty;
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.DUPLICANTABSORBED.NAME;
	}

	public override string GetMessageBody()
	{
		return MISC.NOTIFICATIONS.DUPLICANTABSORBED.MESSAGEBODY;
	}

	public override string GetTooltip()
	{
		return MISC.NOTIFICATIONS.DUPLICANTABSORBED.TOOLTIP;
	}
}
