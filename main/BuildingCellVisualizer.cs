using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SkipSaveFileSerialization]
public class BuildingCellVisualizer : KMonoBehaviour
{
	[Flags]
	private enum Ports : byte
	{
		PowerIn = 0x1,
		PowerOut = 0x2,
		GasIn = 0x4,
		GasOut = 0x8,
		LiquidIn = 0x10,
		LiquidOut = 0x20,
		SolidIn = 0x40,
		SolidOut = 0x80
	}

	private BuildingCellVisualizerResources resources;

	[MyCmpReq]
	private Building building;

	[SerializeField]
	public static Color32 secondOutputColour = new Color(0.9843137f, 0.6901961f, 0.23137255f);

	[SerializeField]
	public static Color32 secondInputColour = new Color(0.9843137f, 0.6901961f, 0.23137255f);

	private const Ports POWER_PORTS = Ports.PowerIn | Ports.PowerOut;

	private const Ports GAS_PORTS = Ports.GasIn | Ports.GasOut;

	private const Ports LIQUID_PORTS = Ports.LiquidIn | Ports.LiquidOut;

	private const Ports SOLID_PORTS = Ports.SolidIn | Ports.SolidOut;

	private const Ports MATTER_PORTS = ~(Ports.PowerIn | Ports.PowerOut);

	private Ports ports;

	private Ports secondary_ports;

	private Sprite diseaseSourceSprite;

	private Color32 diseaseSourceColour;

	private GameObject inputVisualizer;

	private GameObject outputVisualizer;

	private GameObject secondaryInputVisualizer;

	private GameObject secondaryOutputVisualizer;

	private bool enableRaycast;

	private Dictionary<GameObject, Image> icons;

	public bool RequiresPowerInput => (ports & Ports.PowerIn) != ~(Ports.PowerIn | Ports.PowerOut | Ports.GasIn | Ports.GasOut | Ports.LiquidIn | Ports.LiquidOut | Ports.SolidIn | Ports.SolidOut);

	public bool RequiresPowerOutput => (ports & Ports.PowerOut) != ~(Ports.PowerIn | Ports.PowerOut | Ports.GasIn | Ports.GasOut | Ports.LiquidIn | Ports.LiquidOut | Ports.SolidIn | Ports.SolidOut);

	public bool RequiresPower => (ports & (Ports.PowerIn | Ports.PowerOut)) != ~(Ports.PowerIn | Ports.PowerOut | Ports.GasIn | Ports.GasOut | Ports.LiquidIn | Ports.LiquidOut | Ports.SolidIn | Ports.SolidOut);

	public bool RequiresGas => (ports & (Ports.GasIn | Ports.GasOut)) != ~(Ports.PowerIn | Ports.PowerOut | Ports.GasIn | Ports.GasOut | Ports.LiquidIn | Ports.LiquidOut | Ports.SolidIn | Ports.SolidOut);

	public bool RequiresLiquid => (ports & (Ports.LiquidIn | Ports.LiquidOut)) != ~(Ports.PowerIn | Ports.PowerOut | Ports.GasIn | Ports.GasOut | Ports.LiquidIn | Ports.LiquidOut | Ports.SolidIn | Ports.SolidOut);

	public bool RequiresSolid => (ports & (Ports.SolidIn | Ports.SolidOut)) != ~(Ports.PowerIn | Ports.PowerOut | Ports.GasIn | Ports.GasOut | Ports.LiquidIn | Ports.LiquidOut | Ports.SolidIn | Ports.SolidOut);

	public bool RequiresUtilityConnection => (ports & ~(Ports.PowerIn | Ports.PowerOut)) != ~(Ports.PowerIn | Ports.PowerOut | Ports.GasIn | Ports.GasOut | Ports.LiquidIn | Ports.LiquidOut | Ports.SolidIn | Ports.SolidOut);

	public void ConnectedEventWithDelay(float delay, int connectionCount, int cell, string soundName)
	{
		StartCoroutine(ConnectedDelay(delay, connectionCount, cell, soundName));
	}

