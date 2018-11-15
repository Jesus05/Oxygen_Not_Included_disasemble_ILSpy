using System.Collections.Generic;

public class MessageNotification : Notification
{
	public Message message;

	public MessageNotification(Message m)
		: base(m.GetTitle(), NotificationType.Messages, HashedString.Invalid, null, null, false, 0f, null, null)
	{
		message = m;
		if (!message.PlayNotificationSound())
		{
			playSound = false;
		}
		base.ToolTip = ((List<Notification> notifications, object data) => OnToolTip(notifications, m.GetTooltip()));
		hasLocation = false;
	}

	private string OnToolTip(List<Notification> notifications, string tooltipText)
	{
		return tooltipText;
	}
}
