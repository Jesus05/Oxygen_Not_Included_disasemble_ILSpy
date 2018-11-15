using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLoopingSoundUpdater : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public EventInstance ev;

		public int parameterIdx;
	}

	private List<Entry> entries = new List<Entry>();

	public SpeedLoopingSoundUpdater()
		: base("Speed")
	{
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
		float speedParameterValue = GetSpeedParameterValue();
		foreach (Entry entry in entries)
		{
			Entry current = entry;
			current.ev.setParameterValueByIndex(current.parameterIdx, speedParameterValue);
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

	public static float GetSpeedParameterValue()
	{
		return Time.timeScale * 1f;
	}
}