	private IEnumerator ConnectedDelay(float delay, int connectionCount, int cell, string soundName)
	{
		float startTime = Time.realtimeSinceStartup;
		float currentTime = startTime;
		if (currentTime < startTime + delay)
		{
			float num = currentTime + Time.unscaledDeltaTime;
			yield return (object)new WaitForEndOfFrame();
			/*Error: Unable to find new state assignment for yield return*/;
		}
		ConnectedEvent(cell);
		string connectedReleaseSound = GlobalAssets.GetSound(soundName, false);
		if (connectedReleaseSound != null)
		{
			Vector3 position = base.transform.GetPosition();
			EventInstance instance = SoundEvent.BeginOneShot(connectedReleaseSound, position);
			instance.setParameterValue("connectedCount", (float)connectionCount);
			SoundEvent.EndOneShot(instance);
		}
	}

	public void ConnectedEvent(int cell)
	{
		GameObject gameObject = null;
		if ((UnityEngine.Object)inputVisualizer != (UnityEngine.Object)null && Grid.PosToCell(inputVisualizer) == cell)
		{
			gameObject = inputVisualizer;
		}
		else if ((UnityEngine.Object)outputVisualizer != (UnityEngine.Object)null && Grid.PosToCell(outputVisualizer) == cell)
		{
			gameObject = outputVisualizer;
		}
		else if ((UnityEngine.Object)secondaryInputVisualizer != (UnityEngine.Object)null && Grid.PosToCell(secondaryInputVisualizer) == cell)
		{
			gameObject = secondaryInputVisualizer;
		}
		else if ((UnityEngine.Object)secondaryOutputVisualizer != (UnityEngine.Object)null && Grid.PosToCell(secondaryOutputVisualizer) == cell)
		{
			gameObject = secondaryOutputVisualizer;
		}
		if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
		{
			SizePulse pulse = gameObject.gameObject.AddComponent<SizePulse>();
			pulse.speed = 20f;
			pulse.multiplier = 0.75f;
			pulse.updateWhenPaused = true;
			SizePulse sizePulse = pulse;
			sizePulse.onComplete = (System.Action)Delegate.Combine(sizePulse.onComplete, (System.Action)delegate
			{
				UnityEngine.Object.Destroy(pulse);
			});
		}
	}

	protected override void OnSpawn()
	{
		resources = BuildingCellVisualizerResources.Instance();
		enableRaycast = ((UnityEngine.Object)(building as BuildingComplete) != (UnityEngine.Object)null);
		icons = new Dictionary<GameObject, Image>();
	}

