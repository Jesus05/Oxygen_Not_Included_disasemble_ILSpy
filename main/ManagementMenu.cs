using FMOD.Studio;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ManagementMenu : KIconToggleMenu
{
	public class ScreenData
	{
		public KScreen screen;

		public ToggleInfo toggleInfo;

		public int tabIdx;
	}

	[SerializeField]
	private KToggle smallPrefab;

	private ScreenData activeScreen;

	private KButton activeButton;

	public static ManagementMenu Instance;

	public KScreen researchScreen;

	private JobsTableScreen jobsScreen;

	public VitalsTableScreen vitalsScreen;

	public ScheduleScreen scheduleScreen;

	public KScreen reportsScreen;

	private ConsumablesTableScreen consumablesScreen;

	public CodexScreen codexScreen;

	private RolesScreen rolesScreen;

	private StarmapScreen starmapScreen;

	public string colourSchemeDisabled;

	public InstantiateUIPrefabChild instantiator;

	private ToggleInfo jobsInfo;

	private ToggleInfo consumablesInfo;

	private ToggleInfo scheduleInfo;

	private ToggleInfo vitalsInfo;

	private ToggleInfo reportsInfo;

	private ToggleInfo researchInfo;

	private ToggleInfo codexInfo;

	private ToggleInfo rolesInfo;

	private ToggleInfo starmapInfo;

	private Dictionary<ToggleInfo, ScreenData> ScreenInfoMatch = new Dictionary<ToggleInfo, ScreenData>();

	public KButton[] CloseButtons;

	private string rolesTooltip;

	private string rolesTooltipDisabled;

	private string researchTooltip;

	private string researchTooltipDisabled;

	private string starmapTooltip;

	private string starmapTooltipDisabled;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		CodexCache.Init();
		instantiator.Instantiate();
		jobsScreen = instantiator.GetComponentInChildren<JobsTableScreen>(true);
		consumablesScreen = instantiator.GetComponentInChildren<ConsumablesTableScreen>(true);
		vitalsScreen = instantiator.GetComponentInChildren<VitalsTableScreen>(true);
		starmapScreen = (Resources.FindObjectsOfTypeAll(typeof(StarmapScreen))[0] as StarmapScreen);
		codexScreen = instantiator.GetComponentInChildren<CodexScreen>(true);
		scheduleScreen = instantiator.GetComponentInChildren<ScheduleScreen>(true);
		rolesScreen = (Resources.FindObjectsOfTypeAll(typeof(RolesScreen))[0] as RolesScreen);
		Subscribe(Game.Instance.gameObject, 288942073, OnUIClear);
		consumablesInfo = new ToggleInfo(UI.CONSUMABLES, "OverviewUI_consumables_icon", null, Action.ManageConsumables, UI.TOOLTIPS.MANAGEMENTMENU_CONSUMABLES, string.Empty);
		vitalsInfo = new ToggleInfo(UI.VITALS, "OverviewUI_vitals_icon", null, Action.ManageVitals, UI.TOOLTIPS.MANAGEMENTMENU_VITALS, string.Empty);
		reportsInfo = new ToggleInfo(UI.REPORT, "OverviewUI_reports_icon", null, Action.ManageReport, UI.TOOLTIPS.MANAGEMENTMENU_DAILYREPORT, string.Empty);
		researchInfo = new ToggleInfo(UI.RESEARCH, "OverviewUI_research_nav_icon", null, Action.ManageResearch, UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH, string.Empty);
		jobsInfo = new ToggleInfo(UI.JOBS, "OverviewUI_priority_icon", null, Action.ManagePeople, UI.TOOLTIPS.MANAGEMENTMENU_JOBS, string.Empty);
		rolesInfo = new ToggleInfo(UI.ROLES_SCREEN.MANAGEMENT_BUTTON, "OverviewUI_jobs_icon", null, Action.ManageRoles, UI.TOOLTIPS.MANAGEMENTMENU_ROLES, string.Empty);
		starmapInfo = new ToggleInfo(UI.STARMAP.MANAGEMENT_BUTTON, "ic_rocket", null, Action.ManageStarmap, UI.TOOLTIPS.MANAGEMENTMENU_STARMAP, string.Empty);
		codexInfo = new ToggleInfo(UI.CODEX.MANAGEMENT_BUTTON, "OverviewUI_database_icon", null, Action.ManageCodex, UI.TOOLTIPS.MANAGEMENTMENU_CODEX, string.Empty);
		codexInfo.prefabOverride = smallPrefab;
		scheduleInfo = new ToggleInfo(UI.SCHEDULE, null, null, Action.ManageSchedule, UI.TOOLTIPS.MANAGEMENTMENU_SCHEDULE, string.Empty);
		scheduleInfo.instanceOverride = DateTime.Instance.scheduleToggle;
		ScreenInfoMatch.Add(consumablesInfo, new ScreenData
		{
			screen = consumablesScreen,
			tabIdx = 3,
			toggleInfo = consumablesInfo
		});
		ScreenInfoMatch.Add(vitalsInfo, new ScreenData
		{
			screen = vitalsScreen,
			tabIdx = 2,
			toggleInfo = vitalsInfo
		});
		ScreenInfoMatch.Add(reportsInfo, new ScreenData
		{
			screen = reportsScreen,
			tabIdx = 4,
			toggleInfo = reportsInfo
		});
		ScreenInfoMatch.Add(jobsInfo, new ScreenData
		{
			screen = jobsScreen,
			tabIdx = 1,
			toggleInfo = jobsInfo
		});
		ScreenInfoMatch.Add(rolesInfo, new ScreenData
		{
			screen = rolesScreen,
			tabIdx = 0,
			toggleInfo = rolesInfo
		});
		ScreenInfoMatch.Add(codexInfo, new ScreenData
		{
			screen = codexScreen,
			tabIdx = 6,
			toggleInfo = codexInfo
		});
		ScreenInfoMatch.Add(scheduleInfo, new ScreenData
		{
			screen = scheduleScreen,
			tabIdx = 7,
			toggleInfo = scheduleInfo
		});
		ScreenInfoMatch.Add(starmapInfo, new ScreenData
		{
			screen = starmapScreen,
			tabIdx = 7,
			toggleInfo = starmapInfo
		});
		List<ToggleInfo> list = new List<ToggleInfo>();
		list.Add(consumablesInfo);
		list.Add(vitalsInfo);
		list.Add(reportsInfo);
		list.Add(researchInfo);
		list.Add(jobsInfo);
		list.Add(rolesInfo);
		list.Add(starmapInfo);
		list.Add(codexInfo);
		list.Add(scheduleInfo);
		Setup(list);
		base.onSelect += OnButtonClick;
		Components.ResearchCenters.OnAdd += CheckResearch;
		Components.ResearchCenters.OnRemove += CheckResearch;
		Components.RoleStations.OnAdd += CheckRoles;
		Components.RoleStations.OnRemove += CheckRoles;
		Game.Instance.Subscribe(-809948329, CheckResearch);
		Game.Instance.Subscribe(-809948329, CheckRoles);
		Components.Telescopes.OnAdd += CheckStarmap;
		Components.Telescopes.OnRemove += CheckStarmap;
		rolesTooltipDisabled = UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_ROLES_STATION;
		rolesTooltip = UI.TOOLTIPS.MANAGEMENTMENU_ROLES + " " + GameUtil.GetHotkeyString(Action.ManageRoles);
		researchTooltipDisabled = UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_RESEARCH;
		researchTooltip = UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH + " " + GameUtil.GetHotkeyString(Action.ManageResearch);
		starmapTooltipDisabled = UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_TELESCOPE;
		starmapTooltip = UI.TOOLTIPS.MANAGEMENTMENU_STARMAP + " " + GameUtil.GetHotkeyString(Action.ManageResearch);
		CheckResearch(null);
		CheckRoles(null);
		CheckStarmap(null);
		researchInfo.toggle.soundPlayer.AcceptClickCondition = (() => ResearchAvailable() || activeScreen == ScreenInfoMatch[Instance.researchInfo]);
		KButton[] closeButtons = CloseButtons;
		foreach (KButton kButton in closeButtons)
		{
			kButton.onClick += CloseAll;
			kButton.soundPlayer.Enabled = false;
		}
		foreach (KToggle toggle in toggles)
		{
			toggle.soundPlayer.toggle_widget_sound_events[0].PlaySound = false;
			toggle.soundPlayer.toggle_widget_sound_events[1].PlaySound = false;
		}
	}

	public void AddResearchScreen(ResearchScreen researchScreen)
	{
		if (!((Object)this.researchScreen != (Object)null))
		{
			this.researchScreen = researchScreen;
			this.researchScreen.gameObject.SetActive(false);
			ScreenInfoMatch.Add(researchInfo, new ScreenData
			{
				screen = this.researchScreen,
				tabIdx = 5,
				toggleInfo = researchInfo
			});
			this.researchScreen.Show(false);
		}
	}

	public void CheckResearch(object o)
	{
		if (!((Object)researchInfo.toggle == (Object)null))
		{
			bool flag = Components.ResearchCenters.Count <= 0 && !DebugHandler.InstantBuildMode;
			bool active = !flag && activeScreen != null && activeScreen.toggleInfo == researchInfo;
			string tooltip = (!flag) ? researchTooltip : researchTooltipDisabled;
			ConfigureToggle(researchInfo.toggle, flag, active, tooltip, ToggleToolTipTextStyleSetting);
		}
	}

	public void CheckRoles(object o = null)
	{
		if (!((Object)rolesInfo.toggle == (Object)null))
		{
			bool flag = Components.RoleStations.Count <= 0 && !DebugHandler.InstantBuildMode;
			bool active = activeScreen != null && activeScreen.toggleInfo == rolesInfo;
			string tooltip = (!flag) ? rolesTooltip : rolesTooltipDisabled;
			ConfigureToggle(rolesInfo.toggle, flag, active, tooltip, ToggleToolTipTextStyleSetting);
		}
	}

	public void CheckStarmap(object o = null)
	{
		if (!((Object)starmapInfo.toggle == (Object)null))
		{
			bool flag = Components.Telescopes.Count <= 0 && !DebugHandler.InstantBuildMode;
			bool active = activeScreen != null && activeScreen.toggleInfo == starmapInfo;
			string tooltip = (!flag) ? starmapTooltip : starmapTooltipDisabled;
			ConfigureToggle(starmapInfo.toggle, flag, active, tooltip, ToggleToolTipTextStyleSetting);
		}
	}

	private void ConfigureToggle(KToggle toggle, bool disabled, bool active, string tooltip, TextStyleSetting tooltip_style)
	{
		toggle.interactable = active;
		toggle.GetComponent<KToggle>().interactable = !disabled;
		if (disabled)
		{
			toggle.GetComponentInChildren<ImageToggleState>().SetDisabled();
		}
		else
		{
			toggle.GetComponentInChildren<ImageToggleState>().SetActiveState(active);
		}
		ToolTip component = toggle.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		component.AddMultiStringTooltip(tooltip, tooltip_style);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (activeScreen != null && e.TryConsume(Action.Escape))
		{
			ToggleScreen(activeScreen);
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (activeScreen != null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			ToggleScreen(activeScreen);
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	private bool ResearchAvailable()
	{
		return Components.ResearchCenters.Count > 0 || DebugHandler.InstantBuildMode;
	}

	private bool RolesAvailable()
	{
		return Components.RoleStations.Count > 0 || DebugHandler.InstantBuildMode;
	}

	private bool StarmapAvailable()
	{
		return Components.Telescopes.Count > 0 || DebugHandler.InstantBuildMode;
	}

	public void CloseAll()
	{
		if (activeScreen != null)
		{
			if (activeScreen.toggleInfo != null)
			{
				ToggleScreen(activeScreen);
			}
			CloseActive();
			ClearSelection();
		}
	}

	private void OnUIClear(object data)
	{
		CloseAll();
	}

	public void ToggleScreen(ScreenData screenData)
	{
		if (screenData != null)
		{
			if (screenData.toggleInfo == researchInfo && !ResearchAvailable())
			{
				CheckResearch(null);
				CloseActive();
			}
			else if (screenData.toggleInfo == rolesInfo && !RolesAvailable())
			{
				CheckRoles(null);
				CloseActive();
			}
			else if (screenData.toggleInfo == starmapInfo && !StarmapAvailable())
			{
				CheckStarmap(null);
				CloseActive();
			}
			else if (!screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().IsDisabled)
			{
				if (activeScreen != null)
				{
					activeScreen.toggleInfo.toggle.isOn = false;
					activeScreen.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
				}
				if (activeScreen != screenData)
				{
					OverlayScreen.Instance.ToggleOverlay(SimViewMode.None);
					if (activeScreen != null)
					{
						activeScreen.toggleInfo.toggle.ActivateFlourish(false);
					}
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
					AudioMixer.instance.Start(AudioMixerSnapshots.Get().MenuOpenMigrated);
					screenData.toggleInfo.toggle.ActivateFlourish(true);
					screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetActive();
					CloseActive();
					activeScreen = screenData;
					activeScreen.screen.Show(true);
				}
				else
				{
					activeScreen.screen.Show(false);
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
					AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MenuOpenMigrated, STOP_MODE.ALLOWFADEOUT);
					activeScreen.toggleInfo.toggle.ActivateFlourish(false);
					activeScreen = null;
					screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
				}
			}
		}
	}

	public void OnButtonClick(ToggleInfo toggle_info)
	{
		ToggleScreen(ScreenInfoMatch[toggle_info]);
	}

	private void CloseActive()
	{
		if (activeScreen != null)
		{
			activeScreen.toggleInfo.toggle.isOn = false;
			activeScreen.screen.Show(false);
			activeScreen = null;
		}
	}

	public void ToggleResearch()
	{
		if ((Object)researchScreen == (Object)null)
		{
			AddResearchScreen(Object.FindObjectOfType<ResearchScreen>());
		}
		if ((ResearchAvailable() || activeScreen == ScreenInfoMatch[Instance.researchInfo]) && researchInfo != null)
		{
			ToggleScreen(ScreenInfoMatch[Instance.researchInfo]);
		}
	}

	public void ToggleCodex()
	{
		ToggleScreen(ScreenInfoMatch[Instance.codexInfo]);
	}

	public void OpenCodexToEntry(string id)
	{
		if (!codexScreen.gameObject.activeInHierarchy)
		{
			ToggleCodex();
		}
		codexScreen.ChangeArticle(id, false);
	}

	public void ToggleRoles()
	{
		if ((RolesAvailable() || activeScreen == ScreenInfoMatch[Instance.rolesInfo]) && rolesInfo != null)
		{
			ToggleScreen(ScreenInfoMatch[Instance.rolesInfo]);
		}
	}

	public void ToggleStarmap()
	{
		if (starmapInfo != null)
		{
			ToggleScreen(ScreenInfoMatch[Instance.starmapInfo]);
		}
	}

	public void TogglePriorities()
	{
		ToggleScreen(ScreenInfoMatch[Instance.jobsInfo]);
	}

	public void OpenReports(int day)
	{
		if (activeScreen != ScreenInfoMatch[Instance.reportsInfo])
		{
			ToggleScreen(ScreenInfoMatch[Instance.reportsInfo]);
		}
		ReportScreen.Instance.ShowReport(day);
	}
}
