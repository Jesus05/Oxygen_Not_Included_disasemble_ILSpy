using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayModes
{
	public class SolidConveyorMode : Mode
	{
		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private ICollection<Tag> targetIDs = OverlayScreen.SolidConveyorIDs;

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		public SolidConveyorMode()
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.SolidConveyorMap;
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
}
