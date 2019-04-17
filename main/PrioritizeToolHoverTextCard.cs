using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class PrioritizeToolHoverTextCard : HoverTextConfiguration
{
	private string lastUpdatedFilter;

	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		if (!((Object)ToolMenu.Instance.PriorityScreen == (Object)null))
		{
			HoverTextScreen instance = HoverTextScreen.Instance;
			HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
			hoverTextDrawer.BeginShadowBar(false);
			DrawTitle(instance, hoverTextDrawer);
			DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
			hoverTextDrawer.NewLine(26);
			HoverTextDrawer hoverTextDrawer2 = hoverTextDrawer;
			string format = UI.TOOLS.PRIORITIZE.SPECIFIC_PRIORITY;
			PrioritySetting lastSelectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
			hoverTextDrawer2.DrawText(string.Format(format, lastSelectedPriority.priority_value.ToString()), Styles_Title.Standard);
			ToolParameterMenu toolParameterMenu = ToolMenu.Instance.toolParameterMenu;
			string lastEnabledFilter = toolParameterMenu.GetLastEnabledFilter();
			if (lastEnabledFilter != null && lastEnabledFilter != "ALL")
			{
				ConfigureTitle(instance);
			}
			hoverTextDrawer.EndShadowBar();
			hoverTextDrawer.EndDrawing();
		}
	}

	protected override void ConfigureTitle(HoverTextScreen screen)
	{
		ToolParameterMenu toolParameterMenu = ToolMenu.Instance.toolParameterMenu;
		string lastEnabledFilter = toolParameterMenu.GetLastEnabledFilter();
		if (string.IsNullOrEmpty(ToolName) || lastEnabledFilter == "ALL")
		{
			ToolName = Strings.Get(ToolNameStringKey).String.ToUpper();
		}
		if (lastEnabledFilter != null && lastEnabledFilter != "ALL")
		{
			ToolName = Strings.Get(ToolNameStringKey).String.ToUpper() + string.Format(UI.TOOLS.FILTER_HOVERCARD_HEADER, Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + lastEnabledFilter).String.ToUpper());
		}
		lastUpdatedFilter = lastEnabledFilter;
	}
}
