using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Schedule : ISaveLoadable, IListableOption
{
	[Serialize]
	private List<ScheduleBlock> blocks;

	[Serialize]
	private List<Ref<Schedulable>> assigned;

	[Serialize]
	public string name;

	[Serialize]
	public bool alarmActivated = true;

	[Serialize]
	private int[] tones;

	public Action<Schedule> onChanged;

	public Schedule(string name, List<ScheduleGroup> defaultGroups, bool alarmActivated)
	{
		this.name = name;
		this.alarmActivated = alarmActivated;
		blocks = new List<ScheduleBlock>(24);
		assigned = new List<Ref<Schedulable>>();
		tones = GenerateTones();
		SetBlocksToGroupDefaults(defaultGroups);
	}

	public static int GetBlockIdx()
	{
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		int val = (int)(currentCycleAsPercentage * 24f);
		return Math.Min(val, 23);
	}

	public static int GetLastBlockIdx()
	{
		return (GetBlockIdx() + 24 - 1) % 24;
	}

	public void ClearNullReferences()
	{
		assigned.RemoveAll((Ref<Schedulable> x) => (UnityEngine.Object)x.Get() == (UnityEngine.Object)null);
	}

	public void SetBlocksToGroupDefaults(List<ScheduleGroup> defaultGroups)
	{
		blocks.Clear();
		int num = 0;
		for (int i = 0; i < defaultGroups.Count; i++)
		{
			ScheduleGroup scheduleGroup = defaultGroups[i];
			for (int j = 0; j < scheduleGroup.defaultSegments; j++)
			{
				blocks.Add(new ScheduleBlock(scheduleGroup.Name, scheduleGroup.allowedTypes, scheduleGroup.Id));
				num++;
			}
		}
		Changed();
	}

	public void Tick()
	{
		ScheduleBlock block = GetBlock(GetBlockIdx());
		ScheduleBlock block2 = GetBlock(GetLastBlockIdx());
		if (!AreScheduleTypesIdentical(block.allowed_types, block2.allowed_types))
		{
			ScheduleGroup scheduleGroup = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(block.allowed_types);
			ScheduleGroup scheduleGroup2 = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(block2.allowed_types);
			if (alarmActivated && scheduleGroup2.alarm != scheduleGroup.alarm)
			{
				ScheduleManager.Instance.PlayScheduleAlarm(this, block, scheduleGroup.alarm);
			}
			foreach (Ref<Schedulable> item in GetAssigned())
			{
				item.Get().OnScheduleBlocksChanged(this);
			}
		}
		foreach (Ref<Schedulable> item2 in GetAssigned())
		{
			item2.Get().OnScheduleBlocksTick(this);
		}
	}

	string IListableOption.GetProperName()
	{
		return name;
	}

	public int[] GenerateTones()
	{
		int minToneIndex = TuningData<ScheduleManager.Tuning>.Get().minToneIndex;
		int maxToneIndex = TuningData<ScheduleManager.Tuning>.Get().maxToneIndex;
		int firstLastToneSpacing = TuningData<ScheduleManager.Tuning>.Get().firstLastToneSpacing;
		int[] array = new int[4]
		{
			UnityEngine.Random.Range(minToneIndex, maxToneIndex - firstLastToneSpacing + 1),
			UnityEngine.Random.Range(minToneIndex, maxToneIndex + 1),
			UnityEngine.Random.Range(minToneIndex, maxToneIndex + 1),
			0
		};
		array[3] = UnityEngine.Random.Range(array[0] + firstLastToneSpacing, maxToneIndex + 1);
		return array;
	}

	public List<Ref<Schedulable>> GetAssigned()
	{
		if (assigned == null)
		{
			assigned = new List<Ref<Schedulable>>();
		}
		return assigned;
	}

	public int[] GetTones()
	{
		if (tones == null)
		{
			tones = GenerateTones();
		}
		return tones;
	}

	public void SetGroup(int idx, ScheduleGroup group)
	{
		if (0 <= idx && idx < blocks.Count)
		{
			blocks[idx] = new ScheduleBlock(group.Name, group.allowedTypes, group.Id);
			Changed();
		}
	}

	private void Changed()
	{
		foreach (Ref<Schedulable> item in GetAssigned())
		{
			item.Get().OnScheduleChanged(this);
		}
		if (onChanged != null)
		{
			onChanged(this);
		}
	}

	public List<ScheduleBlock> GetBlocks()
	{
		return blocks;
	}

	public ScheduleBlock GetBlock(int idx)
	{
		return blocks[idx];
	}

	public void Assign(Schedulable schedulable)
	{
		if (!IsAssigned(schedulable))
		{
			GetAssigned().Add(new Ref<Schedulable>(schedulable));
		}
		Changed();
	}

	public void Unassign(Schedulable schedulable)
	{
		for (int i = 0; i < GetAssigned().Count; i++)
		{
			if ((UnityEngine.Object)GetAssigned()[i].Get() == (UnityEngine.Object)schedulable)
			{
				GetAssigned().RemoveAt(i);
				break;
			}
		}
		Changed();
	}

	public bool IsAssigned(Schedulable schedulable)
	{
		foreach (Ref<Schedulable> item in GetAssigned())
		{
			if ((UnityEngine.Object)item.Get() == (UnityEngine.Object)schedulable)
			{
				return true;
			}
		}
		return false;
	}

	public static bool AreScheduleTypesIdentical(List<ScheduleBlockType> a, List<ScheduleBlockType> b)
	{
		if (a.Count != b.Count)
		{
			return false;
		}
		foreach (ScheduleBlockType item in a)
		{
			bool flag = false;
			foreach (ScheduleBlockType item2 in b)
			{
				if (item.IdHash == item2.IdHash)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}
}
