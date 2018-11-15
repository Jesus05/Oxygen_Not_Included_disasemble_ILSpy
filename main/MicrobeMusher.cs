using TUNING;
using UnityEngine;

public class MicrobeMusher : Fabricator
{
	[SerializeField]
	public Vector3 mushbarSpawnOffset = Vector3.right;

	private MeterController meter;

	private GameObject visualizer;

	private KAnimLink visualizerLink;

	private static readonly KAnimHashedString meterRationHash = new KAnimHashedString("meter_ration");

	private static readonly KAnimHashedString canHash = new KAnimHashedString("can");

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreType = Db.Get().ChoreTypes.Cook;
		choreTags = GameTags.ChoreTypes.CookingChores;
		fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
		workerStatusItem = Db.Get().DuplicantStatusItems.Mushing;
		attributeConverter = Db.Get().AttributeConverters.CookingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_ration");
		meter.meterController.SetSymbolVisiblity(canHash, false);
		meter.meterController.SetSymbolVisiblity(meterRationHash, false);
		meter.meterController.GetComponent<KBatchedAnimTracker>().skipInitialDisable = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater);
		}, null, null);
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Cook.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override void OnBuildQueued(MachineOrder order)
	{
		base.OnBuildQueued(order);
		InstantiateVisualizer(order);
		UpdateMeter();
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		UpdateMeter();
		return false;
	}

	private void UpdateMeter()
	{
		float workTime = GetWorkTime();
		float positionPercent = (workTime - workTimeRemaining) / workTime;
		meter.SetPositionPercent(positionPercent);
	}

	protected override GameObject CompleteOrder(UserOrder completed_order)
	{
		GameObject gameObject = base.CompleteOrder(completed_order);
		gameObject.transform.SetPosition(gameObject.transform.GetPosition() + mushbarSpawnOffset);
		gameObject.SetActive(true);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		if ((Object)component != (Object)null && component.DiseaseCount > 0)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_DiseaseCooking);
		}
		workTimeRemaining = GetWorkTime();
		UpdateMeter();
		return gameObject;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		DestroyVisualizer();
		base.OnCompleteWork(worker);
	}

	public override void CancelOrder(int idx)
	{
		if (idx == 0)
		{
			DestroyVisualizer();
		}
		base.CancelOrder(idx);
		UpdateMeter();
	}

	private void InstantiateVisualizer(MachineOrder order)
	{
		if ((Object)visualizer != (Object)null)
		{
			DestroyVisualizer();
		}
		visualizer = Util.KInstantiate(order.parentOrder.recipe.FabricationVisualizer, null, null);
		visualizer.transform.parent = meter.meterController.transform;
		visualizer.transform.SetLocalPosition(new Vector3(0f, 0f, 1f));
		visualizer.SetActive(true);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		KBatchedAnimController component2 = visualizer.GetComponent<KBatchedAnimController>();
		if (visualizerLink != null)
		{
			visualizerLink.Unregister();
			visualizerLink = null;
		}
		visualizerLink = new KAnimLink(component, component2);
	}

	private void DestroyVisualizer()
	{
		if ((Object)visualizer != (Object)null)
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
