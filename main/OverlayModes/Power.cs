using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverlayModes
{
	public class Power : Mode
	{
		private struct UpdatePowerInfo
		{
			public KMonoBehaviour item;

			public LocText powerLabel;

			public LocText unitLabel;

			public Generator generator;

			public IEnergyConsumer consumer;

			public UpdatePowerInfo(KMonoBehaviour item, LocText power_label, LocText unit_label, Generator g, IEnergyConsumer c)
			{
				this.item = item;
				powerLabel = power_label;
				unitLabel = unit_label;
				generator = g;
				consumer = c;
			}
		}

		private struct UpdateBatteryInfo
		{
			public Battery battery;

			public BatteryUI ui;

			public UpdateBatteryInfo(Battery battery, BatteryUI ui)
			{
				this.battery = battery;
				this.ui = ui;
			}
		}

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private List<UpdatePowerInfo> updatePowerInfo = new List<UpdatePowerInfo>();

		private List<UpdateBatteryInfo> updateBatteryInfo = new List<UpdateBatteryInfo>();

		private Canvas powerLabelParent;

		private LocText powerLabelPrefab;

		private Vector3 powerLabelOffset;

		private BatteryUI batteryUIPrefab;

		private Vector3 batteryUIOffset;

		private Vector3 batteryUITransformerOffset;

		private Vector3 batteryUISmallTransformerOffset;

		private Color32 consumerColour;

		private Color32 generatorColour;

		private Color32 buildingDisabledColour;

		private Color32 circuitUnpoweredColour;

		private Color32 circuitSafeColour;

		private Color32 circuitStrainingColour;

		private int freePowerLabelIdx;

		private int freeBatteryUIIdx;

		private List<LocText> powerLabels = new List<LocText>();

		private List<BatteryUI> batteryUIList = new List<BatteryUI>();

		private UniformGrid<SaveLoadRoot> partition;

		private List<SaveLoadRoot> queuedAdds = new List<SaveLoadRoot>();

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private HashSet<SaveLoadRoot> privateTargets = new HashSet<SaveLoadRoot>();

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		public Power(Canvas powerLabelParent, LocText powerLabelPrefab, BatteryUI batteryUIPrefab, Vector3 powerLabelOffset, Vector3 batteryUIOffset, Vector3 batteryUITransformerOffset, Vector3 batteryUISmallTransformerOffset, Color consumerColour, Color generatorColour, Color buildingDisabledColour, Color32 circuitUnpoweredColour, Color32 circuitSafeColour, Color32 circuitStrainingColour)
		{
			this.powerLabelParent = powerLabelParent;
			this.powerLabelPrefab = powerLabelPrefab;
			this.batteryUIPrefab = batteryUIPrefab;
			this.powerLabelOffset = powerLabelOffset;
			this.batteryUIOffset = batteryUIOffset;
			this.batteryUITransformerOffset = batteryUITransformerOffset;
			this.batteryUISmallTransformerOffset = batteryUISmallTransformerOffset;
			this.consumerColour = consumerColour;
			this.generatorColour = generatorColour;
			this.buildingDisabledColour = buildingDisabledColour;
			this.circuitUnpoweredColour = circuitUnpoweredColour;
			this.circuitSafeColour = circuitSafeColour;
			this.circuitStrainingColour = circuitStrainingColour;
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.PowerMap;
		}

		public override string GetSoundName()
		{
			return "Power";
		}

		public override void Enable()
		{
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			DragTool.SetLayerMask(selectionMask);
			RegisterSaveLoadListeners();
			partition = Mode.PopulatePartition<SaveLoadRoot>(OverlayScreen.WireIDs);
			GridCompositor.Instance.ToggleMinor(true);
		}

		public override void Disable()
		{
			Mode.ResetDisplayValues(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			DragTool.ClearLayerMask();
			UnregisterSaveLoadListeners();
			partition.Clear();
			layerTargets.Clear();
			privateTargets.Clear();
			queuedAdds.Clear();
			DisablePowerLabels();
			DisableBatteryUIs();
			GridCompositor.Instance.ToggleMinor(false);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (OverlayScreen.WireIDs.Contains(saveLoadTag))
			{
				partition.Add(item);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null) && !((UnityEngine.Object)item.gameObject == (UnityEngine.Object)null))
			{
				if (layerTargets.Contains(item))
				{
					layerTargets.Remove(item);
				}
				partition.Remove(item);
			}
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max, null);
			using (new KProfiler.Region("UpdatePowerOverlay", null))
			{
				IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2((float)min.x, (float)min.y), new Vector2((float)max.x, (float)max.y));
				IEnumerator enumerator = allIntersecting.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						SaveLoadRoot instance = (SaveLoadRoot)enumerator.Current;
						AddTargetIfVisible(instance, min, max, layerTargets, targetLayer, null, null);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				connectedNetworks.Clear();
				float num = 1f;
				GameObject gameObject = null;
				if ((UnityEngine.Object)SelectTool.Instance != (UnityEngine.Object)null && (UnityEngine.Object)SelectTool.Instance.hover != (UnityEngine.Object)null)
				{
					gameObject = SelectTool.Instance.hover.gameObject;
				}
				if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
				{
					IBridgedNetworkItem component = gameObject.GetComponent<IBridgedNetworkItem>();
					if (component != null)
					{
						int networkCell = component.GetNetworkCell();
						visited.Clear();
						FindConnectedNetworks(networkCell, Game.Instance.electricalConduitSystem, connectedNetworks, visited);
						visited.Clear();
						num = ModeUtil.GetHighlightScale();
					}
				}
				CircuitManager circuitManager = Game.Instance.circuitManager;
				foreach (SaveLoadRoot layerTarget in layerTargets)
				{
					if (!((UnityEngine.Object)layerTarget == (UnityEngine.Object)null))
					{
						IBridgedNetworkItem component2 = layerTarget.GetComponent<IBridgedNetworkItem>();
						if (component2 != null)
						{
							KMonoBehaviour kMonoBehaviour = component2 as KMonoBehaviour;
							KBatchedAnimController component3 = kMonoBehaviour.GetComponent<KBatchedAnimController>();
							int networkCell2 = component2.GetNetworkCell();
							UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(networkCell2);
							ushort circuitID = (ushort)((networkForCell == null) ? 65535 : ((ushort)networkForCell.id));
							Color32 tintColour;
							if (circuitManager.HasGenerators(circuitID) || circuitManager.HasBatteries(circuitID))
							{
								float potentialWattsGeneratedByCircuit = circuitManager.GetPotentialWattsGeneratedByCircuit(circuitID);
								float wattsUsedByCircuit = circuitManager.GetWattsUsedByCircuit(circuitID);
								float num2 = wattsUsedByCircuit / potentialWattsGeneratedByCircuit;
								tintColour = ((!(num2 < 0.85f)) ? circuitStrainingColour : circuitSafeColour);
							}
							else
							{
								tintColour = circuitUnpoweredColour;
							}
							if (connectedNetworks.Count > 0 && component2.IsConnectedToNetworks(connectedNetworks))
							{
								tintColour.r = (byte)((float)(int)tintColour.r * num);
								tintColour.g = (byte)((float)(int)tintColour.g * num);
								tintColour.b = (byte)((float)(int)tintColour.b * num);
							}
							component3.TintColour = tintColour;
						}
					}
				}
			}
			queuedAdds.Clear();
			using (new KProfiler.Region("BatteryUI", null))
			{
				foreach (Battery item in Components.Batteries.Items)
				{
					Vector2I vector2I = Grid.PosToXY(item.transform.GetPosition());
					if (min <= vector2I && vector2I <= max)
					{
						SaveLoadRoot component4 = item.GetComponent<SaveLoadRoot>();
						if (!privateTargets.Contains(component4))
						{
							AddBatteryUI(item);
							queuedAdds.Add(component4);
						}
					}
				}
				foreach (Generator item2 in Components.Generators.Items)
				{
					Vector2I vector2I2 = Grid.PosToXY(item2.transform.GetPosition());
					if (min <= vector2I2 && vector2I2 <= max)
					{
						SaveLoadRoot component5 = item2.GetComponent<SaveLoadRoot>();
						if (!privateTargets.Contains(component5))
						{
							privateTargets.Add(component5);
							if ((UnityEngine.Object)item2.GetComponent<PowerTransformer>() == (UnityEngine.Object)null)
							{
								AddPowerLabels(item2);
							}
						}
					}
				}
				foreach (EnergyConsumer item3 in Components.EnergyConsumers.Items)
				{
					Vector2I vector2I3 = Grid.PosToXY(item3.transform.GetPosition());
					if (min <= vector2I3 && vector2I3 <= max)
					{
						SaveLoadRoot component6 = item3.GetComponent<SaveLoadRoot>();
						if (!privateTargets.Contains(component6))
						{
							privateTargets.Add(component6);
							AddPowerLabels(item3);
						}
					}
				}
			}
			foreach (SaveLoadRoot queuedAdd in queuedAdds)
			{
				privateTargets.Add(queuedAdd);
			}
			queuedAdds.Clear();
			UpdatePowerLabels();
		}

		private LocText GetFreePowerLabel()
		{
			LocText locText = null;
			if (freePowerLabelIdx < powerLabels.Count)
			{
				locText = powerLabels[freePowerLabelIdx];
				freePowerLabelIdx++;
			}
			else
			{
				locText = Util.KInstantiateUI<LocText>(powerLabelPrefab.gameObject, powerLabelParent.transform.gameObject, false);
				powerLabels.Add(locText);
				freePowerLabelIdx++;
			}
			return locText;
		}

		private void UpdatePowerLabels()
		{
			foreach (UpdatePowerInfo item2 in updatePowerInfo)
			{
				UpdatePowerInfo current = item2;
				KMonoBehaviour item = current.item;
				LocText powerLabel = current.powerLabel;
				LocText unitLabel = current.unitLabel;
				Generator generator = current.generator;
				IEnergyConsumer consumer = current.consumer;
				if ((UnityEngine.Object)current.item == (UnityEngine.Object)null)
				{
					powerLabel.gameObject.SetActive(false);
				}
				else
				{
					if ((UnityEngine.Object)generator != (UnityEngine.Object)null && consumer == null)
					{
						int num = 2147483647;
						ManualGenerator component = generator.GetComponent<ManualGenerator>();
						if ((UnityEngine.Object)component == (UnityEngine.Object)null)
						{
							generator.GetComponent<Operational>();
							num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
						}
						else
						{
							num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
						}
						powerLabel.text = ((num == 0) ? num.ToString() : ("+" + num.ToString()));
						BuildingEnabledButton component2 = item.GetComponent<BuildingEnabledButton>();
						Color color3 = unitLabel.color = (powerLabel.color = ((!((UnityEngine.Object)component2 != (UnityEngine.Object)null) || component2.IsEnabled) ? generatorColour : buildingDisabledColour));
						Image outputIcon = generator.GetComponent<BuildingCellVisualizer>().GetOutputIcon();
						if ((UnityEngine.Object)outputIcon != (UnityEngine.Object)null)
						{
							outputIcon.color = color3;
						}
					}
					if (consumer != null)
					{
						BuildingEnabledButton component3 = item.GetComponent<BuildingEnabledButton>();
						Color color4 = (!((UnityEngine.Object)component3 != (UnityEngine.Object)null) || component3.IsEnabled) ? consumerColour : buildingDisabledColour;
						int num2 = Mathf.Max(0, Mathf.RoundToInt(consumer.WattsNeededWhenActive));
						string text = num2.ToString();
						powerLabel.text = ((num2 == 0) ? text : ("-" + text));
						powerLabel.color = color4;
						unitLabel.color = color4;
						Image inputIcon = item.GetComponentInChildren<BuildingCellVisualizer>().GetInputIcon();
						if ((UnityEngine.Object)inputIcon != (UnityEngine.Object)null)
						{
							inputIcon.color = color4;
						}
					}
				}
			}
			foreach (UpdateBatteryInfo item3 in updateBatteryInfo)
			{
				UpdateBatteryInfo current2 = item3;
				current2.ui.SetContent(current2.battery);
			}
		}

		private void AddPowerLabels(KMonoBehaviour item)
		{
			IEnergyConsumer componentInChildren = item.gameObject.GetComponentInChildren<IEnergyConsumer>();
			Generator componentInChildren2 = item.gameObject.GetComponentInChildren<Generator>();
			if (componentInChildren != null || (UnityEngine.Object)componentInChildren2 != (UnityEngine.Object)null)
			{
				float num = -10f;
				if ((UnityEngine.Object)componentInChildren2 != (UnityEngine.Object)null)
				{
					LocText freePowerLabel = GetFreePowerLabel();
					freePowerLabel.gameObject.SetActive(true);
					freePowerLabel.gameObject.name = item.gameObject.name + "power label";
					LocText component = freePowerLabel.transform.GetChild(0).GetComponent<LocText>();
					component.gameObject.SetActive(true);
					freePowerLabel.enabled = true;
					component.enabled = true;
					Vector3 a = Grid.CellToPos(componentInChildren2.PowerCell, 0.5f, 0f, 0f);
					freePowerLabel.rectTransform.SetPosition(a + powerLabelOffset + Vector3.up * (num * 0.02f));
					if (componentInChildren != null && componentInChildren.PowerCell == componentInChildren2.PowerCell)
					{
						num -= 15f;
					}
					SetToolTip(freePowerLabel, UI.OVERLAYS.POWER.WATTS_GENERATED);
					updatePowerInfo.Add(new UpdatePowerInfo(item, freePowerLabel, component, componentInChildren2, null));
				}
				if (componentInChildren != null && componentInChildren.GetType() != typeof(Battery))
				{
					LocText freePowerLabel2 = GetFreePowerLabel();
					LocText component2 = freePowerLabel2.transform.GetChild(0).GetComponent<LocText>();
					freePowerLabel2.gameObject.SetActive(true);
					component2.gameObject.SetActive(true);
					freePowerLabel2.gameObject.name = item.gameObject.name + "power label";
					freePowerLabel2.enabled = true;
					component2.enabled = true;
					Vector3 a2 = Grid.CellToPos(componentInChildren.PowerCell, 0.5f, 0f, 0f);
					freePowerLabel2.rectTransform.SetPosition(a2 + powerLabelOffset + Vector3.up * (num * 0.02f));
					SetToolTip(freePowerLabel2, UI.OVERLAYS.POWER.WATTS_CONSUMED);
					updatePowerInfo.Add(new UpdatePowerInfo(item, freePowerLabel2, component2, null, componentInChildren));
				}
			}
		}

		private void DisablePowerLabels()
		{
			freePowerLabelIdx = 0;
			foreach (LocText powerLabel in powerLabels)
			{
				powerLabel.gameObject.SetActive(false);
			}
			updatePowerInfo.Clear();
		}

		private void AddBatteryUI(Battery bat)
		{
			BatteryUI freeBatteryUI = GetFreeBatteryUI();
			freeBatteryUI.SetContent(bat);
			Vector3 b = Grid.CellToPos(bat.PowerCell, 0.5f, 0f, 0f);
			bool flag = (UnityEngine.Object)bat.powerTransformer != (UnityEngine.Object)null;
			float num = 1f;
			Rotatable component = bat.GetComponent<Rotatable>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.GetVisualizerFlipX())
			{
				num = -1f;
			}
			Vector3 b2 = batteryUIOffset;
			if (flag)
			{
				int widthInCells = bat.GetComponent<Building>().Def.WidthInCells;
				b2 = ((widthInCells != 2) ? batteryUITransformerOffset : batteryUISmallTransformerOffset);
			}
			b2.x *= num;
			freeBatteryUI.GetComponent<RectTransform>().SetPosition(Vector3.up + b + b2);
			updateBatteryInfo.Add(new UpdateBatteryInfo(bat, freeBatteryUI));
		}

		private void SetToolTip(LocText label, string text)
		{
			ToolTip component = label.GetComponent<ToolTip>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				component.toolTip = text;
			}
		}

		private void DisableBatteryUIs()
		{
			freeBatteryUIIdx = 0;
			foreach (BatteryUI batteryUI in batteryUIList)
			{
				batteryUI.gameObject.SetActive(false);
			}
			updateBatteryInfo.Clear();
		}

		private BatteryUI GetFreeBatteryUI()
		{
			BatteryUI batteryUI = null;
			if (freeBatteryUIIdx < batteryUIList.Count)
			{
				batteryUI = batteryUIList[freeBatteryUIIdx];
				batteryUI.gameObject.SetActive(true);
				freeBatteryUIIdx++;
			}
			else
			{
				batteryUI = Util.KInstantiateUI<BatteryUI>(batteryUIPrefab.gameObject, powerLabelParent.transform.gameObject, false);
				batteryUIList.Add(batteryUI);
				freeBatteryUIIdx++;
			}
			return batteryUI;
		}

		private void FindConnectedNetworks(int cell, IUtilityNetworkMgr mgr, ICollection<UtilityNetwork> networks, List<int> visited)
		{
			if (!visited.Contains(cell))
			{
				visited.Add(cell);
				UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
				if (networkForCell != null)
				{
					networks.Add(networkForCell);
					UtilityConnections connections = mgr.GetConnections(cell, false);
					if ((connections & UtilityConnections.Right) != 0)
					{
						FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
					}
					if ((connections & UtilityConnections.Left) != 0)
					{
						FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
					}
					if ((connections & UtilityConnections.Up) != 0)
					{
						FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
					}
					if ((connections & UtilityConnections.Down) != 0)
					{
						FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
					}
				}
			}
		}
	}
}
