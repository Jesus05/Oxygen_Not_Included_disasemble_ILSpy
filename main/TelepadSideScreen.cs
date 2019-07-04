using Database;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelepadSideScreen : SideScreenContent
{
	[SerializeField]
	private LocText timeLabel;

	[SerializeField]
	private KButton viewImmigrantsBtn;

	[SerializeField]
	private Telepad targetTelepad;

	[SerializeField]
	private KButton viewColonySummaryBtn;

	[SerializeField]
	private Image newAchievementsEarned;

	[SerializeField]
	private GameObject victoryConditionsContainer;

	[SerializeField]
	private GameObject conditionContainerTemplate;

	[SerializeField]
	private GameObject checkboxLinePrefab;

	private Dictionary<string, Dictionary<ColonyAchievementRequirement, GameObject>> entries = new Dictionary<string, Dictionary<ColonyAchievementRequirement, GameObject>>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		viewImmigrantsBtn.onClick += delegate
		{
			ImmigrantScreen.InitializeImmigrantScreen(targetTelepad);
			Game.Instance.Trigger(288942073, null);
		};
		viewColonySummaryBtn.onClick += delegate
		{
			newAchievementsEarned.gameObject.SetActive(false);
			RetireColonyUtility.SaveColonySummaryData();
			MainMenu.ActivateRetiredColoniesScreen(PauseScreen.Instance.transform.parent.gameObject, SaveGame.Instance.BaseName, null);
		};
		BuildVictoryConditions();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<Telepad>() != (Object)null;
	}

	public override void SetTarget(GameObject target)
	{
		Telepad component = target.GetComponent<Telepad>();
		if ((Object)component == (Object)null)
		{
			Debug.LogError("Target doesn't have a telepad associated with it.");
		}
		else
		{
			targetTelepad = component;
			if ((Object)targetTelepad != (Object)null)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.SetActive(true);
			}
		}
	}

	private void Update()
	{
		if ((Object)targetTelepad != (Object)null)
		{
			if ((Object)GameFlowManager.Instance != (Object)null && GameFlowManager.Instance.IsGameOver())
			{
				base.gameObject.SetActive(false);
				timeLabel.text = UI.UISIDESCREENS.TELEPADSIDESCREEN.GAMEOVER;
				SetContentState(true);
			}
			else
			{
				if (targetTelepad.GetComponent<Operational>().IsOperational)
				{
					timeLabel.text = string.Format(UI.UISIDESCREENS.TELEPADSIDESCREEN.NEXTPRODUCTION, GameUtil.GetFormattedCycles(targetTelepad.GetTimeRemaining(), "F1"));
				}
				else
				{
					base.gameObject.SetActive(false);
				}
				SetContentState(!Immigration.Instance.ImmigrantsAvailable);
			}
			UpdateVictoryConditions();
			UpdateAchievementsUnlocked();
		}
	}

	private void SetContentState(bool isLabel)
	{
		if (timeLabel.gameObject.activeInHierarchy != isLabel)
		{
			timeLabel.gameObject.SetActive(isLabel);
		}
		if (viewImmigrantsBtn.gameObject.activeInHierarchy == isLabel)
		{
			viewImmigrantsBtn.gameObject.SetActive(!isLabel);
		}
	}

	private void BuildVictoryConditions()
	{
		foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
		{
			if (resource.isVictoryCondition)
			{
				Dictionary<ColonyAchievementRequirement, GameObject> dictionary = new Dictionary<ColonyAchievementRequirement, GameObject>();
				GameObject gameObject = Util.KInstantiateUI(conditionContainerTemplate, victoryConditionsContainer, true);
				gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(resource.Name);
				foreach (ColonyAchievementRequirement item in resource.requirementChecklist)
				{
					GameObject gameObject2 = Util.KInstantiateUI(checkboxLinePrefab, gameObject, true);
					gameObject2.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(item.Name());
					gameObject2.GetComponent<ToolTip>().SetSimpleTooltip(item.Description());
					dictionary.Add(item, gameObject2);
				}
				entries.Add(resource.Id, dictionary);
			}
		}
	}

	private void UpdateVictoryConditions()
	{
		foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
		{
			if (resource.isVictoryCondition)
			{
				foreach (ColonyAchievementRequirement item in resource.requirementChecklist)
				{
					entries[resource.Id][item].GetComponent<HierarchyReferences>().GetReference<Image>("Check").enabled = item.Success();
				}
			}
		}
	}

	private void UpdateAchievementsUnlocked()
	{
		if (SaveGame.Instance.GetComponent<ColonyAchievementTracker>().achievementsToDisplay.Count > 0)
		{
			newAchievementsEarned.gameObject.SetActive(true);
		}
	}
}
