using NodeEditorFramework;
using ProcGen.Noise;
using System;
using UnityEngine;

[Node(true, "Noise/Base Noise Node", new Type[]
{
	typeof(NoiseNodeCanvas)
})]
public class BaseNodeEditor : Node
{
	private const string Id = "baseNodeEditor";

	public virtual Type GetObjectType => typeof(BaseNodeEditor);

	public override string GetID => "baseNodeEditor";

	protected SampleSettings settings
	{
		get
		{
			NoiseNodeCanvas noiseNodeCanvas = NodeEditor.curNodeCanvas as NoiseNodeCanvas;
			if ((UnityEngine.Object)noiseNodeCanvas != (UnityEngine.Object)null)
			{
				return noiseNodeCanvas.settings;
			}
			return null;
		}
	}

	public virtual NoiseBase GetTarget()
	{
		return null;
	}

	public override Node Create(Vector2 pos)
	{
		return null;
	}

	protected override void NodeGUI()
	{
		GUILayout.BeginHorizontal();
		if (Inputs != null)
		{
			GUILayout.BeginVertical();
			foreach (NodeInput input in Inputs)
			{
				input.DisplayLayout();
			}
			GUILayout.EndVertical();
		}
		if (Outputs != null)
		{
			GUILayout.BeginVertical();
			foreach (NodeOutput output in Outputs)
			{
				output.DisplayLayout();
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
		if (GUI.changed)
		{
			NodeEditor.RecalculateFrom(this);
		}
	}
}
