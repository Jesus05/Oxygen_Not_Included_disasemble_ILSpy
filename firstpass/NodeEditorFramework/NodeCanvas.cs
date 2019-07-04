using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework
{
	[NodeCanvasType("Default")]
	public class NodeCanvas : ScriptableObject
	{
		public delegate void CompleteLoadCallback(string fileName, NodeCanvas canvas);

		public List<Node> nodes = new List<Node>();

		public NodeEditorState[] editorStates = new NodeEditorState[0];

		public bool livesInScene = false;

		public void Validate()
		{
			if (nodes == null)
			{
				Debug.LogWarning("NodeCanvas '" + base.name + "' nodes were erased and set to null! Automatically fixed!");
				nodes = new List<Node>();
			}
			for (int i = 0; i < nodes.Count; i++)
			{
				Node node = nodes[i];
				if ((Object)node == (Object)null)
				{
					Debug.LogWarning("NodeCanvas '" + base.name + "' contained broken (null) nodes! Automatically fixed!");
					nodes.RemoveAt(i);
					i--;
				}
				else
				{
					for (int j = 0; j < node.Inputs.Count; j++)
					{
						NodeInput nodeInput = node.Inputs[j];
						if ((Object)nodeInput == (Object)null)
						{
							Debug.LogWarning("NodeCanvas '" + base.name + "' Node '" + node.name + "' contained broken (null) NodeKnobs! Automatically fixed!");
							node.Inputs.RemoveAt(j);
							j--;
						}
						else if ((Object)nodeInput.connection != (Object)null && (Object)nodeInput.connection.body == (Object)null)
						{
							nodeInput.connection = null;
						}
					}
					for (int k = 0; k < node.Outputs.Count; k++)
					{
						NodeOutput nodeOutput = node.Outputs[k];
						if ((Object)nodeOutput == (Object)null)
						{
							Debug.LogWarning("NodeCanvas '" + base.name + "' Node '" + node.name + "' contained broken (null) NodeKnobs! Automatically fixed!");
							node.Outputs.RemoveAt(k);
							k--;
						}
						else
						{
							for (int l = 0; l < nodeOutput.connections.Count; l++)
							{
								NodeInput nodeInput2 = nodeOutput.connections[l];
								if ((Object)nodeInput2 == (Object)null || (Object)nodeInput2.body == (Object)null)
								{
									nodeOutput.connections.RemoveAt(l);
									l--;
								}
							}
						}
					}
					for (int m = 0; m < node.nodeKnobs.Count; m++)
					{
						NodeKnob nodeKnob = node.nodeKnobs[m];
						if ((Object)nodeKnob == (Object)null)
						{
							Debug.LogWarning("NodeCanvas '" + base.name + "' Node '" + node.name + "' contained broken (null) NodeKnobs! Automatically fixed!");
							node.nodeKnobs.RemoveAt(m);
							m--;
						}
						else if (nodeKnob is NodeInput)
						{
							NodeInput nodeInput3 = nodeKnob as NodeInput;
							if ((Object)nodeInput3.connection != (Object)null && (Object)nodeInput3.connection.body == (Object)null)
							{
								nodeInput3.connection = null;
							}
						}
						else if (nodeKnob is NodeOutput)
						{
							NodeOutput nodeOutput2 = nodeKnob as NodeOutput;
							for (int n = 0; n < nodeOutput2.connections.Count; n++)
							{
								NodeInput nodeInput4 = nodeOutput2.connections[n];
								if ((Object)nodeInput4 == (Object)null || (Object)nodeInput4.body == (Object)null)
								{
									nodeOutput2.connections.RemoveAt(n);
									n--;
								}
							}
						}
					}
				}
			}
			if (editorStates == null)
			{
				Debug.LogWarning("NodeCanvas '" + base.name + "' editorStates were erased! Automatically fixed!");
				editorStates = new NodeEditorState[0];
			}
			editorStates = (from state in editorStates
			where (Object)state != (Object)null
			select state).ToArray();
			NodeEditorState[] array = editorStates;
			foreach (NodeEditorState nodeEditorState in array)
			{
				if (!nodes.Contains(nodeEditorState.selectedNode))
				{
					nodeEditorState.selectedNode = null;
				}
			}
		}

		public virtual void BeforeSavingCanvas()
		{
		}

		public virtual void AdditionalSaveMethods(string sceneCanvasName, CompleteLoadCallback onComplete)
		{
		}

		public virtual string DrawAdditionalSettings(string sceneCanvasName)
		{
			return sceneCanvasName;
		}

		public virtual void UpdateSettings(string sceneCanvasName)
		{
		}
	}
}
