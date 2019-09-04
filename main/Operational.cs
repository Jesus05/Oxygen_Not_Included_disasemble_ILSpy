using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Operational : KMonoBehaviour
{
	public class Flag
	{
		public enum Type
		{
			Requirement,
			Functional
		}

		public string Name;

		public Type FlagType;

		public Flag(string name, Type type)
		{
			Name = name;
			FlagType = type;
		}
	}

	public struct TimeEntry
	{
		public float startTime;

		public float endTime;

		public TimeEntry(float start, float end)
		{
			startTime = start;
			endTime = end;
		}
	}

	[Serialize]
	public float inactiveStartTime;

	[Serialize]
	public float activeStartTime;

	[Serialize]
	private List<TimeEntry> activeTimes = new List<TimeEntry>();

	[Serialize]
	private List<TimeEntry> inactiveTimes = new List<TimeEntry>();

	public Dictionary<Flag, bool> Flags = new Dictionary<Flag, bool>();

	private static readonly EventSystem.IntraObjectHandler<Operational> OnNewBuildingDelegate = new EventSystem.IntraObjectHandler<Operational>(delegate(Operational component, object data)
	{
		component.OnNewBuilding(data);
	});

	public bool IsOperational
	{
		get;
		private set;
	}

	public bool IsFunctional
	{
		get;
		private set;
	}

	public bool IsActive
	{
		get;
		private set;
	}

	[OnSerializing]
	private void OnSerializing()
	{
		float startingTime = (!IsActive) ? inactiveStartTime : activeStartTime;
		List<TimeEntry> timeEntries = (!IsActive) ? inactiveTimes : activeTimes;
		float time = GameClock.Instance.GetTime();
		AddTimeEntry(timeEntries, startingTime, time);
		activeStartTime = GameClock.Instance.GetTime();
		inactiveStartTime = GameClock.Instance.GetTime();
	}

	protected override void OnPrefabInit()
	{
		UpdateFunctional();
		UpdateOperational();
		Subscribe(-1661515756, OnNewBuildingDelegate);
	}

	public void OnNewBuilding(object data)
	{
		BuildingComplete component = GetComponent<BuildingComplete>();
		if (component.creationTime > 0f)
		{
			inactiveStartTime = component.creationTime;
			activeStartTime = component.creationTime;
			activeTimes.Clear();
			inactiveTimes.Clear();
		}
	}

	public bool IsOperationalType(Flag.Type type)
	{
		if (type == Flag.Type.Functional)
		{
			return IsFunctional;
		}
		return IsOperational;
	}

	public void SetFlag(Flag flag, bool value)
	{
		bool value2 = false;
		if (Flags.TryGetValue(flag, out value2))
		{
			if (value2 != value)
			{
				Flags[flag] = value;
				Trigger(187661686, flag);
			}
		}
		else
		{
			Flags[flag] = value;
			Trigger(187661686, flag);
		}
		if (flag.FlagType == Flag.Type.Functional && value != IsFunctional)
		{
			UpdateFunctional();
		}
		if (value != IsOperational)
		{
			UpdateOperational();
		}
	}

	public bool GetFlag(Flag flag)
	{
		bool value = false;
		Flags.TryGetValue(flag, out value);
		return value;
	}

	private void UpdateFunctional()
	{
		bool isFunctional = true;
		foreach (KeyValuePair<Flag, bool> flag in Flags)
		{
			if (flag.Key.FlagType == Flag.Type.Functional && !flag.Value)
			{
				isFunctional = false;
				break;
			}
		}
		IsFunctional = isFunctional;
		Trigger(-1852328367, IsFunctional);
	}

	private void UpdateOperational()
	{
		Dictionary<Flag, bool>.Enumerator enumerator = Flags.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext())
		{
			if (!enumerator.Current.Value)
			{
				flag = false;
				break;
			}
		}
		if (flag != IsOperational)
		{
			IsOperational = flag;
			if (!IsOperational)
			{
				SetActive(false, false);
			}
			if (IsOperational)
			{
				GetComponent<KPrefabID>().AddTag(GameTags.Operational, false);
			}
			else
			{
				GetComponent<KPrefabID>().RemoveTag(GameTags.Operational);
			}
			Trigger(-592767678, IsOperational);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}

	public void SetActive(bool value, bool force_ignore = false)
	{
		if (IsActive != value)
		{
			float startingTime = (!IsActive) ? inactiveStartTime : activeStartTime;
			List<TimeEntry> timeEntries = (!IsActive) ? inactiveTimes : activeTimes;
			float time = GameClock.Instance.GetTime();
			AddTimeEntry(timeEntries, startingTime, time);
			IsActive = value;
			if (IsActive)
			{
				activeStartTime = time;
			}
			else
			{
				inactiveStartTime = time;
			}
			Trigger(824508782, this);
			Game.Instance.Trigger(-809948329, base.gameObject);
		}
	}

	private void AddTimeEntry(List<TimeEntry> timeEntries, float startingTime, float endingTime)
	{
		if (startingTime != endingTime)
		{
			timeEntries.Add(new TimeEntry(startingTime, endingTime));
		}
	}

	private void ValidateTimeEntries(List<TimeEntry> timeEntries)
	{
		if (timeEntries.Count > 2)
		{
			for (int num = timeEntries.Count - 1; num > 0; num--)
			{
				TimeEntry timeEntry = timeEntries[num];
				for (int num2 = num - 1; num2 >= 0; num2--)
				{
					TimeEntry timeEntry2 = timeEntries[num2];
					if (timeEntry.startTime < timeEntry2.endTime || timeEntry.startTime == timeEntry2.startTime)
					{
						Debug.Assert(false, "ENTRY TIMES OVERLAP!");
					}
				}
			}
		}
	}

	public float GetUptimeForTimeSpan(float duration = 600f)
	{
		float num = SumTimesForTimeSpawn(activeTimes, duration);
		float num2 = SumTimesForTimeSpawn(inactiveTimes, duration);
		float b = GameClock.Instance.GetTime() - duration;
		if (IsActive)
		{
			num += GameClock.Instance.GetTime() - Mathf.Max(activeStartTime, b);
		}
		else
		{
			num2 += GameClock.Instance.GetTime() - Mathf.Max(inactiveStartTime, b);
		}
		float num3 = num + num2;
		Debug.Assert(num3 <= duration, "totalTime is greater than allowed duration!");
		if (num == 0f || num3 == 0f)
		{
			return 0f;
		}
		if (num2 == 0f)
		{
			return 1f;
		}
		return num / num3;
	}

	private float SumTimesForTimeSpawn(List<TimeEntry> times, float duration)
	{
		float num = GameClock.Instance.GetTime() - duration;
		float num2 = 0f;
		foreach (TimeEntry time in times)
		{
			TimeEntry current = time;
			if (!(current.startTime < num) || !(current.endTime < num))
			{
				num2 = ((!(current.startTime < num) || !(current.endTime >= num)) ? (num2 + (current.endTime - current.startTime)) : (num2 + (current.endTime - num)));
			}
		}
		return num2;
	}
}
