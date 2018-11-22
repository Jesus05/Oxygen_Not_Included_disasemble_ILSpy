using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class Artable : Workable
{
	[Serializable]
	public class Stage
	{
		public string id;

		public string name;

		public string anim;

		public int minimumSkill;

		public int decor;

		public bool cheerOnComplete;

		public Status statusItem;

		public Stage(string id, string name, string anim, int minimum_skill, int decor_value, bool cheer_on_complete, Status status_item)
		{
			this.id = id;
			this.name = name;
			this.anim = anim;
			decor = decor_value;
			minimumSkill = minimum_skill;
			cheerOnComplete = cheer_on_complete;
			statusItem = status_item;
		}
	}

	public enum Status
	{
		Ready,
		Ugly,
		Okay,
		Great
	}

	private Dictionary<Status, StatusItem> statuses;

	[SerializeField]
	public List<Stage> stages = new List<Stage>();

	[Serialize]
	private string currentStage;

	private WorkChore<Artable> chore;

	protected string CurrentStage => currentStage;

	protected Artable()
	{
		faceTargetWhenWorking = true;
		statuses = new Dictionary<Status, StatusItem>();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		statuses[Status.Ready] = new StatusItem("AwaitingArting", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
		statuses[Status.Ugly] = new StatusItem("LookingUgly", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
		statuses[Status.Okay] = new StatusItem("LookingOkay", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
		statuses[Status.Great] = new StatusItem("LookingGreat", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 63486);
		workerStatusItem = Db.Get().DuplicantStatusItems.Arting;
		attributeConverter = Db.Get().AttributeConverters.ArtSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		SetWorkTime(80f);
	}

	protected override void OnSpawn()
	{
		if (string.IsNullOrEmpty(currentStage))
		{
			currentStage = "Default";
		}
		SetStage(currentStage, true);
		shouldShowRolePerkStatusItem = false;
		if (currentStage == "Default")
		{
			shouldShowRolePerkStatusItem = true;
			Prioritizable.AddRef(base.gameObject);
			ChoreType art = Db.Get().ChoreTypes.Art;
			Tag[] artChores = GameTags.ChoreTypes.ArtChores;
			chore = new WorkChore<Artable>(art, this, null, artChores, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false);
			chore.AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanArt.id);
		}
		base.OnSpawn();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		AttributeInstance attributeInstance = Db.Get().Attributes.Art.Lookup(worker);
		int art_skill = (int)attributeInstance.GetTotalValue();
		List<Stage> potential_stages = new List<Stage>();
		stages.ForEach(delegate(Stage item)
		{
			potential_stages.Add(item);
		});
		potential_stages.RemoveAll((Stage x) => x.minimumSkill > art_skill || x.id == "Default");
		potential_stages.Sort((Stage x, Stage y) => y.minimumSkill.CompareTo(x.minimumSkill));
		int highest_skill = potential_stages[0].minimumSkill;
		potential_stages.RemoveAll((Stage x) => x.minimumSkill < highest_skill);
		potential_stages.Shuffle();
		SetStage(potential_stages[0].id, false);
		if (potential_stages[0].cheerOnComplete)
		{
			new EmoteChore(worker.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[3]
			{
				"cheer_pre",
				"cheer_loop",
				"cheer_pst"
			}, null);
		}
		else
		{
			new EmoteChore(worker.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_disappointed_kanim", new HashedString[3]
			{
				"disappointed_pre",
				"disappointed_loop",
				"disappointed_pst"
			}, null);
		}
		shouldShowRolePerkStatusItem = false;
		UpdateStatusItem(null);
		Prioritizable.RemoveRef(base.gameObject);
	}

	public virtual void SetStage(string stage_id, bool skip_effect)
	{
		Stage stage = null;
		for (int i = 0; i < stages.Count; i++)
		{
			if (stages[i].id == stage_id)
			{
				stage = stages[i];
				break;
			}
		}
		if (stage == null)
		{
			Debug.LogError("Missing stage: " + stage_id, null);
		}
		else
		{
			currentStage = stage.id;
			GetComponent<KAnimControllerBase>().Play(stage.anim, KAnim.PlayMode.Once, 1f, 0f);
			if (stage.decor != 0)
			{
				AttributeModifier modifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, (float)stage.decor, "Art Quality", false, false, true);
				this.GetAttributes().Add(modifier);
			}
			KSelectable component = GetComponent<KSelectable>();
			component.SetName(stage.name);
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, statuses[stage.statusItem], this);
			shouldShowRolePerkStatusItem = false;
			UpdateStatusItem(null);
		}
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(Artist.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}
}
