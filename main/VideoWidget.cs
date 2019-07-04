using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoWidget : KMonoBehaviour
{
	[SerializeField]
	private VideoClip clip;

	[SerializeField]
	private VideoPlayer thumbnailPlayer;

	[SerializeField]
	private KButton button;

	private RenderTexture renderTexture;

	private RawImage rawImage;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		button.onClick += delegate
		{
			VideoScreen.Instance.PlayVideo(clip, false, "");
		};
		rawImage = thumbnailPlayer.GetComponent<RawImage>();
	}

	public void SetClip(VideoClip clip)
	{
		this.clip = clip;
		renderTexture = new RenderTexture(Convert.ToInt32(clip.width), Convert.ToInt32(clip.height), 16);
		thumbnailPlayer.targetTexture = renderTexture;
		rawImage.texture = renderTexture;
		StartCoroutine(ConfigureThumbnail());
	}

	private IEnumerator ConfigureThumbnail()
	{
		thumbnailPlayer.clip = clip;
		thumbnailPlayer.time = 0.0;
		thumbnailPlayer.Play();
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	private void Update()
	{
		if (thumbnailPlayer.isPlaying && thumbnailPlayer.time > 2.0)
		{
			thumbnailPlayer.Pause();
		}
	}
}
