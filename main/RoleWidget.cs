using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class RoleWidget : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private LocText Name;

	[SerializeField]
	private LocText Description;

	[SerializeField]
	private Image TitleBarBG;

	[SerializeField]
	private GameObject NoSlots;

	[SerializeField]
	private GameObject SlotContainer;

	[SerializeField]
	private RolesScreen rolesScreen;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private ColorStyleSetting[] dropDownColorStyles;

	[SerializeField]
	private RectTransform lines_left;

	[SerializeField]
	public RectTransform lines_right;

	[SerializeField]
	private Color header_color_active;

	[SerializeField]
	private Color header_color_inactive;

	[SerializeField]
	private Color header_color_disabled;

	[SerializeField]
	private Color line_color_default;

	[SerializeField]
	private Color line_color_active;

	[SerializeField]
	private GameObject borderHighlight;

	[SerializeField]
	private ToolTip masteryCount;

	public TextStyleSetting TooltipTextStyle_Header;

	public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

	private List<GameObject> Slots = new List<GameObject>();

	private List<RoleWidget> prerequisiteRoleWidgets = new List<RoleWidget>();

	private UILineRenderer[] lines;

	private List<Vector2> linePoints = new List<Vector2>();

	public string roleID
	{
		get;
		private set;
	}

	public void Refresh(string roleID)
	{
		Name.text = Game.Instance.roleManager.GetRole(roleID).name;
		if (roleID != "NoRole")
		{
			LocText name = Name;
			name.text = name.text + " (" + Game.Instance.roleManager.RoleGroups[Game.Instance.roleManager.GetRole(roleID).roleGroup].Name + ")";
		}
		Description.text = Game.Instance.roleManager.GetRole(roleID).description;
		this.roleID = roleID;
		NoSlots.SetActive(Game.Instance.roleManager.NumberOfSlotsUnlocked(roleID) == 0);
		tooltip.SetSimpleTooltip(Game.Instance.roleManager.RoleTooltip(roleID));
		int count = Game.Instance.roleManager.GetRoleAssignees(roleID).Count;
		if (Slots.Count > count)
		{
			for (int num = Slots.Count - 1; num > count; num--)
			{
				rolesScreen.RecycleSlotWidget(Slots[num]);
				Slots.RemoveAt(num);
			}
		}
		else
		{
			for (int i = Slots.Count; i < count + 1; i++)
			{
				GameObject slotWidget = rolesScreen.GetSlotWidget(SlotContainer);
				Slots.Add(slotWidget);
			}
		}
		List<MinionResume> roleAssignees = Game.Instance.roleManager.GetRoleAssignees(roleID);
		bool flag = false;
		for (int j = 0; j < count; j++)
		{
			RefreshSlot(SlotContainer.transform.GetChild(j).gameObject, roleAssignees[j]);
			if (!SlotContainer.transform.GetChild(j).gameObject.activeSelf)
			{
				SlotContainer.transform.GetChild(j).gameObject.SetActive(true);
			}
		}
		RefreshSlot(SlotContainer.transform.GetChild(count).gameObject, null);
		SlotContainer.transform.GetChild(count).gameObject.SetActive(!flag);
		flag = true;
		bool flag2 = false;
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			MinionResume component = item.GetComponent<MinionResume>();
			if (Game.Instance.roleManager.CanAssignToRole(roleID, component))
			{
				flag2 = true;
			}
		}
		if (flag2 || roleAssignees.Count > 0)
		{
			TitleBarBG.color = ((roleAssignees.Count <= 0) ? header_color_inactive : header_color_active);
		}
		else
		{
			TitleBarBG.color = header_color_disabled;
		}
		string text = string.Empty;
		List<MinionResume> list = new List<MinionResume>();
		foreach (MinionIdentity item2 in Components.LiveMinionIdentities.Items)
		{
			MinionResume component2 = item2.GetComponent<MinionResume>();
			if (component2.MasteryByRoleID[roleID])
			{
				list.Add(component2);
			}
		}
		masteryCount.gameObject.SetActive(list.Count > 0 && roleID != "NoRole");
		foreach (MinionResume item3 in list)
		{
			text = text + "\n    • " + item3.GetProperName();
		}
		masteryCount.SetSimpleTooltip((list.Count <= 0) ? UI.ROLES_SCREEN.WIDGET.NO_MASTERS_TOOLTIP.text : string.Format(UI.ROLES_SCREEN.WIDGET.NUMBER_OF_MASTERS_TOOLTIP, text));
		masteryCount.GetComponentInChildren<LocText>().text = list.Count.ToString();
	}

	public void RefreshLines()
	{
		prerequisiteRoleWidgets.Clear();
		List<Vector2> list = new List<Vector2>();
		RoleAssignmentRequirement[] requirements = Game.Instance.roleManager.GetRole(roleID).requirements;
		foreach (RoleAssignmentRequirement roleAssignmentRequirement in requirements)
		{
			if (roleAssignmentRequirement is PreviousRoleAssignmentRequirement)
			{
				list.Add(rolesScreen.GetRoleWidgetLineTargetPosition((roleAssignmentRequirement as PreviousRoleAssignmentRequirement).previousRoleID));
				prerequisiteRoleWidgets.Add(rolesScreen.GetRoleWidget((roleAssignmentRequirement as PreviousRoleAssignmentRequirement).previousRoleID));
			}
		}
		if (lines != null)
		{
			for (int num = lines.Length - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(lines[num].gameObject);
			}
		}
		linePoints.Clear();
		for (int j = 0; j < list.Count; j++)
		{
			Vector3 position = lines_left.GetPosition();
			float x = position.x;
			Vector2 vector = list[j];
			float num2 = x - vector.x - 12f;
			float y = 0f;
			linePoints.Add(new Vector2(0f, y));
			linePoints.Add(new Vector2(0f - num2, y));
			linePoints.Add(new Vector2(0f - num2, y));
			List<Vector2> list2 = linePoints;
			float x2 = 0f - num2;
			Vector3 position2 = lines_left.GetPosition();
			float y2 = position2.y;
			Vector2 vector2 = list[j];
			list2.Add(new Vector2(x2, 0f - (y2 - vector2.y)));
			List<Vector2> list3 = linePoints;
			float x3 = 0f - num2;
			Vector3 position3 = lines_left.GetPosition();
			float y3 = position3.y;
			Vector2 vector3 = list[j];
			list3.Add(new Vector2(x3, 0f - (y3 - vector3.y)));
			List<Vector2> list4 = linePoints;
			Vector3 position4 = lines_left.GetPosition();
			float x4 = position4.x;
			Vector2 vector4 = list[j];
			float x5 = 0f - (x4 - vector4.x);
			Vector3 position5 = lines_left.GetPosition();
			float y4 = position5.y;
			Vector2 vector5 = list[j];
			list4.Add(new Vector2(x5, 0f - (y4 - vector5.y)));
		}
		lines = new UILineRenderer[linePoints.Count / 2];
		int num3 = 0;
		for (int k = 0; k < linePoints.Count; k += 2)
		{
			GameObject gameObject = new GameObject("Line");
			gameObject.AddComponent<RectTransform>();
			gameObject.transform.SetParent(lines_left.transform);
			gameObject.transform.SetLocalPosition(Vector3.zero);
			gameObject.rectTransform().sizeDelta = Vector2.zero;
			lines[num3] = gameObject.AddComponent<UILineRenderer>();
			lines[num3].color = new Color(0.6509804f, 0.6509804f, 0.6509804f, 1f);
			lines[num3].Points = new Vector2[2]
			{
				linePoints[k],
				linePoints[k + 1]
			};
			num3++;
		}
	}

	private void RefreshSlot(GameObject slot, MinionResume occupier)
	{
		HierarchyReferences component = slot.GetComponent<HierarchyReferences>();
		component.GetReference<CrewPortrait>("Portrait").GetComponentInChildren<KBatchedAnimController>().enabled = ((UnityEngine.Object)occupier != (UnityEngine.Object)null);
		ToolTip reference = component.GetReference<ToolTip>("PortraitTooltip");
		reference.ClearMultiStringTooltip();
		if ((UnityEngine.Object)occupier != (UnityEngine.Object)null)
		{
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(occupier);
			AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(occupier);
			reference.AddMultiStringTooltip(occupier.GetProperName() + "\n\n", TooltipTextStyle_Header);
			if (roleID != "NoRole")
			{
				component.GetReference("UnassignButton").gameObject.SetActive(true);
				component.GetReference("ProgressBar").gameObject.SetActive(true);
				component.GetReference("ProgressBar").gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").text = GameUtil.GetFormattedPercent(Mathf.Floor(100f * (occupier.ExperienceByRoleID[roleID] / Game.Instance.roleManager.GetRole(roleID).experienceRequired)), GameUtil.TimeSlice.None);
				component.GetReference("ProgressBar").gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Progress").fillAmount = ((!occupier.ExperienceByRoleID.ContainsKey(roleID)) ? 0f : (occupier.ExperienceByRoleID[roleID] / Game.Instance.roleManager.GetRole(roleID).experienceRequired));
				component.GetReference("ProgressBar").GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.ROLES_SCREEN.ROLE_PROGRESS, Mathf.RoundToInt(occupier.ExperienceByRoleID[roleID]), Game.Instance.roleManager.GetRole(roleID).experienceRequired));
				component.GetReference<DropDown>("DropDown").gameObject.SetActive(false);
				RoleConfig role = Game.Instance.roleManager.GetRole(roleID);
				if (attributeInstance2.GetTotalValue() > attributeInstance.GetTotalValue())
				{
					if (role.tier > occupier.HighestTierRoleMastered())
					{
						reference.AddMultiStringTooltip(string.Format(UI.ROLES_SCREEN.EXPECTATION_ALERT_JOB, attributeInstance.GetTotalValue(), role.QOLExpectation(), role.name), TooltipTextStyle_AbilityNegativeModifier);
						reference.AddMultiStringTooltip(UI.ROLES_SCREEN.EXPECTATION_ALERT_DESC_JOB, null);
						component.GetReference<Image>("AlertIcon").color = new Color(0.992156863f, 0.5372549f, 0.294117659f);
						component.GetReference<Image>("AlertIcon").gameObject.SetActive(true);
					}
					else
					{
						reference.AddMultiStringTooltip(string.Format(UI.ROLES_SCREEN.EXPECTATION_ALERT_EXPECTATION, attributeInstance.GetTotalValue(), attributeInstance2.GetTotalValue()), TooltipTextStyle_AbilityNegativeModifier);
						reference.AddMultiStringTooltip(UI.ROLES_SCREEN.EXPECTATION_ALERT_DESC_EXPECTATION, null);
						component.GetReference<Image>("AlertIcon").color = new Color(0.7529412f, 0.7529412f, 0.7529412f);
						component.GetReference<Image>("AlertIcon").gameObject.SetActive(true);
					}
				}
				else
				{
					reference.AddMultiStringTooltip(string.Format(UI.ROLES_SCREEN.EXPECTATION_ALERT_EXPECTATION, attributeInstance.GetTotalValue(), attributeInstance2.GetTotalValue()), null);
					component.GetReference<Image>("AlertIcon").gameObject.SetActive(false);
				}
			}
			else
			{
				if (attributeInstance2.GetTotalValue() > attributeInstance.GetTotalValue())
				{
					reference.AddMultiStringTooltip(string.Format(UI.ROLES_SCREEN.EXPECTATION_ALERT_EXPECTATION, attributeInstance.GetTotalValue(), attributeInstance2.GetTotalValue()), TooltipTextStyle_AbilityNegativeModifier);
					reference.AddMultiStringTooltip(UI.ROLES_SCREEN.EXPECTATION_ALERT_DESC_EXPECTATION, null);
					component.GetReference<Image>("AlertIcon").color = new Color(0.7529412f, 0.7529412f, 0.7529412f);
					component.GetReference<Image>("AlertIcon").gameObject.SetActive(true);
				}
				else
				{
					reference.AddMultiStringTooltip(string.Format(UI.ROLES_SCREEN.EXPECTATION_ALERT_EXPECTATION, attributeInstance.GetTotalValue(), attributeInstance2.GetTotalValue()), null);
					component.GetReference<Image>("AlertIcon").gameObject.SetActive(false);
				}
				DropDown reference2 = component.GetReference<DropDown>("DropDown");
				reference2.gameObject.SetActive(true);
				reference2.Initialize(Game.Instance.roleManager.RolesConfigs.Cast<IListableOption>(), OnMinionDropEntryClick, minionDropDownSort, minionDropEntryRefreshAction, false, occupier);
			}
			component.GetReference<CrewPortrait>("Portrait").SetIdentityObject(occupier.GetComponent<MinionIdentity>(), true);
			component.GetReference<LocText>("Label").gameObject.SetActive(true);
			component.GetReference<LayoutElement>("DropDownLayout").minWidth = ((!(roleID == "NoRole")) ? 100f : 64f);
			component.GetReference<LayoutElement>("DropDownLayout").GetComponentInChildren<LocText>().text = ((!(roleID == "NoRole")) ? UI.ROLES_SCREEN.SLOTS.UNASSIGNED : UI.ROLES_SCREEN.SLOTS.PICK_JOB);
			component.GetReference<LocText>("Label").text = occupier.GetProperName();
			if (occupier.CurrentRole != occupier.TargetRole)
			{
				LocText reference3 = component.GetReference<LocText>("Label");
				reference3.text = reference3.text + " " + UI.ROLES_SCREEN.SLOTS.ASSIGNMENT_PENDING;
			}
			component.GetReference("BG").GetComponent<KImage>().ColorState = KImage.ColorSelector.Active;
			bool flag = false;
			foreach (KeyValuePair<HashedString, float> item in occupier.AptitudeByRoleGroup)
			{
				if (item.Value != 0f)
				{
					if (!flag)
					{
						flag = true;
						reference.AddMultiStringTooltip("\n" + UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.APTITUDES + "\n\n", TooltipTextStyle_Header);
					}
					reference.AddMultiStringTooltip("    • " + Game.Instance.roleManager.RoleGroups[item.Key].Name, null);
				}
			}
			reference.AddMultiStringTooltip("\n" + UI.DETAILTABS.STATS.NAME + "\n\n", TooltipTextStyle_Header);
			foreach (AttributeInstance attribute in occupier.GetAttributes())
			{
				if (attribute.Attribute.ShowInUI == Klei.AI.Attribute.Display.Skill)
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
					reference.AddMultiStringTooltip("    • " + attribute.Name + ": " + text + attribute.GetTotalValue() + UIConstants.ColorSuffix, null);
				}
			}
			reference.AddMultiStringTooltip("\n" + UI.ROLES_SCREEN.HIGHEST_EXPECTATIONS_TIER + "\n\n", TooltipTextStyle_Header);
			int num = 0;
			Expectation[] array = Expectations.ExpectationsByTier[occupier.GetComponent<MinionResume>().HighestTierRole()];
			foreach (Expectation expectation in array)
			{
				AttributeModifierExpectation attributeModifierExpectation = expectation as AttributeModifierExpectation;
				if (attributeModifierExpectation != null && attributeModifierExpectation.modifier.AttributeId == Db.Get().Attributes.QualityOfLifeExpectation.Id)
				{
					num = Mathf.RoundToInt(attributeModifierExpectation.modifier.Value);
				}
			}
			reference.AddMultiStringTooltip("    • " + RolesScreen.tierNames[occupier.GetComponent<MinionResume>().HighestTierRole()] + string.Format(UI.ROLES_SCREEN.ADDED_EXPECTATIONS_AMOUNT, num), null);
		}
		else
		{
			slot.GetComponent<MultiToggle>().onClick = delegate
			{
			};
			List<IListableOption> list = new List<IListableOption>();
			foreach (MinionIdentity item2 in Components.LiveMinionIdentities.Items)
			{
				list.Add(item2);
			}
			Action<IListableOption, object> onEntrySelectedAction = delegate(IListableOption minion, object data)
			{
				if (minion != null)
				{
					Game.Instance.roleManager.AssignToRole(roleID, (minion as MinionIdentity).GetComponent<MinionResume>(), false, false);
					rolesScreen.RefreshRoleWidgets();
					rolesScreen.RefreshSideBar();
				}
			};
			DropDown reference4 = component.GetReference<DropDown>("DropDown");
			reference4.gameObject.SetActive(true);
			reference4.Initialize(list, onEntrySelectedAction, roleSlotDropDownSort, roleRefreshAction, false, Game.Instance.roleManager.GetRole(roleID));
			component.GetReference<LocText>("Label").gameObject.SetActive(false);
			component.GetReference<LayoutElement>("DropDownLayout").minWidth = 156f;
			component.GetReference<LayoutElement>("DropDownLayout").GetComponentInChildren<LocText>().text = ((!(roleID == "NoRole")) ? UI.ROLES_SCREEN.SLOTS.UNASSIGNED : UI.ROLES_SCREEN.SLOTS.PICK_DUPLICANT);
			component.GetReference<CrewPortrait>("Portrait").SetIdentityObject(null, true);
			component.GetReference("UnassignButton").gameObject.SetActive(false);
			component.GetReference("ProgressBar").gameObject.SetActive(false);
			component.GetReference("BG").GetComponent<KImage>().ColorState = KImage.ColorSelector.Active;
			component.GetReference("AlertIcon").gameObject.SetActive(false);
		}
		component.GetReference<KButton>("UnassignButton").ClearOnClick();
		component.GetReference<KButton>("UnassignButton").onClick += delegate
		{
			MinionResume minionResume = Game.Instance.roleManager.GetRoleAssignees(roleID)[slot.transform.GetSiblingIndex()];
			Game.Instance.roleManager.Unassign(minionResume, false);
			minionResume.SetTargetRole("NoRole");
			Refresh(roleID);
			rolesScreen.RefreshSideBar();
			rolesScreen.RefreshWidgetPositions();
		};
	}

	private void OnMinionDropEntryClick(IListableOption role, object data)
	{
		if (role != null)
		{
			Game.Instance.roleManager.AssignToRole((role as RoleConfig).id, data as MinionResume, false, false);
			rolesScreen.RefreshRoleWidgets();
			rolesScreen.RefreshSideBar();
		}
	}

	private void minionDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		Image reference = entry.GetComponent<HierarchyReferences>().GetReference<Image>("SkillBox");
		Image reference2 = entry.GetComponent<HierarchyReferences>().GetReference<Image>("AptitudeBox");
		entry.GetComponent<HierarchyReferences>().GetReference("AlertIcon").gameObject.SetActive(false);
		entry.GetComponent<HierarchyReferences>().GetReference("MasteryIcon").gameObject.SetActive(false);
		Image reference3 = entry.GetComponent<HierarchyReferences>().GetReference<Image>("AlertIconCentered");
		Image reference4 = entry.GetComponent<HierarchyReferences>().GetReference<Image>("MasteryIconCentered");
		RoleConfig roleConfig = entry.entryData as RoleConfig;
		MinionResume minionResume = targetData as MinionResume;
		if (roleConfig == null)
		{
			reference.transform.parent.gameObject.SetActive(false);
			reference2.transform.parent.gameObject.SetActive(false);
			reference3.gameObject.SetActive(false);
			reference4.gameObject.SetActive(false);
		}
		else
		{
			entry.tooltip.SetSimpleTooltip(Game.Instance.roleManager.RoleCriteriaString(roleConfig.id, minionResume));
			entry.button.isInteractable = Game.Instance.roleManager.CanAssignToRole(roleConfig.id, minionResume);
			reference.transform.parent.gameObject.SetActive(true);
			float num = 0f;
			for (int i = 0; i < roleConfig.relevantAttributes.Length; i++)
			{
				num += minionResume.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
			}
			float num2 = minionResume.AptitudeByRoleGroup[roleConfig.roleGroup];
			reference.transform.parent.GetComponentInChildren<LocText>(true).text = num.ToString();
			if (roleConfig.relevantAttributes.Length > 0)
			{
				reference.SetAlpha(GameUtil.AttributeSkillToAlpha(num));
			}
			else
			{
				reference.SetAlpha(0f);
			}
			reference2.transform.parent.gameObject.SetActive(num2 > 0f);
			reference2.gameObject.SetActive(num2 > 0f);
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(minionResume);
			int num3 = roleConfig.QOLExpectation();
			if (minionResume.HasMasteredRole(roleConfig.id))
			{
				reference3.gameObject.SetActive(false);
				reference4.gameObject.SetActive(true);
			}
			else if (roleConfig.tier > minionResume.HighestTierRoleMastered() && (float)num3 > attributeInstance.GetTotalValue())
			{
				reference3.gameObject.SetActive(true);
				reference4.gameObject.SetActive(false);
			}
			else
			{
				reference3.gameObject.SetActive(false);
				reference4.gameObject.SetActive(false);
			}
		}
	}

	private int minionDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		RoleConfig roleConfig = a as RoleConfig;
		RoleConfig roleConfig2 = b as RoleConfig;
		MinionResume minionResume = targetData as MinionResume;
		bool flag = Game.Instance.roleManager.CanAssignToRole(roleConfig.id, minionResume);
		bool flag2 = Game.Instance.roleManager.CanAssignToRole(roleConfig2.id, minionResume);
		if (flag && !flag2)
		{
			return 1;
		}
		if (flag2 && !flag)
		{
			return -1;
		}
		float num = minionResume.AptitudeByRoleGroup[roleConfig.roleGroup];
		float num2 = minionResume.AptitudeByRoleGroup[roleConfig2.roleGroup];
		if (num != num2)
		{
			return (num > num2) ? 1 : (-1);
		}
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < roleConfig.relevantAttributes.Length; i++)
		{
			num3 += minionResume.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
		}
		for (int j = 0; j < roleConfig2.relevantAttributes.Length; j++)
		{
			num4 += minionResume.GetAttributes().Get(roleConfig2.relevantAttributes[j]).GetTotalDisplayValue();
		}
		if (num3 > num4)
		{
			return 1;
		}
		if (num4 > num3)
		{
			return -1;
		}
		return 0;
	}

	private int roleSlotDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		MinionResume component = (a as MinionIdentity).GetComponent<MinionResume>();
		MinionResume component2 = (b as MinionIdentity).GetComponent<MinionResume>();
		RoleConfig roleConfig = targetData as RoleConfig;
		if (component.CurrentRole != component2.CurrentRole)
		{
			if (component.CurrentRole == roleConfig.id)
			{
				return -1;
			}
			if (component2.CurrentRole == roleConfig.id)
			{
				return 1;
			}
		}
		bool flag = Game.Instance.roleManager.CanAssignToRole(roleConfig.id, component);
		bool flag2 = Game.Instance.roleManager.CanAssignToRole(roleConfig.id, component);
		if (flag && !flag2)
		{
			return -1;
		}
		if (flag2 && !flag)
		{
			return 1;
		}
		float num = component.AptitudeByRoleGroup[roleConfig.roleGroup];
		float num2 = component2.AptitudeByRoleGroup[roleConfig.roleGroup];
		if (num != num2)
		{
			return (num > num2) ? 1 : (-1);
		}
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < roleConfig.relevantAttributes.Length; i++)
		{
			num3 += component.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
			num4 += component2.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
		}
		if (num3 > num4)
		{
			return 1;
		}
		if (num4 > num3)
		{
			return -1;
		}
		if (component.CurrentRole == "NoRole" != (component2.CurrentRole == "NoRole"))
		{
			if (component.CurrentRole == "NoRole")
			{
				return 1;
			}
			return -1;
		}
		return 0;
	}

	private void roleRefreshAction(DropDownEntry entry, object targetData)
	{
		RoleConfig roleConfig = targetData as RoleConfig;
		MinionIdentity minionIdentity = null;
		if ((UnityEngine.Object)entry != (UnityEngine.Object)null)
		{
			minionIdentity = (entry.entryData as MinionIdentity);
		}
		Image reference = entry.GetComponent<HierarchyReferences>().GetReference<Image>("AptitudeBox");
		Image reference2 = entry.GetComponent<HierarchyReferences>().GetReference<Image>("SkillBox");
		Image reference3 = entry.GetComponent<HierarchyReferences>().GetReference<Image>("AlertIcon");
		Image reference4 = entry.GetComponent<HierarchyReferences>().GetReference<Image>("MasteryIcon");
		entry.GetComponent<HierarchyReferences>().GetReference("AlertIconCentered").gameObject.SetActive(false);
		entry.GetComponent<HierarchyReferences>().GetReference("MasteryIconCentered").gameObject.SetActive(false);
		if ((UnityEngine.Object)minionIdentity == (UnityEngine.Object)null)
		{
			reference.transform.parent.gameObject.SetActive(false);
			reference2.transform.parent.gameObject.SetActive(false);
			reference3.gameObject.SetActive(false);
			reference4.gameObject.SetActive(false);
		}
		else
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			entry.button.isInteractable = Game.Instance.roleManager.CanAssignToRole(roleID, minionIdentity.GetComponent<MinionResume>());
			entry.tooltip.SetSimpleTooltip(Game.Instance.roleManager.RoleCriteriaString(roleID, component));
			if (component.CurrentRole == "NoRole")
			{
				entry.label.text = minionIdentity.GetProperName();
			}
			else
			{
				entry.label.text = string.Format(UI.ROLES_SCREEN.DROPDOWN.NAME_AND_ROLE, minionIdentity.GetProperName(), Game.Instance.roleManager.GetRole(component.CurrentRole).name);
			}
			if (entry.button.isInteractable)
			{
				string currentRole = (entry.entryData as MinionIdentity).GetComponent<MinionResume>().CurrentRole;
			}
			float num = component.AptitudeByRoleGroup[roleConfig.roleGroup];
			reference.transform.parent.gameObject.SetActive(num > 0f);
			if (roleConfig.relevantAttributes.Length > 0)
			{
				reference.gameObject.SetActive(num > 0f);
			}
			else
			{
				reference.gameObject.SetActive(false);
			}
			reference2.transform.parent.gameObject.SetActive(true);
			float num2 = 0f;
			for (int i = 0; i < roleConfig.relevantAttributes.Length; i++)
			{
				num2 += minionIdentity.GetAttributes().Get(roleConfig.relevantAttributes[i]).GetTotalDisplayValue();
			}
			reference2.transform.parent.GetComponentInChildren<LocText>(true).text = num2.ToString();
			if (roleConfig.relevantAttributes.Length > 0)
			{
				reference2.SetAlpha(GameUtil.AttributeSkillToAlpha(num2));
			}
			else
			{
				reference2.SetAlpha(0f);
			}
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(component);
			int num3 = roleConfig.QOLExpectation();
			if (component.HasMasteredRole(roleConfig.id))
			{
				reference3.gameObject.SetActive(false);
				reference4.gameObject.SetActive(true);
			}
			else if (roleConfig.tier > component.HighestTierRoleMastered() && (float)num3 > attributeInstance.GetTotalValue())
			{
				reference3.gameObject.SetActive(true);
				reference4.gameObject.SetActive(false);
			}
			else
			{
				reference3.gameObject.SetActive(false);
				reference4.gameObject.SetActive(false);
			}
		}
	}

	public void ToggleBorderHighlight(bool on)
	{
		borderHighlight.SetActive(on);
		if (lines != null)
		{
			UILineRenderer[] array = lines;
			foreach (UILineRenderer uILineRenderer in array)
			{
				uILineRenderer.LineThickness = (float)((!on) ? 2 : 4);
				uILineRenderer.SetAllDirty();
			}
		}
		for (int j = 0; j < prerequisiteRoleWidgets.Count; j++)
		{
			prerequisiteRoleWidgets[j].ToggleBorderHighlight(on);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ToggleBorderHighlight(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ToggleBorderHighlight(false);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
	}
}
