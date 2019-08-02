using FMOD.Studio;
using System;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class SoundEvent : AnimEvent
{
	public static int IGNORE_INTERVAL = -1;

	protected bool isDynamic;

	public string sound
	{
		get;
		private set;
	}

	public HashedString soundHash
	{
		get;
		private set;
	}

	public bool looping
	{
		get;
		private set;
	}

	public bool ignorePause
	{
		get;
		set;
	}

	public bool shouldCameraScalePosition
	{
		get;
		set;
	}

	public float minInterval
	{
		get;
		private set;
	}

	public EffectorValues noiseValues
	{
		get;
		set;
	}

	public SoundEvent()
	{
	}

	public SoundEvent(string file_name, string sound_name, int frame, bool do_load, bool is_looping, float min_interval, bool is_dynamic)
		: base(file_name, sound_name, frame)
	{
		shouldCameraScalePosition = true;
		if (do_load)
		{
			sound = GlobalAssets.GetSound(sound_name, false);
			soundHash = new HashedString(sound);
			if (sound != null && !(sound == string.Empty))
			{
				goto IL_0055;
			}
		}
		goto IL_0055;
		IL_0055:
		minInterval = min_interval;
		looping = is_looping;
		isDynamic = is_dynamic;
		noiseValues = SoundEventVolumeCache.instance.GetVolume(file_name, sound_name);
	}

	public static bool ShouldPlaySound(KBatchedAnimController controller, string sound, bool is_looping, bool is_dynamic)
	{
		CameraController instance = CameraController.Instance;
		if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
		{
			return true;
		}
		Vector3 position = controller.transform.GetPosition();
		Vector3 offset = controller.Offset;
		position.x += offset.x;
		position.y += offset.y;
		SpeedControlScreen instance2 = SpeedControlScreen.Instance;
		if (is_dynamic)
		{
			if ((UnityEngine.Object)instance2 != (UnityEngine.Object)null && instance2.IsPaused)
			{
				return false;
			}
			if (!instance.IsAudibleSound(position))
			{
				return false;
			}
			return true;
		}
		if (sound == null || IsLowPrioritySound(sound))
		{
			return false;
		}
		if (!instance.IsAudibleSound(position, sound))
		{
			if (!is_looping && !GlobalAssets.IsHighPriority(sound))
			{
				return false;
			}
		}
		else if ((UnityEngine.Object)instance2 != (UnityEngine.Object)null && instance2.IsPaused)
		{
			return false;
		}
		return true;
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		if (ShouldPlaySound(behaviour.controller, sound, looping, isDynamic))
		{
			PlaySound(behaviour);
		}
	}

	protected void PlaySound(AnimEventManager.EventPlayerData behaviour, string sound)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		KBatchedAnimController component = behaviour.GetComponent<KBatchedAnimController>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			Vector3 offset = component.Offset;
			position.x += offset.x;
			position.y += offset.y;
		}
		AudioDebug audioDebug = AudioDebug.Get();
		if ((UnityEngine.Object)audioDebug != (UnityEngine.Object)null && audioDebug.debugSoundEvents)
		{
			Debug.Log(behaviour.name + ", " + sound + ", " + base.frame + ", " + position);
		}
		try
		{
			if (looping)
			{
				LoopingSounds component2 = behaviour.GetComponent<LoopingSounds>();
				if ((UnityEngine.Object)component2 == (UnityEngine.Object)null)
				{
					Debug.Log(behaviour.name + " is missing LoopingSounds component. ");
				}
				else if (!component2.StartSound(sound, behaviour, noiseValues, ignorePause, shouldCameraScalePosition))
				{
					DebugUtil.LogWarningArgs($"SoundEvent has invalid sound [{sound}] on behaviour [{behaviour.name}]");
				}
			}
			else if (!PlayOneShot(sound, behaviour, noiseValues))
			{
				DebugUtil.LogWarningArgs($"SoundEvent has invalid sound [{sound}] on behaviour [{behaviour.name}]");
			}
		}
		catch (Exception ex)
		{
			string text = string.Format(("Error trying to trigger sound [{0}] in behaviour [{1}] [{2}]\n{3}" + sound == null) ? "null" : sound.ToString(), behaviour.GetType().ToString(), ex.Message, ex.StackTrace);
			Debug.LogError(text);
			throw new ArgumentException(text, ex);
		}
	}

	public virtual void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		PlaySound(behaviour, sound);
	}

	public static Vector3 GetCameraScaledPosition(Vector3 pos)
	{
		Vector3 result = Vector3.zero;
		if ((UnityEngine.Object)CameraController.Instance != (UnityEngine.Object)null)
		{
			result = CameraController.Instance.GetVerticallyScaledPosition(pos);
		}
		return result;
	}

	public static FMOD.Studio.EventInstance BeginOneShot(string ev, Vector3 pos)
	{
		return KFMOD.BeginOneShot(ev, GetCameraScaledPosition(pos));
	}

	public static bool EndOneShot(FMOD.Studio.EventInstance instance)
	{
		return KFMOD.EndOneShot(instance);
	}

	public static bool PlayOneShot(string sound, Vector3 pos)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(sound))
		{
			FMOD.Studio.EventInstance instance = BeginOneShot(sound, pos);
			if (instance.isValid())
			{
				result = EndOneShot(instance);
			}
		}
		return result;
	}

	public static bool PlayOneShot(string sound, AnimEventManager.EventPlayerData behaviour, EffectorValues noiseValues)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(sound))
		{
			Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
			FMOD.Studio.EventInstance instance = BeginOneShot(sound, position);
			if (instance.isValid())
			{
				result = EndOneShot(instance);
			}
		}
		return result;
	}

	public override void Stop(AnimEventManager.EventPlayerData behaviour)
	{
		if (looping)
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.StopSound(sound);
			}
		}
	}

	protected static bool IsLowPrioritySound(string sound)
	{
		if (sound != null && Camera.main.orthographicSize > AudioMixer.LOW_PRIORITY_CUTOFF_DISTANCE && !AudioMixer.instance.activeNIS && GlobalAssets.IsLowPriority(sound))
		{
			return true;
		}
		return false;
	}

	protected void PrintSoundDebug(string anim_name, string sound, string sound_name, Vector3 sound_pos)
	{
		if (sound != null)
		{
			Debug.Log(anim_name + ", " + sound_name + ", " + base.frame + ", " + sound_pos);
		}
		else
		{
			Debug.Log("Missing sound: " + anim_name + ", " + sound_name);
		}
	}
}
