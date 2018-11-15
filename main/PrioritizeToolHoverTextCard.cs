using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class PrioritizeToolHoverTextCard : HoverTextConfiguration
{
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
			hoverTextDrawer.EndShadowBar();
			hoverTextDrawer.EndDrawing();
		}
	}
}
