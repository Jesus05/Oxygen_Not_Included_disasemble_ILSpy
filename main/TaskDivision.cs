using System;
using UnityEngine;

internal class TaskDivision<Task, SharedData> where Task : DivisibleTask<SharedData>, new()
{
	public Task[] tasks;

	public TaskDivision()
	{
		tasks = new Task[Math.Max(1, SystemInfo.processorCount - 1)];
		for (int i = 0; i != tasks.Length; i++)
		{
			tasks[i] = new Task();
			Task val = tasks[i];
			val.name += $"{i}";
		}
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
