public struct SchedulerHandle
{
	public SchedulerEntry entry;

	private Scheduler scheduler;

	public float TimeRemaining
	{
		get
		{
			if (!IsValid)
			{
				return -1f;
			}
			return entry.time - scheduler.GetTime();
		}
	}

	public bool IsValid => scheduler != null;

	public SchedulerHandle(Scheduler scheduler, SchedulerEntry entry)
	{
		this.entry = entry;
		this.scheduler = scheduler;
	}

	public void FreeResources()
	{
		entry.FreeResources();
		scheduler = null;
	}

	public void ClearScheduler()
	{
		if (scheduler != null)
		{
			scheduler.Clear(this);
			scheduler = null;
		}
	}
}
