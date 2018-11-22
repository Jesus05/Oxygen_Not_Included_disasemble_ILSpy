using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SkipSaveFileSerialization]
public class BuildingCellVisualizer : KMonoBehaviour
{
	private BuildingCellVisualizerResources resources;

	[MyCmpReq]
	private Building building;

	[SerializeField]
	public static Color32 secondOutputColour = new Color(0.9843137f, 0.6901961f, 0.23137255f);

	[SerializeField]
	public static Color32 secondInputColour = new Color(0.9843137f, 0.6901961f, 0.23137255f);

	private bool requiresPowerInput;

	private bool requiresPowerOutput;

	private bool requiresGasInput;

	private bool requiresGasOutput;

	private bool requiresLiquidInput;

	private bool requiresLiquidOutput;

	private bool requiresSolidInput;

	private bool requiresSolidOutput;

	private Sprite diseaseSourceSprite;

	private Color32 diseaseSourceColour;

	private bool requiresSecondGasInput;

	private bool requiresSecondGasOutput;

	private bool requiresSecondLiquidInput;

	private bool requiresSecondLiquidOutput;

	private GameObject inputVisualizer;

	private GameObject outputVisualizer;

	private GameObject secondaryInputVisualizer;

	private GameObject secondaryOutputVisualizer;

	private bool enableRaycast;

	private Dictionary<GameObject, Image> icons;

	private HashedString previousMode;

	private static readonly EventSystem.IntraObjectHandler<BuildingCellVisualizer> OnBuildingUpgradedDelegate = new EventSystem.IntraObjectHandler<BuildingCellVisualizer>(delegate(BuildingCellVisualizer component, object data)
	{
		component.OnBuildingUpgraded(data);
	});

	public bool RequiresPowerInput => requiresPowerInput;

	public bool RequiresPowerOutput => requiresPowerOutput;

	public bool RequiresPower => requiresPowerInput || requiresPowerOutput;

	public bool RequiresGas => requiresGasInput || requiresGasOutput;

	public bool RequiresLiquid => requiresLiquidInput || requiresLiquidOutput;

	public bool RequiresSolid => requiresSolidInput || requiresSolidOutput;

