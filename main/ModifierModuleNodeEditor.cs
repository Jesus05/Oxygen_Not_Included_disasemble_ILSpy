using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Modifier;
using NodeEditorFramework;
using ProcGen.Noise;
using System;
using System.Collections.Generic;
using UnityEngine;

[Node(false, "Noise/Modify", new Type[]
{
	typeof(NoiseNodeCanvas)
})]
public class ModifierModuleNodeEditor : BaseNodeEditor
{
	private const string Id = "modifierModuleNodeEditor";

	[SerializeField]
	public ProcGen.Noise.Modifier target = new ProcGen.Noise.Modifier();

	public override string GetID => "modifierModuleNodeEditor";

	public override Type GetObjectType => typeof(ModifierModuleNodeEditor);

	public override NoiseBase GetTarget()
	{
		return target;
	}

	public override Node Create(Vector2 pos)
	{
		ModifierModuleNodeEditor modifierModuleNodeEditor = ScriptableObject.CreateInstance<ModifierModuleNodeEditor>();
		modifierModuleNodeEditor.rect = new Rect(pos.x, pos.y, 300f, 100f);
		modifierModuleNodeEditor.name = "Modify";
		modifierModuleNodeEditor.CreateInput("Source", "IModule3D", NodeSide.Left, 10f);
		modifierModuleNodeEditor.CreateInput("Curve Control", "ControlPoints", NodeSide.Left, 50f);
		modifierModuleNodeEditor.CreateInput("Terrace Control", "FloatList", NodeSide.Left, 60f);
		modifierModuleNodeEditor.CreateOutput("Next Node", "IModule3D", NodeSide.Right, 30f);
		return modifierModuleNodeEditor;
	}

	public override bool Calculate()
	{
		IModule3D value = Inputs[0].GetValue<IModule3D>();
		if (value == null)
		{
			return false;
		}
		ControlPointList value2 = Inputs[1].GetValue<ControlPointList>();
		if (target.modifyType == ProcGen.Noise.Modifier.ModifyType.Curve && (value2 == null || value2.points.Count == 0))
		{
			return false;
		}
		FloatList value3 = Inputs[2].GetValue<FloatList>();
		if (target.modifyType == ProcGen.Noise.Modifier.ModifyType.Terrace && (value3 == null || value3.points.Count == 0))
		{
			return false;
		}
		IModule3D module3D = target.CreateModule(value);
		if (module3D == null)
		{
			return false;
		}
		if (target.modifyType == ProcGen.Noise.Modifier.ModifyType.Curve)
		{
			Curve curve = module3D as Curve;
			curve.ClearControlPoints();
			List<ControlPoint> controls = value2.GetControls();
			foreach (ControlPoint item in controls)
			{
				curve.AddControlPoint(item);
			}
		}
		else if (target.modifyType == ProcGen.Noise.Modifier.ModifyType.Terrace)
		{
			Terrace terrace = module3D as Terrace;
			terrace.ClearControlPoints();
			foreach (float point in value3.points)
			{
				float input = point;
				terrace.AddControlPoint(input);
			}
		}
		Outputs[0].SetValue(module3D);
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}
}
