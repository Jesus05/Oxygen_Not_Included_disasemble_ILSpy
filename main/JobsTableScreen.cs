using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JobsTableScreen : TableScreen
{
	public struct PriorityInfo
	{
		public int priority;

		public Sprite sprite;

		public LocString name;

		public PriorityInfo(int priority, Sprite sprite, LocString name)
		{
			this.priority = priority;
			this.sprite = sprite;
			this.name = name;
		}
	}

	private struct SkillEventHandlerID
	{
		public int level_up;

		public int effect_added;

		public int effect_removed;

		public int disease_added;

		public int disease_cured;
	}

	[SerializeField]
	private Color32 skillOutlineColourLow = Color.white;

	[SerializeField]
	private Color32 skillOutlineColourHigh = new Color(0.721568644f, 0.443137258f, 0.5803922f);

	[SerializeField]
	private int skillLevelLow = 1;

	[SerializeField]
	private int skillLevelHigh = 10;

	[SerializeField]
	private KButton settingsButton;

	[SerializeField]
	private KButton resetSettingsButton;

	[SerializeField]
	private KButton toggleAdvancedModeButton;

	[SerializeField]
	private KImage optionsPanel;

	public static JobsTableScreen Instance;

	[SerializeField]
	private bool dynamicRowSpacing = true;

	public TextStyleSetting TooltipTextStyle_Ability;

	public TextStyleSetting TooltipTextStyle_AbilityPositiveModifier;

	public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

	private HashSet<MinionIdentity> dirty_single_minion_rows = new HashSet<MinionIdentity>();

	private static List<PriorityInfo> _priorityInfo;

	private List<Sprite> prioritySprites;

	private List<KeyValuePair<GameObject, SkillEventHandlerID>> EffectListeners = new List<KeyValuePair<GameObject, SkillEventHandlerID>>();

	public static List<PriorityInfo> priorityInfo
	{
		get
		{
			if (_priorityInfo == null)
			{
				List<PriorityInfo> list = new List<PriorityInfo>();
				list.Add(new PriorityInfo(0, Assets.GetSprite("icon_priority_disabled"), UI.JOBSSCREEN.PRIORITY.DISABLED));
				list.Add(new PriorityInfo(1, Assets.GetSprite("icon_priority_down_2"), UI.JOBSSCREEN.PRIORITY.VERYLOW));
				list.Add(new PriorityInfo(2, Assets.GetSprite("icon_priority_down"), UI.JOBSSCREEN.PRIORITY.LOW));
				list.Add(new PriorityInfo(3, Assets.GetSprite("icon_priority_flat"), UI.JOBSSCREEN.PRIORITY.STANDARD));
				list.Add(new PriorityInfo(4, Assets.GetSprite("icon_priority_up"), UI.JOBSSCREEN.PRIORITY.HIGH));
				list.Add(new PriorityInfo(5, Assets.GetSprite("icon_priority_up_2"), UI.JOBSSCREEN.PRIORITY.VERYHIGH));
				list.Add(new PriorityInfo(5, Assets.GetSprite("icon_priority_automatic"), UI.JOBSSCREEN.PRIORITY.VERYHIGH));
				_priorityInfo = list;
			}
			return _priorityInfo;
		}
	}

	public override float GetSortKey()
	{
		return 101f;
	}

	protected override void OnActivate()
	{
		Instance = this;
		title = UI.JOBSSCREEN.TITLE;
		base.OnActivate();
		resetSettingsButton.onClick += OnResetSettingsClicked;
		prioritySprites = new List<Sprite>();
		foreach (PriorityInfo item in priorityInfo)
		{
			PriorityInfo current = item;
			prioritySprites.Add(current.sprite);
		}
		AddPortraitColumn("Portrait", base.on_load_portrait, null, true);
		AddButtonLabelColumn("Names", ConfigureNameLabel, base.get_value_name_label, delegate(GameObject widget_go)
		{
			GetWidgetRow(widget_go).SelectMinion();
		}, delegate(GameObject widget_go)
		{
			GetWidgetRow(widget_go).SelectAndFocusMinion();
		}, base.compare_rows_alphabetical, null, base.on_tooltip_sort_alphabetically, false);
		List<ChoreGroup> list = new List<ChoreGroup>(Db.Get().ChoreGroups.resources);
		from @group in list
		orderby @group.DefaultPersonalPriority descending, @group.Name
		select @group;
		foreach (ChoreGroup item2 in list)
		{
			PrioritizationGroupTableColumn new_column = new PrioritizationGroupTableColumn(item2, LoadValue, ChangePersonalPriority, HoverPersonalPriority, ChangeColumnPriority, HoverChangeColumnPriorityButton, OnSortClicked, OnSortHovered);
			RegisterColumn(item2.Id, new_column);
		}
		PrioritizeRowTableColumn new_column2 = new PrioritizeRowTableColumn(null, ChangeRowPriority, HoverChangeRowPriorityButton);
		RegisterColumn("prioritize_row", new_column2);
		settingsButton.onClick += OnSettingsButtonClicked;
		resetSettingsButton.onClick += OnResetSettingsClicked;
		toggleAdvancedModeButton.onClick += OnAdvancedModeToggleClicked;
		toggleAdvancedModeButton.fgImage.gameObject.SetActive(Game.Instance.advancedPersonalPriorities);
		RefreshEffectListeners();
	}

	private string HoverPersonalPriority(object widget_go_obj)
	{
		GameObject gameObject = widget_go_obj as GameObject;
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = GetWidgetColumn(gameObject) as PrioritizationGroupTableColumn;
		ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
		string text = null;
		TableRow widgetRow = GetWidgetRow(gameObject);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
		{
			string text2 = UI.JOBSSCREEN.HEADER_TOOLTIP.ToString();
			text2 = text2.Replace("{Job}", choreGroup.Name);
			string text3 = UI.JOBSSCREEN.HEADER_DETAILS_TOOLTIP.ToString();
			text3 = text3.Replace("{Description}", choreGroup.description);
			HashSet<string> hashSet = new HashSet<string>();
			foreach (ChoreType choreType in choreGroup.choreTypes)
			{
				hashSet.Add(choreType.Name);
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (string item in hashSet)
			{
				stringBuilder.Append(item);
				if (num < hashSet.Count - 1)
				{
					stringBuilder.Append(", ");
				}
				num++;
			}
			text3 = text3.Replace("{ChoreList}", stringBuilder.ToString());
			return text2.Replace("{Details}", text3);
		}
		case TableRow.RowType.Default:
			text = UI.JOBSSCREEN.NEW_MINION_ITEM_TOOLTIP.ToString();
			break;
		case TableRow.RowType.Minion:
		case TableRow.RowType.StoredMinon:
			text = UI.JOBSSCREEN.ITEM_TOOLTIP.ToString();
			text = text.Replace("{Name}", widgetRow.name);
			break;
		}
		ToolTip componentInChildren = gameObject.GetComponentInChildren<ToolTip>();
		IAssignableIdentity identity = widgetRow.GetIdentity();
		MinionIdentity minionIdentity = identity as MinionIdentity;
		if ((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null)
		{
			IPersonalPriorityManager priorityManager = GetPriorityManager(widgetRow);
			int personalPriority = priorityManager.GetPersonalPriority(choreGroup);
			string newValue = GetPriorityStr(personalPriority);
			string priorityValue = GetPriorityValue(personalPriority);
			if (priorityManager.IsChoreGroupDisabled(choreGroup))
			{
				Trait trait = null;
				Traits component = minionIdentity.GetComponent<Traits>();
				foreach (Trait trait2 in component.TraitList)
				{
					if (trait2.disabledChoreGroups != null)
					{
						ChoreGroup[] disabledChoreGroups = trait2.disabledChoreGroups;
						foreach (ChoreGroup choreGroup2 in disabledChoreGroups)
						{
							if (choreGroup2.IdHash == choreGroup.IdHash)
							{
								trait = trait2;
								break;
							}
						}
						if (trait != null)
						{
							break;
						}
					}
				}
				text = UI.JOBSSCREEN.TRAIT_DISABLED.ToString();
				text = text.Replace("{Name}", minionIdentity.GetProperName());
				text = text.Replace("{Job}", choreGroup.Name);
				text = text.Replace("{Trait}", trait.Name);
				componentInChildren.ClearMultiStringTooltip();
				componentInChildren.AddMultiStringTooltip(text, null);
			}
			else
			{
				text = text.Replace("{Job}", choreGroup.Name);
				text = text.Replace("{Priority}", newValue);
				text = text.Replace("{PriorityValue}", priorityValue);
				componentInChildren.ClearMultiStringTooltip();
				componentInChildren.AddMultiStringTooltip(text, null);
				if ((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null)
				{
					text = "\n" + UI.JOBSSCREEN.MINION_SKILL_TOOLTIP.ToString();
					text = text.Replace("{Name}", minionIdentity.GetProperName());
					text = text.Replace("{Attribute}", choreGroup.attribute.Name);
					AttributeInstance attributeInstance = minionIdentity.GetAttributes().Get(choreGroup.attribute);
					float totalValue = attributeInstance.GetTotalValue();
					TextStyleSetting tooltipTextStyle_Ability = TooltipTextStyle_Ability;
					text += GameUtil.ColourizeString(tooltipTextStyle_Ability.textColor, totalValue.ToString());
					componentInChildren.AddMultiStringTooltip(text, null);
				}
				componentInChildren.AddMultiStringTooltip(UI.HORIZONTAL_RULE + "\n" + GetUsageString(), null);
			}
		}
		else if ((UnityEngine.Object)(identity as StoredMinionIdentity) != (UnityEngine.Object)null)
		{
			componentInChildren.AddMultiStringTooltip(string.Format(UI.JOBSSCREEN.CANNOT_ADJUST_PRIORITY, identity.GetProperName(), (identity as StoredMinionIdentity).GetStorageReason()), null);
		}
		return string.Empty;
	}

	private string HoverChangeColumnPriorityButton(object widget_go_obj)
	{
		GameObject widget_go = widget_go_obj as GameObject;
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn;
		ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
		LocString hEADER_CHANGE_TOOLTIP = UI.JOBSSCREEN.HEADER_CHANGE_TOOLTIP;
		string text = hEADER_CHANGE_TOOLTIP.ToString();
		return text.Replace("{Job}", choreGroup.Name);
	}

	private string GetUsageString()
	{
		return GameUtil.ReplaceHotkeyString(UI.JOBSSCREEN.INCREASE_PRIORITY_TUTORIAL, Action.MouseLeft) + "\n" + GameUtil.ReplaceHotkeyString(UI.JOBSSCREEN.DECREASE_PRIORITY_TUTORIAL, Action.MouseRight);
	}

	private string HoverChangeRowPriorityButton(object widget_go_obj, int delta)
	{
		GameObject widget_go = widget_go_obj as GameObject;
		LocString locString = null;
		LocString locString2 = null;
		string text = null;
		TableRow widgetRow = GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			Debug.Assert(false);
			return null;
		case TableRow.RowType.Default:
			locString = UI.JOBSSCREEN.INCREASE_ROW_PRIORITY_NEW_MINION_TOOLTIP;
			locString2 = UI.JOBSSCREEN.DECREASE_ROW_PRIORITY_NEW_MINION_TOOLTIP;
			break;
		case TableRow.RowType.Minion:
			locString = UI.JOBSSCREEN.INCREASE_ROW_PRIORITY_MINION_TOOLTIP;
			locString2 = UI.JOBSSCREEN.DECREASE_ROW_PRIORITY_MINION_TOOLTIP;
			text = widgetRow.GetIdentity().GetProperName();
			break;
		case TableRow.RowType.StoredMinon:
		{
			StoredMinionIdentity storedMinionIdentity = widgetRow.GetIdentity() as StoredMinionIdentity;
			if ((UnityEngine.Object)storedMinionIdentity != (UnityEngine.Object)null)
			{
				return string.Format(UI.JOBSSCREEN.CANNOT_ADJUST_PRIORITY, storedMinionIdentity.GetProperName(), storedMinionIdentity.GetStorageReason());
			}
			break;
		}
		}
		LocString locString3 = (delta <= 0) ? locString2 : locString;
		string text2 = locString3.ToString();
		if (text != null)
		{
			text2 = text2.Replace("{Name}", text);
		}
		return text2;
	}

	private void OnSortClicked(object widget_go_obj)
	{
		GameObject widget_go = widget_go_obj as GameObject;
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn;
		ChoreGroup chore_group = prioritizationGroupTableColumn.userData as ChoreGroup;
		if (active_sort_column == prioritizationGroupTableColumn)
		{
			sort_is_reversed = !sort_is_reversed;
		}
		active_sort_column = prioritizationGroupTableColumn;
		active_sort_method = delegate(IAssignableIdentity a, IAssignableIdentity b)
		{
			MinionIdentity minionIdentity = a as MinionIdentity;
			MinionIdentity minionIdentity2 = b as MinionIdentity;
			if ((UnityEngine.Object)minionIdentity == (UnityEngine.Object)null && (UnityEngine.Object)minionIdentity2 == (UnityEngine.Object)null)
			{
				return 0;
			}
			if ((UnityEngine.Object)minionIdentity == (UnityEngine.Object)null)
			{
				return -1;
			}
			if ((UnityEngine.Object)minionIdentity2 == (UnityEngine.Object)null)
			{
				return 1;
			}
			ChoreConsumer component = minionIdentity.GetComponent<ChoreConsumer>();
			ChoreConsumer component2 = minionIdentity2.GetComponent<ChoreConsumer>();
			if (component.IsChoreGroupDisabled(chore_group))
			{
				return 1;
			}
			if (component2.IsChoreGroupDisabled(chore_group))
			{
				return -1;
			}
			int personalPriority = component.GetPersonalPriority(chore_group);
			int personalPriority2 = component2.GetPersonalPriority(chore_group);
			if (personalPriority == personalPriority2)
			{
				return minionIdentity.name.CompareTo(minionIdentity2.name);
			}
			return personalPriority2 - personalPriority;
		};
		SortRows();
	}

	private string OnSortHovered(object widget_go_obj)
	{
		GameObject widget_go = widget_go_obj as GameObject;
		PrioritizationGroupTableColumn prioritizationGroupTableColumn = GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn;
		ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
		return UI.JOBSSCREEN.SORT_TOOLTIP.ToString().Replace("{Job}", choreGroup.Name);
	}

	private IPersonalPriorityManager GetPriorityManager(TableRow row)
	{
		IPersonalPriorityManager result = null;
		switch (row.rowType)
		{
		case TableRow.RowType.Default:
			result = Immigration.Instance;
			break;
		case TableRow.RowType.Minion:
		{
			MinionIdentity minionIdentity = row.GetIdentity() as MinionIdentity;
			if ((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null)
			{
				result = minionIdentity.GetComponent<ChoreConsumer>();
			}
			break;
		}
		case TableRow.RowType.StoredMinon:
			result = (row.GetIdentity() as StoredMinionIdentity);
			break;
		}
		return result;
	}

	private LocString GetPriorityStr(int priority)
	{
		priority = Mathf.Clamp(priority, 0, 5);
		LocString result = null;
		foreach (PriorityInfo item in priorityInfo)
		{
			PriorityInfo current = item;
			if (current.priority == priority)
			{
				result = current.name;
			}
		}
		return result;
	}

	private string GetPriorityValue(int priority)
	{
		return (priority * 10).ToString();
	}

	private void LoadValue(IAssignableIdentity minion, GameObject widget_go)
	{
		if (!((UnityEngine.Object)widget_go == (UnityEngine.Object)null))
		{
			PrioritizationGroupTableColumn prioritizationGroupTableColumn = GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn;
			ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
			TableRow widgetRow = GetWidgetRow(widget_go);
			switch (widgetRow.rowType)
			{
			case TableRow.RowType.Header:
				InitializeHeader(choreGroup, widget_go);
				break;
			case TableRow.RowType.Default:
			case TableRow.RowType.Minion:
			case TableRow.RowType.StoredMinon:
			{
				IPersonalPriorityManager priorityManager = GetPriorityManager(widgetRow);
				bool flag = priorityManager.IsChoreGroupDisabled(choreGroup);
				HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
				KImage kImage = component.GetReference("FG") as KImage;
				kImage.raycastTarget = flag;
				ToolTip toolTip = component.GetReference("FGToolTip") as ToolTip;
				toolTip.enabled = flag;
				break;
			}
			}
			IPersonalPriorityManager priorityManager2 = GetPriorityManager(widgetRow);
			if (priorityManager2 != null)
			{
				UpdateWidget(widget_go, choreGroup, priorityManager2);
			}
		}
	}

	private PriorityInfo GetPriorityInfo(int priority)
	{
		PriorityInfo result = default(PriorityInfo);
		for (int i = 0; i < JobsTableScreen.priorityInfo.Count; i++)
		{
			PriorityInfo priorityInfo = JobsTableScreen.priorityInfo[i];
			if (priorityInfo.priority == priority)
			{
				return JobsTableScreen.priorityInfo[i];
			}
		}
		return result;
	}

	private void ChangePersonalPriority(object widget_go_obj, int delta)
	{
		GameObject widget_go = widget_go_obj as GameObject;
		if (widget_go_obj != null)
		{
			TableRow widgetRow = GetWidgetRow(widget_go);
			if (widgetRow.rowType == TableRow.RowType.Header)
			{
				Debug.Assert(false);
			}
			PrioritizationGroupTableColumn prioritizationGroupTableColumn = GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn;
			ChoreGroup chore_group = prioritizationGroupTableColumn.userData as ChoreGroup;
			IPersonalPriorityManager priorityManager = GetPriorityManager(widgetRow);
			ChangePersonalPriority(priorityManager, chore_group, delta, true);
			UpdateWidget(widget_go, chore_group, priorityManager);
		}
	}

	private void ChangeColumnPriority(object widget_go_obj, int new_priority)
	{
		GameObject widget_go = widget_go_obj as GameObject;
		if (widget_go_obj != null)
		{
			TableRow widgetRow = GetWidgetRow(widget_go);
			if (widgetRow.rowType != 0)
			{
				Debug.Assert(false);
			}
			PrioritizationGroupTableColumn prioritizationGroupTableColumn = GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn;
			ChoreGroup choreGroup = prioritizationGroupTableColumn.userData as ChoreGroup;
			foreach (TableRow row in rows)
			{
				IPersonalPriorityManager priorityManager = GetPriorityManager(row);
				if (priorityManager != null)
				{
					priorityManager.SetPersonalPriority(choreGroup, new_priority);
					GameObject widget = row.GetWidget(prioritizationGroupTableColumn);
					UpdateWidget(widget, choreGroup, priorityManager);
				}
			}
		}
	}

	private void ChangeRowPriority(object widget_go_obj, int delta)
	{
		GameObject widget_go = widget_go_obj as GameObject;
		if (widget_go_obj != null)
		{
			TableRow widgetRow = GetWidgetRow(widget_go);
			if (widgetRow.rowType == TableRow.RowType.Header)
			{
				Debug.Assert(false);
			}
			else
			{
				IPersonalPriorityManager priorityManager = GetPriorityManager(widgetRow);
				foreach (TableColumn value in columns.Values)
				{
					PrioritizationGroupTableColumn prioritizationGroupTableColumn = value as PrioritizationGroupTableColumn;
					if (prioritizationGroupTableColumn != null)
					{
						ChoreGroup chore_group = prioritizationGroupTableColumn.userData as ChoreGroup;
						GameObject widget = widgetRow.GetWidget(prioritizationGroupTableColumn);
						ChangePersonalPriority(priorityManager, chore_group, delta, false);
						UpdateWidget(widget, chore_group, priorityManager);
					}
				}
			}
		}
	}

	private void ChangePersonalPriority(IPersonalPriorityManager priority_mgr, ChoreGroup chore_group, int delta, bool wrap_around)
	{
		if (priority_mgr.IsChoreGroupDisabled(chore_group))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
		else
		{
			int personalPriority = priority_mgr.GetPersonalPriority(chore_group);
			personalPriority += delta;
			if (wrap_around)
			{
				personalPriority %= 6;
				if (personalPriority < 0)
				{
					personalPriority += 6;
				}
			}
			personalPriority = Mathf.Clamp(personalPriority, 0, 5);
			priority_mgr.SetPersonalPriority(chore_group, personalPriority);
			if (delta > 0)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
			}
			else
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
			}
		}
	}

	private void UpdateWidget(GameObject widget_go, ChoreGroup chore_group, IPersonalPriorityManager priority_mgr)
	{
		int fgIndex = 0;
		int value = 0;
		bool flag = priority_mgr.IsChoreGroupDisabled(chore_group);
		if (!flag)
		{
			value = priority_mgr.GetPersonalPriority(chore_group);
		}
		value = Mathf.Clamp(value, 0, 5);
		for (int i = 0; i < JobsTableScreen.priorityInfo.Count - 1; i++)
		{
			PriorityInfo priorityInfo = JobsTableScreen.priorityInfo[i];
			if (priorityInfo.priority == value)
			{
				fgIndex = i;
				break;
			}
		}
		OptionSelector component = widget_go.GetComponent<OptionSelector>();
		int num = priority_mgr?.GetAssociatedSkillLevel(chore_group) ?? 0;
		Color32 fillColour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 128);
		if (num > 0)
		{
			float num2 = (float)(num - skillLevelLow);
			num2 /= (float)(skillLevelHigh - skillLevelLow);
			fillColour = Color32.Lerp(skillOutlineColourLow, skillOutlineColourHigh, num2);
		}
		component.ConfigureItem(flag, new OptionSelector.DisplayOptionInfo
		{
			bgOptions = null,
			fgOptions = prioritySprites,
			bgIndex = 0,
			fgIndex = fgIndex,
			fillColour = fillColour
		});
		ToolTip componentInChildren = widget_go.transform.GetComponentInChildren<ToolTip>();
		if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
		{
			componentInChildren.toolTip = HoverPersonalPriority(widget_go);
			componentInChildren.forceRefresh = true;
		}
	}

	public void ToggleColumnSortWidgets(bool show)
	{
		foreach (KeyValuePair<string, TableColumn> column in columns)
		{
			if ((UnityEngine.Object)column.Value.column_sort_toggle != (UnityEngine.Object)null)
			{
				column.Value.column_sort_toggle.gameObject.SetActive(show);
			}
		}
	}

	public void Refresh(MinionResume minion_resume)
	{
		if (!((UnityEngine.Object)this == (UnityEngine.Object)null))
		{
			foreach (TableRow row in rows)
			{
				IAssignableIdentity identity = row.GetIdentity();
				if (!((UnityEngine.Object)(identity as MinionIdentity) == (UnityEngine.Object)null) && !((UnityEngine.Object)(identity as MinionIdentity).gameObject != (UnityEngine.Object)minion_resume.gameObject))
				{
					foreach (TableColumn value in columns.Values)
					{
						PrioritizationGroupTableColumn prioritizationGroupTableColumn = value as PrioritizationGroupTableColumn;
						if (prioritizationGroupTableColumn != null)
						{
							GameObject widget = row.GetWidget(prioritizationGroupTableColumn);
							UpdateWidget(widget, prioritizationGroupTableColumn.userData as ChoreGroup, (identity as MinionIdentity).GetComponent<ChoreConsumer>());
						}
					}
				}
			}
		}
	}

	protected override void RefreshRows()
	{
		base.RefreshRows();
		RefreshEffectListeners();
		if (dynamicRowSpacing)
		{
			SizeRows();
		}
	}

	private void SizeRows()
	{
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		for (int i = 0; i < header_row.transform.childCount; i++)
		{
			Transform child = header_row.transform.GetChild(i);
			LayoutElement component = child.GetComponent<LayoutElement>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && !component.ignoreLayout)
			{
				num3++;
				num += component.minWidth;
			}
			else
			{
				HorizontalOrVerticalLayoutGroup component2 = child.GetComponent<HorizontalOrVerticalLayoutGroup>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
				{
					Vector2 sizeDelta = component2.rectTransform().sizeDelta;
					float x = sizeDelta.x;
					num += x;
					num3++;
				}
			}
		}
		float width = base.gameObject.rectTransform().rect.width;
		num2 = 0f;
		HorizontalLayoutGroup component3 = header_row.GetComponent<HorizontalLayoutGroup>();
		component3.spacing = num2;
		component3.childAlignment = TextAnchor.MiddleLeft;
		foreach (TableRow row in rows)
		{
			row.transform.GetComponentInChildren<HorizontalLayoutGroup>().spacing = num2;
		}
	}

	private void RefreshEffectListeners()
	{
		for (int i = 0; i < EffectListeners.Count; i++)
		{
			GameObject key = EffectListeners[i].Key;
			SkillEventHandlerID value = EffectListeners[i].Value;
			key.Unsubscribe(value.level_up);
			GameObject key2 = EffectListeners[i].Key;
			SkillEventHandlerID value2 = EffectListeners[i].Value;
			key2.Unsubscribe(value2.effect_added);
			GameObject key3 = EffectListeners[i].Key;
			SkillEventHandlerID value3 = EffectListeners[i].Value;
			key3.Unsubscribe(value3.effect_removed);
			GameObject key4 = EffectListeners[i].Key;
			SkillEventHandlerID value4 = EffectListeners[i].Value;
			key4.Unsubscribe(value4.disease_added);
			GameObject key5 = EffectListeners[i].Key;
			SkillEventHandlerID value5 = EffectListeners[i].Value;
			key5.Unsubscribe(value5.effect_added);
		}
		EffectListeners.Clear();
		for (int j = 0; j < Components.LiveMinionIdentities.Count; j++)
		{
			SkillEventHandlerID skillEventHandlerID = default(SkillEventHandlerID);
			MinionIdentity id = Components.LiveMinionIdentities[j];
			Action<object> handler = delegate
			{
				MarkSingleMinionRowDirty(id);
			};
			skillEventHandlerID.level_up = Components.LiveMinionIdentities[j].gameObject.Subscribe(-110704193, handler);
			skillEventHandlerID.effect_added = Components.LiveMinionIdentities[j].gameObject.Subscribe(-1901442097, handler);
			skillEventHandlerID.effect_removed = Components.LiveMinionIdentities[j].gameObject.Subscribe(-1157678353, handler);
			skillEventHandlerID.disease_added = Components.LiveMinionIdentities[j].gameObject.Subscribe(1592732331, handler);
			skillEventHandlerID.disease_cured = Components.LiveMinionIdentities[j].gameObject.Subscribe(77635178, handler);
		}
		for (int k = 0; k < Components.LiveMinionIdentities.Count; k++)
		{
			MinionIdentity id2 = Components.LiveMinionIdentities[k];
			Components.LiveMinionIdentities[k].gameObject.Subscribe(540773776, delegate
			{
				MarkSingleMinionRowDirty(id2);
			});
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (dirty_single_minion_rows.Count != 0)
		{
			foreach (MinionIdentity dirty_single_minion_row in dirty_single_minion_rows)
			{
				if (!((UnityEngine.Object)dirty_single_minion_row == (UnityEngine.Object)null))
				{
					RefreshSingleMinionRow(dirty_single_minion_row);
				}
			}
			dirty_single_minion_rows.Clear();
		}
	}

	protected void MarkSingleMinionRowDirty(MinionIdentity id)
	{
		dirty_single_minion_rows.Add(id);
	}

	private void RefreshSingleMinionRow(IAssignableIdentity id)
	{
		foreach (KeyValuePair<string, TableColumn> column in columns)
		{
			if (column.Value != null && column.Value.on_load_action != null)
			{
				foreach (KeyValuePair<TableRow, GameObject> item in column.Value.widgets_by_row)
				{
					if (!((UnityEngine.Object)item.Value == (UnityEngine.Object)null) && item.Key.GetIdentity() == id)
					{
						column.Value.on_load_action(id, item.Value);
					}
				}
				column.Value.on_load_action(null, rows[0].GetWidget(column.Value));
			}
		}
	}

	protected override void OnCmpDisable()
	{
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		base.OnCmpDisable();
		foreach (TableColumn value in columns.Values)
		{
			foreach (TableRow row in rows)
			{
				GameObject widget = row.GetWidget(value);
				if (!((UnityEngine.Object)widget == (UnityEngine.Object)null))
				{
					GroupSelectorWidget[] componentsInChildren = widget.GetComponentsInChildren<GroupSelectorWidget>();
					if (componentsInChildren != null)
					{
						GroupSelectorWidget[] array = componentsInChildren;
						foreach (GroupSelectorWidget groupSelectorWidget in array)
						{
							groupSelectorWidget.CloseSubPanel();
						}
					}
					GroupSelectorHeaderWidget[] componentsInChildren2 = widget.GetComponentsInChildren<GroupSelectorHeaderWidget>();
					if (componentsInChildren2 != null)
					{
						GroupSelectorHeaderWidget[] array2 = componentsInChildren2;
						foreach (GroupSelectorHeaderWidget groupSelectorHeaderWidget in array2)
						{
							groupSelectorHeaderWidget.CloseSubPanel();
						}
					}
					SelectablePanel[] componentsInChildren3 = widget.GetComponentsInChildren<SelectablePanel>();
					if (componentsInChildren3 != null)
					{
						SelectablePanel[] array3 = componentsInChildren3;
						foreach (SelectablePanel selectablePanel in array3)
						{
							selectablePanel.gameObject.SetActive(false);
						}
					}
				}
			}
		}
		optionsPanel.gameObject.SetActive(false);
	}

	private void GetMouseHoverInfo(out bool is_hovering_screen, out bool is_hovering_button)
	{
		UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
		if ((UnityEngine.Object)current == (UnityEngine.Object)null)
		{
			is_hovering_button = false;
			is_hovering_screen = false;
		}
		else
		{
			List<RaycastResult> list = new List<RaycastResult>();
			PointerEventData pointerEventData = new PointerEventData(current);
			pointerEventData.position = KInputManager.GetMousePos();
			current.RaycastAll(pointerEventData, list);
			bool flag = false;
			bool flag2 = false;
			foreach (RaycastResult item in list)
			{
				if ((UnityEngine.Object)item.gameObject.GetComponent<OptionSelector>() != (UnityEngine.Object)null || ((UnityEngine.Object)item.gameObject.transform.parent != (UnityEngine.Object)null && (UnityEngine.Object)item.gameObject.transform.parent.GetComponent<OptionSelector>() != (UnityEngine.Object)null))
				{
					flag = true;
					flag2 = true;
					break;
				}
				if (HasParent(item.gameObject, base.gameObject))
				{
					flag2 = true;
				}
			}
			is_hovering_screen = flag2;
			is_hovering_button = flag;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		bool flag = false;
		if (e.IsAction(Action.MouseRight))
		{
			GetMouseHoverInfo(out bool is_hovering_screen, out bool _);
			if (is_hovering_screen)
			{
				flag = true;
				if (!e.Consumed)
				{
					e.TryConsume(Action.MouseRight);
				}
			}
		}
		if (!flag)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		bool flag = false;
		if (e.IsAction(Action.MouseRight))
		{
			GetMouseHoverInfo(out bool is_hovering_screen, out bool is_hovering_button);
			if (is_hovering_screen)
			{
				flag = true;
				if (!is_hovering_button)
				{
					UISounds.PlaySound(UISounds.Sound.Negative);
				}
				if (!e.Consumed)
				{
					e.TryConsume(Action.MouseRight);
				}
			}
		}
		if (!flag)
		{
			base.OnKeyUp(e);
		}
	}

	private bool HasParent(GameObject obj, GameObject parent)
	{
		bool result = false;
		Transform transform = parent.transform;
		Transform transform2 = obj.transform;
		while ((UnityEngine.Object)transform2 != (UnityEngine.Object)null)
		{
			if ((UnityEngine.Object)transform2 == (UnityEngine.Object)transform)
			{
				result = true;
				break;
			}
			transform2 = transform2.parent;
		}
		return result;
	}

	private void ConfigureNameLabel(IAssignableIdentity identity, GameObject widget_go)
	{
		on_load_name_label(identity, widget_go);
		if (identity != null)
		{
			string result = string.Empty;
			ToolTip component = widget_go.GetComponent<ToolTip>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				ToolTip toolTip = component;
				toolTip.OnToolTip = (Func<string>)Delegate.Combine(toolTip.OnToolTip, (Func<string>)delegate
				{
					MinionIdentity minionIdentity = identity as MinionIdentity;
					if ((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append("<b>" + UI.DETAILTABS.STATS.NAME + "</b>");
						foreach (AttributeInstance attribute in minionIdentity.GetAttributes())
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
								stringBuilder.Append("\n    • " + attribute.Name + ": " + text + attribute.GetTotalValue() + UIConstants.ColorSuffix);
							}
						}
						result = stringBuilder.ToString();
					}
					else if ((UnityEngine.Object)(identity as StoredMinionIdentity) != (UnityEngine.Object)null)
					{
						result = string.Format(UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (identity as StoredMinionIdentity).GetStorageReason(), identity.GetProperName());
					}
					return result;
				});
			}
		}
	}

	private void InitializeHeader(ChoreGroup chore_group, GameObject widget_go)
	{
		HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
		HierarchyReferences hierarchyReferences = component.GetReference("PrioritizationWidget") as HierarchyReferences;
		GameObject items_root = hierarchyReferences.GetReference("ItemPanel").gameObject;
		if (items_root.transform.childCount <= 0)
		{
			items_root.SetActive(false);
			LocText locText = component.GetReference("Label") as LocText;
			locText.text = chore_group.Name;
			KButton kButton = component.GetReference("PrioritizeButton") as KButton;
			Selectable selectable = items_root.GetComponent<Selectable>();
			kButton.onClick += delegate
			{
				selectable.Select();
				items_root.SetActive(true);
			};
			GameObject gameObject = hierarchyReferences.GetReference("ItemTemplate").gameObject;
			for (int num = 5; num >= 0; num--)
			{
				PriorityInfo priorityInfo = GetPriorityInfo(num);
				if (priorityInfo.name != null)
				{
					GameObject gameObject2 = Util.KInstantiateUI(gameObject, items_root, true);
					KButton component2 = gameObject2.GetComponent<KButton>();
					HierarchyReferences component3 = gameObject2.GetComponent<HierarchyReferences>();
					KImage kImage = component3.GetReference("Icon") as KImage;
					LocText locText2 = component3.GetReference("Label") as LocText;
					int new_priority = num;
					component2.onClick += delegate
					{
						ChangeColumnPriority(widget_go, new_priority);
						UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
					};
					kImage.sprite = priorityInfo.sprite;
					locText2.text = priorityInfo.name;
				}
			}
		}
	}

	private void OnSettingsButtonClicked()
	{
		optionsPanel.gameObject.SetActive(true);
		optionsPanel.GetComponent<Selectable>().Select();
	}

	private void OnResetSettingsClicked()
	{
		if (Game.Instance.advancedPersonalPriorities)
		{
			if ((UnityEngine.Object)Immigration.Instance != (UnityEngine.Object)null)
			{
				Immigration.Instance.ResetPersonalPriorities();
			}
			foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
			{
				if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
				{
					Immigration.Instance.ApplyDefaultPersonalPriorities(item.gameObject);
				}
			}
		}
		else
		{
			foreach (MinionIdentity item2 in Components.LiveMinionIdentities.Items)
			{
				if (!((UnityEngine.Object)item2 == (UnityEngine.Object)null))
				{
					ChoreConsumer component = item2.GetComponent<ChoreConsumer>();
					foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
					{
						component.SetPersonalPriority(resource, 3);
					}
				}
			}
		}
		MarkRowsDirty();
	}

	private void OnAdvancedModeToggleClicked()
	{
		Game.Instance.advancedPersonalPriorities = !Game.Instance.advancedPersonalPriorities;
		toggleAdvancedModeButton.fgImage.gameObject.SetActive(Game.Instance.advancedPersonalPriorities);
	}
}
