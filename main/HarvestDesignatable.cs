using KSerialization;
using STRINGS;
using System;
using UnityEngine;

public class HarvestDesignatable : KMonoBehaviour
{
	public bool defaultHarvestStateWhenPlanted = true;

	public OccupyArea area;

	[Serialize]
	protected bool isMarkedForHarvest;

	[Serialize]
	private bool isInPlanterBox = false;

	public bool showUserMenuButtons = true;

	[Serialize]
	protected bool harvestWhenReady = false;

	public RectTransform HarvestWhenReadyOverlayIcon;

	private Action<object> onEnableOverlayDelegate;

	private static readonly EventSystem.IntraObjectHandler<HarvestDesignatable> OnCancelDelegate = new EventSystem.IntraObjectHandler<HarvestDesignatable>(delegate(HarvestDesignatable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<HarvestDesignatable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<HarvestDesignatable>(delegate(HarvestDesignatable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<HarvestDesignatable> SetInPlanterBoxTrueDelegate = new EventSystem.IntraObjectHandler<HarvestDesignatable>(delegate(HarvestDesignatable component, object data)
	{
		component.SetInPlanterBox(true);
	});

	public bool InPlanterBox => isInPlanterBox;

	public bool MarkedForHarvest
	{
		get
		{
			return isMarkedForHarvest;
		}
		set
		{
			isMarkedForHarvest = value;
		}
	}

	public bool HarvestWhenReady => harvestWhenReady;

	protected HarvestDesignatable()
	{
		onEnableOverlayDelegate = OnEnableOverlay;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(1309017699, SetInPlanterBoxTrueDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (isMarkedForHarvest)
		{
			MarkForHarvest();
		}
		Components.HarvestDesignatables.Add(this);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(2127324410, OnCancelDelegate);
		Game.Instance.Subscribe(1248612973, onEnableOverlayDelegate);
		Game.Instance.Subscribe(1798162660, onEnableOverlayDelegate);
		Game.Instance.Subscribe(2015652040, OnDisableOverlay);
		area = GetComponent<OccupyArea>();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.HarvestDesignatables.Remove(this);
		DestroyOverlayIcon();
		Game.Instance.Unsubscribe(1248612973, onEnableOverlayDelegate);
		Game.Instance.Unsubscribe(2015652040, OnDisableOverlay);
		Game.Instance.Unsubscribe(1798162660, onEnableOverlayDelegate);
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
		if (!((UnityEngine.Object)HarvestWhenReadyOverlayIcon != (UnityEngine.Object)null) && (UnityEngine.Object)GetComponent<AttackableBase>() == (UnityEngine.Object)null)
		{
			HarvestWhenReadyOverlayIcon = Util.KInstantiate(Assets.UIPrefabs.HarvestWhenReadyOverlayIcon, GameScreenManager.Instance.worldSpaceCanvas, null).GetComponent<RectTransform>();
			OccupyArea component = GetComponent<OccupyArea>();
			Extents extents = component.GetExtents();
			KPrefabID component2 = GetComponent<KPrefabID>();
			TransformExtensions.SetPosition(position: component2.HasTag(GameTags.Hanging) ? new Vector3((float)(extents.x + extents.width / 2) + 0.5f, (float)(extents.y + extents.height)) : new Vector3((float)(extents.x + extents.width / 2) + 0.5f, (float)extents.y), transform: HarvestWhenReadyOverlayIcon.transform);
			RefreshOverlayIcon(null);
		}
	}

	private void OnDisableOverlay(object data)
	{
		DestroyOverlayIcon();
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

	public bool CanBeHarvested()
	{
		Harvestable component = GetComponent<Harvestable>();
		if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
		{
			return true;
		}
		return component.CanBeHarvested;
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

	public void SetHarvestWhenReady(bool state)
	{
		harvestWhenReady = state;
		if (harvestWhenReady && CanBeHarvested() && !isMarkedForHarvest)
		{
			MarkForHarvest();
		}
		if (isMarkedForHarvest && !harvestWhenReady)
		{
			OnCancel(null);
			if (CanBeHarvested() && isInPlanterBox)
			{
				KSelectable component = GetComponent<KSelectable>();
				component.AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, this);
			}
		}
		Trigger(-266953818, null);
		RefreshOverlayIcon(null);
	}

	protected virtual void OnCancel(object data = null)
	{
	}

	public virtual void MarkForHarvest()
	{
		if (CanBeHarvested())
		{
			isMarkedForHarvest = true;
			Harvestable component = GetComponent<Harvestable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.OnMarkedForHarvest();
			}
		}
	}

	protected virtual void OnClickHarvestWhenReady()
	{
		SetHarvestWhenReady(true);
	}

	protected virtual void OnClickCancelHarvestWhenReady()
	{
		SetHarvestWhenReady(false);
	}

	public virtual void OnRefreshUserMenu(object data)
	{
		if (showUserMenuButtons)
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
	}
}
