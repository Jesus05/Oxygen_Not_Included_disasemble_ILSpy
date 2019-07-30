using FMOD.Studio;
using System;
using UnityEngine;

public class SculptingSoundEvent : SoundEvent
{
	private const int COUNTER_MODULUS_INVALID = int.MinValue;

	private const int COUNTER_MODULUS_CLEAR = -1;

	private int counterModulus = -2147483648;

	public SculptingSoundEvent(string file_name, string sound_name, int frame, bool do_load, bool is_looping, float min_interval, bool is_dynamic)
		: base(file_name, BaseSoundName(sound_name), frame, do_load, is_looping, min_interval, is_dynamic)
	{
		if (sound_name.Contains(":"))
		{
			string[] array = sound_name.Split(':');
			if (array.Length != 2)
			{
				DebugUtil.LogErrorArgs("Invalid CountedSoundEvent parameter for", file_name + "." + sound_name + "." + frame.ToString() + ":", "'" + sound_name + "'");
			}
			for (int i = 1; i < array.Length; i++)
			{
				ParseParameter(array[i]);
			}
		}
		else
		{
			DebugUtil.LogErrorArgs("CountedSoundEvent for", file_name + "." + sound_name + "." + frame.ToString(), " - Must specify max number of steps on event: '" + sound_name + "'");
		}
	}

	private static string BaseSoundName(string sound_name)
	{
		int num = sound_name.IndexOf(":");
		if (num > 0)
		{
			return sound_name.Substring(0, num);
		}
		return sound_name;
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		if (!string.IsNullOrEmpty(base.sound) && SoundEvent.ShouldPlaySound(behaviour.controller, base.sound, base.looping, isDynamic))
		{
			int num = -1;
			GameObject gameObject = behaviour.controller.gameObject;
			if (counterModulus >= -1)
			{
				HandleVector<int>.Handle h = GameComps.WhiteBoards.GetHandle(gameObject);
				if (!h.IsValid())
				{
					h = GameComps.WhiteBoards.Add(gameObject);
				}
				num = (GameComps.WhiteBoards.HasValue(h, base.soundHash) ? ((int)GameComps.WhiteBoards.GetValue(h, base.soundHash)) : 0);
				int num2 = (counterModulus != -1) ? ((num + 1) % counterModulus) : 0;
				GameComps.WhiteBoards.SetValue(h, base.soundHash, num2);
			}
			Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
			string sound = GlobalAssets.GetSound("Hammer_sculpture", false);
			Worker component = behaviour.GetComponent<Worker>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				Workable workable = component.workable;
				if ((UnityEngine.Object)workable != (UnityEngine.Object)null)
				{
					Building component2 = workable.GetComponent<Building>();
					if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
					{
						BuildingDef def = component2.Def;
						switch (def.name)
						{
						case "MetalSculpture":
							sound = GlobalAssets.GetSound("Hammer_sculpture_metal", false);
							break;
						case "MarbleSculpture":
							sound = GlobalAssets.GetSound("Hammer_sculpture_marble", false);
							break;
						}
					}
				}
			}
			EventInstance instance = SoundEvent.BeginOneShot(sound, position);
			if (instance.isValid())
			{
				if (num >= 0)
				{
					instance.setParameterValue("eventCount", (float)num);
				}
				SoundEvent.EndOneShot(instance);
			}
		}
	}

	private void ParseParameter(string param)
	{
		counterModulus = int.Parse(param);
		if (counterModulus != -1 && counterModulus < 2)
		{
			throw new ArgumentException("CountedSoundEvent modulus must be 2 or larger");
		}
	}
}
