using UnityEngine;

public class StandardMessageDialog : MessageDialog
{
	[SerializeField]
	private LocText description;

	private Message message;

	public override bool CanDisplay(Message message)
	{
		return typeof(Message).IsAssignableFrom(message.GetType());
	}

	public override void SetMessage(Message base_message)
	{
		message = base_message;
		description.text = message.GetMessageBody();
	}

	public override void OnClickAction()
	{
	}
}
