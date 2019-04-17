using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;

public class PhonoboxSoundEvent : SoundEvent
{
	private const string SOUND_PARAM_SONG = "jukeboxSong";

	private const string SOUND_PARAM_PITCH = "jukeboxPitch";

	private int song;

	private int pitch;

	public PhonoboxSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, true, true, min_interval, false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		AudioDebug audioDebug = AudioDebug.Get();
		if ((UnityEngine.Object)audioDebug != (UnityEngine.Object)null && audioDebug.debugSoundEvents)
		{
			Debug.Log(behaviour.name + ", " + base.sound + ", " + base.frame + ", " + position);
		}
		try
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if ((UnityEngine.Object)component == (UnityEngine.Object)null)
			{
				Debug.Log(behaviour.name + " is missing LoopingSounds component. ");
			}
			else if (!component.IsSoundPlaying(base.sound))
			{
				if (component.StartSound(base.sound, behaviour, base.noiseValues, base.ignorePause, true))
				{
					EventDescription eventDescription = RuntimeManager.GetEventDescription(base.sound);
					eventDescription.getParameter("jukeboxSong", out PARAMETER_DESCRIPTION parameter);
					int num = (int)parameter.maximum;
					eventDescription.getParameter("jukeboxPitch", out PARAMETER_DESCRIPTION parameter2);
					int num2 = (int)parameter2.maximum;
					song = UnityEngine.Random.Range(0, num + 1);
					pitch = UnityEngine.Random.Range(0, num2 + 1);
					component.UpdateFirstParameter(base.sound, "jukeboxSong", (float)song);
					component.UpdateSecondParameter(base.sound, "jukeboxPitch", (float)pitch);
				}
				else
				{
					DebugUtil.LogWarningArgs($"SoundEvent has invalid sound [{base.sound}] on behaviour [{behaviour.name}]");
				}
			}
		}
		catch (Exception ex)
		{
			string text = string.Format(("Error trying to trigger sound [{0}] in behaviour [{1}] [{2}]\n{3}" + base.sound == null) ? "null" : base.sound.ToString(), behaviour.GetType().ToString(), ex.Message, ex.StackTrace);
			Debug.LogError(text);
			throw new ArgumentException(text, ex);
		}
	}
}
