using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
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

		private Canvas uiRoot;

		private List<UpdateCropInfo> updateCropInfo = new List<UpdateCropInfo>();

		private int freeHarvestableNotificationIdx;

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

		public override SimViewMode ViewMode()
		{
			return SimViewMode.Crop;
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
}
