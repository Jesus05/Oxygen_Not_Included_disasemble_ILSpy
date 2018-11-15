using NodeEditorFramework;
using ProcGen.Noise;
using System;
using UnityEngine;

public class ControlPointsType : IConnectionTypeDeclaration
{
	public string Identifier => "ControlPoints";

	public Type Type => typeof(ControlPointList);

	public Color Color => Color.green;

	public string InKnobTex => "Textures/In_Knob.png";

	public string OutKnobTex => "Textures/Out_Knob.png";
}
