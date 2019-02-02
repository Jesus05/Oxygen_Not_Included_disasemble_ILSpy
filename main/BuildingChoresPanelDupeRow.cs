using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChoresPanelDupeRow : KMonoBehaviour
{
	public Image icon;

	public LocText label;

	public ToolTip toolTip;

	private ChoreConsumer choreConsumer;

	public KButton button;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		button.onClick += OnClick;
	}

	public void Init(BuildingChoresPanel.DupeEntryData data)
	{
		choreConsumer = data.consumer;
		string str = (data.rank != 1) ? ("#" + data.rank.ToString()) : "Current Errand";
		label.text = data.consumer.name + " -- " + str;
		Image image = icon;
		JobsTableScreen.PriorityInfo priorityInfo = JobsTableScreen.priorityInfo[data.personalPriority];
		image.sprite = priorityInfo.sprite;
		toolTip.toolTip = TooltipForDupe(data.context, data.consumer, data.rank);
	}

	private void OnClick()
	{
		Vector3 pos = choreConsumer.gameObject.transform.GetPosition() + Vector3.up;
		CameraController.Instance.SetTargetPos(pos, 10f, true);
	}

	private static string TooltipForDupe(Chore.Precondition.Context context, ChoreConsumer choreConsumer, int rank)
	{
		string text = UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP;
		float num = 0f;
		int personalPriority = choreConsumer.GetPersonalPriority(context.chore.choreType);
		num += (float)(personalPriority * 10);
		int priority_value = context.chore.masterPriority.priority_value;
		num += (float)priority_value;
		float num2 = (float)context.priority / 10000f;
		num += num2;
		text = text.Replace("{Description}", (!((Object)context.chore.driver == (Object)choreConsumer.choreDriver)) ? UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_DESC_INACTIVE : UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_DESC_ACTIVE);
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
