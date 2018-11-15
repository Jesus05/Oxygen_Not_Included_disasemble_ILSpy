using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public class Sound : Mode
	{
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
					else
					{
						b = SimDebugView.Instance.GetNoisePollutionCategoryColourFromDecibels(num);
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

		public override SimViewMode ViewMode()
		{
			return SimViewMode.NoisePollution;
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
}
