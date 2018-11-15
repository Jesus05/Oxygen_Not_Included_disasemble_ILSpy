using FMOD.Studio;
using UnityEngine;

public class MainMenuSoundEvent : SoundEvent
{
	public MainMenuSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame, true, false, (float)SoundEvent.IGNORE_INTERVAL, false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		EventInstance instance = KFMOD.BeginOneShot(base.sound, Vector3.zero);
		if (instance.isValid())
		{
			instance.setParameterValue("frame", (float)base.frame);
			KFMOD.EndOneShot(instance);
		}
	}
}
