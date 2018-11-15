using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public static class OverlayGUI
	{
		public static PopupMenu currentPopup;

		public static bool HasPopupControl()
		{
			return currentPopup != null;
		}

		public static void StartOverlayGUI()
		{
			if (currentPopup != null && Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
			{
				currentPopup.Draw();
			}
		}

		public static void EndOverlayGUI()
		{
			if (currentPopup != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
			{
				currentPopup.Draw();
			}
		}
	}
}
