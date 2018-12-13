using KSerialization;
using STRINGS;

public class RoleMasteredMessage : Message
{
	[Serialize]
	private Tuple<string, string> description;

	public RoleMasteredMessage()
	{
	}

	public RoleMasteredMessage(MinionResume resume)
	{
		description = new Tuple<string, string>(resume.GetProperName(), resume.CurrentRole);
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		string arg = string.Format(MISC.NOTIFICATIONS.ROLEMASTERED.LINE, description.first, Game.Instance.roleManager.GetRole(description.second).name);
		return string.Format(MISC.NOTIFICATIONS.ROLEMASTERED.MESSAGEBODY, arg);
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.ROLEMASTERED.NAME;
	}

	public override string GetTooltip()
	{
		return string.Format(MISC.NOTIFICATIONS.ROLEMASTERED.TOOLTIP, string.Empty);
	}

	public override bool IsValid()
	{
		return true;
	}
}
