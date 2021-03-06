using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildToolHoverTextCard : HoverTextConfiguration
{
	public BuildingDef currentDef;

	public override void UpdateHoverElements(List<KSelectable> hoverObjects)
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
			hoverTextDrawer.NewLine(min_height);
			hoverTextDrawer.AddIndent(width);
			hoverTextDrawer.DrawText(ResourceRemainingDisplayScreen.instance.GetString(), Styles_BodyText.Standard);
			hoverTextDrawer.EndShadowBar();
			HashedString mode = SimDebugView.Instance.GetMode();
			if (mode == OverlayModes.Logic.ID && hoverObjects != null)
			{
				SelectToolHoverTextCard component2 = SelectTool.Instance.GetComponent<SelectToolHoverTextCard>();
				foreach (KSelectable hoverObject in hoverObjects)
				{
					LogicPorts component3 = hoverObject.GetComponent<LogicPorts>();
					if ((UnityEngine.Object)component3 != (UnityEngine.Object)null && component3.TryGetPortAtCell(cell, out LogicPorts.Port port, out bool isInput))
					{
						bool flag = component3.IsPortConnected(port.id);
						hoverTextDrawer.BeginShadowBar(false);
						int num;
						if (isInput)
						{
							string replacement = (!port.displayCustomName) ? UI.LOGIC_PORTS.PORT_INPUT_DEFAULT_NAME.text : port.description;
							num = component3.GetInputValue(port.id);
							hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_INPUT_HOVER_FMT.Replace("{Port}", replacement).Replace("{Name}", hoverObject.GetProperName().ToUpper()), component2.Styles_Title.Standard);
						}
						else
						{
							string replacement2 = (!port.displayCustomName) ? UI.LOGIC_PORTS.PORT_OUTPUT_DEFAULT_NAME.text : port.description;
							num = component3.GetOutputValue(port.id);
							hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_OUTPUT_HOVER_FMT.Replace("{Port}", replacement2).Replace("{Name}", hoverObject.GetProperName().ToUpper()), component2.Styles_Title.Standard);
						}
						hoverTextDrawer.NewLine(26);
						TextStyleSetting textStyleSetting = (!flag) ? component2.Styles_LogicActive.Standard : ((num != 1) ? component2.Styles_LogicSignalInactive : component2.Styles_LogicActive.Selected);
						hoverTextDrawer.DrawIcon((num != 1 || !flag) ? component2.iconDash : component2.iconActiveAutomationPort, textStyleSetting.textColor, 18, 2);
						hoverTextDrawer.DrawText(port.activeDescription, textStyleSetting);
						hoverTextDrawer.NewLine(26);
						TextStyleSetting textStyleSetting2 = (!flag) ? component2.Styles_LogicStandby.Standard : ((num != 0) ? component2.Styles_LogicSignalInactive : component2.Styles_LogicStandby.Selected);
						hoverTextDrawer.DrawIcon((num != 0 || !flag) ? component2.iconDash : component2.iconActiveAutomationPort, textStyleSetting2.textColor, 18, 2);
						hoverTextDrawer.DrawText(port.inactiveDescription, textStyleSetting2);
						hoverTextDrawer.EndShadowBar();
					}
					LogicGate component4 = hoverObject.GetComponent<LogicGate>();
					if ((UnityEngine.Object)component4 != (UnityEngine.Object)null && component4.TryGetPortAtCell(cell, out LogicGateBase.PortId port2))
					{
						int portValue = component4.GetPortValue(port2);
						bool portConnected = component4.GetPortConnected(port2);
						LogicGate.LogicGateDescriptions.Description portDescription = component4.GetPortDescription(port2);
						hoverTextDrawer.BeginShadowBar(false);
						if (port2 == LogicGateBase.PortId.Output)
						{
							hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_MULTI_OUTPUT_HOVER_FMT.Replace("{Port}", portDescription.name).Replace("{Name}", hoverObject.GetProperName().ToUpper()), component2.Styles_Title.Standard);
						}
						else
						{
							hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_MULTI_INPUT_HOVER_FMT.Replace("{Port}", portDescription.name).Replace("{Name}", hoverObject.GetProperName().ToUpper()), component2.Styles_Title.Standard);
						}
						hoverTextDrawer.NewLine(26);
						TextStyleSetting textStyleSetting3 = (!portConnected) ? component2.Styles_LogicActive.Standard : ((portValue != 1) ? component2.Styles_LogicSignalInactive : component2.Styles_LogicActive.Selected);
						hoverTextDrawer.DrawIcon((portValue != 1 || !portConnected) ? component2.iconDash : component2.iconActiveAutomationPort, textStyleSetting3.textColor, 18, 2);
						hoverTextDrawer.DrawText(portDescription.active, textStyleSetting3);
						hoverTextDrawer.NewLine(26);
						TextStyleSetting textStyleSetting4 = (!portConnected) ? component2.Styles_LogicStandby.Standard : ((portValue != 0) ? component2.Styles_LogicSignalInactive : component2.Styles_LogicStandby.Selected);
						hoverTextDrawer.DrawIcon((portValue != 0 || !portConnected) ? component2.iconDash : component2.iconActiveAutomationPort, textStyleSetting4.textColor, 18, 2);
						hoverTextDrawer.DrawText(portDescription.inactive, textStyleSetting4);
						hoverTextDrawer.EndShadowBar();
					}
				}
			}
			else if (mode == OverlayModes.Power.ID)
			{
				CircuitManager circuitManager = Game.Instance.circuitManager;
				ushort circuitID = circuitManager.GetCircuitID(cell);
				if (circuitID != 65535)
				{
					hoverTextDrawer.BeginShadowBar(false);
					float wattsNeededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitID);
					wattsNeededWhenActive += currentDef.EnergyConsumptionWhenActive;
					float maxSafeWattageForCircuit = circuitManager.GetMaxSafeWattageForCircuit(circuitID);
					Color color = (!(wattsNeededWhenActive >= maxSafeWattageForCircuit)) ? Color.white : Color.red;
					hoverTextDrawer.AddIndent(width);
					hoverTextDrawer.DrawText(string.Format(UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(wattsNeededWhenActive, GameUtil.WattageFormatterUnit.Automatic)), Styles_BodyText.Standard, color, true);
					hoverTextDrawer.EndShadowBar();
				}
			}
		}
		hoverTextDrawer.EndDrawing();
	}
}
