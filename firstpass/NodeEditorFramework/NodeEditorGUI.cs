using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditorGUI
	{
		public static int knobSize = 16;

		public static Color NE_LightColor = new Color(0.4f, 0.4f, 0.4f);

		public static Color NE_TextColor = new Color(0.7f, 0.7f, 0.7f);

		public static Texture2D Background;

		public static Texture2D AALineTex;

		public static Texture2D GUIBox;

		public static Texture2D GUIButton;

		public static Texture2D GUIBoxSelection;

		public static GUISkin nodeSkin;

		public static GUISkin defaultSkin;

		public static GUIStyle nodeLabel;

		public static GUIStyle nodeLabelBold;

		public static GUIStyle nodeLabelSelected;

		public static GUIStyle nodeBox;

		public static GUIStyle nodeBoxBold;

		public static bool Init(bool GUIFunction)
		{
			Background = ResourceManager.LoadTexture("Textures/background.png");
			AALineTex = ResourceManager.LoadTexture("Textures/AALine.png");
			GUIBox = ResourceManager.LoadTexture("Textures/NE_Box.png");
			GUIButton = ResourceManager.LoadTexture("Textures/NE_Button.png");
			GUIBoxSelection = ResourceManager.LoadTexture("Textures/BoxSelection.png");
			if ((bool)Background && (bool)AALineTex && (bool)GUIBox && (bool)GUIButton)
			{
				if (GUIFunction)
				{
					nodeSkin = Object.Instantiate(GUI.skin);
					nodeSkin.label.normal.textColor = NE_TextColor;
					nodeLabel = nodeSkin.label;
					nodeSkin.box.normal.textColor = NE_TextColor;
					nodeSkin.box.normal.background = GUIBox;
					nodeBox = nodeSkin.box;
					nodeSkin.button.normal.textColor = NE_TextColor;
					nodeSkin.button.normal.background = GUIButton;
					nodeSkin.textArea.normal.background = GUIBox;
					nodeSkin.textArea.active.background = GUIBox;
					nodeLabelBold = new GUIStyle(nodeLabel);
					nodeLabelBold.fontStyle = FontStyle.Bold;
					nodeLabelSelected = new GUIStyle(nodeLabel);
					nodeLabelSelected.normal.background = RTEditorGUI.ColorToTex(1, NE_LightColor);
					nodeBoxBold = new GUIStyle(nodeBox);
					nodeBoxBold.fontStyle = FontStyle.Bold;
					return true;
				}
				return true;
			}
			return false;
		}

		public static void StartNodeGUI()
		{
			if ((Object)GUI.skin != (Object)defaultSkin)
			{
				if ((Object)nodeSkin == (Object)null)
				{
					Init(true);
				}
				GUI.skin = nodeSkin;
			}
			OverlayGUI.StartOverlayGUI();
		}

		public static void EndNodeGUI()
		{
			OverlayGUI.EndOverlayGUI();
			if ((Object)GUI.skin == (Object)defaultSkin)
			{
				GUI.skin = defaultSkin;
			}
		}

		public static void DrawConnection(Vector2 startPos, Vector2 endPos, Color col)
		{
			Vector2 vector = (!(startPos.x <= endPos.x)) ? Vector2.left : Vector2.right;
			DrawConnection(startPos, vector, endPos, -vector, col);
		}

		public static void DrawConnection(Vector2 startPos, Vector2 startDir, Vector2 endPos, Vector2 endDir, Color col)
		{
			DrawConnection(startPos, startDir, endPos, endDir, ConnectionDrawMethod.Bezier, col);
		}

		public static void DrawConnection(Vector2 startPos, Vector2 startDir, Vector2 endPos, Vector2 endDir, ConnectionDrawMethod drawMethod, Color col)
		{
			switch (drawMethod)
			{
			case ConnectionDrawMethod.Bezier:
			{
				float d = 80f;
				RTEditorGUI.DrawBezier(startPos, endPos, startPos + startDir * d, endPos + endDir * d, col * Color.gray, null, 3f);
				break;
			}
			case ConnectionDrawMethod.StraightLine:
				RTEditorGUI.DrawLine(startPos, endPos, col * Color.gray, null, 3f);
				break;
			}
		}

		internal static Vector2 GetSecondConnectionVector(Vector2 startPos, Vector2 endPos, Vector2 firstVector)
		{
			if (firstVector.x != 0f && firstVector.y == 0f)
			{
				return (!(startPos.x <= endPos.x)) ? firstVector : (-firstVector);
			}
			if (firstVector.y != 0f && firstVector.x == 0f)
			{
				return (!(startPos.y <= endPos.y)) ? firstVector : (-firstVector);
			}
			return -firstVector;
		}
	}
}
