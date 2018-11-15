using FMOD.Studio;
using System.Collections.Generic;

internal abstract class UserVolumeLoopingUpdater : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public EventInstance ev;

		public int parameterIdx;
	}

	private List<Entry> entries = new List<Entry>();

	private string playerPref;

	public UserVolumeLoopingUpdater(string parameter, string player_pref)
		: base(parameter)
	{
		playerPref = player_pref;
	}

	public override void Add(Sound sound)
	{
		Entry entry = default(Entry);
		entry.ev = sound.ev;
		entry.parameterIdx = sound.description.GetParameterIdx(base.parameter);
		Entry item = entry;
		entries.Add(item);
	}

	public override void Update(float dt)
	{
		if (!string.IsNullOrEmpty(playerPref))
		{
			float @float = KPlayerPrefs.GetFloat(playerPref);
			foreach (Entry entry in entries)
			{
				Entry current = entry;
				current.ev.setParameterValueByIndex(current.parameterIdx, @float);
			}
		}
	}

	public override void Remove(Sound sound)
	{
		int num = 0;
		while (true)
		{
			if (num >= entries.Count)
			{
				return;
			}
			Entry entry = entries[num];
			if (entry.ev.handle == sound.ev.handle)
			{
				break;
			}
			num++;
		}
		entries.RemoveAt(num);
	}
}
