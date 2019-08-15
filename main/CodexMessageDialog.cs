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
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		message.OnCleanUp();
	}
}
