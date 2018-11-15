using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

internal class UpdatePercentCompleteParameter : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public Worker worker;

		public EventInstance ev;

		public int parameterIdx;
	}

	private List<Entry> entries = new List<Entry>();

	public UpdatePercentCompleteParameter()
		: base("percentComplete")
	{
	}

	public override void Add(Sound sound)
	{
		Entry entry = default(Entry);
		entry.worker = sound.transform.GetComponent<Worker>();
		entry.ev = sound.ev;
		entry.parameterIdx = sound.description.GetParameterIdx(base.parameter);
		Entry item = entry;
		entries.Add(item);
	}

	public override void Update(float dt)
	{
		foreach (Entry entry in entries)
		{
			Entry current = entry;
			if (!((Object)current.worker == (Object)null))
			{
				Workable workable = current.worker.workable;
				if (!((Object)workable == (Object)null))
				{
					float percentComplete = workable.GetPercentComplete();
					current.ev.setParameterValueByIndex(current.parameterIdx, percentComplete);
				}
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
