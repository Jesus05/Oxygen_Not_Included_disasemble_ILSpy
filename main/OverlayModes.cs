using FMOD.Studio;
using Klei.AI;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public abstract class OverlayModes
{
	public class GasConduits : ConduitMode
	{
		public static readonly HashedString ID = "GasConduit";

		public GasConduits()
			: base(OverlayScreen.GasVentIDs)
		{
		}

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "GasVent";
		}
	}

	public class LiquidConduits : ConduitMode
	{
		public static readonly HashedString ID = "LiquidConduit";

		public LiquidConduits()
			: base(OverlayScreen.LiquidVentIDs)
		{
		}

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "LiquidVent";
		}
	}

	public abstract class ConduitMode : Mode
	{
		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		private ICollection<Tag> targetIDs;

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		public ConduitMode(ICollection<Tag> ids)
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
			targetIDs = ids;
		}

		public override void Enable()
		{
			RegisterSaveLoadListeners();
			partition = Mode.PopulatePartition<SaveLoadRoot>(targetIDs);
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			DragTool.SetLayerMask(selectionMask);
			GridCompositor.Instance.ToggleMinor(false);
			base.Enable();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			KPrefabID component = item.GetComponent<KPrefabID>();
			Tag saveLoadTag = component.GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
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

		public override void Disable()
		{
			Mode.ResetDisplayValues(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			DragTool.ClearLayerMask();
			UnregisterSaveLoadListeners();
			partition.Clear();
			layerTargets.Clear();
			GridCompositor.Instance.ToggleMinor(false);
			base.Disable();
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max, null);
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
			GameObject gameObject = null;
			if ((UnityEngine.Object)SelectTool.Instance != (UnityEngine.Object)null && (UnityEngine.Object)SelectTool.Instance.hover != (UnityEngine.Object)null)
			{
				gameObject = SelectTool.Instance.hover.gameObject;
			}
			connectedNetworks.Clear();
			float num = 1f;
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				IBridgedNetworkItem component = gameObject.GetComponent<IBridgedNetworkItem>();
				if (component != null)
				{
					int networkCell = component.GetNetworkCell();
					UtilityNetworkManager<FlowUtilityNetwork, Vent> mgr = (!(ViewMode() == LiquidConduits.ID)) ? Game.Instance.gasConduitSystem : Game.Instance.liquidConduitSystem;
					visited.Clear();
					FindConnectedNetworks(networkCell, mgr, connectedNetworks, visited);
					visited.Clear();
					num = ModeUtil.GetHighlightScale();
				}
			}
			Game.ConduitVisInfo conduitVisInfo = (!(ViewMode() == LiquidConduits.ID)) ? Game.Instance.gasConduitVisInfo : Game.Instance.liquidConduitVisInfo;
			foreach (SaveLoadRoot layerTarget in layerTargets)
			{
				if (!((UnityEngine.Object)layerTarget == (UnityEngine.Object)null))
				{
					BuildingDef def = layerTarget.GetComponent<Building>().Def;
					Color32 tintColour = (def.ThermalConductivity == 1f) ? conduitVisInfo.overlayTint : ((!(def.ThermalConductivity < 1f)) ? conduitVisInfo.overlayRadiantTint : conduitVisInfo.overlayInsulatedTint);
					if (connectedNetworks.Count > 0)
					{
						IBridgedNetworkItem component2 = layerTarget.GetComponent<IBridgedNetworkItem>();
						if (component2 != null && component2.IsConnectedToNetworks(connectedNetworks))
						{
							tintColour.r = (byte)((float)(int)tintColour.r * num);
							tintColour.g = (byte)((float)(int)tintColour.g * num);
							tintColour.b = (byte)((float)(int)tintColour.b * num);
						}
					}
					KBatchedAnimController component3 = layerTarget.GetComponent<KBatchedAnimController>();
					component3.TintColour = tintColour;
				}
			}
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
					object endpoint = mgr.GetEndpoint(cell);
					if (endpoint != null)
					{
						(endpoint as FlowUtilityNetwork.NetworkItem)?.GameObject.GetComponent<IBridgedNetworkItem>()?.AddNetworks(networks);
					}
				}
			}
		}
	}

	public class Crop : BasePlantMode
	{
		private struct UpdateCropInfo
		{
			public Harvestable harvestable;

			public GameObject harvestableUI;

			public UpdateCropInfo(Harvestable harvestable, GameObject harvestableUI)
			{
				this.harvestable = harvestable;
				this.harvestableUI = harvestableUI;
			}
		}

		public static readonly HashedString ID = "Crop";

		private Canvas uiRoot;

		private List<UpdateCropInfo> updateCropInfo = new List<UpdateCropInfo>();

		private int freeHarvestableNotificationIdx = 0;

		private List<GameObject> harvestableNotificationList = new List<GameObject>();

		private GameObject harvestableNotificationPrefab;

		private ColorHighlightCondition[] highlightConditions = new ColorHighlightCondition[3]
		{
			new ColorHighlightCondition((KMonoBehaviour h) => new Color(0.956862748f, 0.2509804f, 0.2784314f, 0.75f), delegate(KMonoBehaviour h)
			{
				WiltCondition component = h.GetComponent<WiltCondition>();
				return (UnityEngine.Object)component != (UnityEngine.Object)null && component.IsWilting();
			}),
			new ColorHighlightCondition((KMonoBehaviour h) => new Color(0.9843137f, 0.6901961f, 0.23137255f, 0.75f), (KMonoBehaviour h) => !(h as Harvestable).CanBeHavested),
			new ColorHighlightCondition((KMonoBehaviour h) => new Color(0.419607848f, 0.827451f, 0.5176471f, 0.75f), (KMonoBehaviour h) => (h as Harvestable).CanBeHavested)
		};

		public Crop(Canvas ui_root, GameObject harvestable_notification_prefab)
			: base(OverlayScreen.HarvestableIDs)
		{
			uiRoot = ui_root;
			harvestableNotificationPrefab = harvestable_notification_prefab;
		}

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Harvest";
		}

		public override void Update()
		{
			updateCropInfo.Clear();
			freeHarvestableNotificationIdx = 0;
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max, null);
			IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2((float)min.x, (float)min.y), new Vector2((float)max.x, (float)max.y));
			IEnumerator enumerator = allIntersecting.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Harvestable instance = (Harvestable)enumerator.Current;
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
			foreach (Harvestable layerTarget in layerTargets)
			{
				Vector2I vector2I = Grid.PosToXY(layerTarget.transform.GetPosition());
				if (min <= vector2I && vector2I <= max)
				{
					AddCropUI(layerTarget);
				}
			}
			foreach (UpdateCropInfo item in updateCropInfo)
			{
				UpdateCropInfo current2 = item;
				current2.harvestableUI.GetComponent<HarvestableOverlayWidget>().Refresh(current2.harvestable);
			}
			for (int i = freeHarvestableNotificationIdx; i < harvestableNotificationList.Count; i++)
			{
				if (harvestableNotificationList[i].activeSelf)
				{
					harvestableNotificationList[i].SetActive(false);
				}
			}
			UpdateHighlightTypeOverlay(min, max, layerTargets, targetIDs, highlightConditions, BringToFrontLayerSetting.Constant, targetLayer);
			base.Update();
		}

		public override void Disable()
		{
			DisableHarvestableUINotifications();
			base.Disable();
		}

		private void DisableHarvestableUINotifications()
		{
			freeHarvestableNotificationIdx = 0;
			foreach (GameObject harvestableNotification in harvestableNotificationList)
			{
				harvestableNotification.SetActive(false);
			}
			updateCropInfo.Clear();
		}

		public GameObject GetFreeCropUI()
		{
			GameObject gameObject = null;
			if (freeHarvestableNotificationIdx < harvestableNotificationList.Count)
			{
				gameObject = harvestableNotificationList[freeHarvestableNotificationIdx];
				if (!gameObject.gameObject.activeSelf)
				{
					gameObject.gameObject.SetActive(true);
				}
				freeHarvestableNotificationIdx++;
			}
			else
			{
				gameObject = Util.KInstantiateUI(harvestableNotificationPrefab.gameObject, uiRoot.transform.gameObject, false);
				harvestableNotificationList.Add(gameObject);
				freeHarvestableNotificationIdx++;
			}
			return gameObject;
		}

		private void AddCropUI(Harvestable harvestable)
		{
			GameObject freeCropUI = GetFreeCropUI();
			UpdateCropInfo item = new UpdateCropInfo(harvestable, freeCropUI);
			Vector3 b = Grid.CellToPos(Grid.PosToCell(harvestable), 0.5f, -1.25f, 0f);
			freeCropUI.GetComponent<RectTransform>().SetPosition(Vector3.up + b);
			updateCropInfo.Add(item);
		}
	}

	public class Harvest : BasePlantMode
	{
		public static readonly HashedString ID = "HarvestWhenReady";

		private ColorHighlightCondition[] highlightConditions = new ColorHighlightCondition[1]
		{
			new ColorHighlightCondition((KMonoBehaviour harvestable) => new Color(0.65f, 0.65f, 0.65f, 0.65f), (KMonoBehaviour harvestable) => true)
		};

		public Harvest()
			: base(OverlayScreen.HarvestableIDs)
		{
		}

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Harvest";
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max, null);
			IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2((float)min.x, (float)min.y), new Vector2((float)max.x, (float)max.y));
			IEnumerator enumerator = allIntersecting.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Harvestable instance = (Harvestable)enumerator.Current;
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
			UpdateHighlightTypeOverlay(min, max, layerTargets, targetIDs, highlightConditions, BringToFrontLayerSetting.Constant, targetLayer);
			base.Update();
		}
	}

	public abstract class BasePlantMode : Mode
	{
		protected UniformGrid<Harvestable> partition;

		protected HashSet<Harvestable> layerTargets = new HashSet<Harvestable>();

		protected ICollection<Tag> targetIDs;

		protected int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		public BasePlantMode(ICollection<Tag> ids)
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = LayerMask.GetMask("MaskedOverlay");
			targetIDs = ids;
		}

		public override void Enable()
		{
			RegisterSaveLoadListeners();
			partition = Mode.PopulatePartition<Harvestable>(targetIDs);
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			DragTool.SetLayerMask(selectionMask);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
			{
				Harvestable component = item.GetComponent<Harvestable>();
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
				{
					partition.Add(component);
				}
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null) && !((UnityEngine.Object)item.gameObject == (UnityEngine.Object)null))
			{
				Harvestable component = item.GetComponent<Harvestable>();
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
				{
					if (layerTargets.Contains(component))
					{
						layerTargets.Remove(component);
					}
					partition.Remove(component);
				}
			}
		}

		public override void Disable()
		{
			UnregisterSaveLoadListeners();
			DisableHighlightTypeOverlay(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			partition.Clear();
			layerTargets.Clear();
			SelectTool.Instance.ClearLayerMask();
			DragTool.ClearLayerMask();
		}
	}

	public class Decor : Mode
	{
		public static readonly HashedString ID = "Decor";

		private UniformGrid<DecorProvider> partition;

		private HashSet<DecorProvider> layerTargets = new HashSet<DecorProvider>();

		private List<DecorProvider> workingTargets = new List<DecorProvider>();

		private HashSet<Tag> targetIDs = new HashSet<Tag>();

		private int targetLayer;

		private int cameraLayerMask;

		private ColorHighlightCondition[] highlightConditions = new ColorHighlightCondition[1]
		{
			new ColorHighlightCondition(delegate(KMonoBehaviour dp)
			{
				Color black = Color.black;
				Color b = Color.black;
				if ((UnityEngine.Object)dp != (UnityEngine.Object)null)
				{
					int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
					float decorForCell = (dp as DecorProvider).GetDecorForCell(cell);
					if (decorForCell > 0f)
					{
						b = new Color(0f, 0.8f, 0f, 0.8f);
					}
					else if (decorForCell < 0f)
					{
						b = new Color(1f, 0f, 0f, 0.4f);
					}
				}
				return Color.Lerp(black, b, 0.85f);
			}, (KMonoBehaviour dp) => SelectToolHoverTextCard.highlightedObjects.Contains(dp.gameObject))
		};

		public Decor()
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
		}

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Decor";
		}

		public override void Enable()
		{
			RegisterSaveLoadListeners();
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<DecorProvider>();
			targetIDs.UnionWith(prefabTagsWithComponent);
			Tag[] array = new Tag[6]
			{
				new Tag("Tile"),
				new Tag("MeshTile"),
				new Tag("InsulationTile"),
				new Tag("GasPermeableMembrane"),
				new Tag("CarpetTile"),
				new Tag("MouldingTile")
			};
			Tag[] array2 = array;
			foreach (Tag item in array2)
			{
				targetIDs.Remove(item);
			}
			foreach (Tag gasVentID in OverlayScreen.GasVentIDs)
			{
				targetIDs.Remove(gasVentID);
			}
			foreach (Tag liquidVentID in OverlayScreen.LiquidVentIDs)
			{
				targetIDs.Remove(liquidVentID);
			}
			partition = Mode.PopulatePartition<DecorProvider>(targetIDs);
			Camera.main.cullingMask |= cameraLayerMask;
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max, null);
			partition.GetAllIntersecting(new Vector2((float)min.x, (float)min.y), new Vector2((float)max.x, (float)max.y), workingTargets);
			for (int i = 0; i < workingTargets.Count; i++)
			{
				DecorProvider instance = workingTargets[i];
				AddTargetIfVisible(instance, min, max, layerTargets, targetLayer, null, null);
			}
			UpdateHighlightTypeOverlay(min, max, layerTargets, targetIDs, highlightConditions, BringToFrontLayerSetting.Conditional, targetLayer);
			workingTargets.Clear();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
			{
				DecorProvider component = item.GetComponent<DecorProvider>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					partition.Add(component);
				}
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null) && !((UnityEngine.Object)item.gameObject == (UnityEngine.Object)null))
			{
				DecorProvider component = item.GetComponent<DecorProvider>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					if (layerTargets.Contains(component))
					{
						layerTargets.Remove(component);
					}
					partition.Remove(component);
				}
			}
		}

		public override void Disable()
		{
			DisableHighlightTypeOverlay(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			UnregisterSaveLoadListeners();
			partition.Clear();
			layerTargets.Clear();
		}
	}

	public class Disease : Mode
	{
		private struct UpdateDiseaseInfo
		{
			public DiseaseOverlayWidget ui;

			public AmountInstance valueSrc;

			public UpdateDiseaseInfo(AmountInstance amount_inst, DiseaseOverlayWidget ui)
			{
				this.ui = ui;
				valueSrc = amount_inst;
			}
		}

		public static readonly HashedString ID = "Disease";

		private int cameraLayerMask;

		private int freeDiseaseUI = 0;

		private List<GameObject> diseaseUIList = new List<GameObject>();

		private List<UpdateDiseaseInfo> updateDiseaseInfo = new List<UpdateDiseaseInfo>();

		private HashSet<KMonoBehaviour> layerTargets = new HashSet<KMonoBehaviour>();

		private HashSet<KMonoBehaviour> privateTargets = new HashSet<KMonoBehaviour>();

		private List<KMonoBehaviour> queuedAdds = new List<KMonoBehaviour>();

		private Canvas diseaseUIParent;

		private GameObject diseaseOverlayPrefab;

		public Disease(Canvas diseaseUIParent, GameObject diseaseOverlayPrefab)
		{
			this.diseaseUIParent = diseaseUIParent;
			this.diseaseOverlayPrefab = diseaseOverlayPrefab;
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
		}

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Disease";
		}

		public override void Enable()
		{
			Infrared.Instance.SetMode(Infrared.Mode.Disease);
			CameraController.Instance.ToggleColouredOverlayView(true);
			Camera.main.cullingMask |= cameraLayerMask;
			RegisterSaveLoadListeners();
			foreach (DiseaseSourceVisualizer item in Components.DiseaseSourceVisualizers.Items)
			{
				if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
				{
					item.Show(ViewMode());
				}
			}
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
			{
				KBatchedAnimController component = item.GetComponent<KBatchedAnimController>();
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
				{
					InfraredVisualizerComponents.ClearOverlayColour(component);
				}
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
		}

		public override void Disable()
		{
			foreach (DiseaseSourceVisualizer item in Components.DiseaseSourceVisualizers.Items)
			{
				if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
				{
					item.Show(None.ID);
				}
			}
			UnregisterSaveLoadListeners();
			Camera.main.cullingMask &= ~cameraLayerMask;
			foreach (KMonoBehaviour layerTarget in layerTargets)
			{
				if (!((UnityEngine.Object)layerTarget == (UnityEngine.Object)null))
				{
					float defaultDepth = Mode.GetDefaultDepth(layerTarget);
					Vector3 position = layerTarget.transform.GetPosition();
					position.z = defaultDepth;
					layerTarget.transform.SetPosition(position);
					KBatchedAnimController component = layerTarget.GetComponent<KBatchedAnimController>();
					component.enabled = false;
					component.enabled = true;
				}
			}
			CameraController.Instance.ToggleColouredOverlayView(false);
			Infrared.Instance.SetMode(Infrared.Mode.Disabled);
			OverlayLegend.Instance.DisableDiseaseOverlay();
			Game.Instance.showGasConduitDisease = false;
			Game.Instance.showLiquidConduitDisease = false;
			freeDiseaseUI = 0;
			foreach (UpdateDiseaseInfo item2 in updateDiseaseInfo)
			{
				UpdateDiseaseInfo current3 = item2;
				current3.ui.gameObject.SetActive(false);
			}
			updateDiseaseInfo.Clear();
			privateTargets.Clear();
			layerTargets.Clear();
		}

		public GameObject GetFreeDiseaseUI()
		{
			GameObject gameObject = null;
			if (freeDiseaseUI < diseaseUIList.Count)
			{
				gameObject = diseaseUIList[freeDiseaseUI];
				gameObject.gameObject.SetActive(true);
				freeDiseaseUI++;
			}
			else
			{
				gameObject = Util.KInstantiateUI(diseaseOverlayPrefab, diseaseUIParent.transform.gameObject, false);
				diseaseUIList.Add(gameObject);
				freeDiseaseUI++;
			}
			return gameObject;
		}

		private void AddDiseaseUI(MinionIdentity target)
		{
			GameObject gameObject = GetFreeDiseaseUI();
			DiseaseOverlayWidget component = gameObject.GetComponent<DiseaseOverlayWidget>();
			AmountInstance amount_inst = target.GetComponent<Modifiers>().amounts.Get(Db.Get().Amounts.ImmuneLevel);
			UpdateDiseaseInfo item = new UpdateDiseaseInfo(amount_inst, component);
			KAnimControllerBase component2 = target.GetComponent<KAnimControllerBase>();
			Vector3 position = (!((UnityEngine.Object)component2 != (UnityEngine.Object)null)) ? (target.transform.GetPosition() + Vector3.down) : component2.GetWorldPivot();
			gameObject.GetComponent<RectTransform>().SetPosition(position);
			updateDiseaseInfo.Add(item);
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			using (new KProfiler.Region("UpdateDiseaseCarriers", null))
			{
				queuedAdds.Clear();
				foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
				{
					if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
					{
						Vector2I vector2I = Grid.PosToXY(item.transform.GetPosition());
						if (min <= vector2I && vector2I <= max && !privateTargets.Contains(item))
						{
							AddDiseaseUI(item);
							queuedAdds.Add(item);
						}
					}
				}
				foreach (KMonoBehaviour queuedAdd in queuedAdds)
				{
					privateTargets.Add(queuedAdd);
				}
				queuedAdds.Clear();
			}
			foreach (UpdateDiseaseInfo item2 in updateDiseaseInfo)
			{
				UpdateDiseaseInfo current3 = item2;
				current3.ui.Refresh(current3.valueSrc);
			}
			bool flag = false;
			if (Game.Instance.showLiquidConduitDisease)
			{
				foreach (Tag liquidVentID in OverlayScreen.LiquidVentIDs)
				{
					if (!OverlayScreen.DiseaseIDs.Contains(liquidVentID))
					{
						OverlayScreen.DiseaseIDs.Add(liquidVentID);
						flag = true;
					}
				}
			}
			else
			{
				foreach (Tag liquidVentID2 in OverlayScreen.LiquidVentIDs)
				{
					if (OverlayScreen.DiseaseIDs.Contains(liquidVentID2))
					{
						OverlayScreen.DiseaseIDs.Remove(liquidVentID2);
						flag = true;
					}
				}
			}
			if (Game.Instance.showGasConduitDisease)
			{
				foreach (Tag gasVentID in OverlayScreen.GasVentIDs)
				{
					if (!OverlayScreen.DiseaseIDs.Contains(gasVentID))
					{
						OverlayScreen.DiseaseIDs.Add(gasVentID);
						flag = true;
					}
				}
			}
			else
			{
				foreach (Tag gasVentID2 in OverlayScreen.GasVentIDs)
				{
					if (OverlayScreen.DiseaseIDs.Contains(gasVentID2))
					{
						OverlayScreen.DiseaseIDs.Remove(gasVentID2);
						flag = true;
					}
				}
			}
			if (flag)
			{
				SetLayerZ(-50f);
			}
		}

		private void SetLayerZ(float offset_z)
		{
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			Mode.ClearOutsideViewObjects(layerTargets, min, max, OverlayScreen.DiseaseIDs, delegate(KMonoBehaviour go)
			{
				if ((UnityEngine.Object)go != (UnityEngine.Object)null)
				{
					float defaultDepth2 = Mode.GetDefaultDepth(go);
					Vector3 position2 = go.transform.GetPosition();
					position2.z = defaultDepth2;
					go.transform.SetPosition(position2);
					KBatchedAnimController component2 = go.GetComponent<KBatchedAnimController>();
					component2.enabled = false;
					component2.enabled = true;
				}
			});
			Dictionary<Tag, List<SaveLoadRoot>> lists = SaveLoader.Instance.saveManager.GetLists();
			foreach (Tag diseaseID in OverlayScreen.DiseaseIDs)
			{
				if (lists.TryGetValue(diseaseID, out List<SaveLoadRoot> value))
				{
					foreach (SaveLoadRoot item in value)
					{
						if (!((UnityEngine.Object)item == (UnityEngine.Object)null) && !layerTargets.Contains(item))
						{
							Vector3 position = item.transform.GetPosition();
							if (Grid.IsVisible(Grid.PosToCell(position)) && min <= (Vector2)position && (Vector2)position <= max)
							{
								float defaultDepth = Mode.GetDefaultDepth(item);
								position.z = defaultDepth + offset_z;
								item.transform.SetPosition(position);
								KBatchedAnimController component = item.GetComponent<KBatchedAnimController>();
								component.enabled = false;
								component.enabled = true;
								layerTargets.Add(item);
							}
						}
					}
				}
			}
		}
	}

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

		public static readonly HashedString ID = "Logic";

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

		public override HashedString ViewMode()
		{
			return ID;
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

	public enum BringToFrontLayerSetting
	{
		None,
		Constant,
		Conditional
	}

	public class ColorHighlightCondition
	{
		public Func<KMonoBehaviour, Color> highlight_color;

		public Func<KMonoBehaviour, bool> highlight_condition;

		public ColorHighlightCondition(Func<KMonoBehaviour, Color> highlight_color, Func<KMonoBehaviour, bool> highlight_condition)
		{
			this.highlight_color = highlight_color;
			this.highlight_condition = highlight_condition;
		}
	}

	public class None : Mode
	{
		public static readonly HashedString ID = HashedString.Invalid;

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Off";
		}
	}

	public class PathProber : Mode
	{
		public static readonly HashedString ID = "PathProber";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Off";
		}
	}

	public class Oxygen : Mode
	{
		public static readonly HashedString ID = "Oxygen";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Oxygen";
		}

		public override void Enable()
		{
			base.Enable();
			int defaultLayerMask = SelectTool.Instance.GetDefaultLayerMask();
			int mask = LayerMask.GetMask("MaskedOverlay");
			SelectTool.Instance.SetLayerMask(defaultLayerMask | mask);
		}

		public override void Disable()
		{
			base.Disable();
			SelectTool.Instance.ClearLayerMask();
		}
	}

	public class Light : Mode
	{
		public static readonly HashedString ID = "Light";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Lights";
		}
	}

	public class Priorities : Mode
	{
		public static readonly HashedString ID = "Priorities";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Priorities";
		}
	}

	public class ThermalConductivity : Mode
	{
		public static readonly HashedString ID = "ThermalConductivity";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "HeatFlow";
		}
	}

	public class HeatFlow : Mode
	{
		public static readonly HashedString ID = "HeatFlow";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "HeatFlow";
		}
	}

	public class Rooms : Mode
	{
		public static readonly HashedString ID = "Rooms";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Rooms";
		}
	}

	public abstract class Mode
	{
		private static List<KMonoBehaviour> workingTargets = new List<KMonoBehaviour>();

		public static void Clear()
		{
			workingTargets.Clear();
		}

		public abstract HashedString ViewMode();

		public virtual void Enable()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void Disable()
		{
		}

		public abstract string GetSoundName();

		public void RegisterSaveLoadListeners()
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			saveManager.onRegister += OnSaveLoadRootRegistered;
			saveManager.onUnregister += OnSaveLoadRootUnregistered;
		}

		public void UnregisterSaveLoadListeners()
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			saveManager.onRegister -= OnSaveLoadRootRegistered;
			saveManager.onUnregister -= OnSaveLoadRootUnregistered;
		}

		protected virtual void OnSaveLoadRootRegistered(SaveLoadRoot root)
		{
		}

		protected virtual void OnSaveLoadRootUnregistered(SaveLoadRoot root)
		{
		}

		protected void ProcessExistingSaveLoadRoots()
		{
			foreach (KeyValuePair<Tag, List<SaveLoadRoot>> list in SaveLoader.Instance.saveManager.GetLists())
			{
				foreach (SaveLoadRoot item in list.Value)
				{
					OnSaveLoadRootRegistered(item);
				}
			}
		}

		protected static UniformGrid<T> PopulatePartition<T>(ICollection<Tag> tags) where T : IUniformGridObject
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			Dictionary<Tag, List<SaveLoadRoot>> lists = saveManager.GetLists();
			UniformGrid<T> uniformGrid = new UniformGrid<T>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			foreach (Tag item in (IEnumerable<Tag>)tags)
			{
				List<SaveLoadRoot> value = null;
				if (lists.TryGetValue(item, out value))
				{
					foreach (SaveLoadRoot item2 in value)
					{
						T component = item2.GetComponent<T>();
						if (component != null)
						{
							uniformGrid.Add(component);
						}
					}
				}
			}
			return uniformGrid;
		}

		protected static void ResetDisplayValues<T>(ICollection<T> targets) where T : MonoBehaviour
		{
			foreach (T item in (IEnumerable<T>)targets)
			{
				T current = item;
				if (!((UnityEngine.Object)current == (UnityEngine.Object)null))
				{
					KBatchedAnimController component = current.GetComponent<KBatchedAnimController>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						ResetDisplayValues(component);
					}
				}
			}
		}

		protected static void ResetDisplayValues(KBatchedAnimController controller)
		{
			controller.SetLayer(0);
			controller.HighlightColour = Color.clear;
			controller.TintColour = Color.white;
			controller.SetLayer(controller.GetComponent<KPrefabID>().defaultLayer);
		}

		protected static void RemoveOffscreenTargets<T>(ICollection<T> targets, Vector2I min, Vector2I max, Action<T> on_removed = null) where T : KMonoBehaviour
		{
			ClearOutsideViewObjects(targets, min, max, null, delegate(T cmp)
			{
				if ((UnityEngine.Object)cmp != (UnityEngine.Object)null)
				{
					KBatchedAnimController component = ((Component)cmp).GetComponent<KBatchedAnimController>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						ResetDisplayValues(component);
					}
					if (on_removed != null)
					{
						on_removed(cmp);
					}
				}
			});
			workingTargets.Clear();
		}

		protected static void ClearOutsideViewObjects<T>(ICollection<T> targets, Vector2I vis_min, Vector2I vis_max, ICollection<Tag> item_ids, Action<T> on_remove) where T : KMonoBehaviour
		{
			workingTargets.Clear();
			foreach (T item in (IEnumerable<T>)targets)
			{
				T current = item;
				if (!((UnityEngine.Object)current == (UnityEngine.Object)null))
				{
					Vector2I vector2I = Grid.PosToXY(current.transform.GetPosition());
					if (!(vis_min <= vector2I) || !(vector2I <= vis_max))
					{
						workingTargets.Add((KMonoBehaviour)current);
					}
					else
					{
						KPrefabID component = current.GetComponent<KPrefabID>();
						if (item_ids != null && !item_ids.Contains(component.PrefabTag))
						{
							workingTargets.Add((KMonoBehaviour)current);
						}
					}
				}
			}
			foreach (T workingTarget in workingTargets)
			{
				if (!((UnityEngine.Object)workingTarget == (UnityEngine.Object)null))
				{
					on_remove?.Invoke(workingTarget);
					targets.Remove(workingTarget);
				}
			}
			workingTargets.Clear();
		}

		protected static void RemoveOffscreenTargets<T>(ICollection<T> targets, ICollection<T> working_targets, Vector2I vis_min, Vector2I vis_max, Action<T> on_removed = null, Func<T, bool> special_clear_condition = null) where T : IUniformGridObject
		{
			ClearOutsideViewObjects(targets, working_targets, vis_min, vis_max, delegate(T cmp)
			{
				if (cmp != null && on_removed != null)
				{
					on_removed(cmp);
				}
			});
			if (special_clear_condition != null)
			{
				working_targets.Clear();
				foreach (T item in (IEnumerable<T>)targets)
				{
					if (special_clear_condition(item))
					{
						working_targets.Add(item);
					}
				}
				foreach (T item2 in (IEnumerable<T>)working_targets)
				{
					if (item2 != null)
					{
						if (on_removed != null)
						{
							on_removed(item2);
						}
						targets.Remove(item2);
					}
				}
				working_targets.Clear();
			}
		}

		protected static void ClearOutsideViewObjects<T>(ICollection<T> targets, ICollection<T> working_targets, Vector2I vis_min, Vector2I vis_max, Action<T> on_removed = null) where T : IUniformGridObject
		{
			working_targets.Clear();
			foreach (T item in (IEnumerable<T>)targets)
			{
				if (item != null)
				{
					Vector2 vector = item.PosMin();
					Vector2 vector2 = item.PosMin();
					if (vector2.x < (float)vis_min.x || vector2.y < (float)vis_min.y || (float)vis_max.x < vector.x || (float)vis_max.y < vector.y)
					{
						working_targets.Add(item);
					}
				}
			}
			foreach (T item2 in (IEnumerable<T>)working_targets)
			{
				if (item2 != null)
				{
					on_removed?.Invoke(item2);
					targets.Remove(item2);
				}
			}
			working_targets.Clear();
		}

		protected static float GetDefaultDepth(KMonoBehaviour cmp)
		{
			BuildingComplete component = cmp.GetComponent<BuildingComplete>();
			if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
			{
				return Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			}
			return Grid.GetLayerZ(component.Def.SceneLayer);
		}

		protected void UpdateHighlightTypeOverlay<T>(Vector2I min, Vector2I max, ICollection<T> targets, ICollection<Tag> item_ids, ColorHighlightCondition[] highlights, BringToFrontLayerSetting bringToFrontSetting, int layer) where T : KMonoBehaviour
		{
			foreach (T item in (IEnumerable<T>)targets)
			{
				T current = item;
				if (!((UnityEngine.Object)current == (UnityEngine.Object)null))
				{
					Vector3 position = current.transform.GetPosition();
					int cell = Grid.PosToCell(position);
					if (Grid.IsValidCell(cell) && Grid.IsVisible(cell) && min <= (Vector2)position && (Vector2)position <= max)
					{
						KBatchedAnimController component = current.GetComponent<KBatchedAnimController>();
						if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
						{
							int layer2 = 0;
							Color32 highlightColour = Color.clear;
							if (highlights != null)
							{
								foreach (ColorHighlightCondition colorHighlightCondition in highlights)
								{
									if (colorHighlightCondition.highlight_condition(current))
									{
										highlightColour = colorHighlightCondition.highlight_color(current);
										layer2 = layer;
										break;
									}
								}
							}
							switch (bringToFrontSetting)
							{
							case BringToFrontLayerSetting.Constant:
								component.SetLayer(layer);
								break;
							case BringToFrontLayerSetting.Conditional:
								component.SetLayer(layer2);
								break;
							}
							component.HighlightColour = highlightColour;
						}
					}
				}
			}
		}

		protected void DisableHighlightTypeOverlay<T>(ICollection<T> targets) where T : KMonoBehaviour
		{
			Color32 highlightColour = Color.clear;
			foreach (T item in (IEnumerable<T>)targets)
			{
				T current = item;
				if (!((UnityEngine.Object)current == (UnityEngine.Object)null))
				{
					KBatchedAnimController component = current.GetComponent<KBatchedAnimController>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null)
					{
						component.HighlightColour = highlightColour;
						component.SetLayer(0);
					}
				}
			}
			targets.Clear();
		}

		protected void AddTargetIfVisible<T>(T instance, Vector2I vis_min, Vector2I vis_max, ICollection<T> targets, int layer, Action<T> on_added = null, Func<KMonoBehaviour, bool> should_add = null) where T : IUniformGridObject
		{
			if (!instance.Equals(null))
			{
				Vector2 vector = instance.PosMin();
				Vector2 vector2 = instance.PosMax();
				if (!(vector2.x < (float)vis_min.x) && !(vector2.y < (float)vis_min.y) && !(vector.x > (float)vis_max.x) && !(vector.y > (float)vis_max.y) && !targets.Contains(instance))
				{
					bool flag = false;
					for (int i = (int)vector.y; (float)i <= vector2.y; i++)
					{
						for (int j = (int)vector.x; (float)j <= vector2.x; j++)
						{
							int num = Grid.XYToCell(j, i);
							if (Grid.Visible[num] > 20 || !PropertyTextures.IsFogOfWarEnabled)
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						bool flag2 = true;
						KMonoBehaviour kMonoBehaviour = instance as KMonoBehaviour;
						if ((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null && should_add != null)
						{
							flag2 = should_add(kMonoBehaviour);
						}
						if (flag2)
						{
							if ((UnityEngine.Object)kMonoBehaviour != (UnityEngine.Object)null)
							{
								KBatchedAnimController component = kMonoBehaviour.GetComponent<KBatchedAnimController>();
								if ((UnityEngine.Object)component != (UnityEngine.Object)null)
								{
									component.SetLayer(layer);
								}
							}
							targets.Add(instance);
							on_added?.Invoke(instance);
						}
					}
				}
			}
		}
	}

	public class ModeUtil
	{
		public static float GetHighlightScale()
		{
			return Mathf.SmoothStep(0.5f, 1f, Mathf.Abs(Mathf.Sin(Time.unscaledTime * 4f)));
		}
	}

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

		public static readonly HashedString ID = "Power";

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

		private int freePowerLabelIdx = 0;

		private int freeBatteryUIIdx = 0;

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

		public override HashedString ViewMode()
		{
			return ID;
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

	public class SolidConveyor : Mode
	{
		public static readonly HashedString ID = "SolidConveyor";

		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private ICollection<Tag> targetIDs = OverlayScreen.SolidConveyorIDs;

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		public SolidConveyor()
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
		}

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "LiquidVent";
		}

		public override void Enable()
		{
			RegisterSaveLoadListeners();
			partition = Mode.PopulatePartition<SaveLoadRoot>(targetIDs);
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			DragTool.SetLayerMask(selectionMask);
			GridCompositor.Instance.ToggleMinor(false);
			base.Enable();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			KPrefabID component = item.GetComponent<KPrefabID>();
			Tag saveLoadTag = component.GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
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

		public override void Disable()
		{
			Mode.ResetDisplayValues(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			DragTool.ClearLayerMask();
			UnregisterSaveLoadListeners();
			partition.Clear();
			layerTargets.Clear();
			GridCompositor.Instance.ToggleMinor(false);
			base.Disable();
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max, null);
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
			Color32 tintColour = Color.white;
			foreach (SaveLoadRoot layerTarget in layerTargets)
			{
				if (!((UnityEngine.Object)layerTarget == (UnityEngine.Object)null))
				{
					KBatchedAnimController component = layerTarget.GetComponent<KBatchedAnimController>();
					component.TintColour = tintColour;
				}
			}
		}
	}

	public class Sound : Mode
	{
		public static readonly HashedString ID = "Sound";

		private UniformGrid<NoisePolluter> partition;

		private HashSet<NoisePolluter> layerTargets = new HashSet<NoisePolluter>();

		private HashSet<Tag> targetIDs = new HashSet<Tag>();

		private int targetLayer;

		private int cameraLayerMask;

		private ColorHighlightCondition[] highlightConditions = new ColorHighlightCondition[1]
		{
			new ColorHighlightCondition(delegate(KMonoBehaviour np)
			{
				Color black = Color.black;
				Color b = Color.black;
				float t = 0.8f;
				if ((UnityEngine.Object)np != (UnityEngine.Object)null)
				{
					float num = 0f;
					int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
					num = (np as NoisePolluter).GetNoiseForCell(cell);
					if (num < 36f)
					{
						t = 1f;
						b = new Color(0.4f, 0.4f, 0.4f);
					}
				}
				return Color.Lerp(black, b, t);
			}, delegate(KMonoBehaviour np)
			{
				List<GameObject> highlightedObjects = SelectToolHoverTextCard.highlightedObjects;
				bool result = false;
				for (int i = 0; i < highlightedObjects.Count; i++)
				{
					if ((UnityEngine.Object)highlightedObjects[i] != (UnityEngine.Object)null && (UnityEngine.Object)highlightedObjects[i] == (UnityEngine.Object)np.gameObject)
					{
						result = true;
						break;
					}
				}
				return result;
			})
		};

		public Sound()
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<NoisePolluter>();
			targetIDs.UnionWith(prefabTagsWithComponent);
		}

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Sound";
		}

		public override void Enable()
		{
			RegisterSaveLoadListeners();
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<NoisePolluter>();
			targetIDs.UnionWith(prefabTagsWithComponent);
			partition = Mode.PopulatePartition<NoisePolluter>(targetIDs);
			Camera.main.cullingMask |= cameraLayerMask;
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max, null);
			IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2((float)min.x, (float)min.y), new Vector2((float)max.x, (float)max.y));
			IEnumerator enumerator = allIntersecting.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					NoisePolluter instance = (NoisePolluter)enumerator.Current;
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
			UpdateHighlightTypeOverlay(min, max, layerTargets, targetIDs, highlightConditions, BringToFrontLayerSetting.Conditional, targetLayer);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
			{
				NoisePolluter component = item.GetComponent<NoisePolluter>();
				partition.Add(component);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null) && !((UnityEngine.Object)item.gameObject == (UnityEngine.Object)null))
			{
				NoisePolluter component = item.GetComponent<NoisePolluter>();
				if (layerTargets.Contains(component))
				{
					layerTargets.Remove(component);
				}
				partition.Remove(component);
			}
		}

		public override void Disable()
		{
			DisableHighlightTypeOverlay(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			UnregisterSaveLoadListeners();
			partition.Clear();
			layerTargets.Clear();
		}
	}

	public class Suit : Mode
	{
		public static readonly HashedString ID = "Suit";

		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private ICollection<Tag> targetIDs;

		private List<GameObject> uiList = new List<GameObject>();

		private int freeUiIdx;

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private Canvas uiParent;

		private GameObject overlayPrefab;

		public Suit(Canvas ui_parent, GameObject overlay_prefab)
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
			targetIDs = OverlayScreen.SuitIDs;
			uiParent = ui_parent;
			overlayPrefab = overlay_prefab;
		}

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "SuitRequired";
		}

		public override void Enable()
		{
			partition = new UniformGrid<SaveLoadRoot>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			ProcessExistingSaveLoadRoots();
			RegisterSaveLoadListeners();
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			DragTool.SetLayerMask(selectionMask);
			GridCompositor.Instance.ToggleMinor(false);
			base.Enable();
		}

		public override void Disable()
		{
			UnregisterSaveLoadListeners();
			Mode.ResetDisplayValues(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			DragTool.ClearLayerMask();
			partition.Clear();
			partition = null;
			layerTargets.Clear();
			for (int i = 0; i < uiList.Count; i++)
			{
				uiList[i].SetActive(false);
			}
			GridCompositor.Instance.ToggleMinor(false);
			base.Disable();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			KPrefabID component = item.GetComponent<KPrefabID>();
			Tag saveLoadTag = component.GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
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

		private GameObject GetFreeUI()
		{
			GameObject gameObject = null;
			if (freeUiIdx >= uiList.Count)
			{
				gameObject = Util.KInstantiateUI(overlayPrefab, uiParent.transform.gameObject, false);
				uiList.Add(gameObject);
			}
			else
			{
				gameObject = uiList[freeUiIdx++];
			}
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			return gameObject;
		}

		public override void Update()
		{
			freeUiIdx = 0;
			Grid.GetVisibleExtents(out Vector2I min, out Vector2I max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max, null);
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
			foreach (SaveLoadRoot layerTarget in layerTargets)
			{
				if (!((UnityEngine.Object)layerTarget == (UnityEngine.Object)null))
				{
					KBatchedAnimController component = layerTarget.GetComponent<KBatchedAnimController>();
					component.TintColour = Color.white;
					bool flag = false;
					if (layerTarget.GetComponent<KPrefabID>().HasTag(GameTags.Suit))
					{
						flag = true;
					}
					else
					{
						SuitLocker component2 = layerTarget.GetComponent<SuitLocker>();
						if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
						{
							flag = ((UnityEngine.Object)component2.GetStoredOutfit() != (UnityEngine.Object)null);
						}
					}
					if (flag)
					{
						GameObject freeUI = GetFreeUI();
						freeUI.GetComponent<RectTransform>().SetPosition(layerTarget.transform.GetPosition());
					}
				}
			}
			for (int i = freeUiIdx; i < uiList.Count; i++)
			{
				if (uiList[i].activeSelf)
				{
					uiList[i].SetActive(false);
				}
			}
		}
	}

	public class Temperature : Mode
	{
		public static readonly HashedString ID = "Temperature";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Temperature";
		}

		public override void Enable()
		{
			Infrared.Instance.SetMode(Infrared.Mode.Infrared);
			CameraController.Instance.ToggleColouredOverlayView(true);
			base.Enable();
		}

		public override void Disable()
		{
			Infrared.Instance.SetMode(Infrared.Mode.Disabled);
			CameraController.Instance.ToggleColouredOverlayView(false);
			base.Disable();
		}
	}
}
