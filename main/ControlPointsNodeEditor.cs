using NodeEditorFramework;
using ProcGen.Noise;
using System;
using UnityEngine;

[Node(false, "Noise/Curve Control", new Type[]
{
	typeof(NoiseNodeCanvas)
})]
public class ControlPointsNodeEditor : BaseNodeEditor
{
	private const string Id = "controlPointsNodeEditor";

	[SerializeField]
	public ControlPointList target = new ControlPointList();

	private static float height = 100f;

	public override string GetID => "controlPointsNodeEditor";

	public override Type GetObjectType => typeof(ControlPointsNodeEditor);

	public override NoiseBase GetTarget()
	{
		return target;
	}

	public override Node Create(Vector2 pos)
	{
		ControlPointsNodeEditor controlPointsNodeEditor = ScriptableObject.CreateInstance<ControlPointsNodeEditor>();
		controlPointsNodeEditor.rect = new Rect(pos.x, pos.y, 300f, height);
		controlPointsNodeEditor.name = "Curve Control";
		controlPointsNodeEditor.CreateOutput("Curve", "ControlPoints", NodeSide.Right, 30f);
		return controlPointsNodeEditor;
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
