using System;
using System.Collections.Generic;
using UnityEngine;

public class FilteredDragTool : DragTool
{
	private Dictionary<string, ToolParameterMenu.ToggleState> filterTargets = new Dictionary<string, ToolParameterMenu.ToggleState>();

	private Dictionary<string, ToolParameterMenu.ToggleState> overlayFilterTargets = new Dictionary<string, ToolParameterMenu.ToggleState>();

	private Dictionary<string, ToolParameterMenu.ToggleState> currentFilterTargets;

	private bool active;

	public bool IsActiveLayer(string layer)
	{
		return currentFilterTargets[ToolParameterMenu.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On || (currentFilterTargets.ContainsKey(layer.ToUpper()) && currentFilterTargets[layer.ToUpper()] == ToolParameterMenu.ToggleState.On);
	}

	public bool IsActiveLayer(ObjectLayer layer)
	{
		if (currentFilterTargets.ContainsKey(ToolParameterMenu.FILTERLAYERS.ALL) && currentFilterTargets[ToolParameterMenu.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On)
		{
			return true;
		}
		bool result = false;
		foreach (KeyValuePair<string, ToolParameterMenu.ToggleState> currentFilterTarget in currentFilterTargets)
		{
			if (currentFilterTarget.Value == ToolParameterMenu.ToggleState.On)
			{
				ObjectLayer objectLayerFromFilterLayer = GetObjectLayerFromFilterLayer(currentFilterTarget.Key);
				if (objectLayerFromFilterLayer == layer)
				{
					return true;
				}
			}
		}
		return result;
	}

	protected virtual void GetDefaultFilters(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		filters.Add(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
		filters.Add(ToolParameterMenu.FILTERLAYERS.WIRES, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.BUILDINGS, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.LOGIC, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.BACKWALL, ToolParameterMenu.ToggleState.Off);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ResetFilter(filterTargets);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<SimViewMode>)Delegate.Combine(instance.OnOverlayChanged, new Action<SimViewMode>(OnOverlayChanged));
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<SimViewMode>)Delegate.Remove(instance.OnOverlayChanged, new Action<SimViewMode>(OnOverlayChanged));
		base.OnCleanUp();
	}

	public void ResetFilter()
	{
		ResetFilter(filterTargets);
	}

	protected void ResetFilter(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		filters.Clear();
		GetDefaultFilters(filters);
		currentFilterTargets = filters;
	}

	protected override void OnActivateTool()
	{
		active = true;
		base.OnActivateTool();
		OnOverlayChanged(OverlayScreen.Instance.mode);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		active = false;
		ToolMenu.Instance.toolParameterMenu.ClearMenu();
		base.OnDeactivateTool(new_tool);
	}

	protected string GetFilterLayerFromGameObject(GameObject input)
	{
		BuildingComplete component = input.GetComponent<BuildingComplete>();
		BuildingUnderConstruction component2 = input.GetComponent<BuildingUnderConstruction>();
		if ((bool)component)
		{
			return GetFilterLayerFromObjectLayer(component.Def.ObjectLayer);
		}
		if ((bool)component2)
		{
			return GetFilterLayerFromObjectLayer(component2.Def.ObjectLayer);
		}
		if ((UnityEngine.Object)input.GetComponent<Clearable>() != (UnityEngine.Object)null || (UnityEngine.Object)input.GetComponent<Moppable>() != (UnityEngine.Object)null)
		{
			return "CleanAndClear";
		}
		if ((UnityEngine.Object)input.GetComponent<Diggable>() != (UnityEngine.Object)null)
		{
			return "DigPlacer";
		}
		return "Default";
	}

	protected string GetFilterLayerFromObjectLayer(ObjectLayer gamer_layer)
	{
		switch (gamer_layer)
		{
		case ObjectLayer.Building:
			return "Buildings";
		case ObjectLayer.Wire:
			return "Wires";
		case ObjectLayer.LiquidConduit:
		case ObjectLayer.LiquidConduitConnection:
			return "LiquidPipes";
		case ObjectLayer.GasConduit:
		case ObjectLayer.GasConduitConnection:
			return "GasPipes";
		case ObjectLayer.SolidConduit:
		case ObjectLayer.SolidConduitConnection:
			return "SolidConduits";
		case ObjectLayer.FoundationTile:
			return "Tiles";
		case ObjectLayer.LogicGates:
		case ObjectLayer.LogicWires:
			return "Logic";
		case ObjectLayer.Backwall:
			return "BackWall";
		default:
			return "Default";
		}
	}

	private ObjectLayer GetObjectLayerFromFilterLayer(string filter_layer)
	{
		ObjectLayer objectLayer = ObjectLayer.NumLayers;
		switch (filter_layer.ToLower())
		{
		case "buildings":
			return ObjectLayer.Building;
		case "wires":
			return ObjectLayer.Wire;
		case "liquidpipes":
			return ObjectLayer.LiquidConduit;
		case "gaspipes":
			return ObjectLayer.GasConduit;
		case "solidconduits":
			return ObjectLayer.SolidConduit;
		case "tiles":
			return ObjectLayer.FoundationTile;
		case "logic":
			return ObjectLayer.LogicWires;
		case "backwall":
			return ObjectLayer.Backwall;
		default:
			throw new ArgumentException("Invalid filter layer: " + filter_layer);
		}
	}

	private void OnOverlayChanged(SimViewMode overlay)
	{
		if (active)
		{
			string text = null;
			switch (overlay)
			{
			case SimViewMode.PowerMap:
				text = ToolParameterMenu.FILTERLAYERS.WIRES;
				break;
			case SimViewMode.LiquidVentMap:
				text = ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT;
				break;
			case SimViewMode.GasVentMap:
				text = ToolParameterMenu.FILTERLAYERS.GASCONDUIT;
				break;
			case SimViewMode.SolidConveyorMap:
				text = ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT;
				break;
			case SimViewMode.Logic:
				text = ToolParameterMenu.FILTERLAYERS.LOGIC;
				break;
			}
			currentFilterTargets = filterTargets;
			if (text != null)
			{
				List<string> list = new List<string>(filterTargets.Keys);
				foreach (string item in list)
				{
					filterTargets[item] = ToolParameterMenu.ToggleState.Disabled;
					if (item == text)
					{
						filterTargets[item] = ToolParameterMenu.ToggleState.On;
					}
				}
			}
			else
			{
				if (overlayFilterTargets.Count == 0)
				{
					ResetFilter(overlayFilterTargets);
				}
				currentFilterTargets = overlayFilterTargets;
			}
			ToolMenu.Instance.toolParameterMenu.PopulateMenu(currentFilterTargets);
		}
	}
}
