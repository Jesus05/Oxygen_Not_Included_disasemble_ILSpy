using KSerialization;
using STRINGS;

public class ResearchCompleteMessage : Message
{
	[Serialize]
	private ResourceRef<Tech> tech = new ResourceRef<Tech>();

	public ResearchCompleteMessage()
	{
	}

	public ResearchCompleteMessage(Tech tech)
	{
		this.tech.Set(tech);
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		Tech tech = this.tech.Get();
		string text = "";
		for (int i = 0; i < tech.unlockedItems.Count; i++)
		{
			if (i != 0)
			{
				text += ", ";
			}
			text += tech.unlockedItems[i].Name;
		}
		return string.Format(MISC.NOTIFICATIONS.RESEARCHCOMPLETE.MESSAGEBODY, tech.Name, text);
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.RESEARCHCOMPLETE.NAME;
	}

	public override string GetTooltip()
	{
		Tech tech = this.tech.Get();
		return string.Format(MISC.NOTIFICATIONS.RESEARCHCOMPLETE.TOOLTIP, tech.Name);
	}

	public override bool IsValid()
	{
		return tech.Get() != null;
	}
}
