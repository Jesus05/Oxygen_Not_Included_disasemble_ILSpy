using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChoresPanel : TargetScreen
{
	private class DupeEntryData : IComparable<DupeEntryData>
	{
		public ChoreConsumer consumer;

		public Chore.Precondition.Context context;

		public int personalPriority;

		public int rank;

		public int CompareTo(DupeEntryData other)
		{
			return (personalPriority == other.personalPriority) ? rank.CompareTo(other.rank) : other.personalPriority.CompareTo(personalPriority);
		}
	}

	public GameObject choreGroupPrefab;

	public GameObject chorePrefab;

	public GameObject dupePrefab;

	private GameObject detailsPanel;

	private DetailsPanelDrawer drawer;

	private HierarchyReferences choreGroup;

	private List<HierarchyReferences> choreEntries = new List<HierarchyReferences>();

	private int activeChoreEntries = 0;

	private List<HierarchyReferences> dupeEntries = new List<HierarchyReferences>();

	private int activeDupeEntries = 0;

	private List<DupeEntryData> DupeEntryDatas = new List<DupeEntryData>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		choreGroup = Util.KInstantiateUI<HierarchyReferences>(choreGroupPrefab, base.gameObject, false);
		choreGroup.gameObject.SetActive(true);
	}

	private void Update()
	{
		Refresh();
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
	}

	private void Refresh()
	{
		RefreshDetails();
	}

	private void RefreshDetails()
	{
		List<Chore> chores = GlobalChoreProvider.Instance.chores;
		foreach (Chore item in chores)
		{
			if (!item.isNull && (UnityEngine.Object)item.gameObject == (UnityEngine.Object)selectedTarget)
			{
				AddChoreEntry(item);
			}
		}
		for (int i = activeDupeEntries; i < dupeEntries.Count; i++)
		{
			dupeEntries[i].gameObject.SetActive(false);
		}
		activeDupeEntries = 0;
		for (int j = activeChoreEntries; j < choreEntries.Count; j++)
		{
			choreEntries[j].gameObject.SetActive(false);
		}
		activeChoreEntries = 0;
	}

	private void AddChoreEntry(Chore chore)
	{
		HierarchyReferences choreEntry = GetChoreEntry(GameUtil.GetChoreName(chore, null), GameUtil.ChoreGroupsForChoreType(chore.choreType), choreGroup.GetReference<RectTransform>("EntriesContainer"));
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			ChoreConsumer component = item.GetComponent<ChoreConsumer>();
			int num = -1;
			Chore.Precondition.Context context = default(Chore.Precondition.Context);
			List<Chore.Precondition.Context> suceededPreconditionContexts = component.GetSuceededPreconditionContexts();
			for (int num2 = suceededPreconditionContexts.Count - 1; num2 >= 0; num2--)
			{
				Chore.Precondition.Context context2 = suceededPreconditionContexts[num2];
				if (context2.chore == chore)
				{
					num = suceededPreconditionContexts.Count - num2;
					context = suceededPreconditionContexts[num2];
					break;
				}
			}
			if (num >= 0)
			{
				DupeEntryDatas.Add(new DupeEntryData
				{
					consumer = component,
					context = context,
					personalPriority = component.GetPersonalPriority(chore.choreType),
					rank = num
				});
			}
		}
		DupeEntryDatas.Sort();
		foreach (DupeEntryData dupeEntryData in DupeEntryDatas)
		{
			string str = (dupeEntryData.rank != 1) ? ("#" + dupeEntryData.rank.ToString()) : "Current Errand";
			string label = dupeEntryData.consumer.name + " -- " + str;
			string tooltip = TooltipForDupe(dupeEntryData.context, dupeEntryData.consumer, dupeEntryData.rank);
			JobsTableScreen.PriorityInfo priorityInfo = JobsTableScreen.priorityInfo[dupeEntryData.personalPriority];
			GetDupeEntry(label, tooltip, priorityInfo.sprite, choreEntry.GetReference<RectTransform>("DupeContainer"));
		}
		DupeEntryDatas.Clear();
	}

	private HierarchyReferences GetChoreEntry(string label, string subLabel, RectTransform parent)
	{
		HierarchyReferences hierarchyReferences;
		if (activeChoreEntries >= choreEntries.Count)
		{
			hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(chorePrefab, parent.gameObject, false);
			choreEntries.Add(hierarchyReferences);
		}
		else
		{
			hierarchyReferences = choreEntries[activeChoreEntries];
			hierarchyReferences.transform.SetParent(parent);
			hierarchyReferences.transform.SetAsLastSibling();
		}
		activeChoreEntries++;
		hierarchyReferences.GetReference<LocText>("ChoreLabel").text = label;
		hierarchyReferences.GetReference<LocText>("ChoreSubLabel").text = subLabel;
		hierarchyReferences.gameObject.SetActive(true);
		return hierarchyReferences;
	}

	private HierarchyReferences GetDupeEntry(string label, string tooltip, Sprite icon, RectTransform parent)
	{
		HierarchyReferences hierarchyReferences;
		if (activeDupeEntries >= dupeEntries.Count)
		{
			hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(dupePrefab, parent.gameObject, false);
			dupeEntries.Add(hierarchyReferences);
		}
		else
		{
			hierarchyReferences = dupeEntries[activeDupeEntries];
			hierarchyReferences.transform.SetParent(parent);
			hierarchyReferences.transform.SetAsLastSibling();
		}
		activeDupeEntries++;
		hierarchyReferences.GetReference<LocText>("Label").text = label;
		hierarchyReferences.GetReference<Image>("Icon").sprite = icon;
		hierarchyReferences.GetReference<ToolTip>("ToolTip").toolTip = tooltip;
		hierarchyReferences.gameObject.SetActive(true);
		return hierarchyReferences;
	}

	private string TooltipForDupe(Chore.Precondition.Context context, ChoreConsumer choreConsumer, int rank)
	{
		string text = UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP;
		float num = 0f;
		int personalPriority = choreConsumer.GetPersonalPriority(context.chore.choreType);
		num += (float)(personalPriority * 10);
		int priority_value = context.chore.masterPriority.priority_value;
		num += (float)priority_value;
		float num2 = (float)context.priority / 10000f;
		num += num2;
		text = text.Replace("{Description}", (!((UnityEngine.Object)context.chore.driver == (UnityEngine.Object)choreConsumer.choreDriver)) ? UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_DESC_INACTIVE : UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_DESC_ACTIVE);
		string newValue = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
		string newValue2 = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_NA.text;
		if (context.chore.choreType.groups.Length > 0)
		{
			ChoreGroup choreGroup = context.chore.choreType.groups[0];
			for (int i = 1; i < context.chore.choreType.groups.Length; i++)
			{
				bool auto_assigned = true;
				if (choreConsumer.GetPersonalPriority(choreGroup, out auto_assigned) < choreConsumer.GetPersonalPriority(context.chore.choreType.groups[i], out auto_assigned))
				{
					choreGroup = context.chore.choreType.groups[i];
				}
			}
			newValue2 = choreGroup.Name;
		}
		text = text.Replace("{Name}", choreConsumer.name);
		text = text.Replace("{Rank}", rank.ToString());
		text = text.Replace("{Errand}", GameUtil.GetChoreName(context.chore, context.data));
		text = text.Replace("{Groups}", newValue);
		text = text.Replace("{BestGroup}", newValue2);
		string text2 = text;
		JobsTableScreen.PriorityInfo priorityInfo = JobsTableScreen.priorityInfo[personalPriority];
		text = text2.Replace("{PersonalPriority}", priorityInfo.name.text);
		text = text.Replace("{PersonalPriorityValue}", (personalPriority * 10).ToString());
		text = text.Replace("{Building}", context.chore.gameObject.GetProperName());
		text = text.Replace("{BuildingPriority}", priority_value.ToString());
		text = text.Replace("{TypePriority}", num2.ToString());
		return text.Replace("{TotalPriority}", num.ToString());
	}
}
