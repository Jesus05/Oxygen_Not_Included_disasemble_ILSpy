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
		if (data.context.IsPotentialSuccess())
		{
			string str = (!((Object)data.context.chore.driver == (Object)data.consumer.choreDriver)) ? ("#" + data.rank.ToString()) : "Current Errand";
			label.text = data.consumer.name + " -- " + str;
		}
		else
		{
			Chore.PreconditionInstance preconditionInstance = data.context.chore.GetPreconditions()[data.context.failedPreconditionId];
			string text = preconditionInstance.description;
			bool test = text != null;
			Chore.PreconditionInstance preconditionInstance2 = data.context.chore.GetPreconditions()[data.context.failedPreconditionId];
			DebugUtil.Assert(test, "Chore requires description!", preconditionInstance2.id);
			if ((Object)data.context.chore.driver != (Object)null)
			{
				text = text.Replace("{Assignee}", data.context.chore.driver.GetProperName());
			}
			text = text.Replace("{Selected}", this.GetProperName());
			label.text = data.consumer.name + " -- " + text;
		}
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
		bool flag = context.IsPotentialSuccess();
		string text = (!flag) ? UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_FAILED : UI.DETAILTABS.BUILDING_CHORES.DUPE_TOOLTIP_SUCCEEDED;
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
		if (flag && context.chore.choreType.groups.Length > 0)
		{
			ChoreGroup choreGroup = context.chore.choreType.groups[0];
			for (int i = 1; i < context.chore.choreType.groups.Length; i++)
			{
				if (choreConsumer.GetPersonalPriority(choreGroup) < choreConsumer.GetPersonalPriority(context.chore.choreType.groups[i]))
				{
					choreGroup = context.chore.choreType.groups[i];
				}
			}
			newValue2 = choreGroup.Name;
		}
		text = text.Replace("{Name}", choreConsumer.name);
		text = text.Replace("{Errand}", GameUtil.GetChoreName(context.chore, context.data));
		if (flag)
		{
			text = text.Replace("{Rank}", rank.ToString());
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
		string text3 = text;
		Chore.PreconditionInstance preconditionInstance = context.chore.GetPreconditions()[context.failedPreconditionId];
		string id = preconditionInstance.id;
		Chore.PreconditionInstance preconditionInstance2 = context.chore.GetPreconditions()[context.failedPreconditionId];
		return text3.Replace("{FailedPrecondition}", id + "\n" + preconditionInstance2.description);
	}
}
