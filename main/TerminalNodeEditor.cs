using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using System;
using UnityEngine;

[Node(false, "Noise/Terminus", new Type[]
{
	typeof(NoiseNodeCanvas)
})]
public class TerminalNodeEditor : BaseNodeEditor
{
	private const string Id = "terminalNodeEditor";

	public override string GetID => "terminalNodeEditor";

	public override Type GetObjectType => typeof(TerminalNodeEditor);

	public override Node Create(Vector2 pos)
	{
		TerminalNodeEditor terminalNodeEditor = ScriptableObject.CreateInstance<TerminalNodeEditor>();
		terminalNodeEditor.rect = new Rect(pos.x, pos.y, 250f, 125f);
		terminalNodeEditor.name = "Terminus";
		terminalNodeEditor.CreateInput("Final Node", "IModule3D", NodeSide.Top, 10f);
		terminalNodeEditor.CreateOutput("Display", "IModule3D", NodeSide.Right, 30f);
		return terminalNodeEditor;
	}

	public override bool Calculate()
	{
		if (!allInputsReady())
		{
			return false;
		}
		Outputs[0].SetValue(Inputs[0].GetValue<IModule3D>());
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}
}
