using System;
using System.Collections;
using UnityEngine;

namespace OverlayModes
{
	public class Harvest : BasePlantMode
	{
		private ColorHighlightCondition[] highlightConditions = new ColorHighlightCondition[1]
		{
			new ColorHighlightCondition((KMonoBehaviour harvestable) => new Color(0.65f, 0.65f, 0.65f, 0.65f), (KMonoBehaviour harvestable) => true)
		};

		public Harvest()
			: base(OverlayScreen.HarvestableIDs)
		{
		}

		public override SimViewMode ViewMode()
		{
			return SimViewMode.HarvestWhenReady;
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
}
