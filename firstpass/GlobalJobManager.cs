public static class GlobalJobManager
{
	private static JobManager jobManager;

	static GlobalJobManager()
	{
		jobManager = new JobManager();
	}

	public static void Run(IWorkItemCollection work_items)
	{
		jobManager.Run(work_items);
	}

	public static void Cleanup()
	{
		if (jobManager != null)
		{
			jobManager.Cleanup();
		}
		jobManager = null;
	}
}
