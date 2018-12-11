using STRINGS;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RocketModule : KMonoBehaviour
{
	protected bool isSuspended;

	public LaunchConditionManager conditionManager;

	public List<RocketLaunchCondition> launchConditions = new List<RocketLaunchCondition>();

	public List<RocketFlightCondition> flightConditions = new List<RocketFlightCondition>();

	protected string parentRocketName = UI.STARMAP.DEFAULT_NAME;

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnLaunchDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> OnLandDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.OnLand(data);
	});

	private static readonly EventSystem.IntraObjectHandler<RocketModule> DEBUG_OnDestroyDelegate = new EventSystem.IntraObjectHandler<RocketModule>(delegate(RocketModule component, object data)
	{
		component.DEBUG_OnDestroy(data);
	});

	public RocketLaunchCondition AddLaunchCondition(RocketLaunchCondition condition)
	{
		if (!launchConditions.Contains(condition))
		{
			launchConditions.Add(condition);
		}
		return condition;
	}

	public RocketFlightCondition AddFlightCondition(RocketFlightCondition condition)
	{
		if (!flightConditions.Contains(condition))
		{
			flightConditions.Add(condition);
		}
		return condition;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		conditionManager = FindLaunchConditionManager();
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(conditionManager);
		if (spacecraftFromLaunchConditionManager != null)
		{
			SetParentRocketName(spacecraftFromLaunchConditionManager.GetRocketName());
		}
		RegisterWithConditionManager();
		KSelectable component = GetComponent<KSelectable>();
		if ((Object)component != (Object)null)
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.RocketName, this);
		}
		if ((Object)conditionManager != (Object)null && conditionManager.GetComponent<KPrefabID>().HasTag(GameTags.RocketNotOnGround))
		{
			OnLaunch(null);
		}
		Subscribe(-1056989049, OnLaunchDelegate);
		Subscribe(238242047, OnLandDelegate);
		Subscribe(1502190696, DEBUG_OnDestroyDelegate);
	}

	private void DEBUG_OnDestroy(object data)
	{
		if ((Object)conditionManager != (Object)null && !App.IsExiting && !KMonoBehaviour.isLoadingScene)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(conditionManager);
			conditionManager.DEBUG_TraceModuleDestruction(base.name, spacecraftFromLaunchConditionManager.state.ToString(), new StackTrace(true).ToString());
		}
	}

	public void OnConditionManagerTagsChanged(object data)
	{
		KPrefabID component = conditionManager.GetComponent<KPrefabID>();
		if (component.HasTag(GameTags.RocketNotOnGround))
		{
			OnLaunch(null);
		}
	}

	private void OnLaunch(object data)
	{
		KSelectable component = GetComponent<KSelectable>();
		component.IsSelectable = false;
		if ((Object)SelectTool.Instance.selected == (Object)component)
		{
			SelectTool.Instance.Select(null, false);
		}
		ConduitConsumer component2 = GetComponent<ConduitConsumer>();
		if ((bool)component2)
		{
			ConduitType conduitType = component2.conduitType;
			if (conduitType == ConduitType.Gas || conduitType == ConduitType.Liquid)
			{
				component2.consumptionRate = 0f;
			}
		}
		Deconstructable component3 = GetComponent<Deconstructable>();
		if ((Object)component3 != (Object)null)
		{
			component3.SetAllowDeconstruction(false);
		}
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			GameComps.StructureTemperatures.Disable(handle);
		}
	}

	private void OnLand(object data)
	{
		GetComponent<KSelectable>().IsSelectable = true;
		ConduitConsumer component = GetComponent<ConduitConsumer>();
		if ((bool)component)
		{
			switch (component.conduitType)
			{
			case ConduitType.Gas:
				GetComponent<ConduitConsumer>().consumptionRate = 1f;
				break;
			case ConduitType.Liquid:
				GetComponent<ConduitConsumer>().consumptionRate = 10f;
				break;
			}
		}
		Deconstructable component2 = GetComponent<Deconstructable>();
		if ((Object)component2 != (Object)null)
		{
			component2.SetAllowDeconstruction(true);
		}
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			GameComps.StructureTemperatures.Enable(handle);
		}
	}

	public void RegisterWithConditionManager()
	{
		if ((Object)conditionManager != (Object)null)
		{
			conditionManager.RegisterRocketModule(this);
		}
		else
		{
			Debug.LogWarning("Module conditionManager is null", null);
		}
	}

	protected override void OnCleanUp()
	{
		if ((Object)conditionManager != (Object)null)
		{
			conditionManager.UnregisterRocketModule(this);
		}
		base.OnCleanUp();
	}

	public virtual void OnSuspend(object data)
	{
		isSuspended = true;
	}

	public bool IsSuspended()
	{
		return isSuspended;
	}

	public LaunchConditionManager FindLaunchConditionManager()
	{
		List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>());
		foreach (GameObject item in attachedNetwork)
		{
			LaunchConditionManager component = item.GetComponent<LaunchConditionManager>();
			if ((Object)component != (Object)null)
			{
				return component;
			}
		}
		return null;
	}

	public void SetParentRocketName(string newName)
	{
		parentRocketName = newName;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}

	public string GetParentRocketName()
	{
		return parentRocketName;
	}
}
