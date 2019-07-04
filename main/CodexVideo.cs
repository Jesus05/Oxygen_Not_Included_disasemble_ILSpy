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

	public void ConfigureVideo(VideoWidget videoWidget, string clipName)
	{
		videoWidget.SetClip(Assets.GetVideo(clipName));
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		ConfigureVideo(contentGameObject.GetComponent<VideoWidget>(), name);
		ConfigurePreferredLayout(contentGameObject);
	}
}
