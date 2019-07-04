using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandModuleSideScreen : SideScreenContent
{
	private LaunchConditionManager target;

	public GameObject conditionListContainer;

	public GameObject prefabConditionLineItem;

	public MultiToggle destinationButton;

	public MultiToggle debugVictoryButton;

	private Dictionary<RocketLaunchCondition, GameObject> conditionTable = new Dictionary<RocketLaunchCondition, GameObject>();

	private SchedulerHandle updateHandle;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ScheduleUpdate();
		MultiToggle multiToggle = debugVictoryButton;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			SpaceDestination destination = SpacecraftManager.instance.destinations.Find((SpaceDestination match) => match.GetDestinationType() == Db.Get().SpaceDestinationTypes.Wormhole);
			target.Launch(destination);
		});
		debugVictoryButton.gameObject.SetActive(DebugHandler.InstantBuildMode && CheckHydrogenRocket());
	}

	private bool CheckHydrogenRocket()
	{
		RocketModule rocketModule = target.rocketModules.Find((RocketModule match) => match.GetComponent<RocketEngine>());
		if (!((UnityEngine.Object)rocketModule != (UnityEngine.Object)null))
		{
			return false;
		}
		return rocketModule.GetComponent<RocketEngine>().fuelTag == ElementLoader.FindElementByHash(SimHashes.LiquidHydrogen).tag;
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
		return (UnityEngine.Object)target.GetComponent<LaunchConditionManager>() != (UnityEngine.Object)null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if ((UnityEngine.Object)new_target == (UnityEngine.Object)null)
		{
			Debug.LogError("Invalid gameObject received");
		}
		else
		{
			target = new_target.GetComponent<LaunchConditionManager>();
			if ((UnityEngine.Object)target == (UnityEngine.Object)null)
			{
				Debug.LogError("The gameObject received does not contain a LaunchConditionManager component");
			}
			else
			{
				ClearConditions();
				ConfigureConditions();
				debugVictoryButton.gameObject.SetActive(DebugHandler.InstantBuildMode && CheckHydrogenRocket());
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
			if (item.GetParentCondition() != null && item.GetParentCondition().EvaluateLaunchCondition() == RocketLaunchCondition.LaunchStatus.Failure)
			{
				gameObject.SetActive(false);
			}
			else if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			bool flag2 = item.EvaluateLaunchCondition() != RocketLaunchCondition.LaunchStatus.Failure;
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
