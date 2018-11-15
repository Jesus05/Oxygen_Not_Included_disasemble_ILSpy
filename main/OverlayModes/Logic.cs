using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace OverlayModes
{
	public class Logic : Mode
	{
		private struct BridgeInfo
		{
			public int cell;

			public KBatchedAnimController controller;
		}

		private struct EventInfo
		{
			public HandleVector<int>.Handle uiHandle;
		}

		private struct UIInfo
		{
			public GameObject instance;

			public Image image;

			public int cell;

			public UIInfo(ILogicUIElement ui_elem, LogicModeUI ui_data)
			{
				cell = ui_elem.GetLogicUICell();
				instance = Util.KInstantiate(ui_data.prefab, Grid.CellToPosCCC(cell, Grid.SceneLayer.Front), Quaternion.identity, GameScreenManager.Instance.worldSpaceCanvas, null, true, 0);
				instance.SetActive(true);
				image = instance.GetComponent<Image>();
				image.raycastTarget = false;
				switch (ui_elem.GetLogicPortSpriteType())
				{
				case LogicPortSpriteType.Input:
					image.sprite = ui_data.inputSprite;
					break;
				case LogicPortSpriteType.Output:
					image.sprite = ui_data.outputSprite;
					break;
				case LogicPortSpriteType.ResetUpdate:
					image.sprite = ui_data.resetSprite;
					break;
				}
			}

			public void Release()
			{
				Util.KDestroyGameObject(instance);
			}
		}

		public static HashSet<Tag> HighlightItemIDs = new HashSet<Tag>();

		private int conduitTargetLayer;

		private int objectTargetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private UniformGrid<ILogicUIElement> ioPartition;

		private HashSet<ILogicUIElement> ioTargets = new HashSet<ILogicUIElement>();

		private HashSet<ILogicUIElement> workingIOTargets = new HashSet<ILogicUIElement>();

		private HashSet<KBatchedAnimController> wireControllers = new HashSet<KBatchedAnimController>();

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		private HashSet<BridgeInfo> bridgeControllers = new HashSet<BridgeInfo>();

		private UniformGrid<SaveLoadRoot> gameObjPartition;

		private HashSet<SaveLoadRoot> gameObjTargets = new HashSet<SaveLoadRoot>();

		private LogicModeUI uiAsset;

		private Dictionary<ILogicUIElement, EventInfo> uiNodes = new Dictionary<ILogicUIElement, EventInfo>();

		private KCompactedVector<UIInfo> uiInfo = new KCompactedVector<UIInfo>(0);

		public Logic(LogicModeUI ui_asset)
		{
			conduitTargetLayer = LayerMask.NameToLayer("MaskedOverlay");
			objectTargetLayer = LayerMask.NameToLayer("MaskedOverlayBG");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
			uiAsset = ui_asset;
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.Logic;
		}

		public override string GetSoundName()
		{
			return "Logic";
		}

		public override void Enable()
		{
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			DragTool.SetLayerMask(selectionMask);
			RegisterSaveLoadListeners();
			gameObjPartition = Mode.PopulatePartition<SaveLoadRoot>(HighlightItemIDs);
			ioPartition = CreateLogicUIPartition();
			GridCompositor.Instance.ToggleMinor(true);
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			logicCircuitManager.onElemAdded = (Action<ILogicUIElement>)Delegate.Combine(logicCircuitManager.onElemAdded, new Action<ILogicUIElement>(OnUIElemAdded));
			LogicCircuitManager logicCircuitManager2 = Game.Instance.logicCircuitManager;
			logicCircuitManager2.onElemRemoved = (Action<ILogicUIElement>)Delegate.Combine(logicCircuitManager2.onElemRemoved, new Action<ILogicUIElement>(OnUIElemRemoved));
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterLogicOn);
		}

		public override void Disable()
		{
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			logicCircuitManager.onElemAdded = (Action<ILogicUIElement>)Delegate.Remove(logicCircuitManager.onElemAdded, new Action<ILogicUIElement>(OnUIElemAdded));
			LogicCircuitManager logicCircuitManager2 = Game.Instance.logicCircuitManager;
			logicCircuitManager2.onElemRemoved = (Action<ILogicUIElement>)Delegate.Remove(logicCircuitManager2.onElemRemoved, new Action<ILogicUIElement>(OnUIElemRemoved));
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterLogicOn, STOP_MODE.ALLOWFADEOUT);
			foreach (SaveLoadRoot gameObjTarget in gameObjTargets)
			{
				float defaultDepth = Mode.GetDefaultDepth(gameObjTarget);
				Vector3 position = gameObjTarget.transform.GetPosition();
				position.z = defaultDepth;
				gameObjTarget.transform.SetPosition(position);
			}
			Mode.ResetDisplayValues(gameObjTargets);
			Mode.ResetDisplayValues(wireControllers);
			foreach (BridgeInfo bridgeController in bridgeControllers)
			{
				BridgeInfo current2 = bridgeController;
				if ((UnityEngine.Object)current2.controller != (UnityEngine.Object)null)
				{
					Mode.ResetDisplayValues(current2.controller);
				}
			}
			Camera.main.cullingMask &= ~cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			DragTool.ClearLayerMask();
			UnregisterSaveLoadListeners();
			foreach (UIInfo data in uiInfo.GetDataList())
			{
				data.Release();
			}
			uiInfo.Clear();
			uiNodes.Clear();
			ioPartition.Clear();
			ioTargets.Clear();
			gameObjPartition.Clear();
			gameObjTargets.Clear();
			wireControllers.Clear();
			bridgeControllers.Clear();
			GridCompositor.Instance.ToggleMinor(false);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (HighlightItemIDs.Contains(saveLoadTag))
			{
				gameObjPartition.Add(item);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null) && !((UnityEngine.Object)item.gameObject == (UnityEngine.Object)null))
			{
				if (gameObjTargets.Contains(item))
				{
					gameObjTargets.Remove(item);
				}
				gameObjPartition.Remove(item);
			}
		}

		private void OnUIElemAdded(ILogicUIElement elem)
		{
			ioPartition.Add(elem);
		}

		private void OnUIElemRemoved(ILogicUIElement elem)
		{
			ioPartition.Remove(elem);
			if (ioTargets.Contains(elem))
			{
				ioTargets.Remove(elem);
				FreeUI(elem);
			}
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			Tag wire_id = TagManager.Create("LogicWire");
			Tag bridge_id = TagManager.Create("LogicWireBridge");
			Mode.RemoveOffscreenTargets(gameObjTargets, min, max, delegate(SaveLoadRoot root)
			{
				if (!((UnityEngine.Object)root == (UnityEngine.Object)null))
				{
					KPrefabID component9 = root.GetComponent<KPrefabID>();
					if ((UnityEngine.Object)component9 != (UnityEngine.Object)null)
					{
						Tag prefabTag = component9.PrefabTag;
						if (prefabTag == wire_id)
						{
							wireControllers.Remove(root.GetComponent<KBatchedAnimController>());
						}
						else if (prefabTag == bridge_id)
						{
							KBatchedAnimController controller = root.GetComponent<KBatchedAnimController>();
							bridgeControllers.RemoveWhere((BridgeInfo x) => (UnityEngine.Object)x.controller == (UnityEngine.Object)controller);
						}
					}
				}
			});
			Mode.RemoveOffscreenTargets(ioTargets, workingIOTargets, min, max, FreeUI, null);
			using (new KProfiler.Region("UpdateLogicOverlay", null))
			{
				IEnumerable allIntersecting = gameObjPartition.GetAllIntersecting(new Vector2((float)min.x, (float)min.y), new Vector2((float)max.x, (float)max.y));
				IEnumerator enumerator = allIntersecting.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						SaveLoadRoot saveLoadRoot = (SaveLoadRoot)enumerator.Current;
						if ((UnityEngine.Object)saveLoadRoot != (UnityEngine.Object)null)
						{
							KPrefabID component = saveLoadRoot.GetComponent<KPrefabID>();
							if (component.PrefabTag == wire_id || component.PrefabTag == bridge_id)
							{
								AddTargetIfVisible(saveLoadRoot, min, max, gameObjTargets, conduitTargetLayer, delegate(SaveLoadRoot root)
								{
									if (!((UnityEngine.Object)root == (UnityEngine.Object)null))
									{
										KPrefabID component6 = root.GetComponent<KPrefabID>();
										if (HighlightItemIDs.Contains(component6.PrefabTag))
										{
											if (component6.PrefabTag == wire_id)
											{
												wireControllers.Add(root.GetComponent<KBatchedAnimController>());
											}
											else if (component6.PrefabTag == bridge_id)
											{
												KBatchedAnimController component7 = root.GetComponent<KBatchedAnimController>();
												LogicUtilityNetworkLink component8 = root.GetComponent<LogicUtilityNetworkLink>();
												component8.GetCells(out int linked_cell, out int _);
												bridgeControllers.Add(new BridgeInfo
												{
													cell = linked_cell,
													controller = component7
												});
											}
										}
									}
								}, null);
							}
							else
							{
								AddTargetIfVisible(saveLoadRoot, min, max, gameObjTargets, objectTargetLayer, delegate(SaveLoadRoot root)
								{
									Vector3 position = root.transform.GetPosition();
									position.z += 2f;
									root.transform.SetPosition(position);
									KBatchedAnimController component5 = root.GetComponent<KBatchedAnimController>();
									component5.enabled = false;
									component5.enabled = true;
								}, null);
							}
						}
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
				IEnumerable allIntersecting2 = ioPartition.GetAllIntersecting(new Vector2((float)min.x, (float)min.y), new Vector2((float)max.x, (float)max.y));
				IEnumerator enumerator2 = allIntersecting2.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ILogicUIElement logicUIElement = (ILogicUIElement)enumerator2.Current;
						if (logicUIElement != null)
						{
							AddTargetIfVisible(logicUIElement, min, max, ioTargets, objectTargetLayer, AddUI, (KMonoBehaviour kcmp) => (UnityEngine.Object)kcmp != (UnityEngine.Object)null && HighlightItemIDs.Contains(kcmp.GetComponent<KPrefabID>().PrefabTag));
						}
					}
				}
				finally
				{
					IDisposable disposable2;
					if ((disposable2 = (enumerator2 as IDisposable)) != null)
					{
						disposable2.Dispose();
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
					IBridgedNetworkItem component2 = gameObject.GetComponent<IBridgedNetworkItem>();
					if (component2 != null)
					{
						int networkCell = component2.GetNetworkCell();
						visited.Clear();
						FindConnectedNetworks(networkCell, Game.Instance.logicCircuitSystem, connectedNetworks, visited);
						visited.Clear();
						num = ModeUtil.GetHighlightScale();
					}
				}
				LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
				Color32 colourOn = uiAsset.colourOn;
				Color32 colourOff = uiAsset.colourOff;
				colourOff.a = (colourOn.a = 0);
				foreach (KBatchedAnimController wireController in wireControllers)
				{
					if (!((UnityEngine.Object)wireController == (UnityEngine.Object)null))
					{
						Color32 tintColour = colourOff;
						LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(Grid.PosToCell(wireController.transform.GetPosition()));
						if (networkForCell != null)
						{
							tintColour = ((networkForCell.OutputValue <= 0) ? colourOff : colourOn);
						}
						if (connectedNetworks.Count > 0)
						{
							IBridgedNetworkItem component3 = wireController.GetComponent<IBridgedNetworkItem>();
							if (component3 != null && component3.IsConnectedToNetworks(connectedNetworks))
							{
								tintColour.r = (byte)((float)(int)tintColour.r * num);
								tintColour.g = (byte)((float)(int)tintColour.g * num);
								tintColour.b = (byte)((float)(int)tintColour.b * num);
							}
						}
						wireController.TintColour = tintColour;
					}
				}
				foreach (BridgeInfo bridgeController in bridgeControllers)
				{
					BridgeInfo current2 = bridgeController;
					if (!((UnityEngine.Object)current2.controller == (UnityEngine.Object)null))
					{
						Color32 tintColour2 = colourOff;
						LogicCircuitNetwork networkForCell2 = logicCircuitManager.GetNetworkForCell(current2.cell);
						if (networkForCell2 != null)
						{
							tintColour2 = ((networkForCell2.OutputValue <= 0) ? colourOff : colourOn);
						}
						if (connectedNetworks.Count > 0)
						{
							IBridgedNetworkItem component4 = current2.controller.GetComponent<IBridgedNetworkItem>();
							if (component4 != null && component4.IsConnectedToNetworks(connectedNetworks))
							{
								tintColour2.r = (byte)((float)(int)tintColour2.r * num);
								tintColour2.g = (byte)((float)(int)tintColour2.g * num);
								tintColour2.b = (byte)((float)(int)tintColour2.b * num);
							}
						}
						current2.controller.TintColour = tintColour2;
					}
				}
			}
			UpdateUI();
		}

		private void UpdateUI()
		{
			Color32 colourOn = uiAsset.colourOn;
			Color32 colourOff = uiAsset.colourOff;
			Color32 colourDisconnected = uiAsset.colourDisconnected;
			colourOff.a = (colourOn.a = byte.MaxValue);
			foreach (UIInfo data in uiInfo.GetDataList())
			{
				UIInfo current = data;
				LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(current.cell);
				Color32 c = colourDisconnected;
				if (networkForCell != null)
				{
					c = ((networkForCell.OutputValue <= 0) ? colourOff : colourOn);
				}
				if (current.image.color != (Color)c)
				{
					current.image.color = c;
				}
			}
		}

		private void AddUI(ILogicUIElement ui_elem)
		{
			if (!uiNodes.ContainsKey(ui_elem))
			{
				HandleVector<int>.Handle uiHandle = uiInfo.Allocate(new UIInfo(ui_elem, uiAsset));
				uiNodes.Add(ui_elem, new EventInfo
				{
					uiHandle = uiHandle
				});
			}
		}

		private void FreeUI(ILogicUIElement item)
		{
			if (item != null && uiNodes.TryGetValue(item, out EventInfo value))
			{
				uiInfo.GetData(value.uiHandle).Release();
				uiInfo.Free(value.uiHandle);
				uiNodes.Remove(item);
			}
		}

		protected UniformGrid<ILogicUIElement> CreateLogicUIPartition()
		{
			UniformGrid<ILogicUIElement> uniformGrid = new UniformGrid<ILogicUIElement>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			ReadOnlyCollection<ILogicUIElement> visElements = logicCircuitManager.GetVisElements();
			foreach (ILogicUIElement item in visElements)
			{
				if (item != null)
				{
					uniformGrid.Add(item);
				}
			}
			return uniformGrid;
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
