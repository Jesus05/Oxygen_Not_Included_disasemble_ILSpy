using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleScreenEntry : KMonoBehaviour
{
	[SerializeField]
	private ScheduleBlockButton blockButtonPrefab;

	[SerializeField]
	private ScheduleBlockPainter blockButtonContainer;

	[SerializeField]
	private ScheduleMinionWidget minionWidgetPrefab;

	[SerializeField]
	private GameObject minionWidgetContainer;

	private ScheduleMinionWidget blankMinionWidget;

	[SerializeField]
	private EditableTitleBar title;

	[SerializeField]
	private LocText alarmField;

	[SerializeField]
	private KButton deleteButton;

	[SerializeField]
	private KButton optionsButton;

	[SerializeField]
	private DialogPanel optionsPanel;

	[SerializeField]
	private LocText noteEntryLeft;

	[SerializeField]
	private LocText noteEntryRight;

	private List<ScheduleBlockButton> blockButtons;

	private List<ScheduleMinionWidget> minionWidgets;

	private Dictionary<string, int> blockTypeCounts = new Dictionary<string, int>();

	public Schedule schedule
	{
		get;
		private set;
	}

	public void Setup(Schedule schedule, Dictionary<string, ColorStyleSetting> paintStyles, Action<ScheduleScreenEntry, float> onPaintDragged)
	{
		this.schedule = schedule;
		base.gameObject.name = "Schedule_" + schedule.name;
		title.SetTitle(schedule.name);
		title.OnNameChanged += OnNameChanged;
		blockButtonContainer.Setup(delegate(float f)
		{
			onPaintDragged(this, f);
		});
		int num = 0;
		blockButtons = new List<ScheduleBlockButton>();
		foreach (ScheduleBlock block in schedule.GetBlocks())
		{
			ScheduleBlockButton scheduleBlockButton = Util.KInstantiateUI<ScheduleBlockButton>(blockButtonPrefab.gameObject, blockButtonContainer.gameObject, true);
			scheduleBlockButton.Setup(num++, paintStyles);
			scheduleBlockButton.SetBlockTypes(block.allowed_types);
			blockButtons.Add(scheduleBlockButton);
		}
		minionWidgets = new List<ScheduleMinionWidget>();
		blankMinionWidget = Util.KInstantiateUI<ScheduleMinionWidget>(minionWidgetPrefab.gameObject, minionWidgetContainer, false);
		blankMinionWidget.SetupBlank(schedule);
		RebuildMinionWidgets();
		RefreshNotes();
		RefreshAlarmButton();
		optionsButton.onClick += OnOptionsClicked;
		HierarchyReferences component = optionsPanel.GetComponent<HierarchyReferences>();
		MultiToggle reference = component.GetReference<MultiToggle>("AlarmButton");
		reference.onClick = (System.Action)Delegate.Combine(reference.onClick, new System.Action(OnAlarmClicked));
		component.GetReference<KButton>("ResetButton").onClick += OnResetClicked;
		deleteButton.onClick += OnDeleteClicked;
		schedule.onChanged = (Action<Schedule>)Delegate.Combine(schedule.onChanged, new Action<Schedule>(OnScheduleChanged));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.schedule != null)
		{
			Schedule schedule = this.schedule;
			schedule.onChanged = (Action<Schedule>)Delegate.Remove(schedule.onChanged, new Action<Schedule>(OnScheduleChanged));
		}
	}

	public GameObject GetNameInputField()
	{
		return title.inputField.gameObject;
	}

	private void RebuildMinionWidgets()
	{
		if (MinionWidgetsNeedRebuild())
		{
			foreach (ScheduleMinionWidget minionWidget in minionWidgets)
			{
				Util.KDestroyGameObject(minionWidget);
			}
			minionWidgets.Clear();
			foreach (Ref<Schedulable> item in schedule.GetAssigned())
			{
				ScheduleMinionWidget scheduleMinionWidget = Util.KInstantiateUI<ScheduleMinionWidget>(minionWidgetPrefab.gameObject, minionWidgetContainer, true);
				scheduleMinionWidget.Setup(item.Get());
				minionWidgets.Add(scheduleMinionWidget);
			}
			if (Components.LiveMinionIdentities.Count > schedule.GetAssigned().Count)
			{
				blankMinionWidget.transform.SetAsLastSibling();
				blankMinionWidget.gameObject.SetActive(true);
			}
			else
			{
				blankMinionWidget.gameObject.SetActive(false);
			}
		}
	}

	private bool MinionWidgetsNeedRebuild()
	{
		List<Ref<Schedulable>> assigned = schedule.GetAssigned();
		if (assigned.Count != minionWidgets.Count)
		{
			return true;
		}
		if (assigned.Count != Components.LiveMinionIdentities.Count != blankMinionWidget.gameObject.activeSelf)
		{
			return true;
		}
		for (int i = 0; i < assigned.Count; i++)
		{
			if ((UnityEngine.Object)assigned[i].Get() != (UnityEngine.Object)minionWidgets[i].schedulable)
			{
				return true;
			}
		}
		return false;
	}

	private void OnNameChanged(string newName)
	{
		schedule.name = newName;
		base.gameObject.name = "Schedule_" + schedule.name;
	}

	private void OnOptionsClicked()
	{
		optionsPanel.gameObject.SetActive(!optionsPanel.gameObject.activeSelf);
		optionsPanel.GetComponent<Selectable>().Select();
	}

	private void OnAlarmClicked()
	{
		schedule.alarmActivated = !schedule.alarmActivated;
		RefreshAlarmButton();
	}

	private void RefreshAlarmButton()
	{
		MultiToggle reference = optionsPanel.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("AlarmButton");
		reference.ChangeState(schedule.alarmActivated ? 1 : 0);
		ToolTip component = reference.GetComponent<ToolTip>();
		component.SetSimpleTooltip((!schedule.alarmActivated) ? UI.SCHEDULESCREEN.ALARM_BUTTON_OFF_TOOLTIP : UI.SCHEDULESCREEN.ALARM_BUTTON_ON_TOOLTIP);
		ToolTipScreen.Instance.MarkTooltipDirty(component);
		alarmField.text = ((!schedule.alarmActivated) ? UI.SCHEDULESCREEN.ALARM_TITLE_DISABLED : UI.SCHEDULESCREEN.ALARM_TITLE_ENABLED);
	}

	private void OnResetClicked()
	{
		schedule.SetBlocksToGroupDefaults(Db.Get().ScheduleGroups.allGroups);
	}

	private void OnDeleteClicked()
	{
		ScheduleManager.Instance.DeleteSchedule(schedule);
	}

	private void OnScheduleChanged(Schedule changedSchedule)
	{
		foreach (ScheduleBlockButton blockButton in blockButtons)
		{
			blockButton.SetBlockTypes(changedSchedule.GetBlock(blockButton.idx).allowed_types);
		}
		RefreshNotes();
		RebuildMinionWidgets();
	}

	private void RefreshNotes()
	{
		blockTypeCounts.Clear();
		foreach (ScheduleBlockType resource in Db.Get().ScheduleBlockTypes.resources)
		{
			blockTypeCounts[resource.Id] = 0;
		}
		foreach (ScheduleBlock block in schedule.GetBlocks())
		{
			foreach (ScheduleBlockType allowed_type in block.allowed_types)
			{
				Dictionary<string, int> dictionary;
				string id;
				(dictionary = blockTypeCounts)[id = allowed_type.Id] = dictionary[id] + 1;
			}
		}
		ToolTip component = noteEntryRight.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		int num = 0;
		foreach (KeyValuePair<string, int> blockTypeCount in blockTypeCounts)
		{
			if (blockTypeCount.Value == 0)
			{
				num++;
				component.AddMultiStringTooltip(string.Format(UI.SCHEDULEGROUPS.NOTIME, Db.Get().ScheduleBlockTypes.Get(blockTypeCount.Key).Name), null);
			}
		}
		if (num > 0)
		{
			noteEntryRight.text = string.Format(UI.SCHEDULEGROUPS.MISSINGBLOCKS, num);
		}
		else
		{
			noteEntryRight.text = string.Empty;
		}
		int numBlocks = blockTypeCounts[Db.Get().ScheduleBlockTypes.Recreation.Id];
		string breakBonus = QualityOfLifeNeed.GetBreakBonus(numBlocks);
		if (breakBonus != null)
		{
			Effect effect = Db.Get().effects.Get(breakBonus);
			if (effect != null)
			{
				foreach (AttributeModifier selfModifier in effect.SelfModifiers)
				{
					if (selfModifier.AttributeId == Db.Get().Attributes.QualityOfLife.Id)
					{
						noteEntryLeft.text = string.Format(UI.SCHEDULESCREEN.DOWNTIME_MORALE, selfModifier.GetFormattedString(null, false));
						noteEntryLeft.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.SCHEDULESCREEN.SCHEDULE_DOWNTIME_MORALE, selfModifier.GetFormattedString(null, false)));
					}
				}
			}
		}
	}
}
