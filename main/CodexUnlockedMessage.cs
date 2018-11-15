using STRINGS;

public class CodexUnlockedMessage : Message
{
	private string unlockMessage;

	private string lockId;

	public CodexUnlockedMessage()
	{
	}

	public CodexUnlockedMessage(string lock_id, string unlock_message)
	{
		lockId = lock_id;
		unlockMessage = unlock_message;
	}

	public string GetLockId()
	{
		return lockId;
	}

	public override string GetSound()
	{
		return "AI_Notification_ResearchComplete";
	}

	public override string GetMessageBody()
	{
		return UI.CODEX.CODEX_DISCOVERED_MESSAGE.BODY.Replace("{codex}", unlockMessage);
	}

	public override string GetTitle()
	{
		return UI.CODEX.CODEX_DISCOVERED_MESSAGE.TITLE;
	}

	public override string GetTooltip()
	{
		return GetMessageBody();
	}

	public override bool IsValid()
	{
		return true;
	}
}
