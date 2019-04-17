using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionTodoChoreEntry : KMonoBehaviour
{
	public Image icon;

	public LocText priorityLabel;

	public LocText label;

	public LocText subLabel;

	public LocText moreLabel;

	public List<Sprite> prioritySprites;

	[SerializeField]
	private ColorStyleSetting buttonColorSettingCurrent;

	[SerializeField]
	private ColorStyleSetting buttonColorSettingStandard;

	private Chore targetChore;

	private IStateMachineTarget lastChoreTarget;

	private PrioritySetting lastPrioritySetting;

	public void SetMoreAmount(int amount)
	{
		if (amount == 0)
		{
			moreLabel.gameObject.SetActive(false);
		}
		else
		{
			moreLabel.text = string.Format(UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TRUNCATED_CHORES, amount);
		}
	}

	public void Apply(Chore.Precondition.Context context)
	{
		ChoreConsumer consumer = context.consumerState.consumer;
		if (targetChore != context.chore || !object.ReferenceEquals(context.chore.target, lastChoreTarget) || !(context.chore.masterPriority == lastPrioritySetting))
		{
			targetChore = context.chore;
			lastChoreTarget = context.chore.target;
			lastPrioritySetting = context.chore.masterPriority;
			string choreName = GameUtil.GetChoreName(context.chore, context.data);
			string text = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
			string text2 = (text == null) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_TARGET : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_TARGET_AND_GROUP;
			text2 = text2.Replace("{Target}", (!((Object)context.chore.target.gameObject == (Object)consumer.gameObject)) ? context.chore.target.gameObject.GetProperName() : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.SELF_LABEL.text);
			if (text != null)
			{
				text2 = text2.Replace("{Groups}", text);
			}
			string text3 = (context.chore.masterPriority.priority_class != 0) ? string.Empty : context.chore.masterPriority.priority_value.ToString();
			Sprite sprite = (context.chore.masterPriority.priority_class != 0) ? null : prioritySprites[context.chore.masterPriority.priority_value - 1];
			label.SetText(choreName);
			subLabel.SetText(text2);
			priorityLabel.SetText(text3);
			icon.sprite = sprite;
			moreLabel.text = string.Empty;
			GetComponent<ToolTip>().SetSimpleTooltip(TooltipForChore(context, consumer));
			KButton componentInChildren = GetComponentInChildren<KButton>();
			componentInChildren.ClearOnClick();
			if ((Object)componentInChildren.bgImage != (Object)null)
			{
				componentInChildren.bgImage.colorStyleSetting = ((!((Object)context.chore.driver == (Object)consumer.choreDriver)) ? buttonColorSettingStandard : buttonColorSettingCurrent);
				componentInChildren.bgImage.ApplyColorStyleSetting();
			}
			GameObject gameObject = context.chore.target.gameObject;
			componentInChildren.ClearOnPointerEvents();
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
		}
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
		case PriorityScreen.PriorityClass.topPriority:
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
		text = text.Replace("{Description}", (!((Object)context.chore.driver == (Object)choreConsumer.choreDriver)) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_DESC_INACTIVE : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_DESC_ACTIVE);
		text = text.Replace("{IdleDescription}", (!((Object)context.chore.driver == (Object)choreConsumer.choreDriver)) ? UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLEDESC_INACTIVE : UI.UISIDESCREENS.MINIONTODOSIDESCREEN.TOOLTIP_IDLEDESC_ACTIVE);
		string newValue = GameUtil.ChoreGroupsForChoreType(context.chore.choreType);
		string name = context.chore.choreType.Name;
		if (context.chore.choreType.groups.Length > 0)
		{
			ChoreGroup choreGroup = context.chore.choreType.groups[0];
			for (int i = 1; i < context.chore.choreType.groups.Length; i++)
			{
				if (choreConsumer.GetPersonalPriority(choreGroup) < choreConsumer.GetPersonalPriority(context.chore.choreType.groups[i]))
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
}
