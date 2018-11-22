using System.Collections.Generic;
using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public class PopupMenu
	{
		public delegate void MenuFunction();

		public delegate void MenuFunctionData(object userData);

		public class MenuItem
		{
			public string path;

			public GUIContent content;

			public MenuFunction func;

			public MenuFunctionData funcData;

			public object userData;

			public bool separator = false;

			public bool group = false;

			public Rect groupPos;

			public List<MenuItem> subItems;

			public MenuItem()
			{
				separator = true;
			}

			public MenuItem(string _path, GUIContent _content, bool _group)
			{
				path = _path;
				content = _content;
				group = _group;
				if (group)
				{
					subItems = new List<MenuItem>();
				}
			}

			public MenuItem(string _path, GUIContent _content, MenuFunction _func)
			{
				path = _path;
				content = _content;
				func = _func;
			}

			public MenuItem(string _path, GUIContent _content, MenuFunctionData _func, object _userData)
			{
				path = _path;
				content = _content;
				funcData = _func;
				userData = _userData;
			}

			public void Execute()
			{
				if (funcData != null)
				{
					funcData(userData);
				}
				else if (func != null)
				{
					func();
				}
			}
		}

		public List<MenuItem> menuItems = new List<MenuItem>();

		private Rect position;

		private string selectedPath;

		private MenuItem groupToDraw;

		private float currentItemHeight;

		private bool close;

		public static GUIStyle backgroundStyle;

		public static Texture2D expandRight;

		public static float itemHeight;

		public static GUIStyle selectedLabel;

		public float minWidth;

		public Vector2 Position => position.position;

		public PopupMenu()
		{
			SetupGUI();
		}

		public void SetupGUI()
		{
			backgroundStyle = new GUIStyle(GUI.skin.box);
			backgroundStyle.contentOffset = new Vector2(2f, 2f);
			expandRight = ResourceManager.LoadTexture("Textures/expandRight.png");
			itemHeight = GUI.skin.label.CalcHeight(new GUIContent("text"), 100f);
			selectedLabel = new GUIStyle(GUI.skin.label);
			selectedLabel.normal.background = RTEditorGUI.ColorToTex(1, new Color(0.4f, 0.4f, 0.4f));
		}

		public void Show(Vector2 pos, float MinWidth = 40f)
		{
			minWidth = MinWidth;
			position = calculateRect(pos, menuItems, minWidth);
			selectedPath = "";
			OverlayGUI.currentPopup = this;
		}

		public void AddItem(GUIContent content, bool on, MenuFunctionData func, object userData)
		{
			string path;
			MenuItem menuItem = AddHierarchy(ref content, out path);
			if (menuItem != null)
			{
				menuItem.subItems.Add(new MenuItem(path, content, func, userData));
			}
			else
			{
				menuItems.Add(new MenuItem(path, content, func, userData));
			}
		}

		public void AddItem(GUIContent content, bool on, MenuFunction func)
		{
			string path;
			MenuItem menuItem = AddHierarchy(ref content, out path);
			if (menuItem != null)
			{
				menuItem.subItems.Add(new MenuItem(path, content, func));
			}
			else
			{
				menuItems.Add(new MenuItem(path, content, func));
			}
		}

		public void AddSeparator(string path)
		{
			GUIContent content = new GUIContent(path);
			MenuItem menuItem = AddHierarchy(ref content, out path);
			if (menuItem != null)
			{
				menuItem.subItems.Add(new MenuItem());
			}
			else
			{
				menuItems.Add(new MenuItem());
			}
		}

		private MenuItem AddHierarchy(ref GUIContent content, out string path)
		{
			path = content.text;
			if (!path.Contains("/"))
			{
				return null;
			}
			string[] array = path.Split('/');
			string folderPath = array[0];
			MenuItem menuItem = menuItems.Find((MenuItem item) => item.content != null && item.content.text == folderPath && item.group);
			if (menuItem == null)
			{
				menuItems.Add(menuItem = new MenuItem(folderPath, new GUIContent(folderPath), true));
			}
			for (int i = 1; i < array.Length - 1; i++)
			{
				string folder = array[i];
				folderPath = folderPath + "/" + folder;
				if (menuItem == null)
				{
					Debug.LogError("Parent is null!", null);
				}
				else if (menuItem.subItems == null)
				{
					Debug.LogError("Subitems of " + menuItem.content.text + " is null!", null);
				}
				MenuItem menuItem2 = menuItem.subItems.Find((MenuItem item) => item.content != null && item.content.text == folder && item.group);
				if (menuItem2 == null)
				{
					menuItem.subItems.Add(menuItem2 = new MenuItem(folderPath, new GUIContent(folder), true));
				}
				menuItem = menuItem2;
			}
			path = content.text;
			content = new GUIContent(array[array.Length - 1], content.tooltip);
			return menuItem;
		}

		public void Draw()
		{
			bool flag = DrawGroup(position, menuItems);
			while (groupToDraw != null && !close)
			{
				MenuItem menuItem = groupToDraw;
				groupToDraw = null;
				if (menuItem.group && DrawGroup(menuItem.groupPos, menuItem.subItems))
				{
					flag = true;
				}
			}
			if (!flag || close)
			{
				OverlayGUI.currentPopup = null;
			}
			NodeEditor.RepaintClients();
		}

		private bool DrawGroup(Rect pos, List<MenuItem> menuItems)
		{
			Rect rect = calculateRect(pos.position, menuItems, minWidth);
			Rect rect2 = new Rect(rect);
			rect2.xMax += 20f;
			rect2.xMin -= 20f;
			rect2.yMax += 20f;
			rect2.yMin -= 20f;
			bool result = rect2.Contains(Event.current.mousePosition);
			Vector2 contentOffset = backgroundStyle.contentOffset;
			currentItemHeight = contentOffset.y;
			GUI.BeginGroup(extendRect(rect, backgroundStyle.contentOffset), GUIContent.none, backgroundStyle);
			for (int i = 0; i < menuItems.Count; i++)
			{
				DrawItem(menuItems[i], rect);
				if (close)
				{
					break;
				}
			}
			GUI.EndGroup();
			return result;
		}

		private void DrawItem(MenuItem item, Rect groupRect)
		{
			if (item.separator)
			{
				if (Event.current.type == EventType.Repaint)
				{
					Vector2 contentOffset = backgroundStyle.contentOffset;
					RTEditorGUI.Seperator(new Rect(contentOffset.x + 1f, currentItemHeight + 1f, groupRect.width - 2f, 1f));
				}
				currentItemHeight += 3f;
			}
			else
			{
				Vector2 contentOffset2 = backgroundStyle.contentOffset;
				Rect rect = new Rect(contentOffset2.x, currentItemHeight, groupRect.width, itemHeight);
				if (rect.Contains(Event.current.mousePosition))
				{
					selectedPath = item.path;
				}
				bool flag = selectedPath == item.path || selectedPath.Contains(item.path + "/");
				GUI.Label(rect, item.content, (!flag) ? GUI.skin.label : selectedLabel);
				if (item.group)
				{
					GUI.DrawTexture(new Rect(rect.x + rect.width - 12f, rect.y + (rect.height - 12f) / 2f, 12f, 12f), expandRight);
					if (flag)
					{
						item.groupPos = new Rect(groupRect.x + groupRect.width + 4f, groupRect.y + currentItemHeight - 2f, 0f, 0f);
						groupToDraw = item;
					}
				}
				else if (flag && (Event.current.type == EventType.MouseDown || (Event.current.button != 1 && Event.current.type == EventType.MouseUp)))
				{
					item.Execute();
					close = true;
					Event.current.Use();
				}
				currentItemHeight += itemHeight;
			}
		}

		private static Rect extendRect(Rect rect, Vector2 extendValue)
		{
			rect.x -= extendValue.x;
			rect.y -= extendValue.y;
			rect.width += extendValue.x + extendValue.x;
			rect.height += extendValue.y + extendValue.y;
			return rect;
		}

		private static Rect calculateRect(Vector2 position, List<MenuItem> menuItems, float minWidth)
		{
			float num = minWidth;
			float num2 = 0f;
			for (int i = 0; i < menuItems.Count; i++)
			{
				MenuItem menuItem = menuItems[i];
				if (menuItem.separator)
				{
					num2 += 3f;
				}
				else
				{
					float a = num;
					Vector2 vector = GUI.skin.label.CalcSize(menuItem.content);
					num = Mathf.Max(a, vector.x + (float)((!menuItem.group) ? 10 : 22));
					num2 += itemHeight;
				}
			}
			Vector2 vector2 = new Vector2(num, num2);
			bool flag = position.y + vector2.y <= (float)Screen.height;
			return new Rect(position.x, position.y - ((!flag) ? vector2.y : 0f), vector2.x, vector2.y);
		}
	}
}
