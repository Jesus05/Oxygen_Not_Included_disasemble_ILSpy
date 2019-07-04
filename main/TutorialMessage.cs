using KSerialization;

public class TutorialMessage : GenericMessage
{
	[Serialize]
	public Tutorial.TutorialMessages messageId;

	public string videoClipId;

	public TutorialMessage()
	{
	}

	public TutorialMessage(Tutorial.TutorialMessages messageId, string title, string body, string tooltip, string videoClipId = null)
		: base(title, body, tooltip)
	{
		this.messageId = messageId;
		this.videoClipId = videoClipId;
	}
}
