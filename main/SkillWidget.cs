using Database;
using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SkillWidget : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IEventSystemHandler
{
	[SerializeField]
	private LocText Name;

	[SerializeField]
	private LocText Description;

	[SerializeField]
	private Image TitleBarBG;

	[SerializeField]
	private SkillsScreen skillsScreen;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private RectTransform lines_left;

	[SerializeField]
	public RectTransform lines_right;

	[SerializeField]
	private Color header_color_has_skill;

	[SerializeField]
	private Color header_color_can_assign;

	[SerializeField]
	private Color header_color_disabled;

	[SerializeField]
	private Color line_color_default;

	[SerializeField]
	private Color line_color_active;

	[SerializeField]
	private Image hatImage;

	[SerializeField]
	private GameObject borderHighlight;

	[SerializeField]
	private ToolTip masteryCount;

	[SerializeField]
	private GameObject aptitudeBox;

	[SerializeField]
	private GameObject traitDisabledIcon;

	public TextStyleSetting TooltipTextStyle_Header;

	public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

	private List<SkillWidget> prerequisiteSkillWidgets = new List<SkillWidget>();

	private UILineRenderer[] lines;

	private List<Vector2> linePoints = new List<Vector2>();

	public Material defaultMaterial;

	public Material desaturatedMaterial;

	public string skillID
	{
		get;
		private set;
	}

	public void Refresh(string skillID)
	{
		Skill skill = Db.Get().Skills.Get(skillID);
		if (skill == null)
		{
			Debug.LogWarning("DbSkills is missing skillId " + skillID);
		}
		else
		{
			Name.text = skill.Name;
			LocText name = Name;
			name.text = name.text + "\n(" + Db.Get().SkillGroups.Get(skill.skillGroup).Name + ")";
			this.skillID = skillID;
			tooltip.SetSimpleTooltip(SkillTooltip(skill));
			MinionIdentity minionIdentity = skillsScreen.CurrentlySelectedMinion as MinionIdentity;
			StoredMinionIdentity storedMinionIdentity = skillsScreen.CurrentlySelectedMinion as StoredMinionIdentity;
			MinionResume minionResume = null;
			if ((Object)minionIdentity != (Object)null)
			{
				minionResume = minionIdentity.GetComponent<MinionResume>();
				if (!((Object)minionResume == (Object)null) && (minionResume.HasMasteredSkill(skillID) || minionResume.CanMasterSkill(skillID)))
				{
					TitleBarBG.color = ((!minionResume.HasMasteredSkill(skillID)) ? header_color_can_assign : header_color_has_skill);
					hatImage.material = defaultMaterial;
				}
				else
				{
					TitleBarBG.color = header_color_disabled;
					hatImage.material = desaturatedMaterial;
				}
			}
			else if ((Object)storedMinionIdentity != (Object)null)
			{
				if (storedMinionIdentity.HasMasteredSkill(skillID))
				{
					TitleBarBG.color = header_color_has_skill;
					hatImage.material = defaultMaterial;
				}
				else
				{
					TitleBarBG.color = header_color_disabled;
					hatImage.material = desaturatedMaterial;
				}
			}
			hatImage.sprite = Assets.GetSprite(skill.hat);
			aptitudeBox.SetActive((Object)minionResume != (Object)null && minionResume.AptitudeBySkillGroup.ContainsKey(skill.skillGroup));
			traitDisabledIcon.SetActive((Object)minionResume != (Object)null && minionResume.CheckSkillTraitDisabled(skill.Id));
			string text = string.Empty;
			List<string> list = new List<string>();
			foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
			{
				MinionResume component = item.GetComponent<MinionResume>();
				if ((Object)component != (Object)null && component.HasMasteredSkill(skillID))
				{
					list.Add(component.GetProperName());
				}
			}
			foreach (MinionStorage item2 in Components.MinionStorages.Items)
			{
				foreach (MinionStorage.Info item3 in item2.GetStoredMinionInfo())
				{
					MinionStorage.Info current3 = item3;
					if (current3.serializedMinion != null)
					{
						StoredMinionIdentity storedMinionIdentity2 = current3.serializedMinion.Get<StoredMinionIdentity>();
						if ((Object)storedMinionIdentity2 != (Object)null && storedMinionIdentity2.HasMasteredSkill(skillID))
						{
							list.Add(storedMinionIdentity2.GetProperName());
						}
					}
				}
			}
			masteryCount.gameObject.SetActive(list.Count > 0);
			foreach (string item4 in list)
			{
				text = text + "\n    • " + item4;
			}
			masteryCount.SetSimpleTooltip((list.Count <= 0) ? UI.ROLES_SCREEN.WIDGET.NO_MASTERS_TOOLTIP.text : string.Format(UI.ROLES_SCREEN.WIDGET.NUMBER_OF_MASTERS_TOOLTIP, text));
			masteryCount.GetComponentInChildren<LocText>().text = list.Count.ToString();
		}
	}

	public void RefreshLines()
	{
		prerequisiteSkillWidgets.Clear();
		List<Vector2> list = new List<Vector2>();
		Skill skill = Db.Get().Skills.Get(skillID);
		foreach (string priorSkill in skill.priorSkills)
		{
			list.Add(skillsScreen.GetSkillWidgetLineTargetPosition(priorSkill));
			prerequisiteSkillWidgets.Add(skillsScreen.GetSkillWidget(priorSkill));
		}
		if (lines != null)
		{
			for (int num = lines.Length - 1; num >= 0; num--)
			{
				Object.Destroy(lines[num].gameObject);
			}
		}
		linePoints.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			Vector3 position = lines_left.GetPosition();
			float x = position.x;
			Vector2 vector = list[i];
			float num2 = x - vector.x - 12f;
			float y = 0f;
			linePoints.Add(new Vector2(0f, y));
			linePoints.Add(new Vector2(0f - num2, y));
			linePoints.Add(new Vector2(0f - num2, y));
			List<Vector2> list2 = linePoints;
			float x2 = 0f - num2;
			Vector3 position2 = lines_left.GetPosition();
			float y2 = position2.y;
			Vector2 vector2 = list[i];
			list2.Add(new Vector2(x2, 0f - (y2 - vector2.y)));
			List<Vector2> list3 = linePoints;
			float x3 = 0f - num2;
			Vector3 position3 = lines_left.GetPosition();
			float y3 = position3.y;
			Vector2 vector3 = list[i];
			list3.Add(new Vector2(x3, 0f - (y3 - vector3.y)));
			List<Vector2> list4 = linePoints;
			Vector3 position4 = lines_left.GetPosition();
			float x4 = position4.x;
			Vector2 vector4 = list[i];
			float x5 = 0f - (x4 - vector4.x);
			Vector3 position5 = lines_left.GetPosition();
			float y4 = position5.y;
			Vector2 vector5 = list[i];
			list4.Add(new Vector2(x5, 0f - (y4 - vector5.y)));
		}
		lines = new UILineRenderer[linePoints.Count / 2];
		int num3 = 0;
		for (int j = 0; j < linePoints.Count; j += 2)
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
				linePoints[j],
				linePoints[j + 1]
			};
			num3++;
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
				uILineRenderer.color = ((!on) ? line_color_default : line_color_active);
				uILineRenderer.LineThickness = (float)((!on) ? 2 : 4);
				uILineRenderer.SetAllDirty();
			}
		}
		for (int j = 0; j < prerequisiteSkillWidgets.Count; j++)
		{
			prerequisiteSkillWidgets[j].ToggleBorderHighlight(on);
		}
	}

	public string SkillTooltip(Skill skill)
	{
		string empty = string.Empty;
		empty += SkillPerksString(skill);
		return empty + "\n" + DuplicantSkillString(skill);
	}

	public string SkillPerksString(Skill skill)
	{
		string text = string.Empty;
		foreach (SkillPerk perk in skill.perks)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += "\n";
			}
			text = text + "• " + perk.Name;
		}
		return text;
	}

	public string CriteriaString(Skill skill)
	{
		bool flag = false;
		string empty = string.Empty;
		empty = empty + "<b>" + UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.TITLE + "</b>\n";
		SkillGroup skillGroup = Db.Get().SkillGroups.Get(skill.skillGroup);
		if (skillGroup != null && skillGroup.relevantAttributes != null)
		{
			foreach (string relevantAttribute in skillGroup.relevantAttributes)
			{
				Attribute attribute = Db.Get().Attributes.Get(relevantAttribute);
				if (attribute != null)
				{
					empty = empty + "    • " + string.Format(UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.SKILLGROUP_ENABLED.DESCRIPTION, attribute.Name) + "\n";
					flag = true;
				}
			}
		}
		if (skill.priorSkills.Count > 0)
		{
			flag = true;
			for (int i = 0; i < skill.priorSkills.Count; i++)
			{
				empty = empty + "    • " + $"{Db.Get().Skills.Get(skill.priorSkills[i]).Name}";
				empty += "</color>";
				if (i != skill.priorSkills.Count - 1)
				{
					empty += "\n";
				}
			}
		}
		if (!flag)
		{
			empty = empty + "    • " + string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.NONE, skill.Name);
		}
		return empty;
	}

	public string DuplicantSkillString(Skill skill)
	{
		string text = string.Empty;
		MinionIdentity minionIdentity = skillsScreen.CurrentlySelectedMinion as MinionIdentity;
		if ((Object)minionIdentity != (Object)null)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			if ((Object)component == (Object)null)
			{
				return string.Empty;
			}
			LocString cAN_MASTER = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.CAN_MASTER;
			if (!component.HasMasteredSkill(skill.Id) && !component.CanMasterSkill(skill.Id))
			{
				text += "\n";
				cAN_MASTER = UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.CANNOT_MASTER;
				text += string.Format(cAN_MASTER, minionIdentity.GetProperName(), skill.Name);
			}
		}
		return text;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ToggleBorderHighlight(true);
		skillsScreen.HoverSkill(skillID);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ToggleBorderHighlight(false);
		skillsScreen.HoverSkill(null);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		MinionIdentity minionIdentity = skillsScreen.CurrentlySelectedMinion as MinionIdentity;
		if ((Object)minionIdentity != (Object)null)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			if (DebugHandler.InstantBuildMode && component.AvailableSkillpoints < 1)
			{
				component.ForceAddSkillPoint();
			}
			if ((Object)component != (Object)null && !component.HasMasteredSkill(skillID) && component.CanMasterSkill(skillID))
			{
				component.MasterSkill(skillID);
				skillsScreen.RefreshSkillWidgets();
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
	}
}
