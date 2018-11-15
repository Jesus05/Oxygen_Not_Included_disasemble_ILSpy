using System;
using UnityEngine;

public class UIScheduler : KMonoBehaviour, IScheduler
{
	public class UISchedulerClock : SchedulerClock
	{
		public override float GetTime()
		{
			return Time.unscaledTime;
		}
	}

	private Scheduler scheduler = new Scheduler(new UISchedulerClock());

	public static UIScheduler Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	public SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		return scheduler.Schedule(name, time, callback, callback_data, group);
	}

	private void Update()
	{
		scheduler.Update();
	}

	protected override void OnLoadLevel()
	{
		scheduler.FreeResources();
		scheduler = null;
	}

	public SchedulerGroup CreateGroup()
	{
		return new SchedulerGroup(scheduler);
	}

	public Scheduler GetScheduler()
	{
		return scheduler;
	}
}
