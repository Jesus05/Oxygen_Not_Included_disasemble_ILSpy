using KSerialization;
using STRINGS;
using System;
using TUNING;
using UnityEngine;

public class AutoDisinfectable : Workable
{
	private Chore chore;

	private const float MAX_WORK_TIME = 10f;

	private float diseasePerSecond;

	[MyCmpGet]
	private PrimaryElement primaryElement;

	[Serialize]
	private bool enableAutoDisinfect = true;

	private static readonly EventSystem.IntraObjectHandler<AutoDisinfectable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<AutoDisinfectable>(delegate(AutoDisinfectable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		workerStatusItem = Db.Get().DuplicantStatusItems.Disinfecting;
		resetProgressOnStop = true;
		multitoolContext = "disinfect";
		multitoolHitEffectTag = "fx_disinfect_splash";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		SetWorkTime(10f);
		shouldTransferDiseaseWithWorker = false;
	}

	public void CancelChore()
	{
		if (chore != null)
		{
			chore.Cancel("AutoDisinfectable.CancelChore");
			chore = null;
		}
	}

	public void RefreshChore()
	{
		if (!KMonoBehaviour.isLoadingScene)
		{
			if (!enableAutoDisinfect || !SaveGame.Instance.enableAutoDisinfect)
			{
				if (chore != null)
				{
					chore.Cancel("Autodisinfect Disabled");
					chore = null;
				}
			}
			else if (chore == null || !((UnityEngine.Object)chore.driver != (UnityEngine.Object)null))
			{
				int diseaseCount = primaryElement.DiseaseCount;
				if (chore == null && diseaseCount > SaveGame.Instance.minGermCountForDisinfect)
				{
					chore = new WorkChore<AutoDisinfectable>(Db.Get().ChoreTypes.Disinfect, this, null, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, true, true);
				}
				else if (diseaseCount < SaveGame.Instance.minGermCountForDisinfect && chore != null)
				{
					chore.Cancel("AutoDisinfectable.Update");
					chore = null;
				}
			}
		}
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
		chore = null;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	private void EnableAutoDisinfect()
	{
		enableAutoDisinfect = true;
		RefreshChore();
	}

	private void DisableAutoDisinfect()
	{
		enableAutoDisinfect = false;
		RefreshChore();
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo = null;
		if (!enableAutoDisinfect)
		{
			string iconName = "action_disinfect";
			string text = STRINGS.BUILDINGS.AUTODISINFECTABLE.ENABLE_AUTODISINFECT.NAME;
			System.Action on_click = EnableAutoDisinfect;
			string tooltipText = STRINGS.BUILDINGS.AUTODISINFECTABLE.ENABLE_AUTODISINFECT.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
		}
		else
		{
			string tooltipText = "action_disinfect";
			string text = STRINGS.BUILDINGS.AUTODISINFECTABLE.DISABLE_AUTODISINFECT.NAME;
			System.Action on_click = DisableAutoDisinfect;
			string iconName = STRINGS.BUILDINGS.AUTODISINFECTABLE.DISABLE_AUTODISINFECT.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
		}
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 10f);
	}
}
