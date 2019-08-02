using KSerialization;
using STRINGS;
using System;
using TUNING;

public class Studyable : Workable, ISidescreenButtonControl
{
	public string meterTrackerSymbol;

	public string meterAnim;

	private Chore chore;

	private const float STUDY_WORK_TIME = 3600f;

	[Serialize]
	private bool studied;

	[Serialize]
	private bool markedForStudy;

	private Guid statusItemGuid;

	private Guid additionalStatusItemGuid;

	private MeterController studiedIndicator;

	public bool Studied => studied;

	public string SidescreenTitleKey => "STRINGS.UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.TITLE";

	public string SidescreenStatusMessage
	{
		get
		{
			if (studied)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.STUDIED_STATUS;
			}
			if (markedForStudy)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.PENDING_STATUS;
			}
			return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.SEND_STATUS;
		}
	}

	public string SidescreenButtonText
	{
		get
		{
			if (studied)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.STUDIED_BUTTON;
			}
			if (markedForStudy)
			{
				return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.PENDING_BUTTON;
			}
			return UI.UISIDESCREENS.STUDYABLE_SIDE_SCREEN.SEND_BUTTON;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_use_machine_kanim")
		};
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		workerStatusItem = Db.Get().DuplicantStatusItems.Studying;
		resetProgressOnStop = false;
		requiredSkillPerk = Db.Get().SkillPerks.CanStudyWorldObjects.Id;
		attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		SetWorkTime(3600f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		studiedIndicator = new MeterController(GetComponent<KBatchedAnimController>(), meterTrackerSymbol, meterAnim, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, meterTrackerSymbol);
		Refresh();
	}

	public void CancelChore()
	{
		if (chore != null)
		{
			chore.Cancel("Studyable.CancelChore");
			chore = null;
		}
	}

	public void Refresh()
	{
		if (!KMonoBehaviour.isLoadingScene)
		{
			KSelectable component = GetComponent<KSelectable>();
			if (studied)
			{
				statusItemGuid = component.ReplaceStatusItem(statusItemGuid, Db.Get().MiscStatusItems.Studied, null);
				studiedIndicator.gameObject.SetActive(true);
				studiedIndicator.meterController.Play(meterAnim, KAnim.PlayMode.Loop, 1f, 0f);
				requiredSkillPerk = null;
				UpdateStatusItem(null);
			}
			else
			{
				if (markedForStudy)
				{
					if (chore == null)
					{
						chore = new WorkChore<Studyable>(Db.Get().ChoreTypes.Research, this, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
					}
					statusItemGuid = component.ReplaceStatusItem(statusItemGuid, Db.Get().MiscStatusItems.AwaitingStudy, null);
				}
				else
				{
					CancelChore();
					statusItemGuid = component.RemoveStatusItem(statusItemGuid, false);
				}
				studiedIndicator.gameObject.SetActive(false);
			}
		}
	}

	private void ToggleStudyChore()
	{
		if (DebugHandler.InstantBuildMode)
		{
			studied = true;
			if (chore != null)
			{
				chore.Cancel("debug");
				chore = null;
			}
		}
		else
		{
			markedForStudy = !markedForStudy;
		}
		Refresh();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		studied = true;
		chore = null;
		Refresh();
	}

	public void OnSidescreenButtonPressed()
	{
		ToggleStudyChore();
	}
}
