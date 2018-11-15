using Klei.AI;
using System;
using TUNING;
using UnityEngine;

public class RefineryWorkable : Workable
{
	[MyCmpReq]
	protected Operational operational;

	private Refinery refinery;

	public Action<Worker, float> OnWorkTickActions;

	private Refinery GetRefinery
	{
		get
		{
			if ((UnityEngine.Object)refinery == (UnityEngine.Object)null)
			{
				refinery = GetComponent<Refinery>();
			}
			return refinery;
		}
	}

	public StatusItem WorkerStatusItem
	{
		get
		{
			return workerStatusItem;
		}
		set
		{
			workerStatusItem = value;
		}
	}

	public AttributeConverter AttributeConvertor
	{
		get
		{
			return attributeConverter;
		}
		set
		{
			attributeConverter = value;
		}
	}

	public float AttributeExperienceMultiplier
	{
		get
		{
			return attributeExperienceMultiplier;
		}
		set
		{
			attributeExperienceMultiplier = value;
		}
	}

	protected override void OnSpawn()
	{
		refinery = GetComponent<Refinery>();
		base.OnSpawn();
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(MachineTechnician.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("PowerTechnician", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("MechatronicEngineer", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	public override string GetConversationTopic()
	{
		string conversationTopic = refinery.GetConversationTopic();
		return (conversationTopic == null) ? base.GetConversationTopic() : conversationTopic;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (operational.IsOperational)
		{
			operational.SetActive(true, false);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		operational.SetActive(false, false);
	}

	public void OnCancelOrder()
	{
		workTimeRemaining = GetWorkTime();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (OnWorkTickActions != null)
		{
			OnWorkTickActions(worker, dt);
		}
		return base.OnWorkTick(worker, dt);
	}

	public override float GetWorkTime()
	{
		if (GetRefinery.GetMachineOrders.Count > 0)
		{
			Refinery.MachineOrder machineOrder = refinery.GetMachineOrders[0];
			workTime = machineOrder.parentOrder.recipe.time;
			return workTime;
		}
		return -1f;
	}

	public void CreateOrder(Refinery.MachineOrder buildable_order, ChoreType choreType, Tag[] choreTags)
	{
		buildable_order.chore = new WorkChore<RefineryWorkable>(choreType, this, null, choreTags, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		if (workTimeRemaining <= 0f)
		{
			workTimeRemaining = GetWorkTime();
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		refinery.OnCompleteWork();
	}
}
