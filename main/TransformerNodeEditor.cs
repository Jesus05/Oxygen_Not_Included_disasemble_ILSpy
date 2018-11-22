using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using ProcGen.Noise;
using System;
using UnityEngine;

[Node(false, "Noise/Transformer", new Type[]
{
	typeof(NoiseNodeCanvas)
})]
public class TransformerNodeEditor : BaseNodeEditor
{
	private const string Id = "transformerNodeEditor";

	[SerializeField]
	public Transformer target = new Transformer();

	public override string GetID => "transformerNodeEditor";

	public override Type GetObjectType => typeof(TransformerNodeEditor);

	public override NoiseBase GetTarget()
	{
		return target;
	}

	public override Node Create(Vector2 pos)
	{
		TransformerNodeEditor transformerNodeEditor = ScriptableObject.CreateInstance<TransformerNodeEditor>();
		transformerNodeEditor.rect = new Rect(pos.x, pos.y, 300f, 200f);
		transformerNodeEditor.name = "Transformer";
		transformerNodeEditor.CreateInput("Source", "IModule3D", NodeSide.Left, 10f);
		transformerNodeEditor.CreateInput("X", "IModule3D", NodeSide.Left, 30f);
		transformerNodeEditor.CreateInput("Y", "IModule3D", NodeSide.Left, 40f);
		transformerNodeEditor.CreateInput("Z", "IModule3D", NodeSide.Left, 50f);
		transformerNodeEditor.CreateOutput("Next Node", "IModule3D", NodeSide.Right, 30f);
		return transformerNodeEditor;
	}

	public override bool Calculate()
	{
		IModule3D value = Inputs[0].GetValue<IModule3D>();
		if (value != null)
		{
			IModule3D value2 = Inputs[1].GetValue<IModule3D>();
			IModule3D value3 = Inputs[2].GetValue<IModule3D>();
			IModule3D value4 = Inputs[3].GetValue<IModule3D>();
			if (target.transformerType != Transformer.TransformerType.RotatePoint)
			{
				if (value2 == null)
				{
					return false;
				}
				if (value3 == null)
				{
					return false;
				}
				if (value4 == null)
				{
					return false;
				}
			}
			IModule3D module3D = target.CreateModule(value, value2, value3, value4);
			if (module3D != null)
			{
				Outputs[0].SetValue(module3D);
				return true;
			}
			return false;
		}
		return false;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}
}
