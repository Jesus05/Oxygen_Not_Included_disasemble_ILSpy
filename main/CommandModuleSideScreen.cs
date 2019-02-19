using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandModuleSideScreen : SideScreenContent
{
	private LaunchConditionManager target;

	public GameObject conditionListContainer;

	public GameObject prefabConditionLineItem;

	public MultiToggle destinationButton;

	private Dictionary<RocketLaunchCondition, GameObject> conditionTable = new Dictionary<RocketLaunchCondition, GameObject>();

	private SchedulerHandle updateHandle;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ScheduleUpdate();
	}

	private void ScheduleUpdate()
	{
		updateHandle = UIScheduler.Instance.Schedule("RefreshCommandModuleSideScreen", 1f, delegate
		{
			RefreshConditions();
			ScheduleUpdate();
		}, null, null);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<LaunchConditionManager>() != (Object)null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if ((Object)new_target == (Object)null)
		{
			Debug.LogError("Invalid gameObject received", null);
		}
		else
		{
			target = new_target.GetComponent<LaunchConditionManager>();
			if ((Object)target == (Object)null)
			{
				Debug.LogError("The gameObject received does not contain a LaunchConditionManager component", null);
			}
			else
			{
				ClearConditions();
				ConfigureConditions();
			}
		}
	}

	private void ClearConditions()
	{
		foreach (KeyValuePair<RocketLaunchCondition, GameObject> item in conditionTable)
		{
			Util.KDestroyGameObject(item.Value);
		}
		conditionTable.Clear();
	}

	private void ConfigureConditions()
	{
		foreach (RocketLaunchCondition launchCondition in target.GetLaunchConditionList())
		{
			GameObject value = Util.KInstantiateUI(prefabConditionLineItem, conditionListContainer, true);
			conditionTable.Add(launchCondition, value);
		}
		RefreshConditions();
	}

	public void RefreshConditions()
	{
		bool flag = false;
		List<RocketLaunchCondition> launchConditionList = target.GetLaunchConditionList();
		foreach (RocketLaunchCondition item in launchConditionList)
		{
			if (!conditionTable.ContainsKey(item))
			{
				flag = true;
				break;
			}
			GameObject gameObject = conditionTable[item];
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			if (item.GetParentCondition() != null && !item.GetParentCondition().EvaluateLaunchCondition())
			{
				gameObject.SetActive(false);
			}
			else if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			bool flag2 = item.EvaluateLaunchCondition();
			component.GetReference<LocText>("Label").text = item.GetLaunchStatusMessage(flag2);
			component.GetReference<LocText>("Label").color = ((!flag2) ? Color.red : Color.black);
			component.GetReference<Image>("Box").color = ((!flag2) ? Color.red : Color.black);
			component.GetReference<Image>("Check").gameObject.SetActive(flag2);
			gameObject.GetComponent<ToolTip>().SetSimpleTooltip(item.GetLaunchStatusTooltip(flag2));
		}
		foreach (KeyValuePair<RocketLaunchCondition, GameObject> item2 in conditionTable)
		{
			if (!launchConditionList.Contains(item2.Key))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			ClearConditions();
			ConfigureConditions();
		}
		destinationButton.onClick = delegate
		{
			ManagementMenu.Instance.ToggleStarmap();
		};
	}

	protected override void OnCleanUp()
	{
		updateHandle.ClearScheduler();
		base.OnCleanUp();
	}
}
