using System;
using UnityEngine;

public class UISounds : KMonoBehaviour
{
	public enum Sound
	{
		NegativeNotification,
		PositiveNotification,
		Select,
		Negative,
		Back,
		ClickObject,
		HUD_Mouseover,
		Object_Mouseover,
		ClickHUD
	}

	[Serializable]
	private struct SoundData
	{
		public string name;

		public Sound sound;
	}

	[SerializeField]
	private bool logSounds = false;

	[SerializeField]
	private SoundData[] soundData;

	public static UISounds Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	public static void PlaySound(Sound sound)
	{
		Instance.PlaySoundInternal(sound);
	}

	private void PlaySoundInternal(Sound sound)
	{
		for (int i = 0; i < soundData.Length; i++)
		{
			if (soundData[i].sound == sound)
			{
				if (logSounds)
				{
					DebugUtil.LogArgs("Play sound", soundData[i].name);
				}
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound(soundData[i].name, false));
			}
		}
	}
}
