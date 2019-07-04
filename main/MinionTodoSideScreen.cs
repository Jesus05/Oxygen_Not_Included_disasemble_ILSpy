using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionTodoSideScreen : SideScreenContent
{
	private bool useOffscreenIndicators = false;

	public MinionTodoChoreEntry taskEntryPrefab;

	public GameObject priorityGroupPrefab;

	public GameObject taskEntryContainer;

	public MinionTodoChoreEntry currentTask;

	public LocText currentScheduleBlockLabel;

	private List<Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>> priorityGroups = new List<Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>>();

	private List<MinionTodoChoreEntry> choreEntries = new List<MinionTodoChoreEntry>();

	private List<GameObject> choreTargets = new List<GameObject>();

	private SchedulerHandle refreshHandle;

	private ChoreConsumer choreConsumer;

	[SerializeField]
	private ColorStyleSetting buttonColorSettingCurrent;

	[SerializeField]
	private ColorStyleSetting buttonColorSettingStandard;

	private static List<JobsTableScreen.PriorityInfo> _priorityInfo;

	private int activeChoreEntries = 0;

	public static List<JobsTableScreen.PriorityInfo> priorityInfo
	{
		get
		{
			if (_priorityInfo == null)
			{
				List<JobsTableScreen.PriorityInfo> list = new List<JobsTableScreen.PriorityInfo>();
				list.Add(new JobsTableScreen.PriorityInfo(4, Assets.GetSprite("ic_dupe"), UI.JOBSSCREEN.PRIORITY_CLASS.COMPULSORY));
				list.Add(new JobsTableScreen.PriorityInfo(3, Assets.GetSprite("notification_exclamation"), UI.JOBSSCREEN.PRIORITY_CLASS.EMERGENCY));
				list.Add(new JobsTableScreen.PriorityInfo(2, Assets.GetSprite("status_item_room_required"), UI.JOBSSCREEN.PRIORITY_CLASS.PERSONAL_NEEDS));
				list.Add(new JobsTableScreen.PriorityInfo(1, Assets.GetSprite("status_item_prioritized"), UI.JOBSSCREEN.PRIORITY_CLASS.HIGH));
				list.Add(new JobsTableScreen.PriorityInfo(0, null, UI.JOBSSCREEN.PRIORITY_CLASS.BASIC));
				list.Add(new JobsTableScreen.PriorityInfo(-1, Assets.GetSprite("icon_gear"), UI.JOBSSCREEN.PRIORITY_CLASS.IDLE));
				_priorityInfo = list;
			}
			return _priorityInfo;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		foreach (JobsTableScreen.PriorityInfo item in MinionTodoSideScreen.priorityInfo)
		{
			JobsTableScreen.PriorityInfo current = item;
			PriorityScreen.PriorityClass priority = (PriorityScreen.PriorityClass)current.priority;
			if (priority == PriorityScreen.PriorityClass.basic)
			{
				for (int num = 5; num >= 0; num--)
				{
					Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences> tuple = new Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>(priority, num, Util.KInstantiateUI<HierarchyReferences>(priorityGroupPrefab, taskEntryContainer, false));
					tuple.third.name = "PriorityGroup_" + (string)current.name + "_" + num;
					tuple.third.gameObject.SetActive(true);
					JobsTableScreen.PriorityInfo priorityInfo = JobsTableScreen.priorityInfo[num];
					tuple.third.GetReference<LocText>("Title").text = priorityInfo.name.text.ToUpper();
					tuple.third.GetReference<Image>("PriorityIcon").sprite = priorityInfo.sprite;
					priorityGroups.Add(tuple);
				}
			}
			else
			{
				Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences> tuple2 = new Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>(priority, 3, Util.KInstantiateUI<HierarchyReferences>(priorityGroupPrefab, taskEntryContainer, false));
				tuple2.third.name = "PriorityGroup_" + current.name;
				tuple2.third.gameObject.SetActive(true);
				tuple2.third.GetReference<LocText>("Title").text = current.name.text.ToUpper();
				tuple2.third.GetReference<Image>("PriorityIcon").sprite = current.sprite;
				priorityGroups.Add(tuple2);
			}
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (UnityEngine.Object)target.GetComponent<MinionIdentity>() != (UnityEngine.Object)null && !target.HasTag(GameTags.Dead);
	}

	public override void ClearTarget()
	{
		base.ClearTarget();
		refreshHandle.ClearScheduler();
	}

	public override void SetTarget(GameObject target)
	{
		refreshHandle.ClearScheduler();
		base.SetTarget(target);
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		PopulateElements(null);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		refreshHandle.ClearScheduler();
		if (!show)
		{
			if (useOffscreenIndicators)
			{
				foreach (GameObject choreTarget in choreTargets)
				{
					OffscreenIndicator.Instance.DeactivateIndicator(choreTarget);
				}
			}
		}
		else if (!((UnityEngine.Object)DetailsScreen.Instance.target == (UnityEngine.Object)null))
		{
			choreConsumer = DetailsScreen.Instance.target.GetComponent<ChoreConsumer>();
			PopulateElements(null);
		}
	}

	private void PopulateElements(object data = null)
	{
		refreshHandle.ClearScheduler();
		refreshHandle = UIScheduler.Instance.Schedule("RefreshToDoList", 0.1f, PopulateElements, null, null);
		ListPool<Chore.Precondition.Context, BuildingChoresPanel>.PooledList pooledList = ListPool<Chore.Precondition.Context, BuildingChoresPanel>.Allocate();
		ChoreConsumer.PreconditionSnapshot lastPreconditionSnapshot = choreConsumer.GetLastPreconditionSnapshot();
		if (lastPreconditionSnapshot.doFailedContextsNeedSorting)
		{
			lastPreconditionSnapshot.failedContexts.Sort();
			lastPreconditionSnapshot.doFailedContextsNeedSorting = false;
		}
		pooledList.AddRange(lastPreconditionSnapshot.failedContexts);
		pooledList.AddRange(lastPreconditionSnapshot.succeededContexts);
		Chore.Precondition.Context choreB = default(Chore.Precondition.Context);
		MinionTodoChoreEntry minionTodoChoreEntry = null;
		int num = 0;
		Schedulable component = DetailsScreen.Instance.target.GetComponent<Schedulable>();
		string arg = "";
		Schedule schedule = component.GetSchedule();
		if (schedule != null)
		{
			ScheduleBlock block = schedule.GetBlock(Schedule.GetBlockIdx());
			arg = block.name;
		}
		currentScheduleBlockLabel.SetText(string.Format(UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CURRENT_SCHEDULE_BLOCK, arg));
		choreTargets.Clear();
		bool flag = false;
		activeChoreEntries = 0;
		for (int num2 = pooledList.Count - 1; num2 >= 0; num2--)
		{
			Chore.Precondition.Context context = pooledList[num2];
			if (context.chore != null)
			{
				Chore.Precondition.Context context2 = pooledList[num2];
				if (!context2.chore.target.isNull)
				{
					Chore.Precondition.Context context3 = pooledList[num2];
					if (!((UnityEngine.Object)context3.chore.target.gameObject == (UnityEngine.Object)null) && pooledList[num2].IsPotentialSuccess())
					{
						Chore.Precondition.Context context4 = pooledList[num2];
						if ((UnityEngine.Object)context4.chore.driver == (UnityEngine.Object)choreConsumer.choreDriver)
						{
							currentTask.Apply(pooledList[num2]);
							minionTodoChoreEntry = currentTask;
							choreB = pooledList[num2];
							num = 0;
							flag = true;
						}
						else if (!flag && activeChoreEntries != 0 && GameUtil.AreChoresUIMergeable(pooledList[num2], choreB))
						{
							num++;
							minionTodoChoreEntry.SetMoreAmount(num);
						}
						else
						{
							ChoreConsumer obj = choreConsumer;
							Chore.Precondition.Context context5 = pooledList[num2];
							HierarchyReferences hierarchyReferences = PriorityGroupForPriority(obj, context5.chore);
							MinionTodoChoreEntry choreEntry = GetChoreEntry(hierarchyReferences.GetReference<RectTransform>("EntriesContainer"));
							choreEntry.Apply(pooledList[num2]);
							minionTodoChoreEntry = choreEntry;
							choreB = pooledList[num2];
							num = 0;
							flag = false;
						}
					}
				}
			}
		}
		pooledList.Recycle();
		for (int num3 = choreEntries.Count - 1; num3 >= activeChoreEntries; num3--)
		{
			choreEntries[num3].gameObject.SetActive(false);
		}
		foreach (Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences> priorityGroup in priorityGroups)
		{
			RectTransform reference = priorityGroup.third.GetReference<RectTransform>("EntriesContainer");
			priorityGroup.third.gameObject.SetActive(reference.childCount > 0);
		}
	}

	private MinionTodoChoreEntry GetChoreEntry(RectTransform parent)
	{
		MinionTodoChoreEntry minionTodoChoreEntry;
		if (activeChoreEntries >= choreEntries.Count - 1)
		{
			minionTodoChoreEntry = Util.KInstantiateUI<MinionTodoChoreEntry>(taskEntryPrefab.gameObject, parent.gameObject, false);
			choreEntries.Add(minionTodoChoreEntry);
		}
		else
		{
			minionTodoChoreEntry = choreEntries[activeChoreEntries];
			minionTodoChoreEntry.transform.SetParent(parent);
			minionTodoChoreEntry.transform.SetAsLastSibling();
		}
		activeChoreEntries++;
		minionTodoChoreEntry.gameObject.SetActive(true);
		return minionTodoChoreEntry;
	}

	private HierarchyReferences PriorityGroupForPriority(ChoreConsumer choreConsumer, Chore chore)
	{
		foreach (Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences> priorityGroup in priorityGroups)
		{
			if (priorityGroup.first == chore.masterPriority.priority_class)
			{
				if (chore.masterPriority.priority_class != 0)
				{
					return priorityGroup.third;
				}
				if (priorityGroup.second == choreConsumer.GetPersonalPriority(chore.choreType))
				{
					return priorityGroup.third;
				}
			}
		}
		return null;
	}

	private void Button_onPointerEnter()
	{
		throw new NotImplementedException();
	}
}
