using NodeEditorFramework;
using ProcGen.Noise;
using System;
using UnityEngine;

[Node(false, "Noise/Primitive", new Type[]
{
	typeof(NoiseNodeCanvas)
})]
public class PrimitiveNodeEditor : BaseNodeEditor
{
	private const string Id = "primitiveNodeEditor";

	public Primitive target = new Primitive();

	public override string GetID => "primitiveNodeEditor";

	public override Type GetObjectType => typeof(PrimitiveNodeEditor);

	public override NoiseBase GetTarget()
	{
		return target;
	}

	public override Node Create(Vector2 pos)
	{
		PrimitiveNodeEditor primitiveNodeEditor = ScriptableObject.CreateInstance<PrimitiveNodeEditor>();
		primitiveNodeEditor.target = new Primitive();
		primitiveNodeEditor.rect = new Rect(pos.x, pos.y, 250f, 125f);
		primitiveNodeEditor.name = "Primative";
		primitiveNodeEditor.CreateOutput("Next Node", "IModule3D", NodeSide.Right, 30f);
		return primitiveNodeEditor;
	}

	public override bool Calculate()
	{
		Outputs[0].SetValue(target.CreateModule(0));
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}
}
