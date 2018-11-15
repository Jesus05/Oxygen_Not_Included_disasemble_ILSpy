using UnityEngine;

namespace NodeEditorFramework
{
	public class NodeEditorInputInfo
	{
		public string message;

		public NodeEditorState editorState;

		public Event inputEvent;

		public Vector2 inputPos;

		public NodeEditorInputInfo(NodeEditorState EditorState)
		{
			message = null;
			editorState = EditorState;
			inputEvent = Event.current;
			inputPos = inputEvent.mousePosition;
		}

		public NodeEditorInputInfo(string Message, NodeEditorState EditorState)
		{
			message = Message;
			editorState = EditorState;
			inputEvent = Event.current;
			inputPos = inputEvent.mousePosition;
		}

		public void SetAsCurrentEnvironment()
		{
			NodeEditor.curEditorState = editorState;
			NodeEditor.curNodeCanvas = editorState.canvas;
		}
	}
}
