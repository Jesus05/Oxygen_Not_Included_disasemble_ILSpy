using STRINGS;

public class AchievementEarnedMessage : Message
{
	public override bool ShowDialog()
	{
		return false;
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		return string.Empty;
	}

	public override string GetTitle()
	{
		return MISC.NOTIFICATIONS.COLONY_ACHIEVEMENT_EARNED.NAME;
	}

	public override string GetTooltip()
	{
		return MISC.NOTIFICATIONS.COLONY_ACHIEVEMENT_EARNED.TOOLTIP;
	}

	public override bool IsValid()
	{
		return true;
	}

	public override void OnClick()
	{
		RetireColonyUtility.SaveColonySummaryData();
		MainMenu.ActivateRetiredColoniesScreen(PauseScreen.Instance.transform.parent.gameObject, SaveGame.Instance.BaseName, null);
	}
}
