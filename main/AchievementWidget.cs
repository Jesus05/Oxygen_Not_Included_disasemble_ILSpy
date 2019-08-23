using Database;
using Klei.AI;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementWidget : KMonoBehaviour
{
	private Color color_dark_red = new Color(0.282352954f, 0.160784319f, 0.149019614f);

	private Color color_gold = new Color(1f, 0.635294139f, 0.286274523f);

	private Color color_dark_grey = new Color(0.215686277f, 0.215686277f, 0.215686277f);

	private Color color_grey = new Color(0.6901961f, 0.6901961f, 0.6901961f);

	[SerializeField]
	private RectTransform sheenTransform;

	public AnimationCurve flourish_iconScaleCurve;

	public AnimationCurve flourish_sheenPositionCurve;

	public KBatchedAnimController[] sparks;

	[SerializeField]
	private RectTransform progressParent;

	[SerializeField]
	private GameObject requirementPrefab;

	[SerializeField]
	private Sprite statusSuccessIcon;

	[SerializeField]
	private Sprite statusFailureIcon;

	private int numRequirementsDisplayed;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		MultiToggle component = GetComponent<MultiToggle>();
		component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
		{
			ExpandAchievement();
		});
	}

	private void Update()
	{
	}

	private void ExpandAchievement()
	{
		if ((UnityEngine.Object)SaveGame.Instance != (UnityEngine.Object)null)
		{
			progressParent.gameObject.SetActive(!progressParent.gameObject.activeSelf);
		}
	}

	public void ActivateNewlyAchievedFlourish(float delay = 1f)
	{
		StartCoroutine(Flourish(delay));
	}

	private IEnumerator Flourish(float startDelay)
	{
		SetNeverAchieved();
		if ((UnityEngine.Object)GetComponent<Canvas>() == (UnityEngine.Object)null)
		{
			Canvas canvas = base.gameObject.AddComponent<Canvas>();
			canvas.sortingOrder = 1;
		}
		GetComponent<Canvas>().overrideSorting = true;
		yield return (object)new WaitForSecondsRealtime(startDelay);
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public void SetAchievedNow()
	{
		MultiToggle component = GetComponent<MultiToggle>();
		component.ChangeState(1);
		HierarchyReferences component2 = GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = color_dark_red;
		component2.GetReference<Image>("iconBorder").color = color_gold;
		component2.GetReference<Image>("icon").color = color_gold;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			locText.color = Color.white;
		}
		ConfigureToolTip(GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.ACHIEVED_THIS_COLONY_TOOLTIP);
	}

	public void SetAchievedBefore()
	{
		MultiToggle component = GetComponent<MultiToggle>();
		component.ChangeState(1);
		HierarchyReferences component2 = GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = color_dark_red;
		component2.GetReference<Image>("iconBorder").color = color_gold;
		component2.GetReference<Image>("icon").color = color_gold;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			locText.color = Color.white;
		}
		ConfigureToolTip(GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.ACHIEVED_OTHER_COLONY_TOOLTIP);
	}

	public void SetNeverAchieved()
	{
		MultiToggle component = GetComponent<MultiToggle>();
		component.ChangeState(2);
		HierarchyReferences component2 = GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = color_dark_grey;
		component2.GetReference<Image>("iconBorder").color = color_grey;
		component2.GetReference<Image>("icon").color = color_grey;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			LocText locText2 = locText;
			Color color = locText.color;
			float r = color.r;
			Color color2 = locText.color;
			float g = color2.g;
			Color color3 = locText.color;
			locText2.color = new Color(r, g, color3.b, 0.6f);
		}
		ConfigureToolTip(GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.NOT_ACHIEVED_EVER);
	}

	public void SetNotAchieved()
	{
		MultiToggle component = GetComponent<MultiToggle>();
		component.ChangeState(2);
		HierarchyReferences component2 = GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = color_dark_grey;
		component2.GetReference<Image>("iconBorder").color = color_grey;
		component2.GetReference<Image>("icon").color = color_grey;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			LocText locText2 = locText;
			Color color = locText.color;
			float r = color.r;
			Color color2 = locText.color;
			float g = color2.g;
			Color color3 = locText.color;
			locText2.color = new Color(r, g, color3.b, 0.6f);
		}
		ConfigureToolTip(GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.NOT_ACHIEVED_THIS_COLONY);
	}

	public void SetFailed()
	{
		MultiToggle component = GetComponent<MultiToggle>();
		component.ChangeState(2);
		HierarchyReferences component2 = GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = color_dark_grey;
		component2.GetReference<Image>("iconBG").SetAlpha(0.5f);
		component2.GetReference<Image>("iconBorder").color = color_grey;
		component2.GetReference<Image>("iconBorder").SetAlpha(0.5f);
		component2.GetReference<Image>("icon").color = color_grey;
		component2.GetReference<Image>("icon").SetAlpha(0.5f);
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			LocText locText2 = locText;
			Color color = locText.color;
			float r = color.r;
			Color color2 = locText.color;
			float g = color2.g;
			Color color3 = locText.color;
			locText2.color = new Color(r, g, color3.b, 0.25f);
		}
		ConfigureToolTip(GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.FAILED_THIS_COLONY);
	}

	private void ConfigureToolTip(ToolTip tooltip, string status)
	{
		tooltip.ClearMultiStringTooltip();
		tooltip.AddMultiStringTooltip(status, null);
		if ((UnityEngine.Object)SaveGame.Instance != (UnityEngine.Object)null && !progressParent.gameObject.activeSelf)
		{
			tooltip.AddMultiStringTooltip(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXPAND_TOOLTIP, null);
		}
	}

	public void ShowProgress(ColonyAchievementStatus achievement)
	{
		if (!((UnityEngine.Object)progressParent == (UnityEngine.Object)null))
		{
			numRequirementsDisplayed = 0;
			for (int i = 0; i < achievement.Requirements.Count; i++)
			{
				ColonyAchievementRequirement colonyAchievementRequirement = achievement.Requirements[i];
				if (colonyAchievementRequirement is CritterTypesWithTraits)
				{
					ShowCritterChecklist(colonyAchievementRequirement);
				}
				else if (colonyAchievementRequirement is DupesCompleteChoreInExoSuitForCycles)
				{
					ShowDupesInExoSuitsRequirement(achievement.success, colonyAchievementRequirement);
				}
				else if (colonyAchievementRequirement is DupesVsSolidTransferArmFetch)
				{
					ShowArmsOutPeformingDupesRequirement(achievement.success, colonyAchievementRequirement);
				}
				else if (colonyAchievementRequirement is ProduceXEngeryWithoutUsingYList)
				{
					ShowEngeryWithoutUsing(achievement.success, colonyAchievementRequirement);
				}
				else if (colonyAchievementRequirement is MinimumMorale)
				{
					ShowMinimumMoraleRequirement(achievement.success, colonyAchievementRequirement);
				}
				else
				{
					ShowRequirement(achievement.success, colonyAchievementRequirement);
				}
			}
		}
	}

	private HierarchyReferences GetNextRequirementWidget()
	{
		GameObject gameObject;
		if (progressParent.childCount <= numRequirementsDisplayed)
		{
			gameObject = Util.KInstantiateUI(requirementPrefab, progressParent.gameObject, true);
		}
		else
		{
			gameObject = progressParent.GetChild(numRequirementsDisplayed).gameObject;
			gameObject.SetActive(true);
		}
		numRequirementsDisplayed++;
		return gameObject.GetComponent<HierarchyReferences>();
	}

	private void SetDescription(string str, HierarchyReferences refs)
	{
		LocText reference = refs.GetReference<LocText>("Desc");
		reference.SetText(str);
	}

	private void SetIcon(Sprite sprite, Color color, HierarchyReferences refs)
	{
		Image reference = refs.GetReference<Image>("Icon");
		reference.sprite = sprite;
		reference.color = color;
		reference.gameObject.SetActive(true);
	}

	private void ShowIcon(bool show, HierarchyReferences refs)
	{
		Image reference = refs.GetReference<Image>("Icon");
		reference.gameObject.SetActive(show);
	}

	private void ShowRequirement(bool succeed, ColonyAchievementRequirement req)
	{
		HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
		bool flag = req.Success() || succeed;
		bool flag2 = req.Fail();
		if (flag && !flag2)
		{
			SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
		}
		else if (flag2)
		{
			SetIcon(statusFailureIcon, Color.red, nextRequirementWidget);
		}
		else
		{
			ShowIcon(false, nextRequirementWidget);
		}
		SetDescription(req.GetProgress(flag), nextRequirementWidget);
	}

	private void ShowCritterChecklist(ColonyAchievementRequirement req)
	{
		CritterTypesWithTraits critterTypesWithTraits = req as CritterTypesWithTraits;
		if (req != null)
		{
			foreach (KeyValuePair<Tag, bool> item in critterTypesWithTraits.critterTypesToCheck)
			{
				HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
				if (item.Value)
				{
					SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
				}
				SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TAME_A_CRITTER, item.Key.Name.ProperName()), nextRequirementWidget);
			}
		}
	}

	private void ShowArmsOutPeformingDupesRequirement(bool succeed, ColonyAchievementRequirement req)
	{
		DupesVsSolidTransferArmFetch dupesVsSolidTransferArmFetch = req as DupesVsSolidTransferArmFetch;
		if (dupesVsSolidTransferArmFetch != null)
		{
			HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
			if (succeed)
			{
				SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
			}
			SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ARM_PERFORMANCE, (!succeed) ? dupesVsSolidTransferArmFetch.currentCycleCount : dupesVsSolidTransferArmFetch.numCycles, dupesVsSolidTransferArmFetch.numCycles), nextRequirementWidget);
			if (!succeed)
			{
				Dictionary<int, int> fetchDupeChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchDupeChoreDeliveries;
				Dictionary<int, int> fetchAutomatedChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchAutomatedChoreDeliveries;
				int value = 0;
				fetchDupeChoreDeliveries.TryGetValue(GameClock.Instance.GetCycle(), out value);
				int value2 = 0;
				fetchAutomatedChoreDeliveries.TryGetValue(GameClock.Instance.GetCycle(), out value2);
				nextRequirementWidget = GetNextRequirementWidget();
				if ((float)value < (float)value2 * dupesVsSolidTransferArmFetch.percentage)
				{
					SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
				}
				SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ARM_VS_DUPE_FETCHES, "SolidTransferArm", value2, value), nextRequirementWidget);
			}
		}
	}

	private void ShowDupesInExoSuitsRequirement(bool succeed, ColonyAchievementRequirement req)
	{
		DupesCompleteChoreInExoSuitForCycles dupesCompleteChoreInExoSuitForCycles = req as DupesCompleteChoreInExoSuitForCycles;
		if (dupesCompleteChoreInExoSuitForCycles != null)
		{
			HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
			if (succeed)
			{
				SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
			}
			SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXOSUIT_CYCLES, (!succeed) ? dupesCompleteChoreInExoSuitForCycles.currentCycleStreak : dupesCompleteChoreInExoSuitForCycles.numCycles, dupesCompleteChoreInExoSuitForCycles.numCycles), nextRequirementWidget);
			if (!succeed)
			{
				nextRequirementWidget = GetNextRequirementWidget();
				int num = dupesCompleteChoreInExoSuitForCycles.GetNumberOfDupesForCycle(GameClock.Instance.GetCycle());
				if (num >= Components.LiveMinionIdentities.Count)
				{
					num = Components.LiveMinionIdentities.Count;
					SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
				}
				SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXOSUIT_THIS_CYCLE, num, Components.LiveMinionIdentities.Count), nextRequirementWidget);
			}
		}
	}

	private void ShowEngeryWithoutUsing(bool succeed, ColonyAchievementRequirement req)
	{
		ProduceXEngeryWithoutUsingYList produceXEngeryWithoutUsingYList = req as ProduceXEngeryWithoutUsingYList;
		if (req != null)
		{
			HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
			float productionAmount = produceXEngeryWithoutUsingYList.GetProductionAmount(succeed);
			SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.GENERATE_POWER, GameUtil.GetFormattedRoundedJoules(productionAmount), GameUtil.GetFormattedRoundedJoules(produceXEngeryWithoutUsingYList.amountToProduce)), nextRequirementWidget);
			if (succeed)
			{
				SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
			}
			foreach (Tag disallowedBuilding in produceXEngeryWithoutUsingYList.disallowedBuildings)
			{
				nextRequirementWidget = GetNextRequirementWidget();
				if (Game.Instance.savedInfo.powerCreatedbyGeneratorType.ContainsKey(disallowedBuilding))
				{
					SetIcon(statusFailureIcon, Color.red, nextRequirementWidget);
				}
				else
				{
					SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
				}
				BuildingDef buildingDef = Assets.GetBuildingDef(disallowedBuilding.Name);
				SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.NO_BUILDING, buildingDef.Name), nextRequirementWidget);
			}
		}
	}

	private void ShowMinimumMoraleRequirement(bool success, ColonyAchievementRequirement req)
	{
		MinimumMorale minimumMorale = req as MinimumMorale;
		if (minimumMorale != null)
		{
			if (success)
			{
				ShowRequirement(success, req);
			}
			else
			{
				IEnumerator enumerator = Components.MinionAssignablesProxy.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						MinionAssignablesProxy minionAssignablesProxy = (MinionAssignablesProxy)enumerator.Current;
						GameObject targetGameObject = minionAssignablesProxy.GetTargetGameObject();
						if ((UnityEngine.Object)targetGameObject != (UnityEngine.Object)null && !targetGameObject.HasTag(GameTags.Dead))
						{
							AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(targetGameObject.GetComponent<MinionModifiers>());
							if (attributeInstance != null)
							{
								HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
								if (attributeInstance.GetTotalValue() >= (float)minimumMorale.minimumMorale)
								{
									SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
								}
								SetDescription($"{targetGameObject.GetProperName()} morale: {attributeInstance.GetTotalDisplayValue()}", nextRequirementWidget);
							}
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
		}
	}
}
