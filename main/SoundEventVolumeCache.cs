using System.Collections.Generic;

public class SoundEventVolumeCache : Singleton<SoundEventVolumeCache>
{
	public Dictionary<HashedString, EffectorValues> volumeCache = new Dictionary<HashedString, EffectorValues>();

	public static SoundEventVolumeCache instance => Singleton<SoundEventVolumeCache>.Instance;

	public void AddVolume(string animFile, string eventName, EffectorValues vals)
	{
		HashedString key = new HashedString(animFile + ":" + eventName);
		if (!volumeCache.ContainsKey(key))
		{
			volumeCache.Add(key, vals);
		}
		else
		{
			volumeCache[key] = vals;
		}
	}

	public EffectorValues GetVolume(string animFile, string eventName)
	{
		HashedString key = new HashedString(animFile + ":" + eventName);
		if (!volumeCache.ContainsKey(key))
		{
			return default(EffectorValues);
		}
		return volumeCache[key];
	}
}
