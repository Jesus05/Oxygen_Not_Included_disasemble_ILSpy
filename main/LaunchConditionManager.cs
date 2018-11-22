using System;
using System.Collections.Generic;
using UnityEngine;

public class LaunchConditionManager : KMonoBehaviour, ISim4000ms, ISim1000ms
{
	public HashedString triggerPort;

	public HashedString statusPort;

	private LaunchableRocket launchable;

	private Dictionary<RocketFlightCondition, Guid> conditionStatuses = new Dictionary<RocketFlightCondition, Guid>();

	public List<RocketModule> rocketModules
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		rocketModules = new List<RocketModule>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		launchable = GetComponent<LaunchableRocket>();
		FindModules();
		GetComponent<AttachableBuilding>().onAttachmentNetworkChanged = delegate
		{
			FindModules();
		};
		Subscribe(-1582839653, OnTagsChanged);
	}

	private void OnTagsChanged(object data)
	{
		foreach (RocketModule rocketModule in rocketModules)
		{
			rocketModule.OnConditionManagerTagsChanged(data);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void Sim1000ms(float dt)
	{
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
		if (spacecraftFromLaunchConditionManager != null)
		{
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(spacecraftFromLaunchConditionManager.id);
			LogicPorts component = base.gameObject.GetComponent<LogicPorts>();
			if (component.GetInputValue(triggerPort) == 1 && spacecraftDestination != null && spacecraftDestination.id != -1)
			{
				Launch(spacecraftDestination);
			}
		}
	}

	public void FindModules()
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			RocketModule component = item.GetComponent<RocketModule>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && (UnityEngine.Object)component.conditionManager == (UnityEngine.Object)null)
			{
				component.conditionManager = this;
				component.RegisterWithConditionManager();
			}
		}
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
		if (spacecraftFromLaunchConditionManager != null)
		{
			spacecraftFromLaunchConditionManager.moduleCount = attachedNetwork.Count;
		}
	}

	public void RegisterRocketModule(RocketModule module)
	{
		if (!rocketModules.Contains(module))
		{
			rocketModules.Add(module);
		}
	}

	public void UnregisterRocketModule(RocketModule module)
	{
		rocketModules.Remove(module);
	}

	public List<RocketLaunchCondition> GetLaunchConditionList()
	{
		List<RocketLaunchCondition> list = new List<RocketLaunchCondition>();
		foreach (RocketModule rocketModule in rocketModules)
		{
			foreach (RocketLaunchCondition launchCondition in rocketModule.launchConditions)
			{
				list.Add(launchCondition);
			}
		}
		return list;
	}

	public void Launch(SpaceDestination destination)
	{
		if (destination == null)
		{
			Debug.LogError("Null destination passed to launch", null);
		}
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
		if (spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Grounded && CheckReadyToLaunch() && CheckAbleToFly())
		{
			launchable.Trigger(-1056989049, null);
			SpacecraftManager.instance.SetSpacecraftDestination(this, destination);
			Spacecraft spacecraftFromLaunchConditionManager2 = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
			spacecraftFromLaunchConditionManager2.BeginMission(destination);
		}
	}

	public bool CheckReadyToLaunch()
	{
		foreach (RocketModule rocketModule in rocketModules)
		{
			foreach (RocketLaunchCondition launchCondition in rocketModule.launchConditions)
			{
				if (!launchCondition.EvaluateLaunchCondition())
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool CheckAbleToFly()
	{
		foreach (RocketModule rocketModule in rocketModules)
		{
			foreach (RocketFlightCondition flightCondition in rocketModule.flightConditions)
			{
				if (!flightCondition.EvaluateFlightCondition())
				{
					return false;
				}
			}
		}
		return true;
	}

	private void ClearFlightStatuses()
	{
		KSelectable component = GetComponent<KSelectable>();
		foreach (KeyValuePair<RocketFlightCondition, Guid> conditionStatus in conditionStatuses)
		{
			component.RemoveStatusItem(conditionStatus.Value, false);
		}
		conditionStatuses.Clear();
	}

	public void Sim4000ms(float dt)
	{
		bool flag = CheckReadyToLaunch();
		LogicPorts component = base.gameObject.GetComponent<LogicPorts>();
		if (flag)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
			if (spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Grounded || spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Launching)
			{
				component.SendSignal(statusPort, 1);
			}
			else
			{
				component.SendSignal(statusPort, 0);
			}
			KSelectable component2 = GetComponent<KSelectable>();
			foreach (RocketModule rocketModule in rocketModules)
			{
				foreach (RocketFlightCondition flightCondition in rocketModule.flightConditions)
				{
					if (!flightCondition.EvaluateFlightCondition())
					{
						if (!conditionStatuses.ContainsKey(flightCondition))
						{
							StatusItem failureStatusItem = flightCondition.GetFailureStatusItem();
							conditionStatuses[flightCondition] = component2.AddStatusItem(failureStatusItem, flightCondition);
						}
					}
					else if (conditionStatuses.ContainsKey(flightCondition))
					{
						component2.RemoveStatusItem(conditionStatuses[flightCondition], false);
						conditionStatuses.Remove(flightCondition);
					}
				}
			}
		}
		else
		{
			ClearFlightStatuses();
			component.SendSignal(statusPort, 0);
		}
	}
}
