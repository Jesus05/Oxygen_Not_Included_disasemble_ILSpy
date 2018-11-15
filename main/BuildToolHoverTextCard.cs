using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildToolHoverTextCard : HoverTextConfiguration
{
	public BuildingDef currentDef;

	public override void UpdateHoverElements(List<KSelectable> hoverObjects_dont_use_this_is_null)
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		hoverTextDrawer.BeginShadowBar(false);
		ActionName = ((!((UnityEngine.Object)currentDef != (UnityEngine.Object)null) || !currentDef.DragBuild) ? UI.TOOLS.BUILD.TOOLACTION : UI.TOOLS.BUILD.TOOLACTION_DRAG);
		if ((UnityEngine.Object)currentDef != (UnityEngine.Object)null && currentDef.Name != null)
		{
			ToolName = string.Format(UI.TOOLS.BUILD.NAME, currentDef.Name);
		}
		DrawTitle(instance, hoverTextDrawer);
		DrawInstructions(instance, hoverTextDrawer);
		int cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		int min_height = 26;
		int width = 8;
		if ((UnityEngine.Object)currentDef != (UnityEngine.Object)null)
		{
			Orientation orientation = Orientation.Neutral;
			if ((UnityEngine.Object)PlayerController.Instance.ActiveTool != (UnityEngine.Object)null)
			{
				Type type = PlayerController.Instance.ActiveTool.GetType();
				if (typeof(BuildTool).IsAssignableFrom(type) || typeof(BaseUtilityBuildTool).IsAssignableFrom(type))
				{
					if ((UnityEngine.Object)currentDef.BuildingComplete.GetComponent<Rotatable>() != (UnityEngine.Object)null)
					{
						hoverTextDrawer.NewLine(min_height);
						hoverTextDrawer.AddIndent(width);
						string text = UI.TOOLTIPS.HELP_ROTATE_KEY.ToString();
						text = text.Replace("{Key}", GameUtil.GetActionString(Action.RotateBuilding));
						hoverTextDrawer.DrawText(text, Styles_Instruction.Standard);
					}
					orientation = BuildTool.Instance.GetBuildingOrientation;
					string fail_reason = "Unknown reason";
					Vector3 pos = Grid.CellToPosCCC(cell, Grid.SceneLayer.Building);
					if (!currentDef.IsValidPlaceLocation(BuildTool.Instance.visualizer, pos, orientation, out fail_reason))
					{
						hoverTextDrawer.NewLine(min_height);
						hoverTextDrawer.AddIndent(width);
						hoverTextDrawer.DrawText(fail_reason, HoverTextStyleSettings[1]);
					}
					RoomTracker component = currentDef.BuildingComplete.GetComponent<RoomTracker>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null && !component.SufficientBuildLocation(cell))
					{
						hoverTextDrawer.NewLine(min_height);
						hoverTextDrawer.AddIndent(width);
						hoverTextDrawer.DrawText(UI.TOOLTIPS.HELP_REQUIRES_ROOM, HoverTextStyleSettings[1]);
					}
				}
			}
			CircuitManager circuitManager = Game.Instance.circuitManager;
			ushort circuitID = circuitManager.GetCircuitID(cell);
			if (circuitID != 65535)
			{
				float wattsNeededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitID);
				wattsNeededWhenActive += currentDef.EnergyConsumptionWhenActive;
				float maxSafeWattageForCircuit = circuitManager.GetMaxSafeWattageForCircuit(circuitID);
				Color color = (!(wattsNeededWhenActive >= maxSafeWattageForCircuit)) ? Color.white : Color.red;
				hoverTextDrawer.NewLine(min_height);
				hoverTextDrawer.AddIndent(width);
				hoverTextDrawer.DrawText(string.Format(UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(wattsNeededWhenActive, GameUtil.WattageFormatterUnit.Automatic)), Styles_BodyText.Standard, color, true);
			}
			hoverTextDrawer.NewLine(min_height);
			hoverTextDrawer.AddIndent(width);
			hoverTextDrawer.DrawText(ResourceRemainingDisplayScreen.instance.GetString(), Styles_BodyText.Standard);
		}
		hoverTextDrawer.EndShadowBar();
		hoverTextDrawer.EndDrawing();
	}
}
