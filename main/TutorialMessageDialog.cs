using UnityEngine;

public class TutorialMessageDialog : MessageDialog
{
	[SerializeField]
	private LocText description;

	private TutorialMessage message;

	public override bool CanDontShowAgain => true;

	public override bool CanDisplay(Message message)
	{
		return typeof(TutorialMessage).IsAssignableFrom(message.GetType());
	}

	public override void SetMessage(Message base_message)
	{
		message = (base_message as TutorialMessage);
		description.text = message.GetMessageBody();
	}

	public override void OnClickAction()
	{
	}

	public override void OnDontShowAgain()
	{
		Tutorial.Instance.HideTutorialMessage(message.messageId);
	}
}
