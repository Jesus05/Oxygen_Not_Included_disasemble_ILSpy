using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoScreen : KModalScreen
{
	public static VideoScreen Instance;

	[SerializeField]
	private VideoPlayer videoPlayer;

	[SerializeField]
	private Slideshow slideshow;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton proceedButton;

	[SerializeField]
	private RectTransform overlayContainer;

	[SerializeField]
	private List<VideoOverlay> overlayPrefabs;

	private RawImage screen;

	private RenderTexture renderTexture;

	private string activeAudioSnapshot;

	[SerializeField]
	private Image fadeOverlay;

	private bool victoryLoopQueued = false;

	private string victoryLoopMessage = "";

	private string victoryLoopClip = "";

	private bool videoSkippable = true;

	public System.Action OnStop;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ConsumeMouseScroll = true;
		closeButton.onClick += delegate
		{
			Stop();
		};
		proceedButton.onClick += delegate
		{
			Stop();
		};
		videoPlayer.isLooping = false;
		videoPlayer.loopPointReached += delegate
		{
			if (victoryLoopQueued)
			{
				StartCoroutine(SwitchToVictoryLoop());
			}
			else if (!videoPlayer.isLooping)
			{
				Stop();
			}
		};
		Instance = this;
		Show(false);
	}

	protected override void OnShow(bool show)
	{
		base.transform.SetAsLastSibling();
		base.OnShow(show);
		screen = videoPlayer.gameObject.GetComponent<RawImage>();
	}

	public void DisableAllMedia()
	{
		overlayContainer.gameObject.SetActive(false);
		videoPlayer.gameObject.SetActive(false);
		slideshow.gameObject.SetActive(false);
	}

	public void PlaySlideShow(Sprite[] sprites)
	{
		Show(true);
		DisableAllMedia();
		slideshow.gameObject.SetActive(true);
		slideshow.SetSprites(sprites);
	}

	public override float GetSortKey()
	{
		return 100000f;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.IsAction(Action.Escape))
		{
			if (slideshow.gameObject.activeSelf && e.TryConsume(Action.Escape))
			{
				Stop();
				return;
			}
			if (!videoSkippable)
			{
				return;
			}
		}
		base.OnKeyDown(e);
	}

	public void PlayVideo(VideoClip clip, bool unskippable = false, string overrideAudioSnapshot = "", bool showProceedButton = false)
	{
		for (int i = 0; i < overlayContainer.childCount; i++)
		{
			UnityEngine.Object.Destroy(overlayContainer.GetChild(i).gameObject);
		}
		Show(true);
		videoPlayer.isLooping = false;
		activeAudioSnapshot = ((!string.IsNullOrEmpty(overrideAudioSnapshot)) ? overrideAudioSnapshot : AudioMixerSnapshots.Get().TutorialVideoPlayingSnapshot);
		AudioMixer.instance.Start(activeAudioSnapshot);
		DisableAllMedia();
		videoPlayer.gameObject.SetActive(true);
		renderTexture = new RenderTexture(Convert.ToInt32(clip.width), Convert.ToInt32(clip.height), 16);
		screen.texture = renderTexture;
		videoPlayer.targetTexture = renderTexture;
		videoPlayer.clip = clip;
		videoPlayer.Play();
		videoSkippable = !unskippable;
		closeButton.gameObject.SetActive(videoSkippable);
		proceedButton.gameObject.SetActive(showProceedButton && videoSkippable);
	}

	public void QueueVictoryVideoLoop(bool queue, string message = "", string victoryAchievement = "", string loopVideo = "")
	{
		victoryLoopQueued = queue;
		victoryLoopMessage = message;
		victoryLoopClip = loopVideo;
		OnStop = (System.Action)Delegate.Combine(OnStop, (System.Action)delegate
		{
			RetireColonyUtility.SaveColonySummaryData();
			MainMenu.ActivateRetiredColoniesScreen(base.transform.parent.gameObject, SaveGame.Instance.BaseName, SaveGame.Instance.GetComponent<ColonyAchievementTracker>().achievementsToDisplay.ToArray());
		});
	}

	public void SetOverlayText(string overlayTemplate, List<string> strings)
	{
		VideoOverlay videoOverlay = null;
		foreach (VideoOverlay overlayPrefab in overlayPrefabs)
		{
			if (overlayPrefab.name == overlayTemplate)
			{
				videoOverlay = overlayPrefab;
				break;
			}
		}
		DebugUtil.Assert((UnityEngine.Object)videoOverlay != (UnityEngine.Object)null, "Could not find a template named ", overlayTemplate);
		VideoOverlay videoOverlay2 = Util.KInstantiateUI<VideoOverlay>(videoOverlay.gameObject, overlayContainer.gameObject, true);
		videoOverlay2.SetText(strings);
		overlayContainer.gameObject.SetActive(true);
	}

	private IEnumerator SwitchToVictoryLoop()
	{
		victoryLoopQueued = false;
		Color color = fadeOverlay.color;
		float i = 0f;
		if (i < 1f)
		{
			fadeOverlay.color = new Color(color.r, color.g, color.b, i);
			yield return (object)0;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		fadeOverlay.color = new Color(color.r, color.g, color.b, 1f);
		MusicManager.instance.PlaySong("Music_Victory_03_StoryAndSummary", false);
		MusicManager.instance.SetSongParameter("Music_Victory_03_StoryAndSummary", "songSection", 1f, true);
		closeButton.gameObject.SetActive(true);
		proceedButton.gameObject.SetActive(true);
		SetOverlayText("VictoryEnd", new List<string>
		{
			victoryLoopMessage
		});
		videoPlayer.clip = Assets.GetVideo(victoryLoopClip);
		videoPlayer.isLooping = true;
		videoPlayer.Play();
		proceedButton.gameObject.SetActive(true);
		yield return (object)new WaitForSecondsRealtime(1f);
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public void Stop()
	{
		videoPlayer.Stop();
		screen.texture = null;
		videoPlayer.targetTexture = null;
		AudioMixer.instance.Stop(activeAudioSnapshot, STOP_MODE.ALLOWFADEOUT);
		if (OnStop != null)
		{
			OnStop();
		}
		Show(false);
	}
}
