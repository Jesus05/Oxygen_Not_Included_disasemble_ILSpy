using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public class Suit : Mode
	{
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

		public override SimViewMode ViewMode()
		{
			return SimViewMode.SuitRequiredMap;
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
}
