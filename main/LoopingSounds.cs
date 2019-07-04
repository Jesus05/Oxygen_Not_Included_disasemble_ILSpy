using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class LoopingSounds : KMonoBehaviour
{
	private struct LoopingSoundEvent
	{
		public string asset;

		public HandleVector<int>.Handle handle;
	}

	private List<LoopingSoundEvent> loopingSounds = new List<LoopingSoundEvent>();

	private Dictionary<HashedString, float> lastTimePlayed = new Dictionary<HashedString, float>();

	[SerializeField]
	public bool updatePosition = false;

	public bool IsSoundPlaying(string path)
	{
		foreach (LoopingSoundEvent loopingSound in loopingSounds)
		{
			LoopingSoundEvent current = loopingSound;
			if (current.asset == path)
			{
				return true;
			}
		}
		return false;
	}

	public bool StartSound(string asset, AnimEventManager.EventPlayerData behaviour, EffectorValues noiseValues, bool ignore_pause = false, bool enable_camera_scaled_position = true)
	{
		if (asset != null && !(asset == ""))
		{
			if (!IsSoundPlaying(asset))
			{
				LoopingSoundEvent loopingSoundEvent = default(LoopingSoundEvent);
				loopingSoundEvent.asset = asset;
				LoopingSoundEvent item = loopingSoundEvent;
				Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
				item.handle = LoopingSoundManager.Get().Add(asset, position, base.transform, !ignore_pause, true, enable_camera_scaled_position);
				loopingSounds.Add(item);
			}
			return true;
		}
		Debug.LogWarning("Missing sound");
		return false;
	}

	public bool StartSound(string asset)
	{
		if (asset != null && !(asset == ""))
		{
			if (!IsSoundPlaying(asset))
			{
				LoopingSoundEvent loopingSoundEvent = default(LoopingSoundEvent);
				loopingSoundEvent.asset = asset;
				LoopingSoundEvent item = loopingSoundEvent;
				item.handle = LoopingSoundManager.Get().Add(asset, base.transform.GetPosition(), base.transform, true, true, true);
				loopingSounds.Add(item);
			}
			return true;
		}
		Debug.LogWarning("Missing sound");
		return false;
	}

	public bool StartSound(string asset, bool pause_on_game_pause = true, bool enable_culling = true, bool enable_camera_scaled_position = true)
	{
		if (asset != null && !(asset == ""))
		{
			if (!IsSoundPlaying(asset))
			{
				LoopingSoundEvent loopingSoundEvent = default(LoopingSoundEvent);
				loopingSoundEvent.asset = asset;
				LoopingSoundEvent item = loopingSoundEvent;
				item.handle = LoopingSoundManager.Get().Add(asset, base.transform.GetPosition(), base.transform, pause_on_game_pause, enable_culling, enable_camera_scaled_position);
				loopingSounds.Add(item);
			}
			return true;
		}
		Debug.LogWarning("Missing sound");
		return false;
	}

	public void UpdateVelocity(string asset, Vector2 value)
	{
		foreach (LoopingSoundEvent loopingSound in loopingSounds)
		{
			LoopingSoundEvent current = loopingSound;
			if (current.asset == asset)
			{
				LoopingSoundManager.Get().UpdateVelocity(current.handle, value);
				break;
			}
		}
	}

	public void UpdateFirstParameter(string asset, HashedString parameter, float value)
	{
		foreach (LoopingSoundEvent loopingSound in loopingSounds)
		{
			LoopingSoundEvent current = loopingSound;
			if (current.asset == asset)
			{
				LoopingSoundManager.Get().UpdateFirstParameter(current.handle, parameter, value);
				break;
			}
		}
	}

	public void UpdateSecondParameter(string asset, HashedString parameter, float value)
	{
		foreach (LoopingSoundEvent loopingSound in loopingSounds)
		{
			LoopingSoundEvent current = loopingSound;
			if (current.asset == asset)
			{
				LoopingSoundManager.Get().UpdateSecondParameter(current.handle, parameter, value);
				break;
			}
		}
	}

	private void StopSoundAtIndex(int i)
	{
		LoopingSoundEvent loopingSoundEvent = loopingSounds[i];
		LoopingSoundManager.StopSound(loopingSoundEvent.handle);
	}

	public void StopSound(string asset)
	{
		int num = 0;
		while (true)
		{
			if (num >= loopingSounds.Count)
			{
				return;
			}
			LoopingSoundEvent loopingSoundEvent = loopingSounds[num];
			if (loopingSoundEvent.asset == asset)
			{
				break;
			}
			num++;
		}
		StopSoundAtIndex(num);
		loopingSounds.RemoveAt(num);
	}

	public void StopAllSounds()
	{
		for (int i = 0; i < loopingSounds.Count; i++)
		{
			StopSoundAtIndex(i);
		}
		loopingSounds.Clear();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		StopAllSounds();
	}

	public void SetParameter(string path, HashedString parameter, float value)
	{
		foreach (LoopingSoundEvent loopingSound in loopingSounds)
		{
			LoopingSoundEvent current = loopingSound;
			if (current.asset == path)
			{
				LoopingSoundManager.Get().UpdateFirstParameter(current.handle, parameter, value);
				break;
			}
		}
	}

	public void PlayEvent(GameSoundEvents.Event ev)
	{
		if (AudioDebug.Get().debugGameEventSounds)
		{
			Debug.Log("GameSoundEvent: " + ev.Name);
		}
		List<AnimEvent> events = GameAudioSheets.Get().GetEvents(ev.Name);
		if (events != null)
		{
			Vector2 v = base.transform.GetPosition();
			for (int i = 0; i < events.Count; i++)
			{
				AnimEvent animEvent = events[i];
				SoundEvent soundEvent = animEvent as SoundEvent;
				if (soundEvent == null || soundEvent.sound == null)
				{
					break;
				}
				if (CameraController.Instance.IsAudibleSound(v, soundEvent.sound))
				{
					if (AudioDebug.Get().debugGameEventSounds)
					{
						Debug.Log("GameSound: " + soundEvent.sound);
					}
					float value = 0f;
					if (lastTimePlayed.TryGetValue(soundEvent.soundHash, out value))
					{
						if (Time.time - value > soundEvent.minInterval)
						{
							SoundEvent.PlayOneShot(soundEvent.sound, v);
						}
					}
					else
					{
						SoundEvent.PlayOneShot(soundEvent.sound, v);
					}
					lastTimePlayed[soundEvent.soundHash] = Time.time;
				}
			}
		}
	}
}
