using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillMinionWidget : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private SkillsScreen skillsScreen;

	[SerializeField]
	private CrewPortrait portrait;

	[SerializeField]
	private LocText masteryPoints;

	[SerializeField]
	private LocText morale;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Image hat_background;

	[SerializeField]
	private Color selected_color;

	[SerializeField]
	private Color unselected_color;

	[SerializeField]
	private Color hover_color;

	[SerializeField]
	private DropDown hatDropDown;

	[SerializeField]
	private TextStyleSetting TooltipTextStyle_Header;

	[SerializeField]
	private TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

	public IAssignableIdentity minion
	{
		get;
		private set;
	}

	public void SetMinon(IAssignableIdentity identity)
	{
		minion = identity;
		portrait.SetIdentityObject(minion, true);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ToggleHover(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ToggleHover(false);
	}

	private void ToggleHover(bool on)
	{
		if (skillsScreen.CurrentlySelectedMinion != minion)
		{
			SetColor((!on) ? unselected_color : hover_color);
		}
	}

	private void SetColor(Color color)
	{
		background.color = color;
		if (minion != null && (Object)(minion as StoredMinionIdentity) != (Object)null)
		{
			GetComponent<CanvasGroup>().alpha = 0.6f;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		skillsScreen.CurrentlySelectedMinion = minion;
		KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
	}

	public void Refresh()
	{
		if (minion != null)
		{
			portrait.SetIdentityObject(minion, true);
			string empty = string.Empty;
			MinionIdentity minionIdentity = minion as MinionIdentity;
			hatDropDown.gameObject.SetActive(true);
			if ((Object)minionIdentity != (Object)null)
			{
				MinionResume component = minionIdentity.GetComponent<MinionResume>();
				int availableSkillpoints = component.AvailableSkillpoints;
				int totalSkillPointsGained = component.TotalSkillPointsGained;
				masteryPoints.text = ((availableSkillpoints <= 0) ? "0" : GameUtil.ApplyBoldString(GameUtil.ColourizeString(new Color(0.5f, 1f, 0.5f, 1f), availableSkillpoints.ToString())));
				AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(component);
				AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(component);
				morale.text = $"{attributeInstance.GetTotalValue()}/{attributeInstance2.GetTotalValue()}";
				RefreshToolTip(component);
				List<IListableOption> list = new List<IListableOption>();
				foreach (KeyValuePair<string, bool> item in component.MasteryBySkillID)
				{
					if (item.Value)
					{
						list.Add(new SkillListable(item.Key));
					}
				}
				hatDropDown.Initialize(list, OnHatDropEntryClick, hatDropDownSort, hatDropEntryRefreshAction, false, minion);
				empty = ((!string.IsNullOrEmpty(component.TargetHat)) ? component.TargetHat : component.CurrentHat);
			}
			else
			{
				StoredMinionIdentity storedMinionIdentity = minion as StoredMinionIdentity;
				ToolTip component2 = GetComponent<ToolTip>();
				component2.ClearMultiStringTooltip();
				component2.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, storedMinionIdentity.GetStorageReason(), minion.GetProperName()), null);
				empty = ((!string.IsNullOrEmpty(storedMinionIdentity.targetHat)) ? storedMinionIdentity.targetHat : storedMinionIdentity.currentHat);
				masteryPoints.text = UI.TABLESCREENS.NA;
				morale.text = UI.TABLESCREENS.NA;
			}
			SetColor((skillsScreen.CurrentlySelectedMinion != minion) ? unselected_color : selected_color);
			HierarchyReferences component3 = GetComponent<HierarchyReferences>();
			RefreshHat(empty);
			component3.GetReference("openButton").gameObject.SetActive((Object)minionIdentity != (Object)null);
		}
	}

	private void RefreshToolTip(MinionResume resume)
	{
		if ((Object)resume != (Object)null)
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(resume);
			AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(resume);
			ToolTip component = GetComponent<ToolTip>();
			component.ClearMultiStringTooltip();
			component.AddMultiStringTooltip(minion.GetProperName() + "\n\n", TooltipTextStyle_Header);
			component.AddMultiStringTooltip(string.Format(UI.SKILLS_SCREEN.CURRENT_MORALE, attributeInstance.GetTotalValue(), attributeInstance2.GetTotalValue()), null);
			component.AddMultiStringTooltip("\n" + UI.DETAILTABS.STATS.NAME + "\n\n", TooltipTextStyle_Header);
			foreach (AttributeInstance attribute in resume.GetAttributes())
			{
				if (attribute.Attribute.ShowInUI == Attribute.Display.Skill)
				{
					string text = UIConstants.ColorPrefixWhite;
					if (attribute.GetTotalValue() > 0f)
					{
						text = UIConstants.ColorPrefixGreen;
					}
					else if (attribute.GetTotalValue() < 0f)
					{
						text = UIConstants.ColorPrefixRed;
					}
					component.AddMultiStringTooltip("    • " + attribute.Name + ": " + text + attribute.GetTotalValue() + UIConstants.ColorSuffix, null);
				}
			}
		}
	}

	public void RefreshHat(string hat)
	{
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		component.GetReference("selectedHat").GetComponent<Image>().sprite = Assets.GetSprite((!string.IsNullOrEmpty(hat)) ? hat : "hat_role_none");
	}

	private void OnHatDropEntryClick(IListableOption skill, object data)
	{
		MinionIdentity minionIdentity = minion as MinionIdentity;
		if (!((Object)minionIdentity == (Object)null))
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			if (skill != null)
			{
				HierarchyReferences component2 = GetComponent<HierarchyReferences>();
				component2.GetReference("selectedHat").GetComponent<Image>().sprite = Assets.GetSprite((skill as SkillListable).skillHat);
				if ((Object)component != (Object)null)
				{
					string skillHat = (skill as SkillListable).skillHat;
					component.SetHats(component.CurrentHat, skillHat);
					if (component.OwnsHat(skillHat))
					{
						new PutOnHatChore(component, Db.Get().ChoreTypes.SwitchHat);
					}
				}
			}
			else
			{
				HierarchyReferences component3 = GetComponent<HierarchyReferences>();
				component3.GetReference("selectedHat").GetComponent<Image>().sprite = Assets.GetSprite("hat_role_none");
				if ((Object)component != (Object)null)
				{
					component.SetHats(component.CurrentHat, null);
					component.ApplyTargetHat();
				}
			}
			if (minion == skillsScreen.CurrentlySelectedMinion)
			{
				skillsScreen.selectedHat.sprite = Assets.GetSprite((!string.IsNullOrEmpty(component.TargetHat)) ? component.TargetHat : "hat_role_none");
			}
		}
	}

	private void hatDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		if (entry.entryData != null)
		{
			SkillListable skillListable = entry.entryData as SkillListable;
			entry.image.sprite = Assets.GetSprite(skillListable.skillHat);
		}
	}

	private int hatDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		return 0;
	}
}
