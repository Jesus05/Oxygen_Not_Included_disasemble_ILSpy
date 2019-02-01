using UnityEngine;

internal class TaskDivision<Task, SharedData> where Task : DivisibleTask<SharedData>, new()
{
	public Task[] tasks;

	public TaskDivision(int taskCount)
	{
		tasks = new Task[taskCount];
		for (int i = 0; i != tasks.Length; i++)
		{
			tasks[i] = new Task();
		}
	}

	public TaskDivision()
		: this(SystemInfo.processorCount - 1)
	{
	}

	public void Initialize(int count)
	{
		int num = count / tasks.Length;
		for (int i = 0; i != tasks.Length; i++)
		{
			tasks[i].start = i * num;
			tasks[i].end = tasks[i].start + num;
		}
		DebugUtil.Assert(tasks[tasks.Length - 1].end + count % tasks.Length == count);
		tasks[tasks.Length - 1].end = count;
	}

	public void Run(SharedData sharedData)
	{
		Task[] array = tasks;
		for (int i = 0; i < array.Length; i++)
		{
			Task val = array[i];
			val.Run(sharedData);
		}
	}
}
