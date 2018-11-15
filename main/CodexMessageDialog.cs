using UnityEngine;

public class CodexMessageDialog : MessageDialog
{
	[SerializeField]
	private LocText description;

	private CodexUnlockedMessage message;

	public override bool CanDisplay(Message message)
	{
		return typeof(CodexUnlockedMessage).IsAssignableFrom(message.GetType());
	}

	public override void SetMessage(Message base_message)
	{
		message = (CodexUnlockedMessage)base_message;
		description.text = message.GetMessageBody();
	}

	public override void OnClickAction()
	{
		string lockId = message.GetLockId();
		if (!string.IsNullOrEmpty(lockId))
		{
			string entryForLock = CodexCache.GetEntryForLock(message.GetLockId());
			if (!string.IsNullOrEmpty(entryForLock))
			{
				ManagementMenu.Instance.OpenCodexToEntry(entryForLock);
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		message.OnCleanUp();
	}
}
