using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RocketModule : KMonoBehaviour
{
	protected bool isSuspended;

	public LaunchConditionManager conditionManager;

	public List<RocketLaunchCondition> launchConditions = new List<RocketLaunchCondition>();

	public List<RocketFlightCondition> flightConditions = new List<RocketFlightCondition>();

	private string rocket_module_bg_base_string = "{0}{1}";

	private string rocket_module_bg_affix = "BG";

	private string rocket_module_bg_anim = "on";

	[SerializeField]
	private KAnimFile bgAnimFile = null;

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

	public void SetBGKAnim(KAnimFile anim_file)
	{
		bgAnimFile = anim_file;
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
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.RocketName, this);
		}
		if ((UnityEngine.Object)conditionManager != (UnityEngine.Object)null && conditionManager.GetComponent<KPrefabID>().HasTag(GameTags.RocketNotOnGround))
		{
			OnLaunch(null);
		}
		Subscribe(-1056989049, OnLaunchDelegate);
		Subscribe(238242047, OnLandDelegate);
		Subscribe(1502190696, DEBUG_OnDestroyDelegate);
		FixSorting();
		AttachableBuilding component2 = GetComponent<AttachableBuilding>();
		component2.onAttachmentNetworkChanged = (Action<AttachableBuilding>)Delegate.Combine(component2.onAttachmentNetworkChanged, new Action<AttachableBuilding>(OnAttachmentNetworkChanged));
		if ((UnityEngine.Object)bgAnimFile != (UnityEngine.Object)null)
		{
			AddBGGantry();
		}
	}

	public void FixSorting()
	{
		int num = 0;
		AttachableBuilding component = GetComponent<AttachableBuilding>();
		while ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			BuildingAttachPoint attachedTo = component.GetAttachedTo();
			if (!((UnityEngine.Object)attachedTo != (UnityEngine.Object)null))
			{
				break;
			}
			component = attachedTo.GetComponent<AttachableBuilding>();
			num++;
		}
		Vector3 localPosition = base.transform.GetLocalPosition();
		localPosition.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront) - (float)num * 0.01f;
		base.transform.SetLocalPosition(localPosition);
		KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
		component2.enabled = false;
		component2.enabled = true;
	}

	private void OnAttachmentNetworkChanged(AttachableBuilding ab)
	{
		FixSorting();
	}

	private void AddBGGantry()
	{
		KAnimControllerBase component = GetComponent<KAnimControllerBase>();
		GameObject gameObject = new GameObject();
		gameObject.name = string.Format(rocket_module_bg_base_string, base.name, rocket_module_bg_affix);
		gameObject.SetActive(false);
		Vector3 position = component.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.InteriorWall);
		gameObject.transform.SetPosition(position);
		gameObject.transform.parent = base.transform;
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.AnimFiles = new KAnimFile[1]
		{
			bgAnimFile
		};
		kBatchedAnimController.initialAnim = rocket_module_bg_anim;
		kBatchedAnimController.fgLayer = Grid.SceneLayer.NoLayer;
		kBatchedAnimController.initialMode = KAnim.PlayMode.Paused;
		kBatchedAnimController.FlipX = component.FlipX;
		kBatchedAnimController.FlipY = component.FlipY;
		gameObject.SetActive(true);
	}

	private void DEBUG_OnDestroy(object data)
	{
		if ((UnityEngine.Object)conditionManager != (UnityEngine.Object)null && !App.IsExiting && !KMonoBehaviour.isLoadingScene)
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
		if ((UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)component)
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
		if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
		{
			component3.SetAllowDeconstruction(false);
		}
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			GameComps.StructureTemperatures.Disable(handle);
		}
		ToggleComponent(typeof(ManualDeliveryKG), false);
		ToggleComponent(typeof(ElementConsumer), false);
		ToggleComponent(typeof(ElementConverter), false);
		ToggleComponent(typeof(ConduitDispenser), false);
		ToggleComponent(typeof(SolidConduitDispenser), false);
		ToggleComponent(typeof(EnergyConsumer), false);
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
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
		{
			component2.SetAllowDeconstruction(true);
		}
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			GameComps.StructureTemperatures.Enable(handle);
		}
		ToggleComponent(typeof(ManualDeliveryKG), true);
		ToggleComponent(typeof(ElementConsumer), true);
		ToggleComponent(typeof(ElementConverter), true);
		ToggleComponent(typeof(ConduitDispenser), true);
		ToggleComponent(typeof(SolidConduitDispenser), true);
		ToggleComponent(typeof(EnergyConsumer), true);
	}

	private void ToggleComponent(Type cmpType, bool enabled)
	{
		MonoBehaviour monoBehaviour = (MonoBehaviour)GetComponent(cmpType);
		if ((UnityEngine.Object)monoBehaviour != (UnityEngine.Object)null)
		{
			monoBehaviour.enabled = enabled;
		}
	}

	public void RegisterWithConditionManager()
	{
		if ((UnityEngine.Object)conditionManager != (UnityEngine.Object)null)
		{
			conditionManager.RegisterRocketModule(this);
		}
		else
		{
			Debug.LogWarning("Module conditionManager is null");
		}
	}

	protected override void OnCleanUp()
	{
		if ((UnityEngine.Object)conditionManager != (UnityEngine.Object)null)
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
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
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
