using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class OverlayMenu : KIconToggleMenu
{
	private class OverlayToggleInfo : ToggleInfo
	{
		public SimViewMode simView;

		public string requiredTechItem;

		public OverlayToggleInfo(string text, string icon_name, SimViewMode sim_view, string required_tech_item = "", Action hotKey = Action.NumActions, string tooltip = "", string tooltip_header = "")
			: base(text, icon_name, null, hotKey, tooltip, tooltip_header)
		{
			simView = sim_view;
			requiredTechItem = required_tech_item;
		}

		public bool IsUnlocked()
		{
			if (DebugHandler.InstantBuildMode || string.IsNullOrEmpty(requiredTechItem))
			{
				return true;
			}
			return Db.Get().Techs.IsTechItemComplete(requiredTechItem);
		}
	}

	public static OverlayMenu Instance;

	private List<ToggleInfo> overlay_toggle_infos;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		overlay_toggle_infos = InitializeToggles();
		Setup(overlay_toggle_infos);
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
			foreach (ToggleInfo overlay_toggle_info in overlay_toggle_infos)
			{
				OverlayToggleInfo overlayToggleInfo = (OverlayToggleInfo)overlay_toggle_info;
				overlay_toggle_info.toggle.gameObject.SetActive(overlayToggleInfo.IsUnlocked());
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

	private List<ToggleInfo> InitializeToggles()
	{
		List<ToggleInfo> list = new List<ToggleInfo>();
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.OXYGEN.BUTTON, "overlay_oxygen", SimViewMode.OxygenMap, string.Empty, Action.Overlay1, UI.TOOLTIPS.OXYGENOVERLAYSTRING, UI.OVERLAYS.OXYGEN.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.ELECTRICAL.BUTTON, "overlay_power", SimViewMode.PowerMap, string.Empty, Action.Overlay2, UI.TOOLTIPS.POWEROVERLAYSTRING, UI.OVERLAYS.ELECTRICAL.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.TEMPERATURE.BUTTON, "overlay_temperature", SimViewMode.TemperatureMap, string.Empty, Action.Overlay3, UI.TOOLTIPS.TEMPERATUREOVERLAYSTRING, UI.OVERLAYS.TEMPERATURE.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.HEATFLOW.BUTTON, "overlay_heatflow", SimViewMode.HeatFlow, string.Empty, Action.Overlay4, UI.TOOLTIPS.HEATFLOWOVERLAYSTRING, UI.OVERLAYS.HEATFLOW.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.LIGHTING.BUTTON, "overlay_lights", SimViewMode.Light, string.Empty, Action.Overlay5, UI.TOOLTIPS.LIGHTSOVERLAYSTRING, UI.OVERLAYS.LIGHTING.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.LIQUIDPLUMBING.BUTTON, "overlay_liquidvent", SimViewMode.LiquidVentMap, string.Empty, Action.Overlay6, UI.TOOLTIPS.LIQUIDVENTOVERLAYSTRING, UI.OVERLAYS.LIQUIDPLUMBING.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.GASPLUMBING.BUTTON, "overlay_gasvent", SimViewMode.GasVentMap, string.Empty, Action.Overlay7, UI.TOOLTIPS.GASVENTOVERLAYSTRING, UI.OVERLAYS.GASPLUMBING.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.DECOR.BUTTON, "overlay_decor", SimViewMode.Decor, string.Empty, Action.Overlay8, UI.TOOLTIPS.DECOROVERLAYSTRING, UI.OVERLAYS.DECOR.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.DISEASE.BUTTON, "overlay_disease", SimViewMode.Disease, string.Empty, Action.Overlay9, UI.TOOLTIPS.DISEASEOVERLAYSTRING, UI.OVERLAYS.DISEASE.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.CROPS.BUTTON, "overlay_farming", SimViewMode.Crop, string.Empty, Action.Overlay10, UI.TOOLTIPS.CROPS_OVERLAY_STRING, UI.OVERLAYS.CROPS.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.ROOMS.BUTTON, "overlay_rooms", SimViewMode.Rooms, string.Empty, Action.Overlay11, UI.TOOLTIPS.ROOMSOVERLAYSTRING, UI.OVERLAYS.ROOMS.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.SUIT.BUTTON, "overlay_suit", SimViewMode.SuitRequiredMap, "SuitsOverlay", Action.Overlay12, UI.TOOLTIPS.SUITOVERLAYSTRING, UI.OVERLAYS.SUIT.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.LOGIC.BUTTON, "overlay_logic", SimViewMode.Logic, "AutomationOverlay", Action.Overlay13, UI.TOOLTIPS.LOGICOVERLAYSTRING, UI.OVERLAYS.LOGIC.BUTTON));
		list.Add(new OverlayToggleInfo(UI.OVERLAYS.CONVEYOR.BUTTON, "overlay_conveyor", SimViewMode.SolidConveyorMap, "ConveyorOverlay", Action.Overlay14, UI.TOOLTIPS.CONVEYOR_OVERLAY_STRING, UI.OVERLAYS.CONVEYOR.BUTTON));
		return list;
	}

	private void OnToggleSelect(ToggleInfo toggle_info)
	{
		if (SimDebugView.Instance.GetMode() == ((OverlayToggleInfo)toggle_info).simView)
		{
			OverlayScreen.Instance.ToggleOverlay(SimViewMode.None);
		}
		else if (((OverlayToggleInfo)toggle_info).IsUnlocked())
		{
			OverlayScreen.Instance.ToggleOverlay(((OverlayToggleInfo)toggle_info).simView);
		}
	}

	private void OnOverlayChanged(object overlay_data)
	{
		SimViewMode simViewMode = (SimViewMode)overlay_data;
		for (int i = 0; i < overlay_toggle_infos.Count; i++)
		{
			overlay_toggle_infos[i].toggle.isOn = (((OverlayToggleInfo)overlay_toggle_infos[i]).simView == simViewMode);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (OverlayScreen.Instance.GetMode() != 0 && e.TryConsume(Action.Escape))
			{
				OverlayScreen.Instance.ToggleOverlay(SimViewMode.None);
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
			if (OverlayScreen.Instance.GetMode() != 0 && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
			{
				OverlayScreen.Instance.ToggleOverlay(SimViewMode.None);
			}
			if (!e.Consumed)
			{
				base.OnKeyUp(e);
			}
		}
	}
}
