using FMOD.Studio;
using KSerialization;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class ScheduleManager : KMonoBehaviour, ISim33ms
{
	public class Tuning : TuningData<Tuning>
	{
		public float toneSpacingSeconds;

		public int minToneIndex;

		public int maxToneIndex;

		public int firstLastToneSpacing;
	}

	[Serialize]
	private List<Schedule> schedules;

	[Serialize]
	private int lastIdx = 0;

	[Serialize]
	private int scheduleNameIncrementor = 0;

	public static ScheduleManager Instance;

	public event Action<List<Schedule>> onSchedulesChanged;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (schedules.Count == 0)
		{
			SetupDefaultSchedule();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		schedules = new List<Schedule>();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		if (schedules.Count == 0)
		{
			SetupDefaultSchedule();
		}
		foreach (Schedule schedule in schedules)
		{
			schedule.ClearNullReferences();
		}
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			Schedulable component = item.GetComponent<Schedulable>();
			if (GetSchedule(component) == null)
			{
				schedules[0].Assign(component);
			}
		}
		Components.LiveMinionIdentities.OnAdd += OnAddDupe;
		Components.LiveMinionIdentities.OnRemove += OnRemoveDupe;
	}

	private void OnAddDupe(MinionIdentity minion)
	{
		Schedulable component = minion.GetComponent<Schedulable>();
		if (GetSchedule(component) == null)
		{
			schedules[0].Assign(component);
		}
	}

	private void OnRemoveDupe(MinionIdentity minion)
	{
		Schedulable component = minion.GetComponent<Schedulable>();
		GetSchedule(component)?.Unassign(component);
	}

	private void SetupDefaultSchedule()
	{
		AddSchedule(Db.Get().ScheduleGroups.allGroups, UI.SCHEDULESCREEN.SCHEDULE_NAME_DEFAULT, true);
	}

	public void AddSchedule(List<ScheduleGroup> groups, string name = null, bool alarmOn = false)
	{
		scheduleNameIncrementor++;
		if (name == null)
		{
			name = string.Format(UI.SCHEDULESCREEN.SCHEDULE_NAME_FORMAT, scheduleNameIncrementor.ToString());
		}
		Schedule item = new Schedule(name, groups, alarmOn);
		schedules.Add(item);
		if (this.onSchedulesChanged != null)
		{
			this.onSchedulesChanged(schedules);
		}
	}

	public void DeleteSchedule(Schedule schedule)
	{
		if (schedules.Count != 1)
		{
			List<Ref<Schedulable>> assigned = schedule.GetAssigned();
			schedules.Remove(schedule);
			foreach (Ref<Schedulable> item in assigned)
			{
				schedules[0].Assign(item.Get());
			}
			if (this.onSchedulesChanged != null)
			{
				this.onSchedulesChanged(schedules);
			}
		}
	}

	public Schedule GetSchedule(Schedulable schedulable)
	{
		foreach (Schedule schedule in schedules)
		{
			if (schedule.IsAssigned(schedulable))
			{
				return schedule;
			}
		}
		return null;
	}

	public List<Schedule> GetSchedules()
	{
		return schedules;
	}

	public bool IsAllowed(Schedulable schedulable, ScheduleBlockType schedule_block_type)
	{
		int blockIdx = Schedule.GetBlockIdx();
		ScheduleBlock block = GetSchedule(schedulable).GetBlock(blockIdx);
		return block.IsAllowed(schedule_block_type);
	}

	public void Sim33ms(float dt)
	{
		int blockIdx = Schedule.GetBlockIdx();
		if (blockIdx != lastIdx)
		{
			foreach (Schedule schedule in schedules)
			{
				schedule.Tick();
			}
			lastIdx = blockIdx;
		}
	}

	public void PlayScheduleAlarm(Schedule schedule, ScheduleBlock block, bool forwards)
	{
		Notification notification = new Notification(string.Format(MISC.NOTIFICATIONS.SCHEDULE_CHANGED.NAME, schedule.name, block.name), NotificationType.Good, HashedString.Invalid, (List<Notification> notificationList, object data) => string.Format(MISC.NOTIFICATIONS.SCHEDULE_CHANGED.TOOLTIP, schedule.name, block.name, Db.Get().ScheduleGroups.Get(block.GroupId).notificationTooltip), null, true, 0f, null, null, null);
		GetComponent<Notifier>().Add(notification, "");
		StartCoroutine(PlayScheduleTone(schedule, forwards));
	}

	private IEnumerator PlayScheduleTone(Schedule schedule, bool forwards)
	{
		int[] tones = schedule.GetTones();
		int i = 0;
		if (i < tones.Length)
		{
			int t = (!forwards) ? (tones.Length - 1 - i) : i;
			PlayTone(tones[t], forwards);
			yield return (object)new WaitForSeconds(TuningData<Tuning>.Get().toneSpacingSeconds);
			/*Error: Unable to find new state assignment for yield return*/;
		}
	}

	private void PlayTone(int pitch, bool forwards)
	{
		EventInstance instance = KFMOD.BeginOneShot(GlobalAssets.GetSound("WorkChime_tone", false), Vector3.zero);
		instance.setParameterValue("WorkChime_pitch", (float)pitch);
		instance.setParameterValue("WorkChime_start", (float)(forwards ? 1 : 0));
		KFMOD.EndOneShot(instance);
	}
}
