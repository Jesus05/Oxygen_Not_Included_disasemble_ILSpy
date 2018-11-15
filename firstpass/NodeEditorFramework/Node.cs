using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework
{
	public abstract class Node : ScriptableObject
	{
		public Rect rect = default(Rect);

		internal Vector2 contentOffset = Vector2.zero;

		[SerializeField]
		public List<NodeKnob> nodeKnobs = new List<NodeKnob>();

		[SerializeField]
		public List<NodeInput> Inputs = new List<NodeInput>();

		[SerializeField]
		public List<NodeOutput> Outputs = new List<NodeOutput>();

		[NonSerialized]
		[HideInInspector]
		internal bool calculated = true;

		[NonSerialized]
		private List<Node> recursiveSearchSurpassed;

		[NonSerialized]
		private Node startRecursiveSearchNode;

		public abstract string GetID
		{
			get;
		}

		public virtual bool AllowRecursion => false;

		public virtual bool ContinueCalculation => true;

		protected internal void InitBase()
		{
			NodeEditor.RecalculateFrom(this);
			if (!((UnityEngine.Object)NodeEditor.curNodeCanvas == (UnityEngine.Object)null) && NodeEditor.curNodeCanvas.nodes != null)
			{
				if (!NodeEditor.curNodeCanvas.nodes.Contains(this))
				{
					NodeEditor.curNodeCanvas.nodes.Add(this);
				}
				NodeEditor.RepaintClients();
			}
		}

		public void Delete()
		{
			if (!NodeEditor.curNodeCanvas.nodes.Contains(this))
			{
				throw new UnityException("The Node " + base.name + " does not exist on the Canvas " + NodeEditor.curNodeCanvas.name + "!");
			}
			NodeEditorCallbacks.IssueOnDeleteNode(this);
			NodeEditor.curNodeCanvas.nodes.Remove(this);
			for (int i = 0; i < Outputs.Count; i++)
			{
				NodeOutput nodeOutput = Outputs[i];
				while (nodeOutput.connections.Count != 0)
				{
					nodeOutput.connections[0].RemoveConnection();
				}
				UnityEngine.Object.DestroyImmediate(nodeOutput, true);
			}
			for (int j = 0; j < Inputs.Count; j++)
			{
				NodeInput nodeInput = Inputs[j];
				if ((UnityEngine.Object)nodeInput.connection != (UnityEngine.Object)null)
				{
					nodeInput.connection.connections.Remove(nodeInput);
				}
				UnityEngine.Object.DestroyImmediate(nodeInput, true);
			}
			for (int k = 0; k < nodeKnobs.Count; k++)
			{
				if ((UnityEngine.Object)nodeKnobs[k] != (UnityEngine.Object)null)
				{
					UnityEngine.Object.DestroyImmediate(nodeKnobs[k], true);
				}
			}
			UnityEngine.Object.DestroyImmediate(this, true);
		}

		public static Node Create(string nodeID, Vector2 position)
		{
			return Create(nodeID, position, null);
		}

		public static Node Create(string nodeID, Vector2 position, NodeOutput connectingOutput)
		{
			Node defaultNode = NodeTypes.getDefaultNode(nodeID);
			if ((UnityEngine.Object)defaultNode == (UnityEngine.Object)null)
			{
				throw new UnityException("Cannot create Node with id " + nodeID + " as no such Node type is registered!");
			}
			defaultNode = defaultNode.Create(position);
			defaultNode.InitBase();
			if ((UnityEngine.Object)connectingOutput != (UnityEngine.Object)null)
			{
				foreach (NodeInput input in defaultNode.Inputs)
				{
					if (input.TryApplyConnection(connectingOutput))
					{
						break;
					}
				}
			}
			NodeEditorCallbacks.IssueOnAddNode(defaultNode);
			return defaultNode;
		}

		internal void CheckNodeKnobMigration()
		{
			if (nodeKnobs.Count == 0 && (Inputs.Count != 0 || Outputs.Count != 0))
			{
				nodeKnobs.AddRange(Inputs.Cast<NodeKnob>());
				nodeKnobs.AddRange(Outputs.Cast<NodeKnob>());
			}
		}

		public abstract Node Create(Vector2 pos);

		protected internal abstract void NodeGUI();

		public virtual void DrawNodePropertyEditor()
		{
		}

		public virtual bool Calculate()
		{
			return true;
		}

		protected internal virtual void OnDelete()
		{
		}

		protected internal virtual void OnAddInputConnection(NodeInput input)
		{
		}

		protected internal virtual void OnAddOutputConnection(NodeOutput output)
		{
		}

		public virtual ScriptableObject[] GetScriptableObjects()
		{
			return new ScriptableObject[0];
		}

		protected internal virtual void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSerializableObject)
		{
		}

		public void SerializeInputsAndOutputs(Func<ScriptableObject, ScriptableObject> replaceSerializableObject)
		{
		}

		protected internal virtual void DrawNode()
		{
			Rect rect = this.rect;
			rect.position += NodeEditor.curEditorState.zoomPanAdjust + NodeEditor.curEditorState.panOffset;
			contentOffset = new Vector2(0f, 20f);
			Rect position = new Rect(rect.x, rect.y, rect.width, contentOffset.y);
			GUI.Label(position, base.name, (!((UnityEngine.Object)NodeEditor.curEditorState.selectedNode == (UnityEngine.Object)this)) ? NodeEditorGUI.nodeBox : NodeEditorGUI.nodeBoxBold);
			Rect rect2 = new Rect(rect.x, rect.y + contentOffset.y, rect.width, rect.height - contentOffset.y);
			GUI.BeginGroup(rect2, GUI.skin.box);
			rect2.position = Vector2.zero;
			GUILayout.BeginArea(rect2, GUI.skin.box);
			GUI.changed = false;
			NodeGUI();
			GUILayout.EndArea();
			GUI.EndGroup();
		}

		protected internal virtual void DrawKnobs()
		{
			CheckNodeKnobMigration();
			for (int i = 0; i < nodeKnobs.Count; i++)
			{
				nodeKnobs[i].DrawKnob();
			}
		}

		protected internal virtual void DrawConnections()
		{
			CheckNodeKnobMigration();
			if (Event.current.type == EventType.Repaint)
			{
				for (int i = 0; i < Outputs.Count; i++)
				{
					NodeOutput nodeOutput = Outputs[i];
					Vector2 center = nodeOutput.GetGUIKnob().center;
					Vector2 direction = nodeOutput.GetDirection();
					for (int j = 0; j < nodeOutput.connections.Count; j++)
					{
						NodeInput nodeInput = nodeOutput.connections[j];
						NodeEditorGUI.DrawConnection(center, direction, nodeInput.GetGUIKnob().center, nodeInput.GetDirection(), nodeOutput.typeData.Color);
					}
				}
			}
		}

		protected internal bool allInputsReady()
		{
			for (int i = 0; i < Inputs.Count; i++)
			{
				if ((UnityEngine.Object)Inputs[i].connection == (UnityEngine.Object)null || Inputs[i].connection.IsValueNull)
				{
					return false;
				}
			}
			return true;
		}

		protected internal bool hasUnassignedInputs()
		{
			for (int i = 0; i < Inputs.Count; i++)
			{
				if ((UnityEngine.Object)Inputs[i].connection == (UnityEngine.Object)null)
				{
					return true;
				}
			}
			return false;
		}

		protected internal bool descendantsCalculated()
		{
			for (int i = 0; i < Inputs.Count; i++)
			{
				if ((UnityEngine.Object)Inputs[i].connection != (UnityEngine.Object)null && !Inputs[i].connection.body.calculated)
				{
					return false;
				}
			}
			return true;
		}

		protected internal bool isInput()
		{
			for (int i = 0; i < Inputs.Count; i++)
			{
				if ((UnityEngine.Object)Inputs[i].connection != (UnityEngine.Object)null)
				{
					return false;
				}
			}
			return true;
		}

		public NodeOutput CreateOutput(string outputName, string outputType)
		{
			return NodeOutput.Create(this, outputName, outputType);
		}

		public NodeOutput CreateOutput(string outputName, string outputType, NodeSide nodeSide)
		{
			return NodeOutput.Create(this, outputName, outputType, nodeSide);
		}

		public NodeOutput CreateOutput(string outputName, string outputType, NodeSide nodeSide, float sidePosition)
		{
			return NodeOutput.Create(this, outputName, outputType, nodeSide, sidePosition);
		}

		protected void OutputKnob(int outputIdx)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Outputs[outputIdx].SetPosition();
			}
		}

		public NodeInput CreateInput(string inputName, string inputType)
		{
			return NodeInput.Create(this, inputName, inputType);
		}

		public NodeInput CreateInput(string inputName, string inputType, NodeSide nodeSide)
		{
			return NodeInput.Create(this, inputName, inputType, nodeSide);
		}

		public NodeInput CreateInput(string inputName, string inputType, NodeSide nodeSide, float sidePosition)
		{
			return NodeInput.Create(this, inputName, inputType, nodeSide, sidePosition);
		}

		protected void InputKnob(int inputIdx)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Inputs[inputIdx].SetPosition();
			}
		}

		protected static void ReassignOutputType(ref NodeOutput output, Type newOutputType)
		{
			Node body = output.body;
			string name = output.name;
			IEnumerable<NodeInput> enumerable = from connection in output.connections
			where connection.typeData.Type.IsAssignableFrom(newOutputType)
			select connection;
			output.Delete();
			NodeEditorCallbacks.IssueOnAddNodeKnob(NodeOutput.Create(body, name, newOutputType.AssemblyQualifiedName));
			output = body.Outputs[body.Outputs.Count - 1];
			foreach (NodeInput item in enumerable)
			{
				item.ApplyConnection(output);
			}
		}

		protected static void ReassignInputType(ref NodeInput input, Type newInputType)
		{
			Node body = input.body;
			string name = input.name;
			NodeOutput nodeOutput = null;
			if ((UnityEngine.Object)input.connection != (UnityEngine.Object)null && newInputType.IsAssignableFrom(input.connection.typeData.Type))
			{
				nodeOutput = input.connection;
			}
			input.Delete();
			NodeEditorCallbacks.IssueOnAddNodeKnob(NodeInput.Create(body, name, newInputType.AssemblyQualifiedName));
			input = body.Inputs[body.Inputs.Count - 1];
			if ((UnityEngine.Object)nodeOutput != (UnityEngine.Object)null)
			{
				input.ApplyConnection(nodeOutput);
			}
		}

		public bool isChildOf(Node otherNode)
		{
			if ((UnityEngine.Object)otherNode == (UnityEngine.Object)null || (UnityEngine.Object)otherNode == (UnityEngine.Object)this)
			{
				return false;
			}
			if (BeginRecursiveSearchLoop())
			{
				return false;
			}
			for (int i = 0; i < Inputs.Count; i++)
			{
				NodeOutput connection = Inputs[i].connection;
				if ((UnityEngine.Object)connection != (UnityEngine.Object)null && (UnityEngine.Object)connection.body != (UnityEngine.Object)startRecursiveSearchNode && ((UnityEngine.Object)connection.body == (UnityEngine.Object)otherNode || connection.body.isChildOf(otherNode)))
				{
					StopRecursiveSearchLoop();
					return true;
				}
			}
			EndRecursiveSearchLoop();
			return false;
		}

		internal bool isInLoop()
		{
			if (BeginRecursiveSearchLoop())
			{
				return (UnityEngine.Object)this == (UnityEngine.Object)startRecursiveSearchNode;
			}
			for (int i = 0; i < Inputs.Count; i++)
			{
				NodeOutput connection = Inputs[i].connection;
				if ((UnityEngine.Object)connection != (UnityEngine.Object)null && connection.body.isInLoop())
				{
					StopRecursiveSearchLoop();
					return true;
				}
			}
			EndRecursiveSearchLoop();
			return false;
		}

		internal bool allowsLoopRecursion(Node otherNode)
		{
			if (AllowRecursion)
			{
				return true;
			}
			if ((UnityEngine.Object)otherNode == (UnityEngine.Object)null)
			{
				return false;
			}
			if (BeginRecursiveSearchLoop())
			{
				return false;
			}
			for (int i = 0; i < Inputs.Count; i++)
			{
				NodeOutput connection = Inputs[i].connection;
				if ((UnityEngine.Object)connection != (UnityEngine.Object)null && connection.body.allowsLoopRecursion(otherNode))
				{
					StopRecursiveSearchLoop();
					return true;
				}
			}
			EndRecursiveSearchLoop();
			return false;
		}

		public void ClearCalculation()
		{
			if (!BeginRecursiveSearchLoop())
			{
				calculated = false;
				for (int i = 0; i < Outputs.Count; i++)
				{
					NodeOutput nodeOutput = Outputs[i];
					for (int j = 0; j < nodeOutput.connections.Count; j++)
					{
						nodeOutput.connections[j].body.ClearCalculation();
					}
				}
				EndRecursiveSearchLoop();
			}
		}

		internal bool BeginRecursiveSearchLoop()
		{
			if ((UnityEngine.Object)startRecursiveSearchNode == (UnityEngine.Object)null || recursiveSearchSurpassed == null)
			{
				recursiveSearchSurpassed = new List<Node>();
				startRecursiveSearchNode = this;
			}
			if (recursiveSearchSurpassed.Contains(this))
			{
				return true;
			}
			recursiveSearchSurpassed.Add(this);
			return false;
		}

		internal void EndRecursiveSearchLoop()
		{
			if ((UnityEngine.Object)startRecursiveSearchNode == (UnityEngine.Object)this)
			{
				recursiveSearchSurpassed = null;
				startRecursiveSearchNode = null;
			}
		}

		internal void StopRecursiveSearchLoop()
		{
			recursiveSearchSurpassed = null;
			startRecursiveSearchNode = null;
		}
	}
}
