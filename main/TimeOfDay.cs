using FMOD.Studio;
using KSerialization;
using System.Runtime.Serialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class TimeOfDay : KMonoBehaviour, ISaveLoadable
{
	public enum TimeRegion
	{
		Invalid,
		Day,
		Night
	}

	[Serialize]
	private float scale;

	private TimeRegion timeRegion;

	private EventInstance nightLPEvent;

	public static TimeOfDay Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		timeRegion = GetCurrentTimeRegion();
		UpdateSunlightIntensity();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		UpdateVisuals();
	}

	public TimeRegion GetCurrentTimeRegion()
	{
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		if (currentCycleAsPercentage >= 0.875f)
		{
			return TimeRegion.Night;
		}
		return TimeRegion.Day;
	}

	private void Update()
	{
		UpdateVisuals();
		UpdateAudio();
	}

	private void UpdateVisuals()
	{
		float num = 0.875f;
		float num2 = 0.2f;
		float num3 = 1f;
		float b = 0f;
		if (GameClock.Instance.GetCurrentCycleAsPercentage() >= num)
		{
			b = num3;
		}
		scale = Mathf.Lerp(scale, b, Time.deltaTime * num2);
		float y = UpdateSunlightIntensity();
		Shader.SetGlobalVector("_TimeOfDay", new Vector4(scale, y, 0f, 0f));
	}

	private void UpdateAudio()
	{
		TimeRegion currentTimeRegion = GetCurrentTimeRegion();
		if (currentTimeRegion != timeRegion)
		{
			TriggerSoundChange(currentTimeRegion);
			timeRegion = currentTimeRegion;
			Trigger(1791086652, null);
		}
	}

	public void Sim4000ms(float dt)
	{
		UpdateSunlightIntensity();
	}

	private float UpdateSunlightIntensity()
	{
		float num = 0.875f;
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		float num2 = currentCycleAsPercentage / num;
		if (num2 >= 1f)
		{
			num2 = 0f;
		}
		float num3 = Mathf.Sin(num2 * 3.14159274f);
		Game.Instance.currentSunlightIntensity = num3 * 80000f;
		return num3;
	}

	private void TriggerSoundChange(TimeRegion new_region)
	{
		switch (new_region)
		{
		case TimeRegion.Day:
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().NightStartedMigrated, STOP_MODE.ALLOWFADEOUT);
			if (MusicManager.instance.SongIsPlaying("Stinger_Loop_Night"))
			{
				MusicManager.instance.StopSong("Stinger_Loop_Night", true, STOP_MODE.ALLOWFADEOUT);
			}
			MusicManager.instance.PlaySong("Stinger_Day", false);
			MusicManager.instance.PlayDynamicMusic();
			break;
		case TimeRegion.Night:
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().NightStartedMigrated);
			MusicManager.instance.PlaySong("Stinger_Loop_Night", false);
			break;
		}
	}

	public void SetScale(float new_scale)
	{
		scale = new_scale;
	}
}
