using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class OverlayMenu : KIconToggleMenu
{
	private class OverlayToggleGroup : ToggleInfo
	{
		public List<OverlayToggleInfo> toggleInfoGroup;

		public string requiredTechItem;

		[SerializeField]
		private int activeToggleInfo = 0;

		public OverlayToggleGroup(string text, string icon_name, List<OverlayToggleInfo> toggle_group, string required_tech_item = "", Action hot_key = Action.NumActions, string tooltip = "", string tooltip_header = "")
			: base(text, icon_name, null, hot_key, tooltip, tooltip_header)
		{
			toggleInfoGroup = toggle_group;
		}

		public bool IsUnlocked()
		{
			if (!DebugHandler.InstantBuildMode && !string.IsNullOrEmpty(requiredTechItem))
			{
				return Db.Get().Techs.IsTechItemComplete(requiredTechItem);
			}
			return true;
		}

		public OverlayToggleInfo GetActiveToggleInfo()
		{
			return toggleInfoGroup[activeToggleInfo];
		}
	}

	private class OverlayToggleInfo : ToggleInfo
	{
		public HashedString simView;

		public string requiredTechItem;

		public OverlayToggleInfo(string text, string icon_name, HashedString sim_view, string required_tech_item = "", Action hotKey = Action.NumActions, string tooltip = "", string tooltip_header = "")
			: base(text, icon_name, null, hotKey, tooltip, tooltip_header)
		{
			simView = sim_view;
			requiredTechItem = required_tech_item;
		}

		public bool IsUnlocked()
		{
			if (!DebugHandler.InstantBuildMode && !string.IsNullOrEmpty(requiredTechItem))
			{
				return Db.Get().Techs.IsTechItemComplete(requiredTechItem);
			}
			return true;
		}
	}

	public static OverlayMenu Instance;

	private List<ToggleInfo> overlayToggleInfos;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		InitializeToggles();
		Setup(overlayToggleInfos);
		Game.Instance.Subscribe(1798162660, OnOverlayChanged);
		Game.Instance.Subscribe(-107300940, OnResearchComplete);
		base.onSelect += OnToggleSelect;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RefreshButtons();
	}

	public void Refresh()
	{
		RefreshButtons();
	}

	protected override void RefreshButtons()
	{
		base.RefreshButtons();
		if (!((Object)Research.Instance == (Object)null))
		{
			foreach (ToggleInfo overlayToggleInfo2 in overlayToggleInfos)
			{
				OverlayToggleInfo overlayToggleInfo = (OverlayToggleInfo)overlayToggleInfo2;
				overlayToggleInfo2.toggle.gameObject.SetActive(overlayToggleInfo.IsUnlocked());
			}
		}
	}

	private void OnResearchComplete(object data)
	{
		RefreshButtons();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.Unsubscribe(1798162660, OnOverlayChanged);
	}

	private void InitializeToggleGroups()
	{
	}

	private void InitializeToggles()
	{
		overlayToggleInfos = new List<ToggleInfo>
		{
			new OverlayToggleInfo(UI.OVERLAYS.OXYGEN.BUTTON, "overlay_oxygen", OverlayModes.Oxygen.ID, "", Action.Overlay1, UI.TOOLTIPS.OXYGENOVERLAYSTRING, UI.OVERLAYS.OXYGEN.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.ELECTRICAL.BUTTON, "overlay_power", OverlayModes.Power.ID, "", Action.Overlay2, UI.TOOLTIPS.POWEROVERLAYSTRING, UI.OVERLAYS.ELECTRICAL.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.TEMPERATURE.BUTTON, "overlay_temperature", OverlayModes.Temperature.ID, "", Action.Overlay3, UI.TOOLTIPS.TEMPERATUREOVERLAYSTRING, UI.OVERLAYS.TEMPERATURE.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.TILEMODE.BUTTON, "overlay_materials", OverlayModes.TileMode.ID, "", Action.Overlay4, UI.TOOLTIPS.TILEMODE_OVERLAY_STRING, UI.OVERLAYS.TILEMODE.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.LIGHTING.BUTTON, "overlay_lights", OverlayModes.Light.ID, "", Action.Overlay5, UI.TOOLTIPS.LIGHTSOVERLAYSTRING, UI.OVERLAYS.LIGHTING.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.LIQUIDPLUMBING.BUTTON, "overlay_liquidvent", OverlayModes.LiquidConduits.ID, "", Action.Overlay6, UI.TOOLTIPS.LIQUIDVENTOVERLAYSTRING, UI.OVERLAYS.LIQUIDPLUMBING.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.GASPLUMBING.BUTTON, "overlay_gasvent", OverlayModes.GasConduits.ID, "", Action.Overlay7, UI.TOOLTIPS.GASVENTOVERLAYSTRING, UI.OVERLAYS.GASPLUMBING.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.DECOR.BUTTON, "overlay_decor", OverlayModes.Decor.ID, "", Action.Overlay8, UI.TOOLTIPS.DECOROVERLAYSTRING, UI.OVERLAYS.DECOR.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.DISEASE.BUTTON, "overlay_disease", OverlayModes.Disease.ID, "", Action.Overlay9, UI.TOOLTIPS.DISEASEOVERLAYSTRING, UI.OVERLAYS.DISEASE.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.CROPS.BUTTON, "overlay_farming", OverlayModes.Crop.ID, "", Action.Overlay10, UI.TOOLTIPS.CROPS_OVERLAY_STRING, UI.OVERLAYS.CROPS.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.ROOMS.BUTTON, "overlay_rooms", OverlayModes.Rooms.ID, "", Action.Overlay11, UI.TOOLTIPS.ROOMSOVERLAYSTRING, UI.OVERLAYS.ROOMS.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.SUIT.BUTTON, "overlay_suit", OverlayModes.Suit.ID, "SuitsOverlay", Action.Overlay12, UI.TOOLTIPS.SUITOVERLAYSTRING, UI.OVERLAYS.SUIT.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.LOGIC.BUTTON, "overlay_logic", OverlayModes.Logic.ID, "AutomationOverlay", Action.Overlay13, UI.TOOLTIPS.LOGICOVERLAYSTRING, UI.OVERLAYS.LOGIC.BUTTON),
			new OverlayToggleInfo(UI.OVERLAYS.CONVEYOR.BUTTON, "overlay_conveyor", OverlayModes.SolidConveyor.ID, "ConveyorOverlay", Action.Overlay14, UI.TOOLTIPS.CONVEYOR_OVERLAY_STRING, UI.OVERLAYS.CONVEYOR.BUTTON)
		};
	}

	private void OnToggleSelect(ToggleInfo toggle_info)
	{
		if (SimDebugView.Instance.GetMode() == ((OverlayToggleInfo)toggle_info).simView)
		{
			OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
		}
		else if (((OverlayToggleInfo)toggle_info).IsUnlocked())
		{
			OverlayScreen.Instance.ToggleOverlay(((OverlayToggleInfo)toggle_info).simView);
		}
	}

	private void OnOverlayChanged(object overlay_data)
	{
		HashedString y = (HashedString)overlay_data;
		for (int i = 0; i < overlayToggleInfos.Count; i++)
		{
			overlayToggleInfos[i].toggle.isOn = (((OverlayToggleInfo)overlayToggleInfos[i]).simView == y);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (OverlayScreen.Instance.GetMode() != OverlayModes.None.ID && e.TryConsume(Action.Escape))
			{
				OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
			}
			if (!e.Consumed)
			{
				base.OnKeyDown(e);
			}
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (OverlayScreen.Instance.GetMode() != OverlayModes.None.ID && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
			{
				OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
			}
			if (!e.Consumed)
			{
				base.OnKeyUp(e);
			}
		}
	}
}
