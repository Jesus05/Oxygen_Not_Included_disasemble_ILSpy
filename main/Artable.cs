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

		public int decor;

		public bool cheerOnComplete;

		public Status statusItem;

		public Stage(string id, string name, string anim, int decor_value, bool cheer_on_complete, Status status_item)
		{
			this.id = id;
			this.name = name;
			this.anim = anim;
			decor = decor_value;
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

	public Status CurrentStatus
	{
		get
		{
			foreach (Stage stage in stages)
			{
				if (CurrentStage == stage.id)
				{
					return stage.statusItem;
				}
			}
			return Status.Ready;
		}
	}

	public string CurrentStage => currentStage;

	protected Artable()
	{
		faceTargetWhenWorking = true;
		statuses = new Dictionary<Status, StatusItem>();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		statuses[Status.Ready] = new StatusItem("AwaitingArting", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
		statuses[Status.Ugly] = new StatusItem("LookingUgly", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
		statuses[Status.Okay] = new StatusItem("LookingOkay", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
		statuses[Status.Great] = new StatusItem("LookingGreat", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
		workerStatusItem = Db.Get().DuplicantStatusItems.Arting;
		attributeConverter = Db.Get().AttributeConverters.ArtSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Art.Id;
		skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		requiredSkillPerk = Db.Get().SkillPerks.CanArt.Id;
		SetWorkTime(80f);
	}

	protected override void OnSpawn()
	{
		if (string.IsNullOrEmpty(currentStage))
		{
			currentStage = "Default";
		}
		SetStage(currentStage, true);
		shouldShowSkillPerkStatusItem = false;
		if (currentStage == "Default")
		{
			shouldShowSkillPerkStatusItem = true;
			Prioritizable.AddRef(base.gameObject);
			chore = new WorkChore<Artable>(Db.Get().ChoreTypes.Art, this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, requiredSkillPerk);
		}
		base.OnSpawn();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Status artist_skill = Status.Ugly;
		MinionResume component = worker.GetComponent<MinionResume>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			if (component.HasPerk(Db.Get().SkillPerks.CanArtGreat.Id))
			{
				artist_skill = Status.Great;
			}
			else if (component.HasPerk(Db.Get().SkillPerks.CanArtOkay.Id))
			{
				artist_skill = Status.Okay;
			}
		}
		List<Stage> potential_stages = new List<Stage>();
		stages.ForEach(delegate(Stage item)
		{
			potential_stages.Add(item);
		});
		potential_stages.RemoveAll((Stage x) => x.statusItem > artist_skill || x.id == "Default");
		potential_stages.Sort((Stage x, Stage y) => y.statusItem.CompareTo(x.statusItem));
		Status highest_status = potential_stages[0].statusItem;
		potential_stages.RemoveAll((Stage x) => x.statusItem < highest_status);
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
		shouldShowSkillPerkStatusItem = false;
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
			Debug.LogError("Missing stage: " + stage_id);
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
			shouldShowSkillPerkStatusItem = false;
			UpdateStatusItem(null);
		}
	}
}
