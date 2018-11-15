using FMOD.Studio;
using System;
using UnityEngine;

[Serializable]
public class RemoteSoundEvent : SoundEvent
{
	private const string STATE_PARAMETER = "State";

	public RemoteSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, true, false, min_interval, false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		Workable workable = behaviour.GetComponent<Worker>().workable;
		if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
		{
			Toggleable component = workable.GetComponent<Toggleable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				IToggleHandler toggleHandlerForWorker = component.GetToggleHandlerForWorker(behaviour.GetComponent<Worker>());
				float value = 1f;
				if (toggleHandlerForWorker != null && toggleHandlerForWorker.IsHandlerOn())
				{
					value = 0f;
				}
				EventInstance instance = SoundEvent.BeginOneShot(base.sound, position);
				instance.setParameterValue("State", value);
				SoundEvent.EndOneShot(instance);
			}
		}
	}
}
