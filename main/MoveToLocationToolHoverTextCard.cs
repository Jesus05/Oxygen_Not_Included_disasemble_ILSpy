using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class MoveToLocationToolHoverTextCard : HoverTextConfiguration
{
	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		if (Grid.IsValidCell(num))
		{
			HoverTextScreen instance = HoverTextScreen.Instance;
			HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
			hoverTextDrawer.BeginShadowBar(false);
			DrawTitle(HoverTextScreen.Instance, hoverTextDrawer);
			DrawInstructions(HoverTextScreen.Instance, hoverTextDrawer);
			if (!MoveToLocationTool.Instance.CanMoveTo(num))
			{
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawText(UI.TOOLS.MOVETOLOCATION.UNREACHABLE, HoverTextStyleSettings[1]);
			}
			hoverTextDrawer.EndShadowBar();
			hoverTextDrawer.EndDrawing();
		}
	}
}
