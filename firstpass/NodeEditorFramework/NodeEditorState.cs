using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditorFramework
{
	public class NodeEditorState : ScriptableObject
	{
		public NodeCanvas canvas;

		public NodeEditorState parentEditor;

		public bool drawing = true;

		public Node selectedNode;

		[NonSerialized]
		public Node focusedNode;

		[NonSerialized]
		public NodeKnob focusedNodeKnob;

		public Vector2 panOffset = default(Vector2);

		public float zoom = 1f;

		[NonSerialized]
		public NodeOutput connectOutput;

		[NonSerialized]
		public bool dragNode;

		[NonSerialized]
		public bool panWindow;

		[NonSerialized]
		public Vector2 dragStart;

		[NonSerialized]
		public Vector2 dragPos;

		[NonSerialized]
		public Vector2 dragOffset;

		[NonSerialized]
		public bool navigate;

		[NonSerialized]
		public Rect canvasRect;

		[NonSerialized]
		public Vector2 zoomPanAdjust;

		[NonSerialized]
		public List<Rect> ignoreInput = new List<Rect>();

		public Vector2 zoomPos => canvasRect.size / 2f;
	}
}
