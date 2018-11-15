using System;
using UnityEngine;

public class Scheduler : IScheduler
{
	public FloatHOTQueue<SchedulerEntry> entries = new FloatHOTQueue<SchedulerEntry>();

	private SchedulerClock clock;

	private float previousTime = float.NegativeInfinity;

	public int Count => entries.Count;

	public Scheduler(SchedulerClock clock)
	{
		this.clock = clock;
	}

	public float GetTime()
	{
		return clock.GetTime();
	}

	private SchedulerHandle Schedule(SchedulerEntry entry)
	{
		entries.Enqueue(entry.time, entry);
		return new SchedulerHandle(this, entry);
	}

	private SchedulerHandle Schedule(string name, float time, float time_interval, Action<object> callback, object callback_data, GameObject profiler_obj)
	{
		SchedulerEntry entry = new SchedulerEntry(name, time + clock.GetTime(), time_interval, callback, callback_data, profiler_obj);
		return Schedule(entry);
	}

	public void FreeResources()
	{
		clock = null;
		if (entries != null)
		{
			while (entries.Count > 0)
			{
				entries.Dequeue().Value.FreeResources();
			}
		}
		entries = null;
	}

	public SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		if (group != null && group.scheduler != this)
		{
			Debug.LogError("Scheduler group mismatch!", null);
		}
		SchedulerHandle schedulerHandle = Schedule(name, time, -1f, callback, callback_data, null);
		group?.Add(schedulerHandle);
		return schedulerHandle;
	}

	public void Clear(SchedulerHandle handle)
	{
		handle.entry.Clear();
	}

	public void Update()
	{
		if (Count != 0)
		{
			int count = Count;
			int i = 0;
			using (new KProfiler.Region("Scheduler.Update", null))
			{
				float time = clock.GetTime();
				if (previousTime != time)
				{
					previousTime = time;
					for (; i < count; i++)
					{
						if (!(time >= entries.Peek().Key))
						{
							break;
						}
						SchedulerEntry value = entries.Dequeue().Value;
						if (value.callback != null)
						{
							value.callback(value.callbackData);
						}
					}
				}
			}
		}
	}
}
