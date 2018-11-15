using UnityEngine;

public class CreatureVariationSoundEvent : SoundEvent
{
	public CreatureVariationSoundEvent(string file_name, string sound_name, int frame, bool do_load, bool is_looping, float min_interval, bool is_dynamic)
		: base(file_name, sound_name, frame, do_load, is_looping, min_interval, is_dynamic)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		string sound = base.sound;
		CreatureBrain component = behaviour.GetComponent<CreatureBrain>();
		if ((Object)component != (Object)null && !string.IsNullOrEmpty(component.symbolPrefix))
		{
			string sound2 = GlobalAssets.GetSound(StringFormatter.Combine(component.symbolPrefix, base.name), false);
			if (!string.IsNullOrEmpty(sound2))
			{
				sound = sound2;
			}
		}
		PlaySound(behaviour, sound);
	}
}
