using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionTodoSideScreen : SideScreenContent
{
	private bool useOffscreenIndicators = false;

	public GameObject taskEntryPrefab;

	public GameObject priorityGroupPrefab;

	public GameObject taskEntryContainer;

	public HierarchyReferences currentTask;

	public LocText currentScheduleBlockLabel;

	private List<Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>> priorityGroups = new List<Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>>();

	private List<HierarchyReferences> choreEntries = new List<HierarchyReferences>();

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
					tuple.third.GetReference<LocText>("Title").text = priorityInfo.name;
					tuple.third.GetReference<Image>("PriorityIcon").sprite = priorityInfo.sprite;
					priorityGroups.Add(tuple);
				}
			}
			else
			{
				Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences> tuple2 = new Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>(priority, 3, Util.KInstantiateUI<HierarchyReferences>(priorityGroupPrefab, taskEntryContainer, false));
				tuple2.third.name = "PriorityGroup_" + current.name;
				tuple2.third.gameObject.SetActive(true);
				tuple2.third.GetReference<LocText>("Title").text = current.name;
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
		HierarchyReferences hierarchyReferences = null;
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
							ApplyChoreEntry(currentTask, pooledList[num2]);
							hierarchyReferences = currentTask;
							choreB = pooledList[num2];
							num = 0;
							flag = true;
						}
						else if (!flag && activeChoreEntries != 0 && GameUtil.AreChoresUIMergeable(pooledList[num2], choreB))
						{
							num++;
							hierarchyReferences.GetReference<LocText>("MoreLabelText").text = num + " more";
						}
						else
						{
							ChoreConsumer obj = choreConsumer;
							Chore.Precondition.Context context5 = pooledList[num2];
							HierarchyReferences hierarchyReferences2 = PriorityGroupForPriority(obj, context5.chore);
							HierarchyReferences choreEntry = GetChoreEntry(hierarchyReferences2.GetReference<RectTransform>("EntriesContainer"));
							ApplyChoreEntry(choreEntry, pooledList[num2]);
							hierarchyReferences = choreEntry;
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

	private HierarchyReferences GetChoreEntry(RectTransform parent)
	{
		HierarchyReferences hierarchyReferences;
		if (activeChoreEntries >= choreEntries.Count - 1)
		{
			hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(taskEntryPrefab, parent.gameObject, false);
			choreEntries.Add(hierarchyReferences);
		}
		else
		{
			hierarchyReferences = choreEntries[activeChoreEntries];
			hierarchyReferences.transform.SetParent(parent);
			hierarchyReferences.transform.SetAsLastSibling();
		}
		activeChoreEntries++;
		hierarchyReferences.gameObject.SetActive(true);
		return hierarchyReferences;
	}

	private void ApplyChoreEntry(HierarchyReferences entry, Chore.Precondition.Context context)
	{
		string choreName = GameUtil.GetChoreName(context.chore, context.data);
		string text = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
		string text2 = (text == null) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_TARGET : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_TARGET_AND_GROUP;
		text2 = text2.Replace("{Target}", (!((UnityEngine.Object)context.chore.target.gameObject == (UnityEngine.Object)choreConsumer.gameObject)) ? context.chore.target.gameObject.GetProperName() : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.SELF_LABEL.text);
		if (text != null)
		{
			text2 = text2.Replace("{Groups}", text);
		}
		string text3 = (context.chore.masterPriority.priority_class != 0) ? "" : context.chore.masterPriority.priority_value.ToString();
		entry.GetReference<LocText>("LabelText").SetText(choreName);
		entry.GetReference<LocText>("SubLabelText").SetText(text2);
		entry.GetReference<LocText>("PriorityLabel").SetText(text3);
		entry.GetReference<LocText>("MoreLabelText").text = "";
		entry.GetComponent<ToolTip>().SetSimpleTooltip(TooltipForChore(context, choreConsumer));
		KButton componentInChildren = entry.GetComponentInChildren<KButton>();
		componentInChildren.ClearOnClick();
		if ((UnityEngine.Object)componentInChildren.bgImage != (UnityEngine.Object)null)
		{
			componentInChildren.bgImage.colorStyleSetting = ((!((UnityEngine.Object)context.chore.driver == (UnityEngine.Object)choreConsumer.choreDriver)) ? buttonColorSettingStandard : buttonColorSettingCurrent);
			componentInChildren.bgImage.ApplyColorStyleSetting();
		}
		GameObject choreTarget = context.chore.target.gameObject;
		componentInChildren.ClearOnPointerEvents();
		if (useOffscreenIndicators)
		{
			componentInChildren.onPointerEnter += delegate
			{
				if (context.chore != null && !context.chore.target.isNull)
				{
					if ((UnityEngine.Object)choreTarget.GetComponent<KBatchedAnimController>() == (UnityEngine.Object)null || (UnityEngine.Object)choreTarget.GetComponent<MinionIdentity>() != (UnityEngine.Object)null)
					{
						OffscreenIndicator.Instance.ActivateIndicator(choreTarget, DetailsScreen.Instance.target);
					}
					else
					{
						OffscreenIndicator.Instance.ActivateIndicator(choreTarget);
					}
				}
			};
			componentInChildren.onPointerExit += delegate
			{
				if (context.chore != null && !context.chore.target.isNull)
				{
					OffscreenIndicator.Instance.DeactivateIndicator(context.chore.target.gameObject);
				}
			};
		}
		componentInChildren.GetComponentInChildren<KButton>().onClick += delegate
		{
			if (context.chore != null && !context.chore.target.isNull)
			{
				Vector3 position = context.chore.target.gameObject.transform.position;
				float x = position.x;
				Vector3 position2 = context.chore.target.gameObject.transform.position;
				float y = position2.y + 1f;
				Vector3 position3 = CameraController.Instance.transform.position;
				Vector3 pos = new Vector3(x, y, position3.z);
				CameraController.Instance.SetTargetPos(pos, 10f, true);
			}
		};
		choreTargets.Add(choreTarget);
	}

	private static string TooltipForChore(Chore.Precondition.Context context, ChoreConsumer choreConsumer)
	{
		bool flag = context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.basic || context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.high;
		string text;
		switch (context.chore.masterPriority.priority_class)
		{
		case PriorityScreen.PriorityClass.idle:
			text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLE;
			break;
		case PriorityScreen.PriorityClass.personalNeeds:
			text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_PERSONAL;
			break;
		case PriorityScreen.PriorityClass.emergency:
			text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_EMERGENCY;
			break;
		case PriorityScreen.PriorityClass.compulsory:
			text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_COMPULSORY;
			break;
		default:
			text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_NORMAL;
			break;
		}
		float num = 0f;
		int num2 = (int)context.chore.masterPriority.priority_class * 100;
		num += (float)num2;
		int num3 = flag ? choreConsumer.GetPersonalPriority(context.chore.choreType) : 0;
		num += (float)(num3 * 10);
		int num4 = flag ? context.chore.masterPriority.priority_value : 0;
		num += (float)num4;
		float num5 = (float)context.priority / 10000f;
		num += num5;
		text = text.Replace("{Description}", (!((UnityEngine.Object)context.chore.driver == (UnityEngine.Object)choreConsumer.choreDriver)) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_DESC_INACTIVE : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_DESC_ACTIVE);
		text = text.Replace("{IdleDescription}", (!((UnityEngine.Object)context.chore.driver == (UnityEngine.Object)choreConsumer.choreDriver)) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLEDESC_INACTIVE : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLEDESC_ACTIVE);
		string newValue = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
		string name = context.chore.choreType.Name;
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
			name = choreGroup.Name;
		}
		text = text.Replace("{Name}", choreConsumer.name);
		text = text.Replace("{Errand}", GameUtil.GetChoreName(context.chore, context.data));
		text = text.Replace("{Groups}", newValue);
		text = text.Replace("{BestGroup}", name);
		text = text.Replace("{ClassPriority}", num2.ToString());
		string text2 = text;
		JobsTableScreen.PriorityInfo priorityInfo = JobsTableScreen.priorityInfo[num3];
		text = text2.Replace("{PersonalPriority}", priorityInfo.name.text);
		text = text.Replace("{PersonalPriorityValue}", (num3 * 10).ToString());
		text = text.Replace("{Building}", context.chore.gameObject.GetProperName());
		text = text.Replace("{BuildingPriority}", num4.ToString());
		text = text.Replace("{TypePriority}", num5.ToString());
		return text.Replace("{TotalPriority}", num.ToString());
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
