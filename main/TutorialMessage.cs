using KSerialization;

public class TutorialMessage : GenericMessage
{
	[Serialize]
	public Tutorial.TutorialMessages messageId;

	public TutorialMessage()
	{
	}

	public TutorialMessage(Tutorial.TutorialMessages messageId, string title, string body, string tooltip)
		: base(title, body, tooltip)
	{
		this.messageId = messageId;
	}
}
