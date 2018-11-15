using NodeEditorFramework;
using ProcGen.Noise;
using System;
using UnityEngine;

[Node(false, "Noise/Terrace Control", new Type[]
{
	typeof(NoiseNodeCanvas)
})]
public class FloatPointsNodeEditor : BaseNodeEditor
{
	private const string Id = "floatPointsNodeEditor";

	[SerializeField]
	public FloatList target = new FloatList();

	private static float height = 100f;

	public override string GetID => "floatPointsNodeEditor";

	public override Type GetObjectType => typeof(FloatPointsNodeEditor);

	public override NoiseBase GetTarget()
	{
		return target;
	}

	public override Node Create(Vector2 pos)
	{
		FloatPointsNodeEditor floatPointsNodeEditor = ScriptableObject.CreateInstance<FloatPointsNodeEditor>();
		floatPointsNodeEditor.rect = new Rect(pos.x, pos.y, 300f, height);
		floatPointsNodeEditor.name = "Terrace Control";
		floatPointsNodeEditor.CreateOutput("Terrace", "FloatList", NodeSide.Right, 30f);
		return floatPointsNodeEditor;
	}

	public override bool Calculate()
	{
		Outputs[0].SetValue(target);
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}
}