	public bool RequiresUtilityConnection => RequiresGas || RequiresLiquid || RequiresSolid;

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
		RefreshState();
		Subscribe(-235298596, OnBuildingUpgradedDelegate);
	}

	private void OnBuildingUpgraded(object data)
	{
		RefreshState();
	}

	private void RefreshState()
	{
		BuildingDef def = building.Def;
		requiresPowerInput = CheckRequiresPowerInput(def);
		requiresPowerOutput = CheckRequiresPowerOutput(def);
		requiresGasInput = CheckRequiresGasInput(def);
		requiresGasOutput = CheckRequiresGasOutput(def);
		requiresLiquidInput = CheckRequiresLiquidInput(def);
		requiresLiquidOutput = CheckRequiresLiquidOutput(def);
		requiresSolidInput = CheckRequiresSolidInput(def);
		requiresSolidOutput = CheckRequiresSolidOutput(def);
		DiseaseVisualization.Info info = Assets.instance.DiseaseVisualization.GetInfo(def.DiseaseCellVisName);
		if (info.name != null)
		{
			diseaseSourceSprite = Assets.instance.DiseaseVisualization.overlaySprite;
			diseaseSourceColour = info.overlayColour;
		}
		ISecondaryInput component = def.BuildingComplete.GetComponent<ISecondaryInput>();
		if (component != null)
		{
			ConduitType secondaryConduitType = component.GetSecondaryConduitType();
			requiresSecondGasInput = (secondaryConduitType == ConduitType.Gas);
			requiresSecondLiquidInput = (secondaryConduitType == ConduitType.Liquid);
		}
		ISecondaryOutput component2 = def.BuildingComplete.GetComponent<ISecondaryOutput>();
		if (component2 != null)
		{
			ConduitType secondaryConduitType2 = component2.GetSecondaryConduitType();
			requiresSecondGasOutput = (secondaryConduitType2 == ConduitType.Gas);
			requiresSecondLiquidOutput = (secondaryConduitType2 == ConduitType.Liquid);
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

	public static bool CheckRequiresComponent(BuildingDef def)
	{
		return CheckRequiresPowerInput(def) || CheckRequiresPowerOutput(def) || CheckRequiresGasInput(def) || CheckRequiresGasOutput(def) || CheckRequiresLiquidInput(def) || CheckRequiresLiquidOutput(def) || CheckRequiresSolidInput(def) || CheckRequiresSolidOutput(def) || def.DiseaseCellVisName != null;
	}

	public static bool CheckRequiresPowerInput(BuildingDef def)
	{
		return def.RequiresPowerInput;
	}

	public static bool CheckRequiresPowerOutput(BuildingDef def)
	{
		return def.GeneratorWattageRating > 0f || def.RequiresPowerOutput;
	}

	public static bool CheckRequiresGasInput(BuildingDef def)
	{
		return def.InputConduitType == ConduitType.Gas;
	}

	public static bool CheckRequiresGasOutput(BuildingDef def)
	{
		return def.OutputConduitType == ConduitType.Gas;
	}

	public static bool CheckRequiresLiquidInput(BuildingDef def)
	{
		return def.InputConduitType == ConduitType.Liquid;
	}

	public static bool CheckRequiresLiquidOutput(BuildingDef def)
	{
		return def.OutputConduitType == ConduitType.Liquid;
	}

	public static bool CheckRequiresSolidInput(BuildingDef def)
	{
		return def.InputConduitType == ConduitType.Solid;
	}

	public static bool CheckRequiresSolidOutput(BuildingDef def)
	{
		return def.OutputConduitType == ConduitType.Solid;
	}

	private bool CompareWireConnection(int cell, UtilityConnections[] connections)
	{
		GameObject gameObject = Grid.Objects[cell, 24];
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			Wire component = gameObject.GetComponent<Wire>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				for (int i = 0; i < connections.Length; i++)
				{
					UtilityConnections wireConnections = component.GetWireConnections();
					if ((wireConnections & connections[i]) != 0)
					{
						return false;
					}
				}
			}
		}
		return false;
	}

	private Color GetWireColor(int cell)
	{
		Color result = Color.white;
		GameObject gameObject = Grid.Objects[cell, 24];
		if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
		{
			KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				result = component.TintColour;
			}
		}
		return result;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		Components.BuildingCellVisualizers.Add(this);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		Components.BuildingCellVisualizers.Remove(this);
	}

	public void Tick(HashedString mode)
	{
		if (mode != previousMode)
		{
			DisableIcons();
		}
		if (mode == OverlayModes.Power.ID)
		{
			if (requiresPowerInput || requiresPowerOutput)
			{
				bool flag = (UnityEngine.Object)(building as BuildingPreview) != (UnityEngine.Object)null;
				BuildingEnabledButton component = building.GetComponent<BuildingEnabledButton>();
				int powerInputCell = building.GetPowerInputCell();
				if (requiresPowerInput)
				{
					int circuitID = Game.Instance.circuitManager.GetCircuitID(powerInputCell);
					Color tint = (!((UnityEngine.Object)component != (UnityEngine.Object)null) || component.IsEnabled) ? Color.white : Color.gray;
					Sprite icon_img = (flag || circuitID == 65535) ? resources.electricityInputIcon : resources.electricityConnectedIcon;
					DrawUtilityIcon(powerInputCell, icon_img, ref inputVisualizer, tint, GetWireColor(powerInputCell), 1f, false);
				}
				if (requiresPowerOutput)
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
			if (requiresGasInput || requiresGasOutput || requiresSecondGasOutput || requiresSecondGasInput)
			{
				if (requiresGasInput)
				{
					bool flag3 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityInputCell(), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input = resources.gasIOColours.input;
					Color tint2 = (!flag3) ? input.disconnected : input.connected;
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.gasInputIcon, ref inputVisualizer, tint2);
				}
				if (requiresGasOutput)
				{
					bool flag4 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityOutputCell(), 12];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output = resources.gasIOColours.output;
					Color tint3 = (!flag4) ? output.disconnected : output.connected;
					DrawUtilityIcon(building.GetUtilityOutputCell(), resources.gasOutputIcon, ref outputVisualizer, tint3);
				}
				if (requiresSecondGasInput)
				{
					CellOffset secondaryConduitOffset = building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
					int visualizerCell = GetVisualizerCell(building, secondaryConduitOffset);
					DrawUtilityIcon(visualizerCell, resources.gasInputIcon, ref secondaryInputVisualizer, secondInputColour, Color.white, 1.5f, false);
				}
				if (requiresSecondGasOutput)
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
			if (requiresLiquidInput || requiresLiquidOutput || requiresSecondLiquidOutput || requiresSecondLiquidInput)
			{
				if (requiresLiquidInput)
				{
					bool flag5 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityInputCell(), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input2 = resources.liquidIOColours.input;
					Color tint4 = (!flag5) ? input2.disconnected : input2.connected;
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.liquidInputIcon, ref inputVisualizer, tint4);
				}
				if (requiresLiquidOutput)
				{
					bool flag6 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityOutputCell(), 16];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours output2 = resources.liquidIOColours.output;
					Color tint5 = (!flag6) ? output2.disconnected : output2.connected;
					DrawUtilityIcon(building.GetUtilityOutputCell(), resources.liquidOutputIcon, ref outputVisualizer, tint5);
				}
				if (requiresSecondLiquidInput)
				{
					CellOffset secondaryConduitOffset3 = building.GetComponent<ISecondaryInput>().GetSecondaryConduitOffset();
					int visualizerCell3 = GetVisualizerCell(building, secondaryConduitOffset3);
					DrawUtilityIcon(visualizerCell3, resources.liquidInputIcon, ref secondaryInputVisualizer, secondInputColour, Color.white, 1.5f, false);
				}
				if (requiresSecondLiquidOutput)
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
			if (requiresSolidInput || requiresSolidOutput)
			{
				if (requiresSolidInput)
				{
					bool flag7 = (UnityEngine.Object)null != (UnityEngine.Object)Grid.Objects[building.GetUtilityInputCell(), 20];
					BuildingCellVisualizerResources.ConnectedDisconnectedColours input3 = resources.liquidIOColours.input;
					Color tint6 = (!flag7) ? input3.disconnected : input3.connected;
					DrawUtilityIcon(building.GetUtilityInputCell(), resources.liquidInputIcon, ref inputVisualizer, tint6);
				}
				if (requiresSolidOutput)
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
		else if (mode == OverlayModes.Disease.ID)
		{
			if ((UnityEngine.Object)diseaseSourceSprite != (UnityEngine.Object)null)
			{
				int utilityOutputCell = building.GetUtilityOutputCell();
				DrawUtilityIcon(utilityOutputCell, diseaseSourceSprite, ref inputVisualizer, diseaseSourceColour);
			}
		}
		else
		{
			DisableIcons();
		}
		previousMode = mode;
	}

	private int GetVisualizerCell(Building building, CellOffset offset)
	{
		CellOffset rotatedOffset = building.GetRotatedOffset(offset);
		int cell = building.GetCell();
		return Grid.OffsetCell(cell, rotatedOffset);
	}

	private void DisableIcons()
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
