using STRINGS;
using System.Collections.Generic;

public class PrebuildToolHoverTextCard : HoverTextConfiguration
{
	public PlanScreen.RequirementsState currentReqState;

	public BuildingDef currentDef;

	public override void UpdateHoverElements(List<KSelectable> selected)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		switch (currentReqState)
		{
		case PlanScreen.RequirementsState.Materials:
		case PlanScreen.RequirementsState.Complete:
			hoverTextDrawer.DrawText(UI.TOOLTIPS.NOMATERIAL.text.ToUpper(), HoverTextStyleSettings[0]);
			hoverTextDrawer.NewLine(26);
			hoverTextDrawer.DrawText(UI.TOOLTIPS.SELECTAMATERIAL, HoverTextStyleSettings[1]);
			break;
		case PlanScreen.RequirementsState.Tech:
		{
			TechItem techItem = Db.Get().TechItems.Get(currentDef.PrefabID);
			Tech parentTech = techItem.parentTech;
			hoverTextDrawer.DrawText(string.Format(UI.PRODUCTINFO_RESEARCHREQUIRED, parentTech.Name).ToUpper(), HoverTextStyleSettings[0]);
			break;
		}
		}
		hoverTextDrawer.NewLine(26);
		hoverTextDrawer.DrawIcon(instance.GetSprite("icon_mouse_right"), 18);
		hoverTextDrawer.DrawText(backStr, Styles_Instruction.Standard);
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
