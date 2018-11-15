using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public class Decor : Mode
	{
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
				if ((Object)dp != (Object)null)
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

		public override SimViewMode ViewMode()
		{
			return SimViewMode.Decor;
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
			Tag[] array = new Tag[4]
			{
				new Tag("Tile"),
				new Tag("MeshTile"),
				new Tag("InsulationTile"),
				new Tag("GasPermeableMembrane")
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
				if ((Object)component != (Object)null)
				{
					partition.Add(component);
				}
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!((Object)item == (Object)null) && !((Object)item.gameObject == (Object)null))
			{
				DecorProvider component = item.GetComponent<DecorProvider>();
				if ((Object)component != (Object)null)
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
}
