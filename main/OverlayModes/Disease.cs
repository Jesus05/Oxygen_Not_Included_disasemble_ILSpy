using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
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

		private int cameraLayerMask;

		private int freeDiseaseUI;

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

		public override SimViewMode ViewMode()
		{
			return SimViewMode.Disease;
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
				if (!((Object)item == (Object)null))
				{
					item.Show(ViewMode());
				}
			}
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			if (!((Object)item == (Object)null))
			{
				KBatchedAnimController component = item.GetComponent<KBatchedAnimController>();
				if (!((Object)component == (Object)null))
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
				if (!((Object)item == (Object)null))
				{
					item.Show(SimViewMode.None);
				}
			}
			UnregisterSaveLoadListeners();
			Camera.main.cullingMask &= ~cameraLayerMask;
			foreach (KMonoBehaviour layerTarget in layerTargets)
			{
				if (!((Object)layerTarget == (Object)null))
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
			Vector3 position = (!((Object)component2 != (Object)null)) ? (target.transform.GetPosition() + Vector3.down) : component2.GetWorldPivot();
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
					if (!((Object)item == (Object)null))
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
				if ((Object)go != (Object)null)
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
						if (!((Object)item == (Object)null) && !layerTargets.Contains(item))
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
}
