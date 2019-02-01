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

	public LocText currentScheduleBlockLabel;

	private List<Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>> priorityGroups = new List<Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>>();

	private List<GameObject> activeChoreLabels = new List<GameObject>();

	private List<GameObject> choreTargets = new List<GameObject>();

	private SchedulerHandle refreshHandle;

	private ChoreConsumer choreConsumer;

	[SerializeField]
	private ColorStyleSetting buttonColorSettingCurrent;

	[SerializeField]
	private ColorStyleSetting buttonColorSettingStandard;

	private static List<JobsTableScreen.PriorityInfo> _priorityInfo;

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
		List<Chore.Precondition.Context> suceededPreconditionContexts = choreConsumer.GetSuceededPreconditionContexts();
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
		int num2 = 0;
		for (int num3 = suceededPreconditionContexts.Count - 1; num3 >= 0; num3--)
		{
			Chore.Precondition.Context context2 = suceededPreconditionContexts[num3];
			if (context2.chore != null)
			{
				Chore.Precondition.Context context3 = suceededPreconditionContexts[num3];
				if (!context3.chore.target.isNull)
				{
					Chore.Precondition.Context context4 = suceededPreconditionContexts[num3];
					if (!((UnityEngine.Object)context4.chore.target.gameObject == (UnityEngine.Object)null))
					{
						if (num2 != 0)
						{
							if (GameUtil.AreChoresUIMergeable(suceededPreconditionContexts[num3], choreB))
							{
								num++;
								hierarchyReferences.GetReference<LocText>("MoreLabelText").text = num + " more";
								continue;
							}
							num = 0;
						}
						choreB = suceededPreconditionContexts[num3];
						num2++;
						ChoreConsumer obj = choreConsumer;
						Chore.Precondition.Context context5 = suceededPreconditionContexts[num3];
						HierarchyReferences hierarchyReferences2 = PriorityGroupForPriority(obj, context5.chore);
						HierarchyReferences hierarchyReferences3;
						if (num2 < activeChoreLabels.Count - 1)
						{
							hierarchyReferences3 = activeChoreLabels[num2 - 1].GetComponent<HierarchyReferences>();
							hierarchyReferences3.transform.SetParent(hierarchyReferences2.GetReference<RectTransform>("EntriesContainer"));
							hierarchyReferences3.transform.SetAsLastSibling();
						}
						else
						{
							hierarchyReferences3 = Util.KInstantiateUI<HierarchyReferences>(taskEntryPrefab, hierarchyReferences2.GetReference<RectTransform>("EntriesContainer").gameObject, true);
							activeChoreLabels.Add(hierarchyReferences3.gameObject);
						}
						hierarchyReferences3.gameObject.SetActive(true);
						hierarchyReferences = hierarchyReferences3;
						LocText reference = hierarchyReferences3.GetReference<LocText>("PriorityLabel");
						Chore.Precondition.Context context6 = suceededPreconditionContexts[num3];
						object text;
						if (context6.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.basic)
						{
							Chore.Precondition.Context context7 = suceededPreconditionContexts[num3];
							text = context7.chore.masterPriority.priority_value.ToString();
						}
						else
						{
							text = "";
						}
						reference.SetText((string)text);
						Chore.Precondition.Context context8 = suceededPreconditionContexts[num3];
						Chore chore = context8.chore;
						Chore.Precondition.Context context9 = suceededPreconditionContexts[num3];
						string choreName = GameUtil.GetChoreName(chore, context9.data);
						hierarchyReferences3.GetReference<LocText>("LabelText").SetText(choreName);
						string format = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_TARGET;
						Chore.Precondition.Context context10 = suceededPreconditionContexts[num3];
						object arg2;
						if ((UnityEngine.Object)context10.chore.target.gameObject == (UnityEngine.Object)DetailsScreen.Instance.target.gameObject)
						{
							arg2 = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.SELF_LABEL.text;
						}
						else
						{
							Chore.Precondition.Context context11 = suceededPreconditionContexts[num3];
							arg2 = context11.chore.target.gameObject.GetProperName();
						}
						string text2 = string.Format(format, arg2);
						Chore.Precondition.Context context12 = suceededPreconditionContexts[num3];
						if (context12.chore.choreType.groups != null)
						{
							Chore.Precondition.Context context13 = suceededPreconditionContexts[num3];
							if (context13.chore.choreType.groups.Length > 0)
							{
								string str = text2;
								string format2 = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_GROUP_TYPE;
								Chore.Precondition.Context context14 = suceededPreconditionContexts[num3];
								text2 = str + string.Format(format2, GameUtil.ChoreGroupsForChoreType(context14.chore.choreType));
							}
						}
						hierarchyReferences3.GetReference<LocText>("SubLabelText").SetText(text2);
						hierarchyReferences3.GetReference<LocText>("MoreLabelText").text = "";
						hierarchyReferences3.GetComponent<ToolTip>().SetSimpleTooltip(TooltipForChore(suceededPreconditionContexts[num3], choreConsumer));
						KButton componentInChildren = hierarchyReferences3.GetComponentInChildren<KButton>();
						componentInChildren.ClearOnClick();
						KImage bgImage = componentInChildren.bgImage;
						Chore.Precondition.Context context15 = suceededPreconditionContexts[num3];
						bgImage.colorStyleSetting = ((!((UnityEngine.Object)context15.chore.driver == (UnityEngine.Object)choreConsumer.choreDriver)) ? buttonColorSettingStandard : buttonColorSettingCurrent);
						Chore.Precondition.Context context = suceededPreconditionContexts[num3];
						GameObject choreTarget = context.chore.target.gameObject;
						componentInChildren.ClearOnPointerEvents();
						componentInChildren.onPointerEnter += delegate
						{
							if (context.chore != null && !context.chore.target.isNull && useOffscreenIndicators)
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
							if (useOffscreenIndicators && context.chore != null && !context.chore.target.isNull)
							{
								OffscreenIndicator.Instance.DeactivateIndicator(context.chore.target.gameObject);
							}
						};
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
				}
			}
		}
		for (int num4 = activeChoreLabels.Count - 1; num4 >= num2; num4--)
		{
			activeChoreLabels[num4].SetActive(false);
		}
		foreach (Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences> priorityGroup in priorityGroups)
		{
			RectTransform reference2 = priorityGroup.third.GetReference<RectTransform>("EntriesContainer");
			priorityGroup.third.gameObject.SetActive(reference2.childCount > 0);
		}
	}

	private string TooltipForChore(Chore.Precondition.Context context, ChoreConsumer choreConsumer)
	{
		bool flag = context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.basic || context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.high;
		string text = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP;
		float num = 0f;
		int num2 = flag ? choreConsumer.GetPersonalPriority(context.chore.choreType) : 0;
		num += (float)(num2 * 10);
		int num3 = flag ? context.chore.masterPriority.priority_value : 0;
		num += (float)num3;
		float num4 = (float)context.priority / 10000f;
		num += num4;
		text = text.Replace("{Description}", (!((UnityEngine.Object)context.chore.driver == (UnityEngine.Object)choreConsumer.choreDriver)) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_DESC_INACTIVE : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_DESC_ACTIVE);
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
		text = text.Replace("{Errand}", GameUtil.GetChoreName(context.chore, context.data));
		text = text.Replace("{Groups}", newValue);
		text = text.Replace("{BestGroup}", newValue2);
		string text2 = text;
		object text3;
		if (flag)
		{
			JobsTableScreen.PriorityInfo priorityInfo = JobsTableScreen.priorityInfo[num2];
			text3 = priorityInfo.name.text;
		}
		else
		{
			text3 = UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_NA.text;
		}
		text = text2.Replace("{PersonalPriority}", (string)text3);
		text = text.Replace("{PersonalPriorityValue}", (!flag) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_NA.text : (num2 * 10).ToString());
		text = text.Replace("{Building}", (!flag) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_NA.text : context.chore.gameObject.GetProperName());
		text = text.Replace("{BuildingPriority}", (!flag) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_NA.text : num3.ToString());
		text = text.Replace("{TypePriority}", num4.ToString());
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
