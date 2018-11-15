using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using ProcGen.Noise;
using System;
using UnityEngine;

[Node(false, "Noise/Combine", new Type[]
{
	typeof(NoiseNodeCanvas)
})]
public class CombinerModuleNodeEditor : BaseNodeEditor
{
	private const string Id = "combinerModuleNodeEditor";

	[SerializeField]
	public Combiner target = new Combiner();

	public override string GetID => "combinerModuleNodeEditor";

	public override Type GetObjectType => typeof(CombinerModuleNodeEditor);

	public override NoiseBase GetTarget()
	{
		return target;
	}

	public override Node Create(Vector2 pos)
	{
		CombinerModuleNodeEditor combinerModuleNodeEditor = ScriptableObject.CreateInstance<CombinerModuleNodeEditor>();
		combinerModuleNodeEditor.rect = new Rect(pos.x, pos.y, 300f, 200f);
		combinerModuleNodeEditor.name = "Combine";
		combinerModuleNodeEditor.CreateInput("Source A", "IModule3D", NodeSide.Left, 10f);
		combinerModuleNodeEditor.CreateInput("Source B", "IModule3D", NodeSide.Left, 30f);
		combinerModuleNodeEditor.CreateOutput("Next Node", "IModule3D", NodeSide.Right, 30f);
		return combinerModuleNodeEditor;
	}

	public override bool Calculate()
	{
		if (!allInputsReady())
		{
			return false;
		}
		IModule3D value = Inputs[0].GetValue<IModule3D>();
		if (value == null)
		{
			return false;
		}
		IModule3D value2 = Inputs[1].GetValue<IModule3D>();
		if (value2 == null)
		{
			return false;
		}
		IModule3D module3D = target.CreateModule(value, value2);
		if (module3D == null)
		{
			return false;
		}
		Outputs[0].SetValue(module3D);
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}
}