	private void MapBuilding()
	{
		BuildingDef def = building.Def;
		if (def.CheckRequiresPowerInput())
		{
			ports |= Ports.PowerIn;
		}
		if (def.CheckRequiresPowerOutput())
		{
			ports |= Ports.PowerOut;
		}
		if (def.CheckRequiresGasInput())
		{
			ports |= Ports.GasIn;
		}
		if (def.CheckRequiresGasOutput())
		{
			ports |= Ports.GasOut;
		}
		if (def.CheckRequiresLiquidInput())
		{
			ports |= Ports.LiquidIn;
		}
		if (def.CheckRequiresLiquidOutput())
		{
			ports |= Ports.LiquidOut;
		}
		if (def.CheckRequiresSolidInput())
		{
			ports |= Ports.SolidIn;
		}
		if (def.CheckRequiresSolidOutput())
		{
			ports |= Ports.SolidOut;
		}
		DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(def.DiseaseCellVisName);
		if (info.name != null)
		{
			diseaseSourceSprite = Assets.instance.DiseaseVisualization.overlaySprite;
			diseaseSourceColour = info.overlayColour;
		}
		ISecondaryInput component = def.BuildingComplete.GetComponent<ISecondaryInput>();
		if (component != null)
		{
			switch (component.GetSecondaryConduitType())
			{
			case ConduitType.Gas:
				secondary_ports |= Ports.GasIn;
				break;
			case ConduitType.Liquid:
				secondary_ports |= Ports.LiquidIn;
				break;
			}
		}
		ISecondaryOutput component2 = def.BuildingComplete.GetComponent<ISecondaryOutput>();
		if (component2 != null)
		{
			switch (component2.GetSecondaryConduitType())
			{
			case ConduitType.Gas:
				secondary_ports |= Ports.GasOut;
				break;
			case ConduitType.Liquid:
				secondary_ports |= Ports.LiquidOut;
				break;
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if ((UnityEngine.Object)inputVisualizer != (UnityEngine.Object)null)
		{
			UnityEngine.Object.Destroy(inputVisualizer);
		}
		if ((UnityEngine.Object)outputVisualizer != (UnityEngine.Object)null)
		{
			UnityEngine.Object.Destroy(outputVisualizer);
		}
		if ((UnityEngine.Object)secondaryInputVisualizer != (UnityEngine.Object)null)
		{
			UnityEngine.Object.Destroy(secondaryInputVisualizer);
		}
		if ((UnityEngine.Object)secondaryOutputVisualizer != (UnityEngine.Object)null)
		{
			UnityEngine.Object.Destroy(secondaryOutputVisualizer);
		}
	}

	private Color GetWireColor(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 26];
		if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
		{
			KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
			return (!((UnityEngine.Object)component != (UnityEngine.Object)null)) ? Color.white : ((Color)component.TintColour);
		}
		return Color.white;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		MapBuilding();
		Components.BuildingCellVisualizers.Add(this);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		Components.BuildingCellVisualizers.Remove(this);
	}

	public void DrawIcons(HashedString mode)
	{
		if (mode == OverlayModes.Power.ID)
		{
			if (RequiresPower)
			{
				bool flag = (UnityEngine.Object)(building as BuildingPreview) != (UnityEngine.Object)null;
				BuildingEnabledButton component = building.GetComponent<BuildingEnabledButton>();
				int powerInputCell = building.GetPowerInputCell();
				if (RequiresPowerInput)
				{
					int circuitID = Game.Instance.circuitManager.GetCircuitID(powerInputCell);
					Color tint = (!((UnityEngine.Object)component != (UnityEngine.Object)null) || component.IsEnabled) ? Color.white : Color.gray;
					Sprite icon_img = (flag || circuitID == 65535) ? resources.electricityInputIcon : resources.electricityConnectedIcon;
					DrawUtilityIcon(powerInputCell, icon_img, ref inputVisualizer, tint, GetWireColor(powerInputCell), 1f, false);
				}
				if (RequiresPowerOutput)
				{
					int powerOutputCell = building.GetPowerOutputCell();
					int circuitID2 = Game.Instance.circuitManager.GetCircuitID(powerOutputCell);
					Color color = (!building.Def.UseWhitePowerOutputConnectorColour) ? resources.electricityOutputColor : Color.white;
					Color32 c = (!((UnityEngine.Object)component != (UnityEngine.Object)null) || component.IsEnabled) ? color : Color.gray;
					Sprite icon_img2 = (flag || circuitID2 == 65535) ? resources.electricityInputIcon : resources.electricityConnectedIcon;
					DrawUtilityIcon(powerOutputCell, icon_img2, ref outputVisualizer, c, GetWireColor(powerOutputCell), 1f, false);
				}
			}
			else
			{
				bool flag2 = true;
				Switch component2 = GetComponent<Switch>();
				if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
				{
					int cell = Grid.PosToCell(base.transform.GetPosition());
					Color32 c2 = (!component2.IsHandlerOn()) ? resources.switchOffColor : resources.switchColor;
					DrawUtilityIcon(cell, resources.switchIcon, ref outputVisualizer, c2, Color.white, 1f, false);
					flag2 = false;
				}
				else
				{
					WireUtilityNetworkLink component3 = GetComponent<WireUtilityNetworkLink>();
					if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
					{
						component3.GetCells(out int linked_cell, out int linked_cell2);
						DrawUtilityIcon(linked_cell, (Game.Instance.circuitManager.GetCircuitID(linked_cell) != 65535) ? resources.electricityConnectedIcon : resources.electricityBridgeIcon, ref inputVisualizer, resources.electricityInputColor, Color.white, 1f, false);
						DrawUtilityIcon(linked_cell2, (Game.Instance.circuitManager.GetCircuitID(linked_cell2) != 65535) ? resources.electricityConnectedIcon : resources.electricityBridgeIcon, ref outputVisualizer, resources.electricityInputColor, Color.white, 1f, false);
						flag2 = false;
					}
				}
				if (flag2)
				{
					DisableIcons();
				}
			}
		}
		else if (mode == OverlayModes.GasConduits.ID)
		{
			if (RequiresGas || (secondary_ports & (Ports.GasIn | Ports.GasOut)) != 0)
			{
				if ((ports & Ports.GasIn) != 0)
				{
					bool flag3 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityInputCell(), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input = resources.gasIOColours.input;
					Color tint2 = (!flag3) ? input.disconnected : input.connected;
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.gasInputIcon, ref inputVisualizer, tint2);
				}
				if ((ports & Ports.GasOut) != 0)
				{
					bool flag4 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityOutputCell(), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output = resources.gasIOColours.output;
					Color tint3 = (!flag4) ? output.disconnected : output.connected;
					DrawUtilityIcon(building.GetUtilityOutputCell(), resources.gasOutputIcon, ref outputVisualizer, tint3);
				}
				if ((secondary_ports & Ports.GasIn) != 0)
				{
					CellOffset secondaryConduitOffset = building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
					int visualizerCell = GetVisualizerCell(building, secondaryConduitOffset);
					DrawUtilityIcon(visualizerCell, resources.gasInputIcon, ref secondaryInputVisualizer, secondInputColour, Color.white, 1.5f, false);
				}
				if ((secondary_ports & Ports.GasOut) != 0)
				{
					CellOffset secondaryConduitOffset2 = building.GetComponent<ISecondaryOutput>().GetSecondaryConduitOffset();
					int visualizerCell2 = GetVisualizerCell(building, secondaryConduitOffset2);
					DrawUtilityIcon(visualizerCell2, resources.gasOutputIcon, ref secondaryOutputVisualizer, secondOutputColour, Color.white, 1.5f, false);
				}
			}
			else
			{
				DisableIcons();
			}
		}
		else if (mode == OverlayModes.LiquidConduits.ID)
		{
			if (RequiresLiquid || (secondary_ports & (Ports.LiquidIn | Ports.LiquidOut)) != 0)
			{
				if ((ports & Ports.LiquidIn) != 0)
				{
					bool flag5 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityInputCell(), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input2 = resources.liquidIOColours.input;
					Color tint4 = (!flag5) ? input2.disconnected : input2.connected;
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.liquidInputIcon, ref inputVisualizer, tint4);
				}
				if ((ports & Ports.LiquidOut) != 0)
				{
					bool flag6 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityOutputCell(), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output2 = resources.liquidIOColours.output;
					Color tint5 = (!flag6) ? output2.disconnected : output2.connected;
					DrawUtilityIcon(building.GetUtilityOutputCell(), resources.liquidOutputIcon, ref outputVisualizer, tint5);
				}
				if ((secondary_ports & Ports.LiquidIn) != 0)
				{
					CellOffset secondaryConduitOffset3 = building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
					int visualizerCell3 = GetVisualizerCell(building, secondaryConduitOffset3);
					DrawUtilityIcon(visualizerCell3, resources.liquidInputIcon, ref secondaryInputVisualizer, secondInputColour, Color.white, 1.5f, false);
				}
				if ((secondary_ports & Ports.LiquidOut) != 0)
				{
					CellOffset secondaryConduitOffset4 = building.GetComponent<ISecondaryOutput>().GetSecondaryConduitOffset();
					int visualizerCell4 = GetVisualizerCell(building, secondaryConduitOffset4);
					DrawUtilityIcon(visualizerCell4, resources.liquidOutputIcon, ref secondaryOutputVisualizer, secondOutputColour, Color.white, 1.5f, false);
				}
			}
			else
			{
				DisableIcons();
			}
		}
		else if (mode == OverlayModes.SolidConveyor.ID)
		{
			if (RequiresSolid)
			{
				if ((ports & Ports.SolidIn) != 0)
				{
					bool flag7 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityInputCell(), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input3 = resources.liquidIOColours.input;
					Color tint6 = (!flag7) ? input3.disconnected : input3.connected;
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.liquidInputIcon, ref inputVisualizer, tint6);
				}
				if ((ports & Ports.SolidOut) != 0)
				{
					bool flag8 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityOutputCell(), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output3 = resources.liquidIOColours.output;
					Color tint7 = (!flag8) ? output3.disconnected : output3.connected;
					DrawUtilityIcon(building.GetUtilityOutputCell(), resources.liquidOutputIcon, ref outputVisualizer, tint7);
				}
			}
			else
			{
				DisableIcons();
			}
		}
		else if (mode == OverlayModes.Disease.ID && (UnityEngine.Object)diseaseSourceSprite != (UnityEngine.Object)null)
		{
			int utilityOutputCell = building.GetUtilityOutputCell();
			DrawUtilityIcon(utilityOutputCell, diseaseSourceSprite, ref inputVisualizer, diseaseSourceColour);
		}
	}

	private int GetVisualizerCell(Building building, CellOffset offset)
	{
		CellOffset rotatedOffset = building.GetRotatedOffset(offset);
		int cell = building.GetCell();
		return Grid.OffsetCell(cell, rotatedOffset);
	}

	public void DisableIcons()
	{
		if ((UnityEngine.Object)inputVisualizer != (UnityEngine.Object)null && inputVisualizer.activeInHierarchy)
		{
			inputVisualizer.SetActive(false);
		}
		if ((UnityEngine.Object)outputVisualizer != (UnityEngine.Object)null && outputVisualizer.activeInHierarchy)
		{
			outputVisualizer.SetActive(false);
		}
		if ((UnityEngine.Object)secondaryInputVisualizer != (UnityEngine.Object)null && secondaryInputVisualizer.activeInHierarchy)
		{
			secondaryInputVisualizer.SetActive(false);
		}
		if ((UnityEngine.Object)secondaryOutputVisualizer != (UnityEngine.Object)null && secondaryOutputVisualizer.activeInHierarchy)
		{
			secondaryOutputVisualizer.SetActive(false);
		}
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj)
	{
		DrawUtilityIcon(cell, icon_img, ref visualizerObj, Color.white, Color.white, 1.5f, false);
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj, Color tint)
	{
		DrawUtilityIcon(cell, icon_img, ref visualizerObj, tint, Color.white, 1.5f, false);
	}

	private void DrawUtilityIcon(int cell, Sprite icon_img, ref GameObject visualizerObj, Color tint, Color connectorColor, float scaleMultiplier = 1.5f, bool hideBG = false)
	{
		Vector3 position = Grid.CellToPosCCC(cell, Grid.SceneLayer.Building);
		if ((UnityEngine.Object)visualizerObj == (UnityEngine.Object)null)
		{
			visualizerObj = Util.KInstantiate(Assets.UIPrefabs.ResourceVisualizer, GameScreenManager.Instance.worldSpaceCanvas, null);
			visualizerObj.transform.SetAsFirstSibling();
			icons.Add(visualizerObj, visualizerObj.transform.GetChild(0).GetComponent<Image>());
		}
		if (!visualizerObj.gameObject.activeInHierarchy)
		{
			visualizerObj.gameObject.SetActive(true);
		}
		Image component = visualizerObj.GetComponent<Image>();
		component.enabled = !hideBG;
		icons[visualizerObj].raycastTarget = enableRaycast;
		icons[visualizerObj].sprite = icon_img;
		Transform child = visualizerObj.transform.GetChild(0);
		component = child.gameObject.GetComponent<Image>();
		component.color = tint;
		visualizerObj.transform.SetPosition(position);
		if ((UnityEngine.Object)visualizerObj.GetComponent<SizePulse>() == (UnityEngine.Object)null)
		{
			visualizerObj.transform.localScale = Vector3.one * scaleMultiplier;
		}
	}

	public Image GetOutputIcon()
	{
		return (!((UnityEngine.Object)outputVisualizer == (UnityEngine.Object)null)) ? outputVisualizer.transform.GetChild(0).GetComponent<Image>() : null;
	}

	public Image GetInputIcon()
	{
		return (!((UnityEngine.Object)inputVisualizer == (UnityEngine.Object)null)) ? inputVisualizer.transform.GetChild(0).GetComponent<Image>() : null;
	}
}
