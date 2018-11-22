using NodeEditorFramework.Utilities;
using System.IO;
using UnityEngine;

namespace NodeEditorFramework.Standard
{
	public class RuntimeNodeEditor : MonoBehaviour
	{
		public string canvasPath;

		public NodeCanvas canvas;

		private NodeEditorState state;

		public bool screenSize = false;

		private Rect canvasRect;

		public Rect specifiedRootRect;

		public Rect specifiedCanvasRect;

		private string sceneCanvasName = "";

		private Vector2 loadScenePos;

		public void Start()
		{
			NodeEditor.checkInit(false);
			NodeEditor.initiated = false;
			LoadNodeCanvas(canvasPath);
			FPSCounter.Create();
		}

		public void Update()
		{
			NodeEditor.Update();
			FPSCounter.Update();
		}

		public void OnGUI()
		{
			if ((Object)canvas != (Object)null)
			{
				if ((Object)state == (Object)null)
				{
					NewEditorState();
				}
				NodeEditor.checkInit(true);
				if (NodeEditor.InitiationError)
				{
					GUILayout.Label("Initiation failed! Check console for more information!");
				}
				else
				{
					try
					{
						if (!screenSize && specifiedRootRect.max != specifiedRootRect.min)
						{
							GUI.BeginGroup(specifiedRootRect, NodeEditorGUI.nodeSkin.box);
						}
						NodeEditorGUI.StartNodeGUI();
						canvasRect = ((!screenSize) ? specifiedCanvasRect : new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
						canvasRect.width -= 200f;
						state.canvasRect = canvasRect;
						NodeEditor.DrawCanvas(canvas, state);
						GUILayout.BeginArea(new Rect(canvasRect.x + state.canvasRect.width, state.canvasRect.y, 200f, state.canvasRect.height), NodeEditorGUI.nodeSkin.box);
						SideGUI();
						GUILayout.EndArea();
						NodeEditorGUI.EndNodeGUI();
						if (!screenSize && specifiedRootRect.max != specifiedRootRect.min)
						{
							GUI.EndGroup();
						}
					}
					catch (UnityException exception)
					{
						NewNodeCanvas();
						NodeEditor.ReInit(true);
						Debug.LogError("Unloaded Canvas due to exception in Draw!", null);
						Debug.LogException(exception);
					}
				}
			}
		}

		public void SideGUI()
		{
			GUILayout.Label(new GUIContent("Node Editor (" + canvas.name + ")", "The currently opened canvas in the Node Editor"));
			screenSize = GUILayout.Toggle(screenSize, "Adapt to Screen");
			GUILayout.Label("FPS: " + FPSCounter.currentFPS);
			GUILayout.Label(new GUIContent("Node Editor (" + canvas.name + ")"), NodeEditorGUI.nodeLabelBold);
			if (GUILayout.Button(new GUIContent("New Canvas", "Loads an empty Canvas")))
			{
				NewNodeCanvas();
			}
			GUILayout.Space(6f);
			GUILayout.BeginHorizontal();
			sceneCanvasName = GUILayout.TextField(sceneCanvasName, GUILayout.ExpandWidth(true));
			if (GUILayout.Button(new GUIContent("Save to Scene", "Saves the Canvas to the Scene"), GUILayout.ExpandWidth(false)))
			{
				SaveSceneNodeCanvas(sceneCanvasName);
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button(new GUIContent("Load from Scene", "Loads the Canvas from the Scene")))
			{
				GenericMenu genericMenu = new GenericMenu();
				string[] sceneSaves = NodeEditorSaveManager.GetSceneSaves();
				foreach (string text in sceneSaves)
				{
					genericMenu.AddItem(new GUIContent(text), false, LoadSceneCanvasCallback, text);
				}
				genericMenu.Show(loadScenePos, 40f);
			}
			if (Event.current.type == EventType.Repaint)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				loadScenePos = new Vector2(lastRect.x + 2f, lastRect.yMax + 2f);
			}
			GUILayout.Space(6f);
			if (GUILayout.Button(new GUIContent("Recalculate All", "Initiates complete recalculate. Usually does not need to be triggered manually.")))
			{
				NodeEditor.RecalculateAll(canvas);
			}
			if (GUILayout.Button("Force Re-Init"))
			{
				NodeEditor.ReInit(true);
			}
			NodeEditorGUI.knobSize = RTEditorGUI.IntSlider(new GUIContent("Handle Size", "The size of the Node Input/Output handles"), NodeEditorGUI.knobSize, 12, 20);
			state.zoom = RTEditorGUI.Slider(new GUIContent("Zoom", "Use the Mousewheel. Seriously."), state.zoom, 0.6f, 2f);
		}

		private void LoadSceneCanvasCallback(object save)
		{
			LoadSceneNodeCanvas((string)save);
		}

		public void SaveSceneNodeCanvas(string path)
		{
			canvas.editorStates = new NodeEditorState[1]
			{
				state
			};
			NodeEditorSaveManager.SaveSceneNodeCanvas(path, ref canvas, true);
		}

		public void LoadSceneNodeCanvas(string path)
		{
			if ((Object)(canvas = NodeEditorSaveManager.LoadSceneNodeCanvas(path, true)) == (Object)null)
			{
				NewNodeCanvas();
			}
			else
			{
				state = NodeEditorSaveManager.ExtractEditorState(canvas, "MainEditorState");
				NodeEditor.RecalculateAll(canvas);
			}
		}

		public void LoadNodeCanvas(string path)
		{
			if (!File.Exists(path) || (Object)(canvas = NodeEditorSaveManager.LoadNodeCanvas(path, true)) == (Object)null)
			{
				NewNodeCanvas();
			}
			else
			{
				state = NodeEditorSaveManager.ExtractEditorState(canvas, "MainEditorState");
				NodeEditor.RecalculateAll(canvas);
			}
		}

		public void NewNodeCanvas()
		{
			canvas = ScriptableObject.CreateInstance<NodeCanvas>();
			canvas.name = "New Canvas";
			NewEditorState();
		}

		private void NewEditorState()
		{
			state = ScriptableObject.CreateInstance<NodeEditorState>();
			state.canvas = canvas;
			state.name = "MainEditorState";
			canvas.editorStates = new NodeEditorState[1]
			{
				state
			};
		}
	}
}
