using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

public class JobManager
{
	private class WorkerThread
	{
		private Thread thread;

		private Semaphore semaphore;

		private JobManager jobManager;

		private List<Exception> exceptions;

		[CompilerGenerated]
		private static ParameterizedThreadStart _003C_003Ef__mg_0024cache0;

		public WorkerThread(Semaphore semaphore, JobManager job_manager, string name)
		{
			this.semaphore = semaphore;
			thread = new Thread(ThreadMain, 131072);
			Util.ApplyInvariantCultureToThread(thread);
			thread.Priority = ThreadPriority.AboveNormal;
			thread.Name = name;
			jobManager = job_manager;
			exceptions = new List<Exception>();
			thread.Start(this);
		}

		public void Run()
		{
			KProfiler.BeginThreadProfiling("KJobManager", thread.Name);
			while (true)
			{
				semaphore.WaitOne();
				if (jobManager.isShuttingDown)
				{
					break;
				}
				try
				{
					bool flag = true;
					while (flag)
					{
						flag = jobManager.DoNextWorkItem();
					}
				}
				catch (Exception item)
				{
					exceptions.Add(item);
					errorOccured = true;
					Debugger.Break();
				}
				jobManager.DecrementActiveWorkerThreadCount();
			}
			KProfiler.EndThreadProfiling();
		}

		public void PrintExceptions()
		{
			foreach (Exception exception in exceptions)
			{
				Debug.LogError(exception, null);
			}
		}

		public void Cleanup()
		{
		}

		public static void ThreadMain(object data)
		{
			WorkerThread workerThread = (WorkerThread)data;
			workerThread.Run();
		}
	}

	public static bool errorOccured;

	private List<WorkerThread> threads = new List<WorkerThread>();

	private Semaphore semaphore;

	private IWorkItemCollection workItems;

	private int nextWorkIndex = -1;

	private int workerThreadCount;

	private ManualResetEvent manualResetEvent = new ManualResetEvent(false);

	private static bool runSingleThreaded;

	public bool isShuttingDown
	{
		get;
		private set;
	}

	private void Initialize()
	{
		semaphore = new Semaphore(0, CPUBudget.coreCount);
		for (int i = 0; i < CPUBudget.coreCount; i++)
		{
			threads.Add(new WorkerThread(semaphore, this, $"KWorker{i}"));
		}
	}

	public bool DoNextWorkItem()
	{
		int num = Interlocked.Increment(ref nextWorkIndex);
		if (num < workItems.Count)
		{
			workItems.InternalDoWorkItem(num);
			return true;
		}
		return false;
	}

	public void Cleanup()
	{
		isShuttingDown = true;
		semaphore.Release(threads.Count);
		foreach (WorkerThread thread in threads)
		{
			thread.Cleanup();
		}
		threads.Clear();
	}

	public void Run(IWorkItemCollection work_items)
	{
		if (semaphore == null)
		{
			Initialize();
		}
		if (runSingleThreaded || threads.Count == 0)
		{
			for (int i = 0; i < work_items.Count; i++)
			{
				work_items.InternalDoWorkItem(i);
			}
		}
		else
		{
			workerThreadCount = threads.Count;
			nextWorkIndex = -1;
			workItems = work_items;
			Thread.MemoryBarrier();
			semaphore.Release(threads.Count);
			manualResetEvent.WaitOne();
			manualResetEvent.Reset();
			if (errorOccured)
			{
				foreach (WorkerThread thread in threads)
				{
					thread.PrintExceptions();
				}
			}
		}
	}

	public void DecrementActiveWorkerThreadCount()
	{
		if (Interlocked.Decrement(ref workerThreadCount) == 0)
		{
			manualResetEvent.Set();
		}
	}
}
