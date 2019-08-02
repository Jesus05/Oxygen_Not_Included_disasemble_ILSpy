using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class Sleepable : Workable
{
	private const float STRECH_CHANCE = 0.33f;

	[MyCmpGet]
	private Operational operational;

	public string effectName = "Sleep";

	public List<string> wakeEffects;

	public bool stretchOnWake = true;

	private float wakeTime;

	private bool isDoneSleeping;

	private static readonly HashedString[] normalWorkAnims = new HashedString[2]
	{
		"working_pre",
		"working_loop"
	};

	private static readonly HashedString[] hatWorkAnims = new HashedString[2]
	{
		"hat_pre",
		"working_loop"
	};

	private static readonly HashedString normalWorkPstAnim = "working_pst";

	private static readonly HashedString hatWorkPstAnim = "hat_pst";

	private Sleepable()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
		showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = null;
		synchronizeAnims = false;
		triggerWorkReactions = false;
		lightEfficiencyBonus = false;
	}

	protected override void OnSpawn()
	{
		Components.Sleepables.Add(this);
		SetWorkTime(float.PositiveInfinity);
	}

	public override HashedString[] GetWorkAnims(Worker worker)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		if ((Object)GetComponent<Building>() != (Object)null && (Object)component != (Object)null && component.CurrentHat != null)
		{
			return hatWorkAnims;
		}
		return normalWorkAnims;
	}

	public override HashedString GetWorkPstAnim(Worker worker, bool successfully_completed)
	{
		MinionResume component = worker.GetComponent<MinionResume>();
		if ((Object)GetComponent<Building>() != (Object)null && (Object)component != (Object)null && component.CurrentHat != null)
		{
			return hatWorkPstAnim;
		}
		return normalWorkPstAnim;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if ((Object)operational != (Object)null)
		{
			operational.SetActive(true, false);
		}
		worker.Trigger(-1283701846, this);
		worker.GetComponent<Effects>().Add(effectName, false);
		isDoneSleeping = false;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (isDoneSleeping)
		{
			if (Time.time > wakeTime)
			{
				return true;
			}
		}
		else if (worker.GetSMI<StaminaMonitor.Instance>().ShouldExitSleep())
		{
			isDoneSleeping = true;
			wakeTime = Time.time + Random.value * 3f;
		}
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		if ((Object)operational != (Object)null)
		{
			operational.SetActive(false, false);
		}
		if ((Object)worker != (Object)null)
		{
			Effects component = worker.GetComponent<Effects>();
			component.Remove(effectName);
			if (wakeEffects != null)
			{
				foreach (string wakeEffect in wakeEffects)
				{
					component.Add(wakeEffect, true);
				}
			}
			if (stretchOnWake && Random.value < 0.33f)
			{
				new EmoteChore(worker.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_react_morning_stretch_kanim", new HashedString[1]
				{
					"react"
				}, null);
			}
			if (worker.GetAmounts().Get(Db.Get().Amounts.Stamina).value < worker.GetAmounts().Get(Db.Get().Amounts.Stamina).GetMax())
			{
				worker.Trigger(1338475637, this);
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Sleepables.Remove(this);
	}
}
