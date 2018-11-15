using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
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
					UtilityNetworkManager<FlowUtilityNetwork, Vent> mgr = (ViewMode() != SimViewMode.LiquidVentMap) ? Game.Instance.gasConduitSystem : Game.Instance.liquidConduitSystem;
					visited.Clear();
					FindConnectedNetworks(networkCell, mgr, connectedNetworks, visited);
					visited.Clear();
					num = ModeUtil.GetHighlightScale();
				}
			}
			Game.ConduitVisInfo conduitVisInfo = (ViewMode() != SimViewMode.LiquidVentMap) ? Game.Instance.gasConduitVisInfo : Game.Instance.liquidConduitVisInfo;
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
}
