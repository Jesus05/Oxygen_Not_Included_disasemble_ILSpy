using System.Collections.Generic;
using UnityEngine;

public class CodexVideo : CodexWidget<CodexVideo>
{
	public string name
	{
		get;
		set;
	}

	public string videoName
	{
		get
		{
			return "--> " + (name ?? "NULL");
		}
		set
		{
			name = value;
		}
	}

	public string overlayName
	{
		get;
		set;
	}

	public List<string> overlayTexts
	{
		get;
		set;
	}

	public void ConfigureVideo(VideoWidget videoWidget, string clipName, string overlayName = null, List<string> overlayTexts = null)
	{
		videoWidget.SetClip(Assets.GetVideo(clipName), overlayName, overlayTexts);
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		ConfigureVideo(contentGameObject.GetComponent<VideoWidget>(), name, overlayName, overlayTexts);
		ConfigurePreferredLayout(contentGameObject);
	}
}
