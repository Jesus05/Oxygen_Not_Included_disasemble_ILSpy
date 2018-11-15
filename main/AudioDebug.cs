using UnityEngine;

public class AudioDebug : KMonoBehaviour
{
	private static AudioDebug instance;

	public bool musicEnabled;

	public bool debugSoundEvents;

	public bool debugFloorSounds;

	public bool debugGameEventSounds;

	public bool debugNotificationSounds;

	public bool debugVoiceSounds;

	public static AudioDebug Get()
	{
		return instance;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
	}

	public void ToggleMusic()
	{
		if ((Object)Game.Instance != (Object)null)
		{
			Game.Instance.SetMusicEnabled(musicEnabled);
		}
		musicEnabled = !musicEnabled;
	}
}
