using FMOD.Studio;
using System.Collections.Generic;

internal class UpdateConsumedMassParameter : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public CreatureCalorieMonitor.Instance creatureCalorieMonitor;

		public EventInstance ev;

		public int parameterIdx;
	}

	private List<Entry> entries = new List<Entry>();

	public UpdateConsumedMassParameter()
		: base("consumedMass")
	{
	}

	public override void Add(Sound sound)
	{
		Entry entry = default(Entry);
		entry.creatureCalorieMonitor = sound.transform.GetSMI<CreatureCalorieMonitor.Instance>();
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
			if (!current.creatureCalorieMonitor.IsNullOrStopped())
			{
				float fullness = current.creatureCalorieMonitor.stomach.GetFullness();
				current.ev.setParameterValueByIndex(current.parameterIdx, fullness);
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
