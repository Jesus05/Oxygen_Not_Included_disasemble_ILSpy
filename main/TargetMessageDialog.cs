using UnityEngine;

public class TargetMessageDialog : MessageDialog
{
	[SerializeField]
	private LocText description;

	private TargetMessage message;

	public override bool CanDisplay(Message message)
	{
		return typeof(TargetMessage).IsAssignableFrom(message.GetType());
	}

	public override void SetMessage(Message base_message)
	{
		message = (TargetMessage)base_message;
		description.text = message.GetMessageBody();
	}

	public override void OnClickAction()
	{
		MessageTarget target = message.GetTarget();
		SelectTool.Instance.SelectAndFocus(target.GetPosition(), target.GetSelectable());
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		message.OnCleanUp();
	}
}
