using KSerialization;

public class TutorialMessage : GenericMessage
{
	[Serialize]
	public Tutorial.TutorialMessages messageId;

	public string videoClipId;

	public string videoOverlayName;

	public string videoTitleText;

	public TutorialMessage()
	{
	}

	public TutorialMessage(Tutorial.TutorialMessages messageId, string title, string body, string tooltip, string videoClipId = null, string videoOverlayName = null, string videoTitleText = null)
		: base(title, body, tooltip)
	{
		this.messageId = messageId;
		this.videoClipId = videoClipId;
		this.videoOverlayName = videoOverlayName;
		this.videoTitleText = videoTitleText;
	}
}
