using KSerialization;
using STRINGS;
using System;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Harvestable : Workable
{
	[Serialize]
	protected bool isMarkedForHarvest;

	[Serialize]
	protected bool canBeHarvested;

	[Serialize]
	protected bool harvestWhenReady;

	public bool defaultHarvestStateWhenPlanted = true;

	public RectTransform HarvestWhenReadyOverlayIcon;

	[Serialize]
	private bool isInPlanterBox;

	protected Chore chore;

	public OccupyArea area;

	private Action<object> onEnableOverlayDelegate;

	private static readonly EventSystem.IntraObjectHandler<Harvestable> ForceCancelHarvestDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.ForceCancelHarvest(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Harvestable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Harvestable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Harvestable> SetInPlanterBoxTrueDelegate = new EventSystem.IntraObjectHandler<Harvestable>(delegate(Harvestable component, object data)
	{
		component.SetInPlanterBox(true);
	});

	public Worker completed_by
	{
		get;
		protected set;
	}

	public bool HarvestWhenReady => harvestWhenReady;

	public bool CanBeHavested => canBeHarvested;

	protected Harvestable()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		onEnableOverlayDelegate = OnEnableOverlay;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("JuniorFarmer", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_QUICK);
		resume.AddExperienceIfRole("Farmer", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_QUICK);
		resume.AddExperienceIfRole("SeniorFarmer", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_QUICK);
	}

	private void OnEnableOverlay(object data)
	{
		if ((HashedString)data == OverlayModes.Harvest.ID)
		{
			CreateOverlayIcon();
		}
		else
		{
			DestroyOverlayIcon();
		}
	}

	private void DestroyOverlayIcon()
	{
		if ((UnityEngine.Object)HarvestWhenReadyOverlayIcon != (UnityEngine.Object)null)
		{
			UnityEngine.Object.Destroy(HarvestWhenReadyOverlayIcon.gameObject);
			HarvestWhenReadyOverlayIcon = null;
		}
	}

	private void CreateOverlayIcon()
	{
		if (!((UnityEngine.Object)HarvestWhenReadyOverlayIcon != (UnityEngine.Object)null) && (UnityEngine.Object)GetComponent<Harvestable>() != (UnityEngine.Object)null && (UnityEngine.Object)GetComponent<AttackableBase>() == (UnityEngine.Object)null)
		{
			HarvestWhenReadyOverlayIcon = Util.KInstantiate(Assets.UIPrefabs.HarvestWhenReadyOverlayIcon, GameScreenManager.Instance.worldSpaceCanvas, null).GetComponent<RectTransform>();
			OccupyArea component = GetComponent<OccupyArea>();
			Extents extents = component.GetExtents();
			KPrefabID component2 = GetComponent<KPrefabID>();
			TransformExtensions.SetPosition(position: component2.HasTag(GameTags.Hanging) ? new Vector3((float)(extents.x + extents.width / 2) + 0.5f, (float)(extents.y + extents.height)) : new Vector3((float)(extents.x + extents.width / 2) + 0.5f, (float)extents.y), transform: HarvestWhenReadyOverlayIcon.transform);
			RefreshOverlayIcon(null);
		}
	}

	private void RefreshOverlayIcon(object data = null)
	{
		if ((UnityEngine.Object)HarvestWhenReadyOverlayIcon != (UnityEngine.Object)null)
		{
			if (Grid.IsVisible(Grid.PosToCell(base.gameObject)) || ((UnityEngine.Object)CameraController.Instance != (UnityEngine.Object)null && CameraController.Instance.FreeCameraEnabled))
			{
				if (!HarvestWhenReadyOverlayIcon.gameObject.activeSelf)
				{
					HarvestWhenReadyOverlayIcon.gameObject.SetActive(true);
				}
			}
			else if (HarvestWhenReadyOverlayIcon.gameObject.activeSelf)
			{
				HarvestWhenReadyOverlayIcon.gameObject.SetActive(false);
			}
			HierarchyReferences component = HarvestWhenReadyOverlayIcon.GetComponent<HierarchyReferences>();
			if (harvestWhenReady)
			{
				component.GetReference("On").gameObject.SetActive(true);
				component.GetReference("Off").gameObject.SetActive(false);
			}
			else
			{
				component.GetReference("On").gameObject.SetActive(false);
				component.GetReference("Off").gameObject.SetActive(true);
			}
		}
	}

	private void OnDisableOverlay(object data)
	{
		DestroyOverlayIcon();
	}

	public void SetInPlanterBox(bool state)
	{
		if (state)
		{
			if (!isInPlanterBox)
			{
				isInPlanterBox = true;
				SetHarvestWhenReady(defaultHarvestStateWhenPlanted);
			}
		}
		else
		{
			isInPlanterBox = false;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Harvesting;
		multitoolContext = "harvest";
		multitoolHitEffectTag = "fx_harvest_splash";
		Subscribe(1309017699, SetInPlanterBoxTrueDelegate);
	}

	protected override void OnSpawn()
	{
		Subscribe(2127324410, ForceCancelHarvestDelegate);
		SetWorkTime(10f);
		Subscribe(2127324410, OnCancelDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		faceTargetWhenWorking = true;
		Components.Harvestables.Add(this);
		attributeConverter = Db.Get().AttributeConverters.HarvestSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		area = GetComponent<OccupyArea>();
		if (isMarkedForHarvest)
		{
			MarkForHarvest();
		}
		Game.Instance.Subscribe(1248612973, onEnableOverlayDelegate);
		Game.Instance.Subscribe(1798162660, onEnableOverlayDelegate);
		Game.Instance.Subscribe(2015652040, OnDisableOverlay);
	}

	public void Harvest()
	{
		isMarkedForHarvest = false;
		chore = null;
		Trigger(1272413801, this);
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
		component.RemoveStatusItem(Db.Get().MiscStatusItems.Operating, false);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public void SetHarvestWhenReady(bool state)
	{
		harvestWhenReady = state;
		if (harvestWhenReady && canBeHarvested && !isMarkedForHarvest)
		{
			MarkForHarvest();
		}
		if (isMarkedForHarvest && !harvestWhenReady)
		{
			OnCancel(null);
			if (canBeHarvested && isInPlanterBox)
			{
				KSelectable component = GetComponent<KSelectable>();
				component.AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, this);
			}
		}
		RefreshOverlayIcon(null);
	}

	public void SetCanBeHarvested(bool state)
	{
		canBeHarvested = state;
		KSelectable component = GetComponent<KSelectable>();
		if (canBeHarvested)
		{
			component.AddStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest, null);
			if (harvestWhenReady)
			{
				MarkForHarvest();
			}
			else if (isInPlanterBox)
			{
				component.AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, this);
			}
		}
		else
		{
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest, false);
			component.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, false);
		}
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public virtual void MarkForHarvest()
	{
		if (canBeHarvested)
		{
			KSelectable component = GetComponent<KSelectable>();
			if (chore == null)
			{
				chore = new WorkChore<Harvestable>(Db.Get().ChoreTypes.Harvest, this, null, null, true, null, null, null, true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
				component.AddStatusItem(Db.Get().MiscStatusItems.PendingHarvest, this);
			}
			isMarkedForHarvest = true;
			component.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, false);
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		completed_by = worker;
		Harvest();
	}

	protected virtual void OnCancel(object data)
	{
		if (chore != null)
		{
			chore.Cancel("Cancel harvest");
			chore = null;
			KSelectable component = GetComponent<KSelectable>();
			component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
			SetHarvestWhenReady(false);
		}
		isMarkedForHarvest = false;
	}

	public bool HasChore()
	{
		if (chore == null)
		{
			return false;
		}
		return true;
	}

	protected virtual void OnClickHarvestWhenReady()
	{
		SetHarvestWhenReady(true);
	}

	protected virtual void OnClickCancelHarvestWhenReady()
	{
		SetHarvestWhenReady(false);
	}

	public virtual void ForceCancelHarvest(object data = null)
	{
		OnCancel(null);
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public virtual void OnRefreshUserMenu(object data)
	{
		object buttonInfo;
		if (harvestWhenReady)
		{
			string iconName = "action_harvest";
			string text = UI.USERMENUACTIONS.CANCEL_HARVEST_WHEN_READY.NAME;
			System.Action on_click = delegate
			{
				OnClickCancelHarvestWhenReady();
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.GAMEOBJECTEFFECTS.PLANT_DO_NOT_HARVEST, base.transform, 1.5f, false);
			};
			string tooltipText = UI.USERMENUACTIONS.CANCEL_HARVEST_WHEN_READY.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
		}
		else
		{
			string tooltipText = "action_harvest";
			string text = UI.USERMENUACTIONS.HARVEST_WHEN_READY.NAME;
			System.Action on_click = delegate
			{
				OnClickHarvestWhenReady();
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.GAMEOBJECTEFFECTS.PLANT_MARK_FOR_HARVEST, base.transform, 1.5f, false);
			};
			string iconName = UI.USERMENUACTIONS.HARVEST_WHEN_READY.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
		}
		KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		DestroyOverlayIcon();
		Game.Instance.Unsubscribe(1248612973, onEnableOverlayDelegate);
		Game.Instance.Unsubscribe(2015652040, OnDisableOverlay);
		Game.Instance.Unsubscribe(1798162660, onEnableOverlayDelegate);
		Components.Harvestables.Remove(this);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
	}
}
