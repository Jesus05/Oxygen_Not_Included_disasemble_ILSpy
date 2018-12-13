using NodeEditorFramework.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditor
	{
		public static string editorPath = "Assets/Plugins/Node_Editor/";

		public static NodeCanvas curNodeCanvas;

		public static NodeEditorState curEditorState;

		internal static System.Action NEUpdate;

		public static System.Action ClientRepaints;

		public static bool initiated;

		public static bool InitiationError;

		public static List<Node> workList;

		private static int calculationCount;

		public static void Update()
		{
			if (NEUpdate != null)
			{
				NEUpdate();
			}
		}

		public static void RepaintClients()
		{
			if (ClientRepaints != null)
			{
				ClientRepaints();
			}
		}

		public static void checkInit(bool GUIFunction)
		{
			if (!initiated && !InitiationError)
			{
				ReInit(GUIFunction);
			}
		}

		public static void ReInit(bool GUIFunction)
		{
			CheckEditorPath();
			ResourceManager.SetDefaultResourcePath(editorPath + "Resources/");
			if (!NodeEditorGUI.Init(GUIFunction))
			{
				InitiationError = true;
			}
			else
			{
				ConnectionTypes.FetchTypes();
				NodeTypes.FetchNodes();
				NodeCanvasManager.GetAllCanvasTypes();
				NodeEditorCallbacks.SetupReceivers();
				NodeEditorCallbacks.IssueOnEditorStartUp();
				GUIScaleUtility.CheckInit();
				NodeEditorInputSystem.SetupInput();
				initiated = GUIFunction;
			}
		}

		public static void CheckEditorPath()
		{
		}

		public static void DrawCanvas(NodeCanvas nodeCanvas, NodeEditorState editorState)
		{
			if (editorState.drawing)
			{
				checkInit(true);
				DrawSubCanvas(nodeCanvas, editorState);
			}
		}

		private static void DrawSubCanvas(NodeCanvas nodeCanvas, NodeEditorState editorState)
		{
			if (editorState.drawing)
			{
				NodeCanvas nodeCanvas2 = curNodeCanvas;
				NodeEditorState nodeEditorState = curEditorState;
				curNodeCanvas = nodeCanvas;
				curEditorState = editorState;
				if (Event.current.type == EventType.Repaint)
				{
					float num = curEditorState.zoom / (float)NodeEditorGUI.Background.width;
					float num2 = curEditorState.zoom / (float)NodeEditorGUI.Background.height;
					Vector2 vector = curEditorState.zoomPos + curEditorState.panOffset / curEditorState.zoom;
					GUI.DrawTextureWithTexCoords(texCoords: new Rect((0f - vector.x) * num, (vector.y - curEditorState.canvasRect.height) * num2, curEditorState.canvasRect.width * num, curEditorState.canvasRect.height * num2), position: curEditorState.canvasRect, image: NodeEditorGUI.Background);
				}
				NodeEditorInputSystem.HandleInputEvents(curEditorState);
				if (Event.current.type != EventType.Layout)
				{
					curEditorState.ignoreInput = new List<Rect>();
				}
				Rect rect = curEditorState.canvasRect;
				curEditorState.zoomPanAdjust = GUIScaleUtility.BeginScale(ref rect, curEditorState.zoomPos, curEditorState.zoom, false);
				if (curEditorState.navigate)
				{
					Vector2 startPos = ((!((UnityEngine.Object)curEditorState.selectedNode != (UnityEngine.Object)null)) ? curEditorState.panOffset : curEditorState.selectedNode.rect.center) + curEditorState.zoomPanAdjust;
					Vector2 mousePosition = Event.current.mousePosition;
					RTEditorGUI.DrawLine(startPos, mousePosition, Color.green, null, 3f);
					RepaintClients();
				}
				if ((UnityEngine.Object)curEditorState.connectOutput != (UnityEngine.Object)null)
				{
					NodeOutput connectOutput = curEditorState.connectOutput;
					Vector2 center = connectOutput.GetGUIKnob().center;
					Vector2 direction = connectOutput.GetDirection();
					Vector2 mousePosition2 = Event.current.mousePosition;
					Vector2 secondConnectionVector = NodeEditorGUI.GetSecondConnectionVector(center, mousePosition2, direction);
					NodeEditorGUI.DrawConnection(center, direction, mousePosition2, secondConnectionVector, connectOutput.typeData.Color);
					RepaintClients();
				}
				if (Event.current.type == EventType.Layout && (UnityEngine.Object)curEditorState.selectedNode != (UnityEngine.Object)null)
				{
					curNodeCanvas.nodes.Remove(curEditorState.selectedNode);
					curNodeCanvas.nodes.Add(curEditorState.selectedNode);
				}
				for (int i = 0; i < curNodeCanvas.nodes.Count; i++)
				{
					curNodeCanvas.nodes[i].DrawConnections();
				}
				for (int j = 0; j < curNodeCanvas.nodes.Count; j++)
				{
					Node node = curNodeCanvas.nodes[j];
					node.DrawNode();
					if (Event.current.type == EventType.Repaint)
					{
						node.DrawKnobs();
					}
				}
				GUIScaleUtility.EndScale();
				NodeEditorInputSystem.HandleLateInputEvents(curEditorState);
				curNodeCanvas = nodeCanvas2;
				curEditorState = nodeEditorState;
			}
		}

		public static Node NodeAtPosition(Vector2 canvasPos)
		{
			NodeKnob focusedKnob;
			return NodeAtPosition(curEditorState, canvasPos, out focusedKnob);
		}

		public static Node NodeAtPosition(Vector2 canvasPos, out NodeKnob focusedKnob)
		{
			return NodeAtPosition(curEditorState, canvasPos, out focusedKnob);
		}

		public static Node NodeAtPosition(NodeEditorState editorState, Vector2 canvasPos, out NodeKnob focusedKnob)
		{
			focusedKnob = null;
			if (NodeEditorInputSystem.shouldIgnoreInput(editorState))
			{
				return null;
			}
			NodeCanvas canvas = editorState.canvas;
			for (int num = canvas.nodes.Count - 1; num >= 0; num--)
			{
				Node node = canvas.nodes[num];
				if (node.rect.Contains(canvasPos))
				{
					return node;
				}
				for (int i = 0; i < node.nodeKnobs.Count; i++)
				{
					if (node.nodeKnobs[i].GetCanvasSpaceKnob().Contains(canvasPos))
					{
						focusedKnob = node.nodeKnobs[i];
						return node;
					}
				}
			}
			return null;
		}

		public static Vector2 ScreenToCanvasSpace(Vector2 screenPos)
		{
			return ScreenToCanvasSpace(curEditorState, screenPos);
		}

		public static Vector2 ScreenToCanvasSpace(NodeEditorState editorState, Vector2 screenPos)
		{
			return (screenPos - editorState.canvasRect.position - editorState.zoomPos) * editorState.zoom - editorState.panOffset;
		}

		public static void RecalculateAll(NodeCanvas nodeCanvas)
		{
			workList = new List<Node>();
			foreach (Node node in nodeCanvas.nodes)
			{
				if (node.isInput())
				{
					node.ClearCalculation();
					workList.Add(node);
				}
			}
			StartCalculation();
		}

		public static void RecalculateFrom(Node node)
		{
			node.ClearCalculation();
			List<Node> list = new List<Node>();
			list.Add(node);
			workList = list;
			StartCalculation();
		}

		public static void StartCalculation()
		{
			checkInit(false);
			if (!InitiationError && workList != null && workList.Count != 0)
			{
				calculationCount = 0;
				bool flag = false;
				int num = 0;
				while (!flag)
				{
					flag = true;
					for (int i = 0; i < workList.Count; i++)
					{
						if (ContinueCalculation(workList[i]))
						{
							flag = false;
						}
					}
					num++;
				}
			}
		}

		private static bool ContinueCalculation(Node node)
		{
			if (node.calculated)
			{
				return false;
			}
			if ((node.descendantsCalculated() || node.isInLoop()) && node.Calculate())
			{
				node.calculated = true;
				calculationCount++;
				workList.Remove(node);
				if (node.ContinueCalculation && calculationCount < 1000)
				{
					for (int i = 0; i < node.Outputs.Count; i++)
					{
						NodeOutput nodeOutput = node.Outputs[i];
						if (!nodeOutput.calculationBlockade)
						{
							for (int j = 0; j < nodeOutput.connections.Count; j++)
							{
								ContinueCalculation(nodeOutput.connections[j].body);
							}
						}
					}
				}
				else if (calculationCount >= 1000)
				{
					Debug.LogError("Stopped calculation because of suspected Recursion. Maximum calculation iteration is currently at 1000!", null);
				}
				return true;
			}
			if (!workList.Contains(node))
			{
				workList.Add(node);
			}
			return false;
		}
	}
}
