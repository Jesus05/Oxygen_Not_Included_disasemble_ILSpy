using System;
using System.Collections.Generic;

public class StateMachineManager : Singleton<StateMachineManager>, IScheduler
{
	private Scheduler scheduler;

	private Dictionary<Type, StateMachine> stateMachines = new Dictionary<Type, StateMachine>();

	private Dictionary<Type, List<Action<StateMachine>>> stateMachineCreatedCBs = new Dictionary<Type, List<Action<StateMachine>>>();

	private static object[] parameters = new object[2];

	public void RegisterScheduler(Scheduler scheduler)
	{
		this.scheduler = scheduler;
	}

	public SchedulerHandle Schedule(string name, float time, Action<object> callback, object callback_data = null, SchedulerGroup group = null)
	{
		return scheduler.Schedule(name, time, callback, callback_data, group);
	}

	public SchedulerGroup CreateSchedulerGroup()
	{
		return new SchedulerGroup(scheduler);
	}

	public StateMachine CreateStateMachine(Type type)
	{
		StateMachine value = null;
		if (!stateMachines.TryGetValue(type, out value))
		{
			value = (StateMachine)Activator.CreateInstance(type);
			value.CreateStates(value);
			value.BindStates();
			value.InitializeStateMachine();
			stateMachines[type] = value;
			if (stateMachineCreatedCBs.TryGetValue(type, out List<Action<StateMachine>> value2))
			{
				foreach (Action<StateMachine> item in value2)
				{
					item(value);
				}
				return value;
			}
		}
		return value;
	}

	public T CreateStateMachine<T>()
	{
		return (T)(object)CreateStateMachine(typeof(T));
	}

	public static void ResetParameters()
	{
		for (int i = 0; i < parameters.Length; i++)
		{
			parameters[i] = null;
		}
	}

	public StateMachine.Instance CreateSMIFromDef(IStateMachineTarget master, StateMachine.BaseDef def)
	{
		parameters[0] = master;
		parameters[1] = def;
		StateMachine stateMachine = Singleton<StateMachineManager>.Instance.CreateStateMachine(def.GetStateMachineType());
		Type stateMachineInstanceType = stateMachine.GetStateMachineInstanceType();
		return (StateMachine.Instance)Activator.CreateInstance(stateMachineInstanceType, parameters);
	}

	public void Clear()
	{
		if (scheduler != null)
		{
			scheduler.FreeResources();
		}
		if (stateMachines != null)
		{
			stateMachines.Clear();
		}
	}

	public void AddStateMachineCreatedCallback(Type sm_type, Action<StateMachine> cb)
	{
		if (!stateMachineCreatedCBs.TryGetValue(sm_type, out List<Action<StateMachine>> value))
		{
			value = new List<Action<StateMachine>>();
			stateMachineCreatedCBs[sm_type] = value;
		}
		value.Add(cb);
	}

	public void RemoveStateMachineCreatedCallback(Type sm_type, Action<StateMachine> cb)
	{
		if (stateMachineCreatedCBs.TryGetValue(sm_type, out List<Action<StateMachine>> value))
		{
			value.Remove(cb);
		}
	}
}
