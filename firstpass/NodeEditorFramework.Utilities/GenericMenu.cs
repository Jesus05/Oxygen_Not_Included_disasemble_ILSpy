using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public class GenericMenu
	{
		private static PopupMenu popup;

		public Vector2 Position => popup.Position;

		public GenericMenu()
		{
			popup = new PopupMenu();
		}

		public void ShowAsContext()
		{
			popup.Show(GUIScaleUtility.GUIToScreenSpace(Event.current.mousePosition), 40f);
		}

		public void Show(Vector2 pos, float MinWidth = 40f)
		{
			popup.Show(GUIScaleUtility.GUIToScreenSpace(pos), MinWidth);
		}

		public void AddItem(GUIContent content, bool on, PopupMenu.MenuFunctionData func, object userData)
		{
			popup.AddItem(content, on, func, userData);
		}

		public void AddItem(GUIContent content, bool on, PopupMenu.MenuFunction func)
		{
			popup.AddItem(content, on, func);
		}

		public void AddSeparator(string path)
		{
			popup.AddSeparator(path);
		}
	}
}
