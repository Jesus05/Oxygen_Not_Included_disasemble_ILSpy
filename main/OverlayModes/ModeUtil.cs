using UnityEngine;

namespace OverlayModes
{
	public class ModeUtil
	{
		public static float GetHighlightScale()
		{
			return Mathf.SmoothStep(0.5f, 1f, Mathf.Abs(Mathf.Sin(Time.unscaledTime * 4f)));
		}
	}
}
