using System;
using UnityEngine;

[Serializable]
public class BuildingDamageSoundEvent : SoundEvent
{
	public BuildingDamageSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame, false, false, (float)SoundEvent.IGNORE_INTERVAL, false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		Worker component = behaviour.GetComponent<Worker>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			SoundEvent.PlayOneShot(GlobalAssets.GetSound("Building_Dmg_Metal", false), position);
		}
		else
		{
			Workable workable = component.workable;
			if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
			{
				Building component2 = workable.GetComponent<Building>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
				{
					BuildingDef def = component2.Def;
					string name = StringFormatter.Combine(base.name, "_", def.AudioCategory);
					string sound = GlobalAssets.GetSound(name, false);
					if (sound == null)
					{
						name = "Building_Dmg_Metal";
						sound = GlobalAssets.GetSound(name, false);
					}
					if (sound != null)
					{
						SoundEvent.PlayOneShot(sound, position);
					}
				}
			}
		}
	}
}
