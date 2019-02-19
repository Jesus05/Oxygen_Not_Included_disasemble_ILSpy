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
			if (personalPriority != other.personalPriority)
			{
				return other.personalPriority.CompareTo(personalPriority);
			}
			if (rank != other.rank)
			{
				return rank.CompareTo(other.rank);
			}
			if (consumer.GetProperName() != other.consumer.GetProperName())
			{
				return consumer.GetProperName().CompareTo(other.consumer.GetProperName());
			}
			return consumer.GetInstanceID().CompareTo(other.consumer.GetInstanceID());
		}
	}

	public GameObject choreGroupPrefab;

	public GameObject chorePrefab;

	public BuildingChoresPanelDupeRow dupePrefab;

	private GameObject detailsPanel;

	private DetailsPanelDrawer drawer;

	private HierarchyReferences choreGroup;

	private List<HierarchyReferences> choreEntries = new List<HierarchyReferences>();

	private int activeChoreEntries;

	private List<BuildingChoresPanelDupeRow> dupeEntries = new List<BuildingChoresPanelDupeRow>();

	private int activeDupeEntries;

	private List<DupeEntryData> DupeEntryDatas = new List<DupeEntryData>();

	public override bool IsValidForTarget(GameObject target)
	{
		KPrefabID component = target.GetComponent<KPrefabID>();
		return (UnityEngine.Object)component != (UnityEngine.Object)null && component.HasTag(GameTags.HasChores) && !component.HasTag(GameTags.Minion);
	}

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
		FetchChore fetchChore = chore as FetchChore;
		ListPool<Chore.Precondition.Context, BuildingChoresPanel>.PooledList pooledList = ListPool<Chore.Precondition.Context, BuildingChoresPanel>.Allocate();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			pooledList.Clear();
			ChoreConsumer component = item.GetComponent<ChoreConsumer>();
			Chore.Precondition.Context context = default(Chore.Precondition.Context);
			ChoreConsumer.PreconditionSnapshot lastPreconditionSnapshot = component.GetLastPreconditionSnapshot();
			if (lastPreconditionSnapshot.doFailedContextsNeedSorting)
			{
				lastPreconditionSnapshot.failedContexts.Sort();
				lastPreconditionSnapshot.doFailedContextsNeedSorting = false;
			}
			pooledList.AddRange(lastPreconditionSnapshot.failedContexts);
			pooledList.AddRange(lastPreconditionSnapshot.succeededContexts);
			int num = -1;
			int num2 = 0;
			for (int num3 = pooledList.Count - 1; num3 >= 0; num3--)
			{
				Chore.Precondition.Context context2 = pooledList[num3];
				if ((UnityEngine.Object)context2.chore.driver != (UnityEngine.Object)null)
				{
					Chore.Precondition.Context context3 = pooledList[num3];
					if ((UnityEngine.Object)context3.chore.driver != (UnityEngine.Object)component.choreDriver)
					{
						continue;
					}
				}
				bool flag = pooledList[num3].IsPotentialSuccess();
				if (flag)
				{
					num2++;
				}
				Chore.Precondition.Context context4 = pooledList[num3];
				FetchAreaChore fetchAreaChore = context4.chore as FetchAreaChore;
				Chore.Precondition.Context context5 = pooledList[num3];
				if (context5.chore == chore || (fetchChore != null && fetchAreaChore != null && fetchAreaChore.smi.SameDestination(fetchChore)))
				{
					num = ((!flag) ? 2147483647 : num2);
					context = pooledList[num3];
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
		pooledList.Recycle();
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
