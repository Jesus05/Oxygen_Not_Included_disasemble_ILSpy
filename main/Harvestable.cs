using KSerialization;
using TUNING;

[SerializationConfig(MemberSerialization.OptIn)]
public class Harvestable : Workable
{
	public HarvestDesignatable harvestDesignatable;

	[Serialize]
	protected bool canBeHarvested = false;

	protected Chore chore;

	private static readonly EventSystem.IntraObjectHandler<Harvestable> ForceCancelHarvestDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.ForceCancelHarvest(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Harvestable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Harvestable> OnUprootedDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.OnUprooted(data);
	});

	public Worker completed_by
	{
		get;
		protected set;
	}

	public bool CanBeHarvested => canBeHarvested;

	protected Harvestable()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Harvesting;
		multitoolContext = "harvest";
		multitoolHitEffectTag = "fx_harvest_splash";
	}

	protected override void OnSpawn()
	{
		harvestDesignatable = GetComponent<HarvestDesignatable>();
		Subscribe(2127324410, ForceCancelHarvestDelegate);
		SetWorkTime(10f);
		Subscribe(2127324410, OnCancelDelegate);
		faceTargetWhenWorking = true;
		Components.Harvestables.Add(this);
		attributeConverter = Db.Get().AttributeConverters.HarvestSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Farming.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
	}

	public void OnUprooted(object data)
	{
		if (canBeHarvested)
		{
			Harvest();
		}
	}

	public void Harvest()
	{
		harvestDesignatable.MarkedForHarvest = false;
		chore = null;
		Trigger(1272413801, this);
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
		component.RemoveStatusItem(Db.Get().MiscStatusItems.Operating, false);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public void OnMarkedForHarvest()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (chore == null)
		{
			chore = new WorkChore<Harvestable>(Db.Get().ChoreTypes.Harvest, this, null, true, null, null, null, true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			component.AddStatusItem(Db.Get().MiscStatusItems.PendingHarvest, this);
		}
		component.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, false);
	}

	public void SetCanBeHarvested(bool state)
	{
		canBeHarvested = state;
		KSelectable component = GetComponent<KSelectable>();
		if (canBeHarvested)
		{
			component.AddStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest, null);
			if (harvestDesignatable.HarvestWhenReady)
			{
				harvestDesignatable.MarkForHarvest();
			}
			else if (harvestDesignatable.InPlanterBox)
			{
				component.AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, this);
			}
		}
		else
		{
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest, false);
			component.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, false);
		}
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		completed_by = worker;
		Harvest();
	}

	protected virtual void OnCancel(object data)
	{
		if (chore != null)
		{
			chore.Cancel("Cancel harvest");
			chore = null;
			KSelectable component = GetComponent<KSelectable>();
			component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
			harvestDesignatable.SetHarvestWhenReady(false);
		}
		harvestDesignatable.MarkedForHarvest = false;
	}

	public bool HasChore()
	{
		if (chore != null)
		{
			return true;
		}
		return false;
	}

	public virtual void ForceCancelHarvest(object data = null)
	{
		OnCancel(null);
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Harvestables.Remove(this);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
	}
}
