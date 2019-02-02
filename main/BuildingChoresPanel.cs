using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingChoresPanel : TargetScreen
{
	public class DupeEntryData : IComparable<DupeEntryData>
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

	public BuildingChoresPanelDupeRow dupePrefab;

	private GameObject detailsPanel;

	private DetailsPanelDrawer drawer;

	private HierarchyReferences choreGroup;

	private List<HierarchyReferences> choreEntries = new List<HierarchyReferences>();

	private int activeChoreEntries = 0;

	private List<BuildingChoresPanelDupeRow> dupeEntries = new List<BuildingChoresPanelDupeRow>();

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
			GetDupeEntry(dupeEntryData, choreEntry.GetReference<RectTransform>("DupeContainer"));
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

	private BuildingChoresPanelDupeRow GetDupeEntry(DupeEntryData data, RectTransform parent)
	{
		BuildingChoresPanelDupeRow buildingChoresPanelDupeRow;
		if (activeDupeEntries >= dupeEntries.Count)
		{
			buildingChoresPanelDupeRow = Util.KInstantiateUI<BuildingChoresPanelDupeRow>(dupePrefab.gameObject, parent.gameObject, false);
			dupeEntries.Add(buildingChoresPanelDupeRow);
		}
		else
		{
			buildingChoresPanelDupeRow = dupeEntries[activeDupeEntries];
			buildingChoresPanelDupeRow.transform.SetParent(parent);
			buildingChoresPanelDupeRow.transform.SetAsLastSibling();
		}
		activeDupeEntries++;
		buildingChoresPanelDupeRow.Init(data);
		buildingChoresPanelDupeRow.gameObject.SetActive(true);
		return buildingChoresPanelDupeRow;
	}
}
