using STRINGS;
using System.Collections.Generic;

public class EmptyPipeToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		ToolParameterMenu toolParameterMenu = ToolMenu.Instance.toolParameterMenu;
		string lastEnabledFilter = toolParameterMenu.GetLastEnabledFilter();
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		DrawTitle(instance, hoverTextDrawer);
		DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
		if (lastEnabledFilter != null && lastEnabledFilter != "ALL")
		{
			ConfigureTitle(instance);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
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
	}
}
