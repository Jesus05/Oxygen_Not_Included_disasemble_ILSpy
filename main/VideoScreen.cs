using FMOD.Studio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoScreen : KModalScreen
{
	private struct TextQueue
	{
		public string value;

		public float time;

		public float duration;
	}

	public static VideoScreen Instance;

	[SerializeField]
	private VideoPlayer videoPlayer;

	[SerializeField]
	private Slideshow slideshow;

	[SerializeField]
	private KButton closeButton;

	private RawImage screen;

	private RenderTexture renderTexture;

	private string activeAudioSnapshot;

	private bool victoryLoopQueued = false;

	private string victoryLoopMessage = "";

	private bool videoSkippable = true;

	public System.Action OnStop;

	[SerializeField]
	private LocText textOverlay;

	private List<TextQueue> textQueues = new List<TextQueue>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ConsumeMouseScroll = true;
		closeButton.onClick += delegate
		{
			Stop();
		};
		videoPlayer.isLooping = false;
		videoPlayer.loopPointReached += delegate
		{
			if (victoryLoopQueued)
			{
				SwitchToVictoryLoop();
			}
			else
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

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.IsAction(Action.Escape) || videoSkippable)
		{
			base.OnKeyDown(e);
		}
	}

	public void PlayVideo(VideoClip clip, bool unskippable = false, string overrideAudioSnapshot = "")
	{
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
	}

	public void QueueVictoryVideoLoop(bool queue, string message = "", string victoryAchievement = "")
	{
		victoryLoopQueued = queue;
		victoryLoopMessage = message;
		OnStop = (System.Action)Delegate.Combine(OnStop, (System.Action)delegate
		{
			RetireColonyUtility.SaveColonySummaryData();
			MainMenu.ActivateRetiredColoniesScreen(base.transform.parent.gameObject, SaveGame.Instance.BaseName, SaveGame.Instance.GetComponent<ColonyAchievementTracker>().achievementsToDisplay.ToArray());
		});
	}

	private void Update()
	{
		if (videoPlayer.isPlaying)
		{
			bool flag = false;
			for (int i = 0; i < textQueues.Count; i++)
			{
				double time = videoPlayer.time;
				TextQueue textQueue = textQueues[i];
				if (time >= (double)textQueue.time)
				{
					double time2 = videoPlayer.time;
					TextQueue textQueue2 = textQueues[i];
					float time3 = textQueue2.time;
					TextQueue textQueue3 = textQueues[i];
					if (time2 < (double)(time3 + textQueue3.duration))
					{
						LocText locText = textOverlay;
						TextQueue textQueue4 = textQueues[i];
						locText.SetText(textQueue4.value);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				textOverlay.SetText("");
			}
		}
	}

	public void AddTextQueue(string value, float time, float duration)
	{
		TextQueue item = default(TextQueue);
		item.value = value;
		item.time = time;
		item.duration = duration;
		textQueues.Add(item);
	}

	public void ClearTextQueues()
	{
		textQueues.Clear();
	}

	private void SwitchToVictoryLoop()
	{
		MusicManager.instance.PlaySong("Music_Victory_03_StoryAndSummary", false);
		MusicManager.instance.SetSongParameter("Music_Victory_03_StoryAndSummary", "songSection", 1f, true);
		closeButton.gameObject.SetActive(true);
		ClearTextQueues();
		AddTextQueue(victoryLoopMessage, 0f, 9999f);
		videoPlayer.clip = Assets.GetVideo("Placeholder_grey");
		videoPlayer.isLooping = true;
		videoPlayer.Play();
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
