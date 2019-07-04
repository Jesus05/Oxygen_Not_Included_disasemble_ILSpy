using KSerialization;
using TUNING;

public class Disinfectable : Workable
{
	private Chore chore;

	[Serialize]
	private bool isMarkedForDisinfect;

	private const float MAX_WORK_TIME = 10f;

	private float diseasePerSecond;

	private static readonly EventSystem.IntraObjectHandler<Disinfectable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Disinfectable>(delegate(Disinfectable component, object data)
	{
		component.OnCancel(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		workerStatusItem = Db.Get().DuplicantStatusItems.Disinfecting;
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		multitoolContext = "disinfect";
		multitoolHitEffectTag = "fx_disinfect_splash";
		Subscribe(2127324410, OnCancelDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (isMarkedForDisinfect)
		{
			MarkForDisinfect(true);
		}
		SetWorkTime(10f);
		shouldTransferDiseaseWithWorker = false;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		diseasePerSecond = (float)GetComponent<PrimaryElement>().DiseaseCount / 10f;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		base.OnWorkTick(worker, dt);
		PrimaryElement component = GetComponent<PrimaryElement>();
		component.AddDisease(component.DiseaseIdx, -(int)(diseasePerSecond * dt + 0.5f), "Disinfectable.OnWorkTick");
		return false;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		PrimaryElement component = GetComponent<PrimaryElement>();
		component.AddDisease(component.DiseaseIdx, -component.DiseaseCount, "Disinfectable.OnCompleteWork");
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForDisinfection, this);
		isMarkedForDisinfect = false;
		chore = null;
		Game.Instance.userMenu.Refresh(base.gameObject);
		Prioritizable.RemoveRef(base.gameObject);
	}

	private void ToggleMarkForDisinfect()
	{
		if (isMarkedForDisinfect)
		{
			CancelDisinfection();
		}
		else
		{
			SetWorkTime(10f);
			MarkForDisinfect(false);
		}
	}

	private void CancelDisinfection()
	{
		if (isMarkedForDisinfect)
		{
			Prioritizable.RemoveRef(base.gameObject);
			ShowProgressBar(false);
			isMarkedForDisinfect = false;
			chore.Cancel("disinfection cancelled");
			chore = null;
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForDisinfection, this);
		}
	}

	public void MarkForDisinfect(bool force = false)
	{
		if (!isMarkedForDisinfect || force)
		{
			isMarkedForDisinfect = true;
			Prioritizable.AddRef(base.gameObject);
			chore = new WorkChore<Disinfectable>(Db.Get().ChoreTypes.Disinfect, this, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
			GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.MarkedForDisinfection, this);
		}
	}

	private void OnCancel(object data)
	{
		CancelDisinfection();
	}
}
