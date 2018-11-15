using UnityEngine;

namespace NodeEditorFramework
{
	public abstract class NodeEditorCallbackReceiver : MonoBehaviour
	{
		public virtual void OnEditorStartUp()
		{
		}

		public virtual void OnLoadCanvas(NodeCanvas canvas)
		{
		}

		public virtual void OnLoadEditorState(NodeEditorState editorState)
		{
		}

		public virtual void OnSaveCanvas(NodeCanvas canvas)
		{
		}

		public virtual void OnSaveEditorState(NodeEditorState editorState)
		{
		}

		public virtual void OnAddNode(Node node)
		{
		}

		public virtual void OnDeleteNode(Node node)
		{
		}

		public virtual void OnMoveNode(Node node)
		{
		}

		public virtual void OnAddNodeKnob(NodeKnob knob)
		{
		}

		public virtual void OnAddConnection(NodeInput input)
		{
		}

		public virtual void OnRemoveConnection(NodeInput input)
		{
		}
	}
}
