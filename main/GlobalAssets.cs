using FMOD;
using FMOD.Studio;
using FMODUnity;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAssets : MonoBehaviour
{
	private static Dictionary<string, string> SoundTable = new Dictionary<string, string>();

	private static HashSet<string> LowPrioritySounds = new HashSet<string>();

	private static HashSet<string> HighPrioritySounds = new HashSet<string>();

	private void Awake()
	{
		if (SoundTable.Count == 0)
		{
			Bank[] array = null;
			try
			{
				if (RuntimeManager.StudioSystem.getBankList(out array) != 0)
				{
					array = null;
				}
			}
			catch
			{
				array = null;
			}
			if (array != null)
			{
				Bank[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					Bank bank = array2[i];
					EventDescription[] array3;
					RESULT eventList = bank.getEventList(out array3);
					string path;
					if (eventList != 0)
					{
						bank.getPath(out path);
						Debug.LogError($"ERROR [{eventList}] loading FMOD events for bank [{path}]");
					}
					else
					{
						for (int j = 0; j < array3.Length; j++)
						{
							EventDescription eventDescription = array3[j];
							eventDescription.getPath(out path);
							string simpleSoundEventName = Assets.GetSimpleSoundEventName(path);
							simpleSoundEventName = simpleSoundEventName.ToLowerInvariant();
							if (simpleSoundEventName.Length > 0 && !SoundTable.ContainsKey(simpleSoundEventName))
							{
								SoundTable[simpleSoundEventName] = path;
								if (path.ToLower().Contains("lowpriority") || simpleSoundEventName.Contains("lowpriority"))
								{
									LowPrioritySounds.Add(path);
								}
								else if (path.ToLower().Contains("highpriority") || simpleSoundEventName.Contains("highpriority"))
								{
									HighPrioritySounds.Add(path);
								}
							}
						}
					}
				}
			}
		}
		SetDefaults.Initialize();
		LocString.CreateLocStringKeys(typeof(DUPLICANTS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(MISC), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(UI), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(ELEMENTS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(CREATURES), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(SETITEMS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(RESEARCH), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(ITEMS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(INPUT), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(INPUT_BINDINGS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(BUILDING.STATUSITEMS), "STRINGS.BUILDING.");
		LocString.CreateLocStringKeys(typeof(BUILDING.DETAILS), "STRINGS.BUILDING.");
		LocString.CreateLocStringKeys(typeof(LORE), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(CODEX), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(WORLDS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(WORLD_TRAITS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(COLONY_ACHIEVEMENTS), "STRINGS.");
		LocString.CreateLocStringKeys(typeof(VIDEOS), "STRINGS.");
	}

	public static string GetSound(string name, bool force_no_warning = false)
	{
		if (name == null)
		{
			return null;
		}
		name = name.ToLowerInvariant();
		string value = null;
		SoundTable.TryGetValue(name, out value);
		return value;
	}

	public static bool IsLowPriority(string path)
	{
		return LowPrioritySounds.Contains(path);
	}

	public static bool IsHighPriority(string path)
	{
		return HighPrioritySounds.Contains(path);
	}
}
