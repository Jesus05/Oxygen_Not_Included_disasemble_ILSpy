using System;

public class GameScheduler : KMonoBehaviour, IScheduler
{
	public class GameSchedulerClock : SchedulerClock
	{
		public override float GetTime()
		{
			return GameClock.Instance.GetTime();
		}
	}

	private Scheduler scheduler = new Scheduler(new GameSchedulerClock());

	public static GameScheduler Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		Singleton<StateMachineManager>.Instance.RegisterScheduler(scheduler);
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
