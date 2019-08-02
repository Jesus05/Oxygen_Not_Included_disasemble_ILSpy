using STRINGS;
using System;

namespace KMod
{
	public struct Event
	{
		public EventType event_type;

		public Label mod;

		public string details;

		public static void GetUIStrings(EventType err_type, out string title, out string title_tooltip)
		{
			title = string.Empty;
			title_tooltip = string.Empty;
			switch (err_type)
			{
			case EventType.LoadError:
				title = UI.FRONTEND.MOD_EVENTS.REQUIRED;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.REQUIRED;
				break;
			case EventType.NotFound:
				title = UI.FRONTEND.MOD_EVENTS.NOT_FOUND;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.NOT_FOUND;
				break;
			case EventType.InstallInfoInaccessible:
				title = UI.FRONTEND.MOD_EVENTS.INSTALL_INFO_INACCESSIBLE;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.INSTALL_INFO_INACCESSIBLE;
				break;
			case EventType.OutOfOrder:
				title = UI.FRONTEND.MOD_EVENTS.OUT_OF_ORDER;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.OUT_OF_ORDER;
				break;
			case EventType.ActiveDuringCrash:
				title = UI.FRONTEND.MOD_EVENTS.ACTIVE_DURING_CRASH;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.ACTIVE_DURING_CRASH;
				break;
			case EventType.ExpectedActive:
				title = UI.FRONTEND.MOD_EVENTS.EXPECTED_ENABLED;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.EXPECTED_ENABLED;
				break;
			case EventType.ExpectedInactive:
				title = UI.FRONTEND.MOD_EVENTS.EXPECTED_DISABLED;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.EXPECTED_DISABLED;
				break;
			case EventType.VersionUpdate:
				title = UI.FRONTEND.MOD_EVENTS.VERSION_UPDATE;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.VERSION_UPDATE;
				break;
			case EventType.AvailableContentChanged:
				title = UI.FRONTEND.MOD_EVENTS.AVAILABLE_CONTENT_CHANGED;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.AVAILABLE_CONTENT_CHANGED;
				break;
			case EventType.InstallFailed:
				title = UI.FRONTEND.MOD_EVENTS.INSTALL_FAILED;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.INSTALL_FAILED;
				break;
			case EventType.Installed:
				title = UI.FRONTEND.MOD_EVENTS.INSTALLED;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.INSTALLED;
				break;
			case EventType.Uninstalled:
				title = UI.FRONTEND.MOD_EVENTS.UNINSTALLED;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.UNINSTALLED;
				break;
			case EventType.RestartRequested:
				title = UI.FRONTEND.MOD_EVENTS.REQUIRES_RESTART;
				title_tooltip = UI.FRONTEND.MODS.REQUIRES_RESTART;
				break;
			case EventType.BadWorldGen:
				title = UI.FRONTEND.MOD_EVENTS.BAD_WORLD_GEN;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.BAD_WORLD_GEN;
				break;
			case EventType.Deactivated:
				title = UI.FRONTEND.MOD_EVENTS.DEACTIVATED;
				title_tooltip = UI.FRONTEND.MOD_EVENTS.TOOLTIPS.DEACTIVATED;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
