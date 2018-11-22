using Klei.AI;
using System;
using TUNING;
using UnityEngine;

public class ComplexFabricatorWorkable : Workable
{
	[MyCmpReq]
	protected Operational operational;

	private ComplexFabricator fabricator;

	public Action<Worker, float> OnWorkTickActions;

	public MeterController meter;

	protected GameObject visualizer;

	protected KAnimLink visualizerLink;

	private ComplexFabricator GetFabricator
	{
		get
		{
			if ((UnityEngine.Object)fabricator == (UnityEngine.Object)null)
			{
				fabricator = GetComponent<ComplexFabricator>();
			}
			return fabricator;
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
		fabricator = GetComponent<ComplexFabricator>();
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
		string conversationTopic = fabricator.GetConversationTopic();
		return (conversationTopic == null) ? base.GetConversationTopic() : conversationTopic;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (operational.IsOperational)
		{
			operational.SetActive(true, false);
			InstantiateVisualizer(fabricator.GetMachineOrders()[0]);
		}
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		operational.SetActive(false, false);
	}

	public void ResetWorkTime()
	{
		workTimeRemaining = GetWorkTime();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (OnWorkTickActions != null)
		{
			OnWorkTickActions(worker, dt);
		}
		if (meter != null)
		{
			UpdateMeter(worker, dt);
		}
		return base.OnWorkTick(worker, dt);
	}

	public override float GetWorkTime()
	{
		if (GetFabricator.GetMachineOrders().Count <= 0)
		{
			return -1f;
		}
		ComplexFabricator.MachineOrder machineOrder = fabricator.GetMachineOrders()[0];
		workTime = machineOrder.parentOrder.recipe.time;
		return workTime;
	}

	public void CreateOrder(ComplexFabricator.MachineOrder buildable_order, ChoreType choreType, Tag[] choreTags)
	{
		buildable_order.chore = new WorkChore<ComplexFabricatorWorkable>(choreType, this, null, choreTags, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false);
		if (workTimeRemaining <= 0f)
		{
			workTimeRemaining = GetWorkTime();
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		fabricator.OnCompleteMachineOrder();
		DestroyVisualizer();
	}

	private void InstantiateVisualizer(ComplexFabricator.MachineOrder order)
	{
		if ((UnityEngine.Object)visualizer != (UnityEngine.Object)null)
		{
			DestroyVisualizer();
		}
		if (visualizerLink != null)
		{
			visualizerLink.Unregister();
			visualizerLink = null;
		}
		if (!((UnityEngine.Object)order.parentOrder.recipe.FabricationVisualizer == (UnityEngine.Object)null))
		{
			visualizer = Util.KInstantiate(order.parentOrder.recipe.FabricationVisualizer, null, null);
			visualizer.transform.parent = meter.meterController.transform;
			visualizer.transform.SetLocalPosition(new Vector3(0f, 0f, 1f));
			visualizer.SetActive(true);
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			KBatchedAnimController component2 = visualizer.GetComponent<KBatchedAnimController>();
			visualizerLink = new KAnimLink(component, component2);
		}
	}

	private void UpdateMeter(Worker worker, float dt)
	{
		float workTime = GetWorkTime();
		float positionPercent = (workTime - base.WorkTimeRemaining) / workTime;
		meter.SetPositionPercent(positionPercent);
	}

	private void DestroyVisualizer()
	{
		if ((UnityEngine.Object)visualizer != (UnityEngine.Object)null)
		{
			if (visualizerLink != null)
			{
				visualizerLink.Unregister();
				visualizerLink = null;
			}
			Util.KDestroyGameObject(visualizer);
			visualizer = null;
		}
	}
}
