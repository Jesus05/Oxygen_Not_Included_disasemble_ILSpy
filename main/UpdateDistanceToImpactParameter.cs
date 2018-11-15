using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

internal class UpdateDistanceToImpactParameter : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public Comet comet;

		public EventInstance ev;

		public int parameterIdx;
	}

	private List<Entry> entries = new List<Entry>();

	public UpdateDistanceToImpactParameter()
		: base("distanceToImpact")
	{
	}

	public override void Add(Sound sound)
	{
		Entry entry = default(Entry);
		entry.comet = sound.transform.GetComponent<Comet>();
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
			if (!((Object)current.comet == (Object)null))
			{
				float soundDistance = current.comet.GetSoundDistance();
				current.ev.setParameterValueByIndex(current.parameterIdx, soundDistance);
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
