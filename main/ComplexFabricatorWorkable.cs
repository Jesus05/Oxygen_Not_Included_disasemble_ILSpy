using Klei.AI;
using System;
using TUNING;
using UnityEngine;

public class ComplexFabricatorWorkable : Workable
{
	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private ComplexFabricator fabricator;

	public Action<Worker, float> OnWorkTickActions;

	public MeterController meter;

	protected GameObject visualizer;

	protected KAnimLink visualizerLink;

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

	public AttributeConverter AttributeConverter
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

	public string SkillExperienceSkillGroup
	{
		set
		{
			skillExperienceSkillGroup = value;
		}
	}

	public float SkillExperienceMultiplier
	{
		set
		{
			skillExperienceMultiplier = value;
		}
	}

	public ComplexRecipe CurrentWorkingOrder => (!((UnityEngine.Object)fabricator != (UnityEngine.Object)null)) ? null : fabricator.CurrentWorkingOrder;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
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
			if (fabricator.CurrentWorkingOrder != null)
			{
				InstantiateVisualizer(fabricator.CurrentWorkingOrder);
			}
			else
			{
				DebugUtil.DevAssertArgs(false, "ComplexFabricatorWorkable.OnStartWork called but CurrentMachineOrder is null", base.gameObject);
			}
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (OnWorkTickActions != null)
		{
			OnWorkTickActions(worker, dt);
		}
		UpdateOrderProgress(worker, dt);
		return base.OnWorkTick(worker, dt);
	}

	public override float GetWorkTime()
	{
		ComplexRecipe currentWorkingOrder = fabricator.CurrentWorkingOrder;
		if (currentWorkingOrder != null)
		{
			workTime = currentWorkingOrder.time;
			return workTime;
		}
		return -1f;
	}

	public Chore CreateWorkChore(ChoreType choreType, float order_progress)
	{
		WorkChore<ComplexFabricatorWorkable> result = new WorkChore<ComplexFabricatorWorkable>(choreType, this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		workTimeRemaining = GetWorkTime() * (1f - order_progress);
		return result;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		fabricator.CompleteWorkingOrder();
		DestroyVisualizer();
	}

	private void InstantiateVisualizer(ComplexRecipe recipe)
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
		if (!((UnityEngine.Object)recipe.FabricationVisualizer == (UnityEngine.Object)null))
		{
			visualizer = Util.KInstantiate(recipe.FabricationVisualizer, null, null);
			visualizer.transform.parent = meter.meterController.transform;
			visualizer.transform.SetLocalPosition(new Vector3(0f, 0f, 1f));
			visualizer.SetActive(true);
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			KBatchedAnimController component2 = visualizer.GetComponent<KBatchedAnimController>();
			visualizerLink = new KAnimLink(component, component2);
		}
	}

	private void UpdateOrderProgress(Worker worker, float dt)
	{
		float workTime = GetWorkTime();
		float num = Mathf.Clamp01((workTime - base.WorkTimeRemaining) / workTime);
		if ((bool)fabricator)
		{
			fabricator.OrderProgress = num;
		}
		if (meter != null)
		{
			meter.SetPositionPercent(num);
		}
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
