using UnityEngine;

namespace OverlayModes
{
	public class Oxygen : Mode
	{
		public override SimViewMode ViewMode()
		{
			return SimViewMode.OxygenMap;
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
}
