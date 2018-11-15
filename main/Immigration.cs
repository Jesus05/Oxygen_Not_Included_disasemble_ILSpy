using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Immigration : KMonoBehaviour, ISaveLoadable, ISim200ms, IPersonalPriorityManager
{
	public float[] spawnInterval;

	public int[] spawnTable;

	[Serialize]
	private Dictionary<HashedString, int> defaultPersonalPriorities = new Dictionary<HashedString, int>();

	[Serialize]
	public float timeBeforeSpawn = float.PositiveInfinity;

	[Serialize]
	private bool bImmigrantAvailable;

	[Serialize]
	private int spawnIdx;

	[Serialize]
	private bool stopped;

	public static Immigration Instance;

	public bool ImmigrantsAvailable => bImmigrantAvailable;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		bImmigrantAvailable = false;
		Instance = this;
		int num = Math.Min(spawnIdx, spawnInterval.Length - 1);
		timeBeforeSpawn = spawnInterval[num];
		ResetPersonalPriorities();
	}

	public int SpawnMinions()
	{
		bImmigrantAvailable = false;
		spawnIdx++;
		int num = Math.Min(spawnIdx, spawnInterval.Length - 1);
		timeBeforeSpawn = spawnInterval[num];
		return spawnTable[num];
	}

	public float GetTimeRemaining()
	{
		return timeBeforeSpawn;
	}

	public float GetTotalWaitTime()
	{
		int num = Math.Min(spawnIdx, spawnInterval.Length - 1);
		return spawnInterval[num];
	}

	public void Sim200ms(float dt)
	{
		if (!stopped && !bImmigrantAvailable)
		{
			timeBeforeSpawn -= dt;
			timeBeforeSpawn = Math.Max(timeBeforeSpawn, 0f);
			if (timeBeforeSpawn <= 0f)
			{
				bImmigrantAvailable = true;
			}
		}
	}

	public void Stop()
	{
		stopped = true;
		bImmigrantAvailable = false;
		timeBeforeSpawn = spawnInterval[Math.Min(spawnIdx, spawnInterval.Length - 1)];
	}

	public void Restart()
	{
		stopped = false;
	}

	public int GetPersonalPriority(ChoreGroup group, out bool auto_assigned)
	{
		auto_assigned = false;
		if (defaultPersonalPriorities.TryGetValue(group.IdHash, out int value))
		{
			return value;
		}
		value = 3;
		return value;
	}

	public void SetPersonalPriority(ChoreGroup group, int value, bool is_auto_assigned)
	{
		defaultPersonalPriorities[group.IdHash] = value;
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		return 0;
	}

	public bool CanRoleManageChoreGroup(ChoreGroup group)
	{
		return false;
	}

	public void ApplyDefaultPersonalPriorities(GameObject minion)
	{
		IPersonalPriorityManager instance = Instance;
		IPersonalPriorityManager component = minion.GetComponent<ChoreConsumer>();
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			bool auto_assigned;
			int personalPriority = instance.GetPersonalPriority(resource, out auto_assigned);
			component.SetPersonalPriority(resource, personalPriority, false);
		}
	}

	public void ResetPersonalPriorities()
	{
		bool advancedPersonalPriorities = Game.Instance.advancedPersonalPriorities;
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			defaultPersonalPriorities[resource.IdHash] = ((!advancedPersonalPriorities) ? 3 : resource.DefaultPersonalPriority);
		}
	}

	public bool IsChoreGroupDisabled(ChoreGroup g)
	{
		return false;
	}
}
