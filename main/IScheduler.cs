using System;

public interface IScheduler
{
	SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null);
}
