using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditorCallbacks
	{
		private static int receiverCount;

		private static List<NodeEditorCallbackReceiver> callbackReceiver;

		public static System.Action OnEditorStartUp = null;

		public static Action<NodeCanvas> OnLoadCanvas;

		public static Action<NodeEditorState> OnLoadEditorState;

		public static Action<NodeCanvas> OnSaveCanvas;

		public static Action<NodeEditorState> OnSaveEditorState;

		public static Action<Node> OnAddNode;

		public static Action<Node> OnDeleteNode;

		public static Action<Node> OnMoveNode;

		public static Action<NodeKnob> OnAddNodeKnob;

		public static Action<NodeInput> OnAddConnection;

		public static Action<NodeInput> OnRemoveConnection;

		public static void SetupReceivers()
		{
			callbackReceiver = new List<NodeEditorCallbackReceiver>(UnityEngine.Object.FindObjectsOfType<NodeEditorCallbackReceiver>());
			receiverCount = callbackReceiver.Count;
		}

		public static void IssueOnEditorStartUp()
		{
			if (OnEditorStartUp != null)
			{
				OnEditorStartUp();
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnEditorStartUp();
				}
			}
		}

		public static void IssueOnLoadCanvas(NodeCanvas canvas)
		{
			if (OnLoadCanvas != null)
			{
				OnLoadCanvas(canvas);
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnLoadCanvas(canvas);
				}
			}
		}

		public static void IssueOnLoadEditorState(NodeEditorState editorState)
		{
			if (OnLoadEditorState != null)
			{
				OnLoadEditorState(editorState);
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnLoadEditorState(editorState);
				}
			}
		}

		public static void IssueOnSaveCanvas(NodeCanvas canvas)
		{
			if (OnSaveCanvas != null)
			{
				OnSaveCanvas(canvas);
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnSaveCanvas(canvas);
				}
			}
		}

		public static void IssueOnSaveEditorState(NodeEditorState editorState)
		{
			if (OnSaveEditorState != null)
			{
				OnSaveEditorState(editorState);
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnSaveEditorState(editorState);
				}
			}
		}

		public static void IssueOnAddNode(Node node)
		{
			if (OnAddNode != null)
			{
				OnAddNode(node);
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnAddNode(node);
				}
			}
		}

		public static void IssueOnDeleteNode(Node node)
		{
			if (OnDeleteNode != null)
			{
				OnDeleteNode(node);
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnDeleteNode(node);
					node.OnDelete();
				}
			}
		}

		public static void IssueOnMoveNode(Node node)
		{
			if (OnMoveNode != null)
			{
				OnMoveNode(node);
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnMoveNode(node);
				}
			}
		}

		public static void IssueOnAddNodeKnob(NodeKnob nodeKnob)
		{
			if (OnAddNodeKnob != null)
			{
				OnAddNodeKnob(nodeKnob);
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnAddNodeKnob(nodeKnob);
				}
			}
		}

		public static void IssueOnAddConnection(NodeInput input)
		{
			if (OnAddConnection != null)
			{
				OnAddConnection(input);
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnAddConnection(input);
				}
			}
		}

		public static void IssueOnRemoveConnection(NodeInput input)
		{
			if (OnRemoveConnection != null)
			{
				OnRemoveConnection(input);
			}
			for (int i = 0; i < receiverCount; i++)
			{
				if ((UnityEngine.Object)callbackReceiver[i] == (UnityEngine.Object)null)
				{
					callbackReceiver.RemoveAt(i--);
				}
				else
				{
					callbackReceiver[i].OnRemoveConnection(input);
				}
			}
		}
	}
}
