using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
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
				if (!((Object)component == (Object)null))
				{
					partition.Add(component);
				}
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!((Object)item == (Object)null) && !((Object)item.gameObject == (Object)null))
			{
				Harvestable component = item.GetComponent<Harvestable>();
				if (!((Object)component == (Object)null))
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
}
