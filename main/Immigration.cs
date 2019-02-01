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

	private CarePackageInfo[] carePackages;

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
		ConfigureCarePackages();
	}

	private void ConfigureCarePackages()
	{
		carePackages = new CarePackageInfo[33]
		{
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, 0),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, 0),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, 0),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, 12),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, 0),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, 0),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 3000f, 12),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 500f, 12),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 500f, 12),
			new CarePackageInfo("PrickleGrassSeed", 3f, 0),
			new CarePackageInfo("LeafyPlantSeed", 3f, 0),
			new CarePackageInfo("CactusPlantSeed", 3f, 0),
			new CarePackageInfo("MushroomSeed", 1f, 0),
			new CarePackageInfo("PrickleFlowerSeed", 2f, 0),
			new CarePackageInfo("ColdBreatherSeed", 1f, 0),
			new CarePackageInfo("FieldRation", 5f, 0),
			new CarePackageInfo("CookedMeat", 3f, 12),
			new CarePackageInfo("BasicForagePlant", 6f, 0),
			new CarePackageInfo("LightBugBaby", 1f, 0),
			new CarePackageInfo("HatchBaby", 1f, 0),
			new CarePackageInfo("DreckoBaby", 1f, 0),
			new CarePackageInfo("MoleBaby", 1f, 12),
			new CarePackageInfo("PuftBaby", 1f, 0),
			new CarePackageInfo("OilfloaterBaby", 1f, 12),
			new CarePackageInfo("Pacu", 8f, 12),
			new CarePackageInfo("LightBugEgg", 3f, 0),
			new CarePackageInfo("DreckoEgg", 3f, 0),
			new CarePackageInfo("HatchEgg", 3f, 0),
			new CarePackageInfo("MoleEgg", 3f, 12),
			new CarePackageInfo("PuftEgg", 3f, 0),
			new CarePackageInfo("OilfloaterEgg", 3f, 12),
			new CarePackageInfo("VitaminSupplement", 3f, 0),
			new CarePackageInfo("Funky_Vest", 1f, 0)
		};
	}

	public int EndImmigration()
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

	public CarePackageInfo RandomCarePackage()
	{
		List<CarePackageInfo> list = new List<CarePackageInfo>();
		CarePackageInfo[] array = carePackages;
		foreach (CarePackageInfo carePackageInfo in array)
		{
			if (carePackageInfo.cycleRequirement <= GameClock.Instance.GetCycle())
			{
				list.Add(carePackageInfo);
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
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
