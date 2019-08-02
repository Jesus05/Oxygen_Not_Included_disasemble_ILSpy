using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialMessageDialog : MessageDialog
{
	[SerializeField]
	private LocText description;

	private TutorialMessage message;

	[SerializeField]
	private GameObject videoWidgetPrefab;

	private VideoWidget videoWidget;

	public override bool CanDontShowAgain => true;

	public override bool CanDisplay(Message message)
	{
		return typeof(TutorialMessage).IsAssignableFrom(message.GetType());
	}

	public override void SetMessage(Message base_message)
	{
		message = (base_message as TutorialMessage);
		description.text = message.GetMessageBody();
		if (!string.IsNullOrEmpty(message.videoClipId))
		{
			VideoClip video = Assets.GetVideo(message.videoClipId);
			SetVideo(video, message.videoOverlayName, message.videoTitleText);
		}
	}

	public void SetVideo(VideoClip clip, string overlayName, string titleText)
	{
		if ((Object)videoWidget == (Object)null)
		{
			videoWidget = Util.KInstantiateUI(videoWidgetPrefab, base.transform.gameObject, true).GetComponent<VideoWidget>();
			videoWidget.transform.SetAsFirstSibling();
		}
		videoWidget.SetClip(clip, overlayName, new List<string>
		{
			titleText,
			VIDEOS.TUTORIAL_HEADER
		});
	}

	public override void OnClickAction()
	{
	}

	public override void OnDontShowAgain()
	{
		Tutorial.Instance.HideTutorialMessage(message.messageId);
	}
}
