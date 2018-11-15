using System.Collections.Generic;

public class SchedulerGroup
{
	private List<SchedulerHandle> handles = new List<SchedulerHandle>();

	public Scheduler scheduler
	{
		get;
		private set;
	}

	public SchedulerGroup(Scheduler scheduler)
	{
		this.scheduler = scheduler;
		Reset();
	}

	public void FreeResources()
	{
		if (scheduler != null)
		{
			scheduler.FreeResources();
		}
		scheduler = null;
		if (handles != null)
		{
			handles.Clear();
		}
		handles = null;
	}

	public void Reset()
	{
		foreach (SchedulerHandle handle in handles)
		{
			handle.ClearScheduler();
		}
		handles.Clear();
	}

	public void Add(SchedulerHandle handle)
	{
		handles.Add(handle);
	}
}
