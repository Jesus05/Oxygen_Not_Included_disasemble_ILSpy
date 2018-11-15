using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MusicManager : KMonoBehaviour, ISerializationCallbackReceiver
{
	[Serializable]
	[DebuggerDisplay("{fmodEvent}")]
	public class SongInfo
	{
		[EventRef]
		public string fmodEvent;

		[NonSerialized]
		public int priority;

		[NonSerialized]
		public bool interruptsActiveMusic;

		[NonSerialized]
		public bool dynamic;

		[NonSerialized]
		public bool useTimeOfDay;

		[NonSerialized]
		public int numberOfVariations;

		[NonSerialized]
		public FMOD.Studio.EventInstance ev;

		[NonSerialized]
		public List<string> songsOnHold = new List<string>();

		[NonSerialized]
		public PLAYBACK_STATE musicPlaybackState;

		[NonSerialized]
		public bool playHook = true;

		[NonSerialized]
		public float sfxAttenuationPercentage = 65f;
	}

	[Serializable]
	[DebuggerDisplay("{fmodEvent}")]
	public class DynamicSong
	{
		[EventRef]
		public string fmodEvent;

		[Tooltip("Some songs are set up to have Morning, Daytime, Hook, and Intro sections. Toggle this ON if this song has those sections.")]
		[SerializeField]
		public bool useTimeOfDay;

		[Tooltip("Some songs have different possible start locations. Enter how many start locations this song is set up to support.")]
		[SerializeField]
		public int numberOfVariations;
	}

	[Serializable]
	[DebuggerDisplay("{fmodEvent}")]
	public class Stinger
	{
		[EventRef]
		public string fmodEvent;
	}

	[Serializable]
	[DebuggerDisplay("{fmodEvent}")]
	public class Minisong
	{
		[EventRef]
		public string fmodEvent;
	}

	public class DynamicSongPlaylist
	{
		public Dictionary<string, SongInfo> songMap = new Dictionary<string, SongInfo>();

		public List<string> unplayedSongs = new List<string>();

		private string lastSongPlayed = string.Empty;

		public string GetNextSong()
		{
			string key;
			if (unplayedSongs.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, unplayedSongs.Count);
				key = unplayedSongs[index];
				unplayedSongs.RemoveAt(index);
			}
			else
			{
				ResetUnplayedSongs();
				bool flag = unplayedSongs.Count > 1;
				if (flag)
				{
					for (int i = 0; i < unplayedSongs.Count; i++)
					{
						if (unplayedSongs[i] == lastSongPlayed)
						{
							unplayedSongs.Remove(unplayedSongs[i]);
							break;
						}
					}
				}
				int index2 = UnityEngine.Random.Range(0, unplayedSongs.Count);
				key = unplayedSongs[index2];
				unplayedSongs.RemoveAt(index2);
				if (flag)
				{
					unplayedSongs.Add(lastSongPlayed);
				}
			}
			lastSongPlayed = key;
			return Assets.GetSimpleSoundEventName(songMap[key].fmodEvent);
		}

		public void ResetUnplayedSongs()
		{
			unplayedSongs.Clear();
			foreach (KeyValuePair<string, SongInfo> item in songMap)
			{
				unplayedSongs.Add(item.Key);
			}
		}
	}

	public enum TypeOfMusic
	{
		DynamicSong,
		MiniSong,
		None
	}

	private const string VARIATION_ID = "variation";

	private const string INTERRUPTED_DIMMED_ID = "interrupted_dimmed";

	private SongInfo[] songs;

	[Header("Song Lists")]
	[Tooltip("Play during the daytime. The mix of the song is affected by the player's input, like pausing the sim, activating an overlay, or zooming in and out.")]
	[SerializeField]
	private DynamicSong[] fullSongs;

	[Tooltip("Simple dynamic songs which are more ambient in nature, which play quietly during \"non-music\" days. These are affected by Pause and OverlayActive.")]
	[SerializeField]
	private Stinger[] miniSongs;

	[Tooltip("Triggered by in-game events, such as completing research or night-time falling. They will temporarily interrupt a dynamicSong, fading the dynamicSong back in after the stinger is complete.")]
	[SerializeField]
	private Stinger[] stingers;

	[Tooltip("Generally songs that don't play during gameplay, while a menu is open. For example, the ESC menu or the Starmap.")]
	[SerializeField]
	private SongInfo[] menuSongs;

	private Dictionary<string, SongInfo> songMap = new Dictionary<string, SongInfo>();

	public Dictionary<string, SongInfo> activeSongs = new Dictionary<string, SongInfo>();

	[NonSerialized]
	public List<string> MusicDebugLog = new List<string>();

	private DynamicSongPlaylist fullSongPlaylist = new DynamicSongPlaylist();

	private DynamicSongPlaylist miniSongPlaylist = new DynamicSongPlaylist();

	[NonSerialized]
	public SongInfo activeDynamicSong;

	[NonSerialized]
	public DynamicSongPlaylist activePlaylist;

	private TypeOfMusic nextMusicType;

	private int musicTypeIterator;

	[Space]
	[Header("Tuning Values")]
	[Tooltip("Just before night-time (88%), dynamic music fades out. At which point of the day should the music fade?")]
	[SerializeField]
	private float duskTimePercentage = 85f;

	[Tooltip("If we load into a save and the day is almost over, we shouldn't play music because it will stop soon anyway. At what point of the day should we not play music?")]
	[SerializeField]
	private float loadGameCutoffPercentage = 50f;

	[Tooltip("When dynamic music is active, we play a snapshot which attenuates the ambience and SFX. What intensity should that snapshot be applied?")]
	[SerializeField]
	private float dynamicMusicSFXAttenuationPercentage = 65f;

	[Tooltip("When mini songs are active, we play a snapshot which attenuates the ambience and SFX. What intensity should that snapshot be applied?")]
	[SerializeField]
	private float miniSongSFXAttenuationPercentage;

	[SerializeField]
	private TypeOfMusic[] musicStyleOrder;

	[NonSerialized]
	public bool alwaysPlayMusic;

	private float time;

	private float timeOfDayUpdateRate = 2f;

	private static MusicManager _instance;

	public Dictionary<string, SongInfo> SongMap => songMap;

	public Dictionary<string, SongInfo> ActiveSongs => activeSongs;

	public static MusicManager instance => _instance;

	private void Log(string s)
	{
	}

	public void PlaySong(string song_name, bool canWait = false)
	{
		Log("Play: " + song_name);
		if (AudioDebug.Get().musicEnabled)
		{
			SongInfo value = null;
			if (!songMap.TryGetValue(song_name, out value))
			{
				Output.LogError("Unknown song:", song_name);
			}
			else if (activeSongs.ContainsKey(song_name))
			{
				Output.LogWarning("Trying to play duplicate song:", song_name);
			}
			else if (activeSongs.Count == 0)
			{
				value.ev = KFMOD.CreateInstance(value.fmodEvent);
				if (!value.ev.isValid())
				{
					Output.LogWarning("Failed to find FMOD event [" + value.fmodEvent + "]");
				}
				int num = (value.numberOfVariations <= 0) ? (-1) : UnityEngine.Random.Range(1, value.numberOfVariations + 1);
				if (num != -1)
				{
					value.ev.setParameterValue("variation", (float)num);
				}
				value.ev.start();
				activeSongs[song_name] = value;
				if (value.dynamic)
				{
					activeDynamicSong = value;
				}
			}
			else
			{
				List<string> list = new List<string>(activeSongs.Keys);
				if (value.interruptsActiveMusic)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!activeSongs[list[i]].interruptsActiveMusic)
						{
							SongInfo songInfo = activeSongs[list[i]];
							songInfo.ev.setParameterValue("interrupted_dimmed", 1f);
							Log("Dimming: " + Assets.GetSimpleSoundEventName(songInfo.fmodEvent));
							value.songsOnHold.Add(list[i]);
						}
					}
					value.ev = KFMOD.CreateInstance(value.fmodEvent);
					if (!value.ev.isValid())
					{
						Output.LogWarning("Failed to find FMOD event [" + value.fmodEvent + "]");
					}
					value.ev.start();
					value.ev.release();
					activeSongs[song_name] = value;
				}
				else
				{
					int num2 = 0;
					foreach (string key in activeSongs.Keys)
					{
						SongInfo songInfo2 = activeSongs[key];
						if (!songInfo2.interruptsActiveMusic && songInfo2.priority > num2)
						{
							num2 = songInfo2.priority;
						}
					}
					if (value.priority >= num2)
					{
						for (int j = 0; j < list.Count; j++)
						{
							SongInfo songInfo3 = activeSongs[list[j]];
							FMOD.Studio.EventInstance ev = songInfo3.ev;
							if (!songInfo3.interruptsActiveMusic)
							{
								ev.setParameterValue("interrupted_dimmed", 1f);
								ev.stop(STOP_MODE.ALLOWFADEOUT);
								activeSongs.Remove(list[j]);
								list.Remove(list[j]);
							}
						}
						value.ev = KFMOD.CreateInstance(value.fmodEvent);
						if (!value.ev.isValid())
						{
							Output.LogWarning("Failed to find FMOD event [" + value.fmodEvent + "]");
						}
						int num3 = (value.numberOfVariations <= 0) ? (-1) : UnityEngine.Random.Range(1, value.numberOfVariations + 1);
						if (num3 != -1)
						{
							value.ev.setParameterValue("variation", (float)num3);
						}
						value.ev.start();
						activeSongs[song_name] = value;
					}
				}
			}
		}
	}

	public void StopSong(string song_name, bool shouldLog = true, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
	{
		if (shouldLog)
		{
			Log("Stop: " + song_name);
		}
		SongInfo value = null;
		if (!songMap.TryGetValue(song_name, out value))
		{
			Output.LogError("Unknown song:", song_name);
		}
		else if (!activeSongs.ContainsKey(song_name))
		{
			Output.LogWarning("Trying to stop a song that isn't playing:", song_name);
		}
		else
		{
			FMOD.Studio.EventInstance ev = value.ev;
			ev.stop(stopMode);
			ev.release();
			if (value.dynamic)
			{
				activeDynamicSong = null;
			}
			if (value.songsOnHold.Count > 0)
			{
				for (int i = 0; i < value.songsOnHold.Count; i++)
				{
					if (activeSongs.TryGetValue(value.songsOnHold[i], out SongInfo value2) && value2.ev.isValid())
					{
						FMOD.Studio.EventInstance ev2 = value2.ev;
						Log("Undimming: " + Assets.GetSimpleSoundEventName(value2.fmodEvent));
						ev2.setParameterValue("interrupted_dimmed", 0f);
						value.songsOnHold.Remove(value.songsOnHold[i]);
					}
					else
					{
						value.songsOnHold.Remove(value.songsOnHold[i]);
					}
				}
			}
			activeSongs.Remove(song_name);
		}
	}

	public void KillAllSongs(STOP_MODE stop_mode = STOP_MODE.IMMEDIATE)
	{
		Log("Kill All Songs");
		if (DynamicMusicIsActive())
		{
			StopDynamicMusic(true);
		}
		List<string> list = new List<string>(activeSongs.Keys);
		for (int i = 0; i < list.Count; i++)
		{
			StopSong(list[i], true, STOP_MODE.ALLOWFADEOUT);
		}
	}

	public void SetSongParameter(string song_name, string parameter_name, float parameter_value, bool shouldLog = true)
	{
		if (shouldLog)
		{
			Log($"Set Param {song_name}: {parameter_name}, {parameter_value}");
		}
		SongInfo value = null;
		if (activeSongs.TryGetValue(song_name, out value))
		{
			FMOD.Studio.EventInstance ev = value.ev;
			if (ev.isValid())
			{
				ev.setParameterValue(parameter_name, parameter_value);
			}
		}
	}

	public bool SongIsPlaying(string song_name)
	{
		SongInfo value = null;
		if (activeSongs.TryGetValue(song_name, out value) && value.musicPlaybackState != PLAYBACK_STATE.STOPPED)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		ClearFinishedSongs();
		if (DynamicMusicIsActive())
		{
			SetDynamicMusicZoomLevel();
			SetDynamicMusicTimeSinceLastJob();
			if (activeDynamicSong.useTimeOfDay)
			{
				SetDynamicMusicTimeOfDay();
			}
			if ((UnityEngine.Object)GameClock.Instance != (UnityEngine.Object)null && GameClock.Instance.GetCurrentCycleAsPercentage() >= duskTimePercentage / 100f)
			{
				StopDynamicMusic(false);
			}
		}
	}

	private void ClearFinishedSongs()
	{
		if (activeSongs.Count > 0)
		{
			ListPool<string, MusicManager>.PooledList pooledList = ListPool<string, MusicManager>.Allocate();
			foreach (KeyValuePair<string, SongInfo> activeSong in activeSongs)
			{
				SongInfo value = activeSong.Value;
				FMOD.Studio.EventInstance ev = value.ev;
				ev.getPlaybackState(out value.musicPlaybackState);
				if (value.musicPlaybackState == PLAYBACK_STATE.STOPPED || value.musicPlaybackState == PLAYBACK_STATE.STOPPING)
				{
					pooledList.Add(activeSong.Key);
					foreach (string item in value.songsOnHold)
					{
						SetSongParameter(item, "interrupted_dimmed", 0f, true);
					}
					value.songsOnHold.Clear();
				}
			}
			foreach (string item2 in pooledList)
			{
				activeSongs.Remove(item2);
			}
			pooledList.Recycle();
		}
	}

	public void OnEscapeMenu(bool paused)
	{
		foreach (KeyValuePair<string, SongInfo> activeSong in activeSongs)
		{
			if (activeSong.Value != null)
			{
				StartFadeToPause(activeSong.Value.ev, paused, 0.25f);
			}
		}
	}

	public void StartFadeToPause(FMOD.Studio.EventInstance inst, bool paused, float fadeTime = 0.25f)
	{
		if (paused)
		{
			StartCoroutine(FadeToPause(inst, fadeTime));
		}
		else
		{
			StartCoroutine(FadeToUnpause(inst, fadeTime));
		}
	}

	private IEnumerator FadeToPause(FMOD.Studio.EventInstance inst, float fadeTime)
	{
		inst.getVolume(out float startVolume, out float targetVolume);
		targetVolume = 0f;
		float lerpTime2 = 0f;
		if (lerpTime2 < 1f)
		{
			float lerpedVolume = Mathf.Lerp(t: lerpTime2 + Time.unscaledDeltaTime / fadeTime, a: startVolume, b: targetVolume);
			inst.setVolume(lerpedVolume);
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		inst.setPaused(true);
	}

	private IEnumerator FadeToUnpause(FMOD.Studio.EventInstance inst, float fadeTime)
	{
		inst.getVolume(out float startVolume, out float targetVolume);
		targetVolume = 1f;
		float lerpTime2 = 0f;
		inst.setPaused(false);
		if (lerpTime2 < 1f)
		{
			float lerpedVolume = Mathf.Lerp(t: lerpTime2 + Time.unscaledDeltaTime / fadeTime, a: startVolume, b: targetVolume);
			inst.setVolume(lerpedVolume);
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!RuntimeManager.IsInitialized)
		{
			base.enabled = false;
		}
		else if (KPlayerPrefs.HasKey(AudioOptionsScreen.AlwaysPlayMusicKey))
		{
			alwaysPlayMusic = ((KPlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayMusicKey) == 1) ? true : false);
		}
	}

	public void PlayDynamicMusic()
	{
		if (DynamicMusicIsActive())
		{
			Log("Trying to play DynamicMusic when it is already playing.");
		}
		else
		{
			string nextDynamicSong = GetNextDynamicSong();
			if (!(nextDynamicSong == "NONE"))
			{
				PlaySong(nextDynamicSong, false);
				if (activeSongs.TryGetValue(nextDynamicSong, out SongInfo value))
				{
					activeDynamicSong = value;
					AudioMixer.instance.Start(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot);
					if ((UnityEngine.Object)SpeedControlScreen.Instance != (UnityEngine.Object)null && SpeedControlScreen.Instance.IsPaused)
					{
						SetDynamicMusicPaused();
					}
					if ((UnityEngine.Object)OverlayScreen.Instance != (UnityEngine.Object)null && OverlayScreen.Instance.mode != 0)
					{
						SetDynamicMusicOverlayActive();
					}
					SetDynamicMusicPlayHook();
					string key = "Volume_Music";
					if (KPlayerPrefs.HasKey(key))
					{
						float @float = KPlayerPrefs.GetFloat(key);
						AudioMixer.instance.SetSnapshotParameter(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, "userVolume_Music", @float, true);
					}
					AudioMixer.instance.SetSnapshotParameter(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, "intensity", value.sfxAttenuationPercentage / 100f, true);
				}
				else
				{
					Log("DynamicMusic song " + nextDynamicSong + " did not start.");
					string text = string.Empty;
					foreach (KeyValuePair<string, SongInfo> activeSong in activeSongs)
					{
						text = text + activeSong.Key + ", ";
						Debug.Log(text, null);
					}
					KCrashReporter.Assert(false, "Song failed to play: " + nextDynamicSong);
				}
			}
		}
	}

	public void StopDynamicMusic(bool stopImmediate = false)
	{
		if (activeDynamicSong != null)
		{
			STOP_MODE stopMode = stopImmediate ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT;
			Log("Stop DynamicMusic: " + Assets.GetSimpleSoundEventName(activeDynamicSong.fmodEvent));
			StopSong(Assets.GetSimpleSoundEventName(activeDynamicSong.fmodEvent), true, stopMode);
			activeDynamicSong = null;
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, STOP_MODE.ALLOWFADEOUT);
		}
	}

	public string GetNextDynamicSong()
	{
		string result = string.Empty;
		if (alwaysPlayMusic && nextMusicType == TypeOfMusic.None)
		{
			while (nextMusicType == TypeOfMusic.None)
			{
				CycleToNextMusicType();
			}
		}
		switch (nextMusicType)
		{
		case TypeOfMusic.DynamicSong:
			result = fullSongPlaylist.GetNextSong();
			activePlaylist = fullSongPlaylist;
			break;
		case TypeOfMusic.MiniSong:
			result = miniSongPlaylist.GetNextSong();
			activePlaylist = miniSongPlaylist;
			break;
		case TypeOfMusic.None:
			result = "NONE";
			activePlaylist = null;
			break;
		}
		CycleToNextMusicType();
		return result;
	}

	private void CycleToNextMusicType()
	{
		musicTypeIterator = ++musicTypeIterator % musicStyleOrder.Length;
		nextMusicType = musicStyleOrder[musicTypeIterator];
	}

	public bool DynamicMusicIsActive()
	{
		return (activeDynamicSong != null) ? true : false;
	}

	public void SetDynamicMusicPaused()
	{
		if (DynamicMusicIsActive())
		{
			SetSongParameter(Assets.GetSimpleSoundEventName(activeDynamicSong.fmodEvent), "Paused", 1f, true);
		}
	}

	public void SetDynamicMusicUnpaused()
	{
		if (DynamicMusicIsActive())
		{
			SetSongParameter(Assets.GetSimpleSoundEventName(activeDynamicSong.fmodEvent), "Paused", 0f, true);
		}
	}

	public void SetDynamicMusicZoomLevel()
	{
		if ((UnityEngine.Object)CameraController.Instance != (UnityEngine.Object)null)
		{
			float parameter_value = 100f - Camera.main.orthographicSize / 20f * 100f;
			SetSongParameter(Assets.GetSimpleSoundEventName(activeDynamicSong.fmodEvent), "zoomPercentage", parameter_value, false);
		}
	}

	public void SetDynamicMusicTimeSinceLastJob()
	{
		SetSongParameter(Assets.GetSimpleSoundEventName(activeDynamicSong.fmodEvent), "secsSinceNewJob", Time.time - Game.Instance.LastTimeWorkStarted, false);
	}

	public void SetDynamicMusicTimeOfDay()
	{
		if (time >= timeOfDayUpdateRate)
		{
			SetSongParameter(Assets.GetSimpleSoundEventName(activeDynamicSong.fmodEvent), "timeOfDay", GameClock.Instance.GetCurrentCycleAsPercentage(), false);
			time = 0f;
		}
		time += Time.deltaTime;
	}

	public void SetDynamicMusicOverlayActive()
	{
		if (DynamicMusicIsActive())
		{
			SetSongParameter(Assets.GetSimpleSoundEventName(activeDynamicSong.fmodEvent), "overlayActive", 1f, true);
		}
	}

	public void SetDynamicMusicOverlayInactive()
	{
		if (DynamicMusicIsActive())
		{
			SetSongParameter(Assets.GetSimpleSoundEventName(activeDynamicSong.fmodEvent), "overlayActive", 0f, true);
		}
	}

	public void SetDynamicMusicPlayHook()
	{
		if (DynamicMusicIsActive())
		{
			string simpleSoundEventName = Assets.GetSimpleSoundEventName(activeDynamicSong.fmodEvent);
			SetSongParameter(simpleSoundEventName, "playHook", (!activeDynamicSong.playHook) ? 0f : 1f, true);
			activePlaylist.songMap[simpleSoundEventName].playHook = !activePlaylist.songMap[simpleSoundEventName].playHook;
		}
	}

	public bool ShouldPlayDynamicMusicLoadedGame()
	{
		if (GameClock.Instance.GetCurrentCycleAsPercentage() <= loadGameCutoffPercentage / 100f)
		{
			return true;
		}
		return false;
	}

	protected override void OnPrefabInit()
	{
		_instance = this;
		fullSongPlaylist.ResetUnplayedSongs();
		miniSongPlaylist.ResetUnplayedSongs();
		nextMusicType = musicStyleOrder[musicTypeIterator];
	}

	protected override void OnCleanUp()
	{
		_instance = null;
	}

	[ContextMenu("Reload")]
	private void ReloadSongs()
	{
		songMap.Clear();
		DynamicSong[] array = fullSongs;
		foreach (DynamicSong dynamicSong in array)
		{
			string simpleSoundEventName = Assets.GetSimpleSoundEventName(dynamicSong.fmodEvent);
			SongInfo songInfo = new SongInfo();
			songInfo.fmodEvent = dynamicSong.fmodEvent;
			songInfo.priority = 100;
			songInfo.interruptsActiveMusic = false;
			songInfo.dynamic = true;
			songInfo.useTimeOfDay = dynamicSong.useTimeOfDay;
			songInfo.numberOfVariations = dynamicSong.numberOfVariations;
			songInfo.sfxAttenuationPercentage = dynamicMusicSFXAttenuationPercentage;
			songMap[simpleSoundEventName] = songInfo;
			fullSongPlaylist.songMap[simpleSoundEventName] = songInfo;
		}
		Stinger[] array2 = miniSongs;
		foreach (Stinger stinger in array2)
		{
			string simpleSoundEventName2 = Assets.GetSimpleSoundEventName(stinger.fmodEvent);
			SongInfo songInfo2 = new SongInfo();
			songInfo2.fmodEvent = stinger.fmodEvent;
			songInfo2.priority = 100;
			songInfo2.interruptsActiveMusic = false;
			songInfo2.dynamic = true;
			songInfo2.useTimeOfDay = false;
			songInfo2.numberOfVariations = 5;
			songInfo2.sfxAttenuationPercentage = miniSongSFXAttenuationPercentage;
			songMap[simpleSoundEventName2] = songInfo2;
			miniSongPlaylist.songMap[simpleSoundEventName2] = songInfo2;
		}
		Stinger[] array3 = stingers;
		foreach (Stinger stinger2 in array3)
		{
			string simpleSoundEventName3 = Assets.GetSimpleSoundEventName(stinger2.fmodEvent);
			SongInfo songInfo3 = new SongInfo();
			songInfo3.fmodEvent = stinger2.fmodEvent;
			songInfo3.priority = 100;
			songInfo3.interruptsActiveMusic = true;
			songInfo3.dynamic = false;
			songInfo3.useTimeOfDay = false;
			songInfo3.numberOfVariations = 0;
			SongMap[simpleSoundEventName3] = songInfo3;
		}
		SongInfo[] array4 = menuSongs;
		foreach (SongInfo songInfo4 in array4)
		{
			string simpleSoundEventName4 = Assets.GetSimpleSoundEventName(songInfo4.fmodEvent);
			SongInfo songInfo5 = new SongInfo();
			songInfo5.fmodEvent = songInfo4.fmodEvent;
			songInfo5.priority = 100;
			songInfo5.interruptsActiveMusic = true;
			songInfo5.dynamic = false;
			songInfo5.useTimeOfDay = false;
			songInfo5.numberOfVariations = 0;
			SongMap[simpleSoundEventName4] = songInfo5;
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		ReloadSongs();
	}
}
